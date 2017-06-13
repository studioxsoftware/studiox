using System.Threading.Tasks;
using StudioX.Application.Features;
using StudioX.Authorization;
using StudioX.Configuration.Startup;
using StudioX.Dependency;
using StudioX.Runtime.Session;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Authorization
{
    public class AuthorizationInterceptorTests : TestBaseWithLocalIocManager
    {
        private readonly MyTestClassToBeAuthorizedSync syncObj;
        private readonly MyTestClassToBeAuthorizedAsync asyncObj;

        public AuthorizationInterceptorTests()
        {
            //SUT: AuthorizationInterceptor and AuthorizeAttributeHelper
            LocalIocManager.IocContainer.Register(
                Component.For<IFeatureChecker>().Instance(Substitute.For<IFeatureChecker>())
                );

            LocalIocManager.Register<IAuthorizationConfiguration, AuthorizationConfiguration>();
            LocalIocManager.Register<AuthorizationInterceptor>(DependencyLifeStyle.Transient);
            LocalIocManager.Register<IAuthorizationHelper, AuthorizationHelper>(DependencyLifeStyle.Transient);
            LocalIocManager.IocContainer.Register(
                Component.For<MyTestClassToBeAuthorizedSync>().Interceptors<AuthorizationInterceptor>().LifestyleTransient(),
                Component.For<MyTestClassToBeAuthorizedAsync>().Interceptors<AuthorizationInterceptor>().LifestyleTransient()
                );

            //Mock session
            var session = Substitute.For<IStudioXSession>();
            session.TenantId.Returns(1);
            session.UserId.Returns(1);
            LocalIocManager.IocContainer.Register(Component.For<IStudioXSession>().Instance(session));

            //Mock permission checker
            var permissionChecker = Substitute.For<IPermissionChecker>();
            permissionChecker.IsGrantedAsync("Permission1").Returns(true);
            permissionChecker.IsGrantedAsync("Permission2").Returns(true);
            permissionChecker.IsGrantedAsync("Permission3").Returns(false); //Permission3 is not granted
            LocalIocManager.IocContainer.Register(Component.For<IPermissionChecker>().Instance(permissionChecker));

            syncObj = LocalIocManager.Resolve<MyTestClassToBeAuthorizedSync>();
            asyncObj = LocalIocManager.Resolve<MyTestClassToBeAuthorizedAsync>();
        }

        [Fact]
        public void TestAuthorizationSync()
        {
            //Authorized methods
            
            syncObj.MethodWithoutPermission();
            syncObj.CalledMethodWithoutPermission.ShouldBe(true);

            syncObj.MethodWithPermission1().ShouldBe(42);
            syncObj.CalledMethodWithPermission1.ShouldBe(true);

            syncObj.MethodWithPermission1AndPermission2();
            syncObj.CalledMethodWithPermission1AndPermission2.ShouldBe(true);

            syncObj.MethodWithPermission1AndPermission3();
            syncObj.CalledMethodWithPermission1AndPermission3.ShouldBe(true);
            
            //Non authorized methods

            Assert.Throws<StudioXAuthorizationException>(() => syncObj.MethodWithPermission3());
            syncObj.CalledMethodWithPermission3.ShouldBe(false);

            Assert.Throws<StudioXAuthorizationException>(() => syncObj.MethodWithPermission1AndPermission3WithRequireAll());
            syncObj.CalledMethodWithPermission1AndPermission3WithRequireAll.ShouldBe(false);
        }

        [Fact]
        public async Task TestAuthorizationAsync()
        {
            //Authorized methods

            await asyncObj.MethodWithoutPermission();
            asyncObj.CalledMethodWithoutPermission.ShouldBe(true);

            (await asyncObj.MethodWithPermission1Async()).ShouldBe(42);
            asyncObj.CalledMethodWithPermission1.ShouldBe(true);

            await asyncObj.MethodWithPermission1AndPermission2Async();
            asyncObj.CalledMethodWithPermission1AndPermission2.ShouldBe(true);

            await asyncObj.MethodWithPermission1AndPermission3Async();
            asyncObj.CalledMethodWithPermission1AndPermission3.ShouldBe(true);

            //Non authorized methods

            await Assert.ThrowsAsync<StudioXAuthorizationException>(async () => await asyncObj.MethodWithPermission3Async());
            asyncObj.CalledMethodWithPermission3.ShouldBe(false);

            await Assert.ThrowsAsync<StudioXAuthorizationException>(async () => await asyncObj.MethodWithPermission1AndPermission3WithRequireAllAsync());
            asyncObj.CalledMethodWithPermission1AndPermission3WithRequireAll.ShouldBe(false);
        }

        public class MyTestClassToBeAuthorizedSync
        {
            public bool CalledMethodWithoutPermission { get; private set; }

            public bool CalledMethodWithPermission1 { get; private set; }

            public bool CalledMethodWithPermission3 { get; private set; }

            public bool CalledMethodWithPermission1AndPermission2 { get; private set; }

            public bool CalledMethodWithPermission1AndPermission3 { get; private set; }

            public bool CalledMethodWithPermission1AndPermission3WithRequireAll { get; private set; }

            public virtual void MethodWithoutPermission()
            {
                CalledMethodWithoutPermission = true;
            }

            [StudioXAuthorize("Permission1")]
            public virtual int MethodWithPermission1()
            {
                CalledMethodWithPermission1 = true;
                return 42;
            }

            //Should not be called since Permission3 is not granted
            [StudioXAuthorize("Permission3")]
            public virtual void MethodWithPermission3()
            {
                CalledMethodWithPermission3 = true;
            }

            //Should be called since both of Permission1 and Permission2 are granted
            [StudioXAuthorize("Permission1", "Permission2")]
            public virtual void MethodWithPermission1AndPermission2()
            {
                CalledMethodWithPermission1AndPermission2 = true;
            }

            //Should be called. Permission3 is not granted but Permission1 is granted.
            [StudioXAuthorize("Permission1", "Permission3")]
            public virtual void MethodWithPermission1AndPermission3()
            {
                CalledMethodWithPermission1AndPermission3 = true;
            }

            //Should not be called. Permission3 is not granted and it required all permissions must be granted
            [StudioXAuthorize("Permission1", "Permission3", RequireAllPermissions = true)]
            public virtual void MethodWithPermission1AndPermission3WithRequireAll()
            {
                CalledMethodWithPermission1AndPermission3WithRequireAll = true;
            }
        }

        public class MyTestClassToBeAuthorizedAsync
        {
            public bool CalledMethodWithoutPermission { get; private set; }
            
            public bool CalledMethodWithPermission1 { get; private set; }
            
            public bool CalledMethodWithPermission3 { get; private set; }
            
            public bool CalledMethodWithPermission1AndPermission2 { get; private set; }
            
            public bool CalledMethodWithPermission1AndPermission3 { get; private set; }
            
            public bool CalledMethodWithPermission1AndPermission3WithRequireAll { get; private set; }

            public virtual async Task MethodWithoutPermission()
            {
                CalledMethodWithoutPermission = true;
                await Task.Delay(1);
            }

            [StudioXAuthorize("Permission1")]
            public virtual async Task<int> MethodWithPermission1Async()
            {
                CalledMethodWithPermission1 = true;
                await Task.Delay(1);
                return 42;
            }

            //Should not be called since Permission3 is not granted
            [StudioXAuthorize("Permission3")]
            public virtual async Task MethodWithPermission3Async()
            {
                CalledMethodWithPermission3 = true;
                await Task.Delay(1);
            }

            //Should be called since both of Permission1 and Permission2 are granted
            [StudioXAuthorize("Permission1", "Permission2")]
            public virtual async Task MethodWithPermission1AndPermission2Async()
            {
                CalledMethodWithPermission1AndPermission2 = true;
                await Task.Delay(1);
            }

            //Should be called. Permission3 is not granted but Permission1 is granted.
            [StudioXAuthorize("Permission1", "Permission3")]
            public virtual async Task MethodWithPermission1AndPermission3Async()
            {
                CalledMethodWithPermission1AndPermission3 = true;
                await Task.Delay(1);
            }

            //Should not be called. Permission3 is not granted and it required all permissions must be granted
            [StudioXAuthorize("Permission1", "Permission3", RequireAllPermissions = true)]
            public virtual async Task MethodWithPermission1AndPermission3WithRequireAllAsync()
            {
                CalledMethodWithPermission1AndPermission3WithRequireAll = true;
                await Task.Delay(1);
            }
        }
    }
}
