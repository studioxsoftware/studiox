using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using StudioX.Application.Services;
using StudioX.Application.Services.Dto;
using StudioX.Boilerplate.EntityFrameworkCore;
using StudioX.Domain.Entities;
using StudioX.Runtime.Validation;
using Xunit;

namespace StudioX.Boilerplate.Tests
{
    public abstract class BoilerplateAsyncServiceTestBase<TEntity, TEntityDto, TPrimaryKey, TService, TCreateDto,
        TUpdateDto> : BoilerplateTestBase
        where TService :
        AsyncCrudAppService<TEntity, TEntityDto, TPrimaryKey, PagedResultRequestDto, TCreateDto, TUpdateDto>
        where TEntity : class, IEntity<TPrimaryKey>
        where TEntityDto : class, IEntityDto<TPrimaryKey>
        where TUpdateDto : class, IEntityDto<TPrimaryKey>
        where TPrimaryKey : IComparable
    {
        protected TService AppService { get; }

        protected BoilerplateAsyncServiceTestBase()
        {
            AppService = Resolve<TService>();
        }

        // move this to return on method
        protected TPrimaryKey[] keys;

        protected async Task Create(int entityCount)
        {
            var ids = new List<TPrimaryKey>();
            for (var i = 0; i < entityCount; i++)
            {
                var id = await UsingDbContextAsync(async context =>
                {
                    var entity = await CreateEntity(i);
                    context.Set<TEntity>().Add(entity);

                    context.SaveChanges();

                    return entity.Id;
                });

                ids.Add(id);
            }

            keys = ids.ToArray();
        }

        protected abstract Task<TEntity> CreateEntity(int entityNumer);

        protected abstract TCreateDto GetCreateInput();

        protected abstract TUpdateDto GetUpdateInput(TPrimaryKey key);

        protected virtual async Task<TUpdateDto> CheckForValidationErrors(Func<Task<TUpdateDto>> callBack)
        {
            try
            {
                return await callBack();
            }
            catch (StudioXException ave)
            {
                if (ave is StudioXValidationException)
                {
                    throw CreateShouldAssertException(ave as StudioXValidationException);
                }

                throw new ShouldAssertException(ave.Message);
            }
        }

        protected virtual async Task<IEntityDto<TPrimaryKey>> CheckForValidationErrors(
            Func<Task<IEntityDto<TPrimaryKey>>> function)
        {
            try
            {
                return await function();
            }
            catch (StudioXException ave)
            {
                if (ave is StudioXValidationException)
                {
                    throw CreateShouldAssertException(ave as StudioXValidationException);
                }

                throw new ShouldAssertException(ave.Message);
            }
        }

        private ShouldAssertException CreateShouldAssertException(StudioXValidationException ave)
        {
            var message = "";
            foreach (var error in ave.ValidationErrors)
            {
                message += error.ErrorMessage + "\n";
            }

            return new ShouldAssertException(message);
        }

        [Fact]
        public async Task CreateTest()
        {
            //Arrange
            var createInput = GetCreateInput();

            //Act
            var createdEntityInput = await CheckForValidationErrors(async () => 
                await AppService.Create(createInput)
            ) as TEntityDto;

            //Assert
            await UsingDbContextAsync(async context =>
            {
                var savedEntity =
                    await context.Set<TEntity>().FirstOrDefaultAsync(e => e.Id.CompareTo(createdEntityInput.Id) == 0);
                savedEntity.ShouldNotBeNull();

                await CreateChecks(context, createInput);
            });
        }

        public virtual async Task CreateChecks(BoilerplateDbContext context, TCreateDto createInput)
        {
        }

        [Fact]
        public async Task GetTest()
        {
            //Arrange
            await Create(1);

            //Act
            var entity = await AppService.Get(new EntityDto<TPrimaryKey>(keys[0]));

            //Assert
            entity.ShouldNotBeNull();
        }

        [Fact]
        public virtual async Task GetAllTest()
        {
            //Arrange
            await Create(20);

            //Act
            var users = await AppService.GetAll(
                new PagedResultRequestDto {MaxResultCount = 10, SkipCount = 0}
            );

            //Assert
            users.Items.Count.ShouldBe(10);
        }

        [Fact]
        public virtual async Task GetAllPagingTest()
        {
            //Arrange
            await Create(20);

            //Act
            var users = await AppService.GetAll(
                new PagedResultRequestDto {MaxResultCount = 10, SkipCount = 10}
            );

            //Assert
            users.Items.Count.ShouldBe(10);
        }

        [Fact]
        public async Task UpdateTest()
        {
            //Arrange
            await Create(1);

            //
            var updateDto = GetUpdateInput(keys[0]);

            //Act
            //Assuming TUpdateDto is TEntityDto, otherwise atribute mapping is required 
            var updatedEntityDto = await CheckForValidationErrors(
                    async () => await AppService.Update(updateDto)
                ) as TEntityDto;

            //Assert, should check an updated field
            updatedEntityDto.ShouldNotBeNull();
            updatedEntityDto.Id.ShouldBe(keys[0]);

            await UsingDbContextAsync(async (context, updatedDto) => { await UpdateChecks(context, updatedDto); },
                updatedEntityDto);
        }

        public virtual async Task UpdateChecks(BoilerplateDbContext context, TEntityDto updatedDto)
        {
        }

        [Fact]
        public async Task DeleteTest()
        {
            //Arrange
            await Create(1);

            //Act
            await AppService.Delete(new EntityDto<TPrimaryKey>(keys[0]));

            //Assert
            await UsingDbContextAsync(async context =>
            {
                var savedEntity = await context.Set<TEntity>().FirstOrDefaultAsync(e => e.Id.CompareTo(keys[0]) == 0);

                if (savedEntity is ISoftDelete)
                {
                    (savedEntity as ISoftDelete).IsDeleted.ShouldBeTrue();
                }
                else
                {
                    savedEntity.ShouldBeNull();
                }

                await DeleteChecks(context, keys[0]);
            });
        }

        public virtual async Task DeleteChecks(BoilerplateDbContext context, TPrimaryKey key)
        {
        }

        #region UsingDbContextAsync extensions to allow adding TEntityDto to lambda method

        protected async Task UsingDbContextAsync(Func<BoilerplateDbContext, TEntityDto, Task> action,
            TEntityDto updateDto)
        {
            await UsingDbContextAsync(StudioXSession.TenantId, action, updateDto);
        }

        protected async Task UsingDbContextAsync(int? tenantId, Func<BoilerplateDbContext, TEntityDto, Task> action,
            TEntityDto updateDto)
        {
            using (UsingTenantId(tenantId))
            {
                using (var context = LocalIocManager.Resolve<BoilerplateDbContext>())
                {
                    await action(context, updateDto);
                    await context.SaveChangesAsync();
                }
            }
        }

        #endregion
    }
}