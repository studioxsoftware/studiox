namespace StudioX.Dependency
{
    internal class DisposableDependencyObjectWrapper : DisposableDependencyObjectWrapper<object>,
        IDisposableDependencyObjectWrapper
    {
        public DisposableDependencyObjectWrapper(IIocResolver iocResolver, object obj)
            : base(iocResolver, obj)
        {
        }
    }

    internal class DisposableDependencyObjectWrapper<T> : IDisposableDependencyObjectWrapper<T>
    {
        private readonly IIocResolver iocResolver;

        public T Object { get; }

        public DisposableDependencyObjectWrapper(IIocResolver iocResolver, T obj)
        {
            this.iocResolver = iocResolver;
            Object = obj;
        }

        public void Dispose()
        {
            iocResolver.Release(Object);
        }
    }
}