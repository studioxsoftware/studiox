using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using StudioX.Dependency;
using StudioX.Extensions;
using StudioX.Web;

namespace StudioX.WebApi.Controllers.Dynamic.Scripting.TypeScript
{
    internal class TypeScriptServiceProxyGenerator : ITransientDependency
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

            script.AppendLine("     export class " + this.controllerInfo.ServiceName.Substring(this.controllerInfo.ServiceName.IndexOf('/') + 1));
            script.AppendLine("     {");
            script.AppendLine("         static $inject = ['$http'];");
            script.AppendLine("         constructor(private $http: ng.IHttpService){");
            script.AppendLine("     }");
            foreach (var methodInfo in this.controllerInfo.Actions.Values)
            {
                PrepareInputParameterTypes(methodInfo.Method);
                List<Type> newTypes = new List<Type>();
                var returnType = TypeScriptHelper.GetTypeContractName(methodInfo.Method.ReturnType, newTypes);
                this.AddNewTypesIfRequired(newTypes.ToArray());
                if (returnType == "void")
                {
                    script.AppendLine(string.Format("           public {0} = function ({1}): studiox.IPromise ",
                        methodInfo.ActionName.ToCamelCase(), GetMethodInputParameter(methodInfo.Method)));
                    script.AppendLine("{");
                    script.AppendLine("                    var self = this;");
                    script.AppendLine("                    return self.$http(angular.extend({");
                    script.AppendLine("                        studiox: true,");
                    script.AppendLine("                        url: studiox.appPath + '" + ActionScriptingHelper.GenerateUrlWithParameters(this.controllerInfo, methodInfo) + "',");
                    script.AppendLine("                        method: '" + methodInfo.Verb.ToString().ToUpper(CultureInfo.InvariantCulture) + "',");

                    if (methodInfo.Verb == HttpVerb.Get)
                    {
                        script.AppendLine("                        params: " + ActionScriptingHelper.GenerateBody(methodInfo));
                    }
                    else
                    {
                        script.AppendLine("                        data: JSON.stringify(" + ActionScriptingHelper.GenerateBody(methodInfo) + ")");
                    }

                    script.AppendLine("                    }, httpParams));");

                    script.AppendLine("}");
                }

                else
                {
                    script.AppendLine(string.Format("           public {0} = function ({1}): studiox.IGenericPromise<{2}> ", methodInfo.ActionName.ToCamelCase(), 
                        GetMethodInputParameter(methodInfo.Method), returnType));
                    script.AppendLine("{");
                    script.AppendLine("                    var self = this;");
                    script.AppendLine("                    return self.$http(angular.extend({");
                    script.AppendLine("                        studiox: true,");
                    script.AppendLine("                        url: studiox.appPath + '" + ActionScriptingHelper.GenerateUrlWithParameters(this.controllerInfo, methodInfo) + "',");
                    script.AppendLine("                        method: '" + methodInfo.Verb.ToString().ToUpper(CultureInfo.InvariantCulture) + "',");

                    if (methodInfo.Verb == HttpVerb.Get)
                    {
                        script.AppendLine("                        params: " + ActionScriptingHelper.GenerateBody(methodInfo));
                    }
                    else
                    {
                        script.AppendLine("                        data: JSON.stringify(" + ActionScriptingHelper.GenerateBody(methodInfo) + ")");
                    }

                    script.AppendLine("                    }, httpParams));");

                    script.AppendLine("}");
                }
            }
            
            script.AppendLine("     }");

            script.AppendLine("angular.module('studiox').service('studiox.services." + this.controllerInfo.ServiceName.Replace("/", ".") + "', studiox.services." + this.controllerInfo.ServiceName.Replace("/", ".")+");");

            while (typesToBeDone != null && typesToBeDone.Count > 0)
            {
                Type type = typesToBeDone.First();

                script.AppendLine(GenerateTypeScript(type));
                doneTypes.Add(type);
                typesToBeDone.RemoveWhere(x => x == type);
            }
            return script.ToString();
        }
        protected string GetMethodInputParameter(MethodInfo methodInfo)
        {
            var script = new StringBuilder();
            
            List<Type> newTypes = new List<Type>();
            foreach (var parameter in methodInfo.GetParameters())
            {
                script.Append(string.Format("{0} : {1},", parameter.Name.ToCamelCase(), TypeScriptHelper.GetTypeContractName(parameter.ParameterType, newTypes)));
            }
            script.Append("httpParams?: any");
            this.AddNewTypesIfRequired(newTypes.ToArray());
            return script.ToString();

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
            myscript.AppendLine("     export class " + TypeScriptHelper.GetTypeContractName(type, newTypes));
            myscript.AppendLine("         {");
            foreach (var property in type.GetProperties())
            {
                myscript.AppendLine(string.Format("{0} : {1} ;", property.Name.ToCamelCase(), TypeScriptHelper.GetTypeContractName(property.PropertyType, newTypes)));
            }
            this.AddNewTypesIfRequired(newTypes.ToArray());
            myscript.AppendLine("         }");
            return myscript.ToString();
        }

        private void AddNewTypesIfRequired(params Type[] newTypes)
        {
            foreach (var type in newTypes)
                if (this.CanAddToBeDone(type))
                    typesToBeDone.Add(type);
        }

        protected void PrepareInputParameterTypes(MethodInfo methodInfo)
        {
            foreach (var parameter in methodInfo.GetParameters())
            {
                AddNewTypesIfRequired(parameter.ParameterType);
            }
        }

        protected void PrepareOutputParameterTypes(MethodInfo methodInfo)
        {
            if (this.CanAddToBeDone(methodInfo.ReturnType))
            {
                typesToBeDone.Add(methodInfo.ReturnType);
            }
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
