using System;
using System.Collections.Generic;
using System.Linq;

using StudioX.Dapper.Repositories;
using StudioX.Dapper.Tests.Entities;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;

using Shouldly;

using Xunit;

namespace StudioX.Dapper.Tests
{
    public class DapperRepositoryTests : DapperApplicationTestBase
    {
        private readonly IDapperRepository<Product> productDapperRepository;
        private readonly IRepository<Product> productRepository;
        private readonly IUnitOfWorkManager unitOfWorkManager;
        private readonly IRepository<ProductDetail> productDetailRepository;
        private readonly IDapperRepository<ProductDetail> productDetailDapperRepository;

        public DapperRepositoryTests()
        {
            productDapperRepository = Resolve<IDapperRepository<Product>>();
            productRepository = Resolve<IRepository<Product>>();
            unitOfWorkManager = Resolve<IUnitOfWorkManager>();
            productDetailRepository = Resolve<IRepository<ProductDetail>>();
            productDetailDapperRepository = Resolve<IDapperRepository<ProductDetail>>();
        }

        [Fact]
        public void DapperReposioryTests()
        {
            using (IUnitOfWorkCompleteHandle uow = unitOfWorkManager.Begin())
            {
                // Insert operation should work and tenant, creation audit properties must be set
                productDapperRepository.Insert(new Product("TShirt"));
                Product insertedProduct = productDapperRepository.GetAll(x => x.Name == "TShirt").FirstOrDefault();

                insertedProduct.ShouldNotBeNull();
                insertedProduct.TenantId.ShouldBe(StudioXSession.TenantId);
                insertedProduct.CreationTime.ShouldNotBeNull();
                insertedProduct.CreatorUserId.ShouldBe(StudioXSession.UserId);

                // Update operation should work and Modification Audits should be set
                productDapperRepository.Insert(new Product("TShirt"));
                Product productToUpdate = productDapperRepository.GetAll(x => x.Name == "TShirt").FirstOrDefault();
                productToUpdate.Name = "Pants";
                productDapperRepository.Update(productToUpdate);

                productToUpdate.ShouldNotBeNull();
                productToUpdate.TenantId.ShouldBe(StudioXSession.TenantId);
                productToUpdate.CreationTime.ShouldNotBeNull();
                productToUpdate.LastModifierUserId.ShouldBe(StudioXSession.UserId);

                // Get method should return single
                productDapperRepository.Insert(new Product("TShirt"));
                Action getAction = () => productDapperRepository.Single(x => x.Name == "TShirt");

                getAction.ShouldThrow<InvalidOperationException>("Sequence contains more than one element");

                // Select * from syntax should work
                IEnumerable<Product> products = productDapperRepository.Query("select * from Products");

                products.Count().ShouldBeGreaterThan(0);

                // Ef and Dapper should work under same transaction
                Product productFromEf = productRepository.FirstOrDefault(x => x.Name == "TShirt");
                Product productFromDapper = productDapperRepository.Single(productFromEf.Id);

                productFromDapper.Name.ShouldBe(productFromEf.Name);
                productFromDapper.TenantId.ShouldBe(productFromEf.TenantId);

                // Soft Delete should work for Dapper
                productDapperRepository.Insert(new Product("SoftDeletableProduct"));

                Product toSoftDeleteProduct = productDapperRepository.Single(x => x.Name == "SoftDeletableProduct");

                productDapperRepository.Delete(toSoftDeleteProduct);

                toSoftDeleteProduct.IsDeleted.ShouldBe(true);
                toSoftDeleteProduct.DeleterUserId.ShouldBe(StudioXSession.UserId);
                toSoftDeleteProduct.TenantId.ShouldBe(StudioXSession.TenantId);

                Product softDeletedProduct = productRepository.FirstOrDefault(x => x.Name == "SoftDeletableProduct");
                softDeletedProduct.ShouldBeNull();

                Product softDeletedProductFromDapper = productDapperRepository.FirstOrDefault(x => x.Name == "SoftDeletableProduct");
                softDeletedProductFromDapper.ShouldBeNull();

                using (unitOfWorkManager.Current.DisableFilter(StudioXDataFilters.SoftDelete))
                {
                    Product softDeletedProductWhenFilterDisabled = productRepository.FirstOrDefault(x => x.Name == "SoftDeletableProduct");
                    softDeletedProductWhenFilterDisabled.ShouldNotBeNull();

                    Product softDeletedProductFromDapperWhenFilterDisabled = productDapperRepository.Single(x => x.Name == "SoftDeletableProduct");
                    softDeletedProductFromDapperWhenFilterDisabled.ShouldNotBeNull();
                }

                using (StudioXSession.Use(2, 266))
                {
                    int productWithTenant2Id = productDapperRepository.InsertAndGetId(new Product("ProductWithTenant2"));

                    Product productWithTenant2 = productRepository.Get(productWithTenant2Id);

                    productWithTenant2.TenantId.ShouldBe(1); //Not sure about that?,Because we changed TenantId to 2 in this scope !!! StudioX.TenantId = 2 now NOT 1 !!!
                }

                using (unitOfWorkManager.Current.SetTenantId(3))
                {
                    int productWithTenant3Id = productDapperRepository.InsertAndGetId(new Product("ProductWithTenant3"));

                    Product productWithTenant3 = productRepository.Get(productWithTenant3Id);

                    productWithTenant3.TenantId.ShouldBe(3);
                }

                Product productWithTenantId3FromDapper = productDapperRepository.FirstOrDefault(x => x.Name == "ProductWithTenant3");
                productWithTenantId3FromDapper.ShouldBeNull();

                using (unitOfWorkManager.Current.SetTenantId(3))
                {
                    Product productWithTenantId3FromDapperInsideTenantScope = productDapperRepository.FirstOrDefault(x => x.Name == "ProductWithTenant3");
                    productWithTenantId3FromDapperInsideTenantScope.ShouldNotBeNull();
                }
               
                using (unitOfWorkManager.Current.SetTenantId(StudioXSession.TenantId))
                {
                    int productWithTenantId40 = productDapperRepository.InsertAndGetId(new Product("ProductWithTenantId40"));

                    Product productWithTenant40 = productRepository.Get(productWithTenantId40);

                    productWithTenant40.TenantId.ShouldBe(StudioXSession.TenantId);
                    productWithTenant40.CreatorUserId.ShouldBe(StudioXSession.UserId);
                }


                //Second DbContext tests
                int productDetailId =productDetailRepository.InsertAndGetId(new ProductDetail("Woman"));
                productDetailDapperRepository.Get(productDetailId).ShouldNotBeNull();


                uow.Complete();
            }
        }
    }
}
