using StudioX.Domain.Entities;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.EntityFramework.Repositories;
using StudioX.MultiTenancy;
using StudioX.Tests;
using NSubstitute;
using Shouldly;
using Xunit;

namespace StudioX.EntityFramework.Tests
{
    public class DbContextTypeMatcherTests : TestBaseWithLocalIocManager
    {
        private int? _tenantId = 1;

        private readonly DbContextTypeMatcher _matcher;

        public DbContextTypeMatcherTests()
        {
            var fakeUow = Substitute.For<IUnitOfWork>();
            fakeUow.GetTenantId().Returns(callInfo => _tenantId);
            var fakeCurrentUowProvider = Substitute.For<ICurrentUnitOfWorkProvider>();
            fakeCurrentUowProvider.Current.Returns(fakeUow);

            _matcher = new DbContextTypeMatcher(fakeCurrentUowProvider);
            _matcher.Populate(new []
            {
                typeof(MyDerivedDbContext1),
                typeof(MyDerivedDbContext2),
                typeof(MyDerivedDbContext3)
            });
        }

        [Fact]
        public void ShouldGetSameTypesForDefinedNonAbstractTypes()
        {
            _matcher.GetConcreteType(typeof(MyDerivedDbContext1)).ShouldBe(typeof(MyDerivedDbContext1));
            _matcher.GetConcreteType(typeof(MyDerivedDbContext2)).ShouldBe(typeof(MyDerivedDbContext2));
            _matcher.GetConcreteType(typeof(MyDerivedDbContext3)).ShouldBe(typeof(MyDerivedDbContext3));
        }

        [Fact]
        public void ShouldGetSameTypesForUndefinedNonAbstractTypes()
        {
            _matcher.GetConcreteType(typeof(MyDerivedDbContextNotDefined)).ShouldBe(typeof(MyDerivedDbContextNotDefined));
        }

        [Fact]
        public void ShouldGetSingleDbContextForCurrentTenancySideWhenBaseDbContextRequested()
        {
            //Should return MyDerivedDbContext3 since it defines MultiTenancySides.Tenant
            _matcher.GetConcreteType(typeof(MyCommonDbContext)).ShouldBe(typeof(MyDerivedDbContext3));
        }

        [Fact]
        public void ShouldThrowExceptionIfMultipleDbContextForCurrentTenancySideWhenBaseDbContextRequested()
        {
            _tenantId = null; //switching to host side (which have more than 1 dbcontext)
            _matcher.GetConcreteType(typeof(MyCommonDbContext)).ShouldBe(typeof(MyDerivedDbContext1));
        }

        private abstract class MyCommonDbContext : StudioXDbContext
        {

        }

        [MultiTenancySide(MultiTenancySides.Host)]
        private class MyDerivedDbContext1 : MyCommonDbContext
        {

        }

        [AutoRepositoryTypes( //Does not matter parameters for these tests
            typeof(IRepository<>), 
            typeof(IRepository<,>), 
            typeof(EfRepositoryBase<,>), 
            typeof(EfRepositoryBase<,,>)
            )]
        [MultiTenancySide(MultiTenancySides.Host)]
        private class MyDerivedDbContext2 : MyCommonDbContext
        {

        }

        [MultiTenancySide(MultiTenancySides.Tenant)]
        private class MyDerivedDbContext3 : MyCommonDbContext
        {

        }

        private class MyDerivedDbContextNotDefined : MyCommonDbContext
        {

        }

        private class MyCommonEntity : Entity
        {

        }
    }
}
