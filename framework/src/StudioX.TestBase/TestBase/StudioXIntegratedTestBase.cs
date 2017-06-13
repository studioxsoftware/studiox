using System;
using System.Reflection;
using StudioX.Dependency;
using StudioX.Modules;
using StudioX.Runtime.Session;
using StudioX.TestBase.Runtime.Session;

namespace StudioX.TestBase
{
    /// <summary>
    /// This is the base class for all tests integrated to StudioX.
    /// </summary>
    public abstract class StudioXIntegratedTestBase<TStartupModule> : IDisposable 
        where TStartupModule : StudioXModule
    {
        /// <summary>
        /// Local <see cref="IIocManager"/> used for this test.
        /// </summary>
        protected IIocManager LocalIocManager { get; }

        protected StudioXBootstrapper StudioXBootstrapper { get; }

        /// <summary>
        /// Gets Session object. Can be used to change current user and tenant in tests.
        /// </summary>
        protected TestStudioXSession StudioXSession { get; private set; }

        protected StudioXIntegratedTestBase(bool initializeStudioX = true)
        {
            LocalIocManager = new IocManager();
            StudioXBootstrapper = StudioXBootstrapper.Create<TStartupModule>(LocalIocManager);

            if (initializeStudioX)
            {
                InitializeStudioX();
            }
        }

        protected void InitializeStudioX()
        {
            LocalIocManager.RegisterIfNot<IStudioXSession, TestStudioXSession>();

            PreInitialize();

            StudioXBootstrapper.Initialize();

            PostInitialize();

            StudioXSession = LocalIocManager.Resolve<TestStudioXSession>();
        }

        /// <summary>
        /// This method can be overrided to replace some services with fakes.
        /// </summary>
        protected virtual void PreInitialize()
        {

        }

        /// <summary>
        /// This method can be overrided to replace some services with fakes.
        /// </summary>
        protected virtual void PostInitialize()
        {

        }

        public virtual void Dispose()
        {
            StudioXBootstrapper.Dispose();
            LocalIocManager.Dispose();
        }

        /// <summary>
        /// A shortcut to resolve an object from <see cref="LocalIocManager"/>.
        /// Also registers <typeparamref name="T"/> as transient if it's not registered before.
        /// </summary>
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <returns>The object instance</returns>
        protected T Resolve<T>()
        {
            EnsureClassRegistered(typeof(T));
            return LocalIocManager.Resolve<T>();
        }

        /// <summary>
        /// A shortcut to resolve an object from <see cref="LocalIocManager"/>.
        /// Also registers <typeparamref name="T"/> as transient if it's not registered before.
        /// </summary>
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The object instance</returns>
        protected T Resolve<T>(object argumentsAsAnonymousType)
        {
            EnsureClassRegistered(typeof(T));
            return LocalIocManager.Resolve<T>(argumentsAsAnonymousType);
        }

        /// <summary>
        /// A shortcut to resolve an object from <see cref="LocalIocManager"/>.
        /// Also registers <paramref name="type"/> as transient if it's not registered before.
        /// </summary>
        /// <param name="type">Type of the object to get</param>
        /// <returns>The object instance</returns>
        protected object Resolve(Type type)
        {
            EnsureClassRegistered(type);
            return LocalIocManager.Resolve(type);
        }

        /// <summary>
        /// A shortcut to resolve an object from <see cref="LocalIocManager"/>.
        /// Also registers <paramref name="type"/> as transient if it's not registered before.
        /// </summary>
        /// <param name="type">Type of the object to get</param>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The object instance</returns>
        protected object Resolve(Type type, object argumentsAsAnonymousType)
        {
            EnsureClassRegistered(type);
            return LocalIocManager.Resolve(type, argumentsAsAnonymousType);
        }

        /// <summary>
        /// Registers given type if it's not registered before.
        /// </summary>
        /// <param name="type">Type to check and register</param>
        /// <param name="lifeStyle">Lifestyle</param>
        protected void EnsureClassRegistered(Type type, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Transient)
        {
            if (!LocalIocManager.IsRegistered(type))
            {
                if (!type.GetTypeInfo().IsClass || type.GetTypeInfo().IsAbstract)
                {
                    throw new StudioXException("Can not register " + type.Name + ". It should be a non-abstract class. If not, it should be registered before.");
                }

                LocalIocManager.Register(type, lifeStyle);
            }
        }
    }
}
