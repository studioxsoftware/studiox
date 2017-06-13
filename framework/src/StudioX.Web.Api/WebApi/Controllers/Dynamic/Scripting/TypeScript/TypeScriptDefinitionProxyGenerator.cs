using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using StudioX.Dependency;
using StudioX.Extensions;

namespace StudioX.WebApi.Controllers.Dynamic.Scripting.TypeScript
{
    internal class TypeScriptDefinitionProxyGenerator : ITransientDependency
    {
        private DynamicApiControllerInfo controllerInfo;
        private HashSet<Type> typesToBeDone = new HashSet<Type>();
        private HashSet<Type> doneTypes = new HashSet<Type>();
        private string servicePrefix = "";
        public string Generate(DynamicApiControllerInfo controllerInfo, string servicePrefix)
        {
            if (this.servicePrefix != servicePrefix)
            {
                //if there is a change in servicePrefix, we need to generate the types again
                this.servicePrefix = servicePrefix;
                typesToBeDone = new HashSet<Type>();
                doneTypes = new HashSet<Type>();
            }
            this.controllerInfo = controllerInfo;

            var script = new StringBuilder();

            script.AppendLine("     interface " + this.controllerInfo.ServiceName.Substring(this.controllerInfo.ServiceName.IndexOf('/') + 1));
            script.AppendLine("     {");

            foreach (var methodInfo in this.controllerInfo.Actions.Values)
            {
                PrepareInputParameterTypes(methodInfo.Method);
                
                List<Type> newTypes = new List<Type>();
                var returnType = TypeScriptHelper.GetTypeContractName(methodInfo.Method.ReturnType, newTypes);
                this.AddNewTypesIfRequired(newTypes.ToArray());
                if (returnType == "void")
                    script.AppendLine(string.Format("            {0} ({1}): studiox.IPromise; ", methodInfo.ActionName.ToCamelCase(), GetMethodInputParameter(methodInfo.Method)));
                else
                    script.AppendLine(string.Format("            {0} ({1}): studiox.IGenericPromise<{2}>; ", methodInfo.ActionName.ToCamelCase(), GetMethodInputParameter(methodInfo.Method), returnType));

            }

            script.AppendLine("     }");
            while (typesToBeDone != null && typesToBeDone.Count > 0)
            {
                Type type = typesToBeDone.First();

                script.AppendLine(GenerateTypeScript(type));
                doneTypes.Add(type);
                typesToBeDone.RemoveWhere(x => x == type);
            }
            return script.ToString();
        }

        private void AddNewTypesIfRequired(params Type[] newTypes)
        {
            foreach (var type in newTypes)
                if (this.CanAddToBeDone(type))
                    typesToBeDone.Add(type);
        }

        protected string GetMethodInputParameter(MethodInfo methodInfo)
        {
            var script = new StringBuilder();
            List<Type> newTypes = new List<Type>();
            foreach (var parameter in methodInfo.GetParameters())
            {
                script.Append(string.Format("{0} : {1},", parameter.Name.ToCamelCase(), TypeScriptHelper.GetTypeContractName(parameter.ParameterType,newTypes)));
            }
            this.AddNewTypesIfRequired(newTypes.ToArray());
            script.Append("httpParams?: any");
            return script.ToString();

        }
 
        protected void PrepareInputParameterTypes(MethodInfo methodInfo)
        {
            foreach (var parameter in methodInfo.GetParameters())
            {
                AddNewTypesIfRequired(parameter.ParameterType);
            }
        }

        protected string GenerateTypeScript(Type type)
        {
            if (type.IsArray ||
                (type.IsGenericType && (typeof(List<>).IsAssignableFrom(type.GetGenericTypeDefinition()) ||
                typeof(ICollection<>).IsAssignableFrom(type.GetGenericTypeDefinition()) ||
                typeof(IEnumerable<>).IsAssignableFrom(type.GetGenericTypeDefinition())
                ))
                )
            {
                if (type.GetElementType() != null)
                {
                    this.AddNewTypesIfRequired(type.GetElementType());
                }
                return "";
            }

            if (type.IsGenericType && typeof(Nullable<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
            {
                return "";
            }



            var myscript = new StringBuilder();
            List<Type> newTypes = new List<Type>();
            myscript.AppendLine("     interface " + TypeScriptHelper.GetTypeContractName(type,newTypes));
            myscript.AppendLine("         {");
            foreach (var property in type.GetProperties())
            {
                myscript.AppendLine(string.Format("{0} : {1} ;", property.Name.ToCamelCase(),TypeScriptHelper.GetTypeContractName(property.PropertyType,newTypes)));
            }
            this.AddNewTypesIfRequired(newTypes.ToArray());
            myscript.AppendLine("         }");
            return myscript.ToString();
        }
        private bool CanAddToBeDone(Type type)
        {
            if (type == typeof(Task))
                return false;
            if (typesToBeDone.Count(z => z.FullName == type.FullName) == 0 && !TypeScriptHelper.IsIgnorantType(type) && !TypeScriptHelper.IsBasicType(type) && doneTypes.Count(z => z.FullName == type.FullName) == 0)
                return true;
            return false;
        }
    }
}
