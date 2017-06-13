using System;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.TestBase.SampleApplication.GuidEntities;
using Shouldly;
using Xunit;

namespace StudioX.TestBase.SampleApplication.Tests.GuidEntities
{
    public class GuidEntityTests : SampleApplicationTestBase
    {
        private readonly IUnitOfWorkManager unitOfWorkManager;
        private readonly IRepository<TestEntityWithGuidPk, Guid> testEntityWithGuidPkRepository;
        private readonly IRepository<TestEntityWithGuidPkAndDbGeneratedValue, Guid> testEntityWithGuidPkAndDbGeneratedValueRepository;

        public GuidEntityTests()
        {
            unitOfWorkManager = Resolve<IUnitOfWorkManager>();
            testEntityWithGuidPkRepository = Resolve<IRepository<TestEntityWithGuidPk, Guid>>();
            testEntityWithGuidPkAndDbGeneratedValueRepository = Resolve<IRepository<TestEntityWithGuidPkAndDbGeneratedValue, Guid>>();
        }

        [Fact]
        public void ShouldSetIdOnInsertForNonDbGenerated()
        {
            //Arrange
            var entity = new TestEntityWithGuidPk();
            Guid assignedId;

            //Act
            using (var uow = unitOfWorkManager.Begin())
            {
                testEntityWithGuidPkRepository.Insert(entity);

                //Assert: It should be set
                assignedId = entity.Id;
                assignedId.ShouldNotBe(Guid.Empty);

                uow.Complete();
            }

            //Assert: It should still be the same
            entity.Id.ShouldBe(assignedId);
        }

        [Fact]
        public void ShouldNotSetIdOnInsertForDbGenerated()
        {
            //Arrange
            var entity = new TestEntityWithGuidPkAndDbGeneratedValue();

            //Act
            using (var uow = unitOfWorkManager.Begin())
            {
                testEntityWithGuidPkAndDbGeneratedValueRepository.Insert(entity);

                //Assert: It should not be set yet, since UOW is not completed
                entity.Id.ShouldBe(Guid.Empty);

                uow.Complete();
            }

            //Assert: It should be assigned by database
            entity.Id.ShouldNotBe(Guid.Empty);
        }

        [Fact]
        public void ShouldSetIdOnInsertAndGetIdForDbGenerated()
        {
            //Arrange
            var entity = new TestEntityWithGuidPkAndDbGeneratedValue();

            //Act
            using (var uow = unitOfWorkManager.Begin())
            {
                testEntityWithGuidPkAndDbGeneratedValueRepository.InsertAndGetId(entity);

                //Assert: It should be set yet, since InsertAndGetId saved to database
                entity.Id.ShouldNotBe(Guid.Empty);

                uow.Complete();
            }
        }
    }
}
