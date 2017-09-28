using System;
using System.Collections.Generic;
using System.Linq;

namespace StudioX.Dependency
{
    public class ScopedIocResolver : IScopedIocResolver
    {
        private readonly IIocResolver iocResolver;
        private readonly List<object> resolvedObjects;

        public ScopedIocResolver(IIocResolver iocResolver)
        {
            this.iocResolver = iocResolver;
            resolvedObjects = new List<object>();
        }

        public T Resolve<T>()
        {
            return Resolve<T>(typeof(T));
        }

        public T Resolve<T>(Type type)
        {
            return (T)Resolve(type);
        }

        public T Resolve<T>(object argumentsAsAnonymousType)
        {
            return (T)Resolve(typeof(T), argumentsAsAnonymousType);
        }

        public object Resolve(Type type)
        {
            return Resolve(type, null);
        }

        public object Resolve(Type type, object argumentsAsAnonymousType)
        {
            var resolvedObject = argumentsAsAnonymousType != null
                ? iocResolver.Resolve(type, argumentsAsAnonymousType)
                : iocResolver.Resolve(type);

            resolvedObjects.Add(resolvedObject);
            return resolvedObject;
        }

        public T[] ResolveAll<T>()
        {
            return ResolveAll(typeof(T)).OfType<T>().ToArray();
        }

        public T[] ResolveAll<T>(object argumentsAsAnonymousType)
        {
            return ResolveAll(typeof(T), argumentsAsAnonymousType).OfType<T>().ToArray();
        }

        public object[] ResolveAll(Type type)
        {
            return ResolveAll(type, null);
        }

        public object[] ResolveAll(Type type, object argumentsAsAnonymousType)
        {
            var resolvedObjects = argumentsAsAnonymousType != null
                ? iocResolver.ResolveAll(type, argumentsAsAnonymousType)
                : iocResolver.ResolveAll(type);

            this.resolvedObjects.AddRange(resolvedObjects);
            return resolvedObjects;
        }

        public void Release(object obj)
        {
            resolvedObjects.Remove(obj);
            iocResolver.Release(obj);
        }

        public bool IsRegistered(Type type)
        {
            return iocResolver.IsRegistered(type);
        }

        public bool IsRegistered<T>()
        {
            return IsRegistered(typeof(T));
        }

        public void Dispose()
        {
            resolvedObjects.ForEach(iocResolver.Release);
        }
    }
}
