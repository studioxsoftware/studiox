using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using StudioX.Dependency;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.Events.Bus;
using StudioX.Events.Bus.Entities;
using StudioX.Events.Bus.Handlers;
using StudioX.TestBase.SampleApplication.Messages;
using StudioX.TestBase.SampleApplication.People;
using Shouldly;
using Xunit;

namespace StudioX.TestBase.SampleApplication.Tests.People
{
    public class PersonRepositoryTestsForEntityChangeEvents : SampleApplicationTestBase
    {
        private readonly IRepository<Person> personRepository;

        public PersonRepositoryTestsForEntityChangeEvents()
        {
            personRepository = Resolve<IRepository<Person>>();
        }
        
        [Theory]
        [InlineData(TransactionScopeOption.Required)]
        [InlineData(TransactionScopeOption.RequiresNew)]
        [InlineData(TransactionScopeOption.Suppress)]
        public void ShouldTriggerAllEventsOnCreateForAllTransactionScopes(TransactionScopeOption scopeOption)
        {
            var changingTriggerCount = 0;
            var creatingTriggerCount = 0;

            var changedTriggerCount = 0;
            var createdTriggerCount = 0;

            Resolve<IEventBus>().Register<EntityChangingEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("Long");
                    eventData.Entity.IsTransient().ShouldBe(false);
                    changingTriggerCount++;
                    changedTriggerCount.ShouldBe(0);
                });

            Resolve<IEventBus>().Register<EntityCreatingEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("Long");
                    eventData.Entity.IsTransient().ShouldBe(false);
                    creatingTriggerCount++;
                    createdTriggerCount.ShouldBe(0);
                });

            Resolve<IEventBus>().Register<EntityChangedEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("Long");
                    eventData.Entity.IsTransient().ShouldBe(false);
                    changingTriggerCount.ShouldBe(1);
                    changedTriggerCount++;
                });

            Resolve<IEventBus>().Register<EntityCreatedEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("Long");
                    eventData.Entity.IsTransient().ShouldBe(false);
                    creatingTriggerCount.ShouldBe(1);
                    createdTriggerCount++;
                });

            using (var uow = Resolve<IUnitOfWorkManager>().Begin(scopeOption))
            {
                personRepository.Insert(new Person { ContactListId = 1, Name = "Long" });

                changingTriggerCount.ShouldBe(0);
                creatingTriggerCount.ShouldBe(0);

                changedTriggerCount.ShouldBe(0);
                createdTriggerCount.ShouldBe(0);

                uow.Complete();
            }

            changingTriggerCount.ShouldBe(1);
            creatingTriggerCount.ShouldBe(1);

            changedTriggerCount.ShouldBe(1);
            createdTriggerCount.ShouldBe(1);
        }

        [Fact]
        public void ShouldTriggerEventOnUpdate()
        {
            var triggerCount = 0;

            Resolve<IEventBus>().Register<EntityUpdatedEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("Long2");
                    triggerCount++;
                });

            var person = personRepository.Single(p => p.Name == "Long");
            person.Name = "Long2";
            personRepository.Update(person);

            triggerCount.ShouldBe(1);
        }

        [Fact]
        public void ShouldTriggerEventOnDelete()
        {
            var triggerCount = 0;

            Resolve<IEventBus>().Register<EntityDeletedEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("Long");
                    eventData.Entity.CreatorUserId.ShouldNotBeNull();
                    eventData.Entity.CreatorUserId.ShouldBe(42);
                    triggerCount++;
                });

            var person = personRepository.Single(p => p.Name == "Long");
            personRepository.Delete(person.Id);

            triggerCount.ShouldBe(1);
        }

        [Fact]
        public void ShouldRollbackUOWInUpdatingEvent()
        {
            //Arrange
            var updatingTriggerCount = 0;
            var updatedTriggerCount = 0;

            Resolve<IEventBus>().Register<EntityUpdatingEventData<Person>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("Long2");
                    updatingTriggerCount++;

                    throw new ApplicationException("A sample exception to rollback the UOW.");
                });

            Resolve<IEventBus>().Register<EntityUpdatedEventData<Person>>(
                eventData =>
                {
                    //Should not come here, since UOW is failed
                    updatedTriggerCount++;
                });

            //Act
            try
            {
                using (var uow = Resolve<IUnitOfWorkManager>().Begin())
                {
                    var person = personRepository.Single(p => p.Name == "Long");
                    person.Name = "Long2";
                    personRepository.Update(person);

                    uow.Complete();
                }

                Assert.True(false, "Should not come here since ApplicationException is thrown!");
            }
            catch (ApplicationException)
            {
                //hiding exception
            }

            //Assert
            updatingTriggerCount.ShouldBe(1);
            updatedTriggerCount.ShouldBe(0);

            personRepository
                .FirstOrDefault(p => p.Name == "Long")
                .ShouldNotBeNull(); //should not be changed since we throw exception to rollback the transaction
        }

        [Fact]
        public void ShouldRollbackUOWInDeletingEvent()
        {
            Resolve<IEventBus>().Register<EntityDeletingEventData<Person>>(
                eventData =>
                {
                    throw new ApplicationException("A sample exception to rollback the UOW.");
                });

            //Act
            try
            {
                using (var uow = Resolve<IUnitOfWorkManager>().Begin())
                {
                    var person = personRepository.Single(p => p.Name == "Long");
                    personRepository.Delete(person);
                    uow.Complete();
                }

                Assert.True(false, "Should not come here since ApplicationException is thrown!");
            }
            catch (ApplicationException)
            {
                //hiding exception
            }
            
            personRepository
                .FirstOrDefault(p => p.Name == "Long")
                .ShouldNotBeNull();
        }

        [Fact]
        public async Task ShouldInsertANewEntityOnEntityCreatingEvent()
        {
            var person = await personRepository.InsertAsync(new Person { Name = "Tuana", ContactListId = 1 });
            person.IsTransient().ShouldBeFalse();

            var text = string.Format("{0} is being created with Id = {1}!", person.Name, person.Id);
            UsingDbContext(context =>
            {
                var message = context.Messages.FirstOrDefault(m => m.Text == text && m.TenantId == PersonHandler.FakeTenantId);
                message.ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task ShouldInsertANewEntityOnEntityCreatedEvent()
        {
            var person = await personRepository.InsertAsync(new Person { Name = "Tuana", ContactListId = 1 });
            person.IsTransient().ShouldBeFalse();

            var text = string.Format("{0} is created with Id = {1}!", person.Name, person.Id);
            UsingDbContext(context =>
            {
                var message = context.Messages.FirstOrDefault(m => m.Text == text && m.TenantId == PersonHandler.FakeTenantId);
                message.ShouldNotBeNull();
            });
        }

        public class PersonHandler : IEventHandler<EntityCreatingEventData<Person>>, IEventHandler<EntityCreatedEventData<Person>>, ITransientDependency
        {
            public const int FakeTenantId = 65910381;

            private readonly IRepository<Message> messageRepository;

            public PersonHandler(IRepository<Message> messageRepository)
            {
                this.messageRepository = messageRepository;
            }

            public void HandleEvent(EntityCreatingEventData<Person> eventData)
            {
                messageRepository.Insert(new Message(FakeTenantId, string.Format("{0} is being created with Id = {1}!", eventData.Entity.Name, eventData.Entity.Id)));
            }

            public void HandleEvent(EntityCreatedEventData<Person> eventData)
            {
                messageRepository.Insert(new Message(FakeTenantId, string.Format("{0} is created with Id = {1}!", eventData.Entity.Name, eventData.Entity.Id)));
            }
        }
    }
}
