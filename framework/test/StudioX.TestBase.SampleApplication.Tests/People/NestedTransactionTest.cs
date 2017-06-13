using System;
using System.Linq;
using System.Transactions;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.TestBase.SampleApplication.People;
using Shouldly;
using Xunit;

namespace StudioX.TestBase.SampleApplication.Tests.People
{
    public class NestedTransactionTest : SampleApplicationTestBase
    {
        private readonly IRepository<Person> personRepository;

        public NestedTransactionTest()
        {
            personRepository = Resolve<IRepository<Person>>();
        }

        [Fact]
        public void ShouldSuppressOuterTransaction()
        {
            var outerUowPersonName = Guid.NewGuid().ToString("N");
            var innerUowPersonName = Guid.NewGuid().ToString("N");

            var unitOfWorkManager = Resolve<IUnitOfWorkManager>();

            Assert.Throws<ApplicationException>(() =>
            {
                using (var uow = unitOfWorkManager.Begin())
                {
                    var contactList = UsingDbContext(context => context.ContactLists.First());

                    personRepository.Insert(new Person
                    {
                        Name = outerUowPersonName,
                        ContactListId = contactList.Id
                    });

                    using (var innerUow = unitOfWorkManager.Begin(TransactionScopeOption.Suppress))
                    {
                        personRepository.Insert(new Person
                        {
                            Name = innerUowPersonName,
                            ContactListId = contactList.Id
                        });

                        innerUow.Complete();
                    }

                    throw new ApplicationException("This exception is thown to rollback outer transaction!");
                }

                return;
            }).Message.ShouldBe("This exception is thown to rollback outer transaction!");

            UsingDbContext(context =>
            {
                context.People.FirstOrDefault(n => n.Name == outerUowPersonName).ShouldBeNull();
                context.People.FirstOrDefault(n => n.Name == innerUowPersonName).ShouldNotBeNull();
            });
        }
    }
}
