using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.TestBase.SampleApplication.People;
using Shouldly;
using Xunit;

namespace StudioX.TestBase.SampleApplication.Tests.Uow
{
    public class UnitOfWorkEventTests : SampleApplicationTestBase
    {
        private readonly IRepository<Person> personRepository;
        private readonly IUnitOfWorkManager unitOfWorkManager;

        public UnitOfWorkEventTests()
        {
            personRepository = Resolve<IRepository<Person>>();
            unitOfWorkManager = Resolve<IUnitOfWorkManager>();
        }

        [Fact]
        public void ShouldTriggerCompletedWhenUowSucceed()
        {
            var completeCount = 0;
            var disposeCount = 0;

            using (var uow = unitOfWorkManager.Begin())
            {
                personRepository.Insert(new Person {ContactListId = 1, Name = "john"});

                unitOfWorkManager.Current.Completed += (sender, args) =>
                {
                    unitOfWorkManager.Current.ShouldBe(null);
                    completeCount++;
                };

                unitOfWorkManager.Current.Disposed += (sender, args) =>
                {
                    unitOfWorkManager.Current.ShouldBe(null);
                    completeCount.ShouldBe(1);
                    disposeCount++;
                };

                uow.Complete();
            }

            UsingDbContext(context => context.People.Any(p => p.Name == "john").ShouldBe(true));

            completeCount.ShouldBe(1);
            disposeCount.ShouldBe(1);
        }

        [Fact]
        public void ShouldTriggerFailedWhenThrowExceptionInUow()
        {
            var failedCount = 0;
            var disposeCount = 0;

            Assert.Throws<ApplicationException>(
                new Action(() =>
                {
                    using (var uow = unitOfWorkManager.Begin())
                    {
                        personRepository.Insert(new Person()); //Name is intentionally not set to cause exception

                        unitOfWorkManager.Current.ShouldNotBe(null);

                        unitOfWorkManager.Current.Failed += (sender, args) =>
                        {
                            unitOfWorkManager.Current.ShouldBe(null);
                            args.Exception.ShouldBe(null); //Can not set it!
                            failedCount++;
                        };

                        unitOfWorkManager.Current.Disposed += (sender, args) =>
                        {
                            unitOfWorkManager.Current.ShouldBe(null);
                            failedCount.ShouldBe(1);
                            disposeCount++;
                        };

                        throw new ApplicationException("This is throwed to make uow failed");
                    }
                }));

            failedCount.ShouldBe(1);
            disposeCount.ShouldBe(1);
        }

        [Fact]
        public void ShouldTriggerFailedWhenSaveChangesFails()
        {
            var failedCount = 0;
            var disposeCount = 0;

            using (var uow = unitOfWorkManager.Begin())
            {
                personRepository.Insert(new Person()); //Name is intentionally not set to cause exception

                unitOfWorkManager.Current.Failed += (sender, args) =>
                {
                    unitOfWorkManager.Current.ShouldBe(null);
                    args.Exception.ShouldNotBe(null);
                    args.Exception.ShouldBeOfType(typeof(DbEntityValidationException));
                    failedCount++;
                };

                unitOfWorkManager.Current.Disposed += (sender, args) =>
                {
                    unitOfWorkManager.Current.ShouldBe(null);
                    failedCount.ShouldBe(1);
                    disposeCount++;
                };

                Assert.Throws<DbEntityValidationException>(() => uow.Complete());
            }

            failedCount.ShouldBe(1);
            disposeCount.ShouldBe(1);
        }

        [Fact]
        public async Task ShouldReturnBackToOuterUowOnNestedUows()
        {
            using (var uow = unitOfWorkManager.Begin())
            {
                var outerUow = unitOfWorkManager.Current;

                outerUow.Completed += (sender, args) => { unitOfWorkManager.Current.ShouldBe(null); };

                using (var uowInner = unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
                {
                    var innerUow = unitOfWorkManager.Current;

                    innerUow.Completed += (sender, args) => { unitOfWorkManager.Current.ShouldBe(outerUow); };

                    await uowInner.CompleteAsync();
                }

                await uow.CompleteAsync();
            }
        }
    }
}