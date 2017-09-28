using System;
using System.Reflection;

namespace StudioX.Aspects
{
    //THIS NAMESPACE IS WORK-IN-PROGRESS

    internal abstract class AspectAttribute : Attribute
    {
        public Type InterceptorType { get; set; }

        protected AspectAttribute(Type interceptorType)
        {
            InterceptorType = interceptorType;
        }
    }

    internal interface IStudioXInterceptionContext
    {
        object Target { get; }

        MethodInfo Method { get; }

        object[] Arguments { get; }

        object ReturnValue { get; }

        bool Handled { get; set; }
    }

    internal interface IStudioXBeforeExecutionInterceptionContext : IStudioXInterceptionContext
    {

    }


    internal interface IStudioXAfterExecutionInterceptionContext : IStudioXInterceptionContext
    {
        Exception Exception { get; }
    }

    internal interface IStudioXInterceptor<TAspect>
    {
        TAspect Aspect { get; set; }

        void BeforeExecution(IStudioXBeforeExecutionInterceptionContext context);

        void AfterExecution(IStudioXAfterExecutionInterceptionContext context);
    }

    internal abstract class StudioXInterceptorBase<TAspect> : IStudioXInterceptor<TAspect>
    {
        public TAspect Aspect { get; set; }

        public virtual void BeforeExecution(IStudioXBeforeExecutionInterceptionContext context)
        {
        }

        public virtual void AfterExecution(IStudioXAfterExecutionInterceptionContext context)
        {
        }
    }
}
