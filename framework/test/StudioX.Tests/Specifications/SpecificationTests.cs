using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using StudioX.Specifications;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Specifications
{
    public class SpecificationTests
    {
        private readonly IQueryable<Customer> customers;

        public SpecificationTests()
        {
            customers = new List<Customer>
            {
                new Customer("John", 17, 47000, "England"),
                new Customer("Tuana", 2, 500, "Turkey"),
                new Customer("Martin", 43, 16000, "USA"),
                new Customer("Lee", 32, 24502, "China"),
                new Customer("Douglas", 42, 42000, "England"),
                new Customer("Abelard", 14, 2332, "German"),
                new Customer("Neo", 16, 120000, "USA"),
                new Customer("Daan", 39, 6000, "Netherlands"),
                new Customer("Alessandro", 22, 8271, "Italy"),
                new Customer("Noah", 33, 82192, "Belgium")
            }.AsQueryable();
        }

        [Fact]
        public void AnyShouldReturnAll()
        {
            customers
                .Where(new AnySpecification<Customer>()) //Implicitly converted to Expression!
                .Count()
                .ShouldBe(customers.Count());
        }

        [Fact]
        public void NoneShouldReturnEmpty()
        {
            customers
                .Where(new NoneSpecification<Customer>().ToExpression())
                .Count()
                .ShouldBe(0);
        }

        [Fact]
        public void NotShouldReturnReverseResult()
        {
            customers
                .Where(new EuropeanCustomerSpecification().Not().ToExpression())
                .Count()
                .ShouldBe(3);
        }

        [Fact]
        public void ShouldSupportNativeExpressionsAndCombinations()
        {
            customers
                .Where(new ExpressionSpecification<Customer>(c => c.Age >= 18).ToExpression())
                .Count()
                .ShouldBe(6);

            customers
                .Where(new EuropeanCustomerSpecification().And(new ExpressionSpecification<Customer>(c => c.Age >= 18)).ToExpression())
                .Count()
                .ShouldBe(4);
        }

        [Fact]
        public void CustomSpecificationTest()
        {
            customers
                .Where(new EuropeanCustomerSpecification().ToExpression())
                .Count()
                .ShouldBe(7);

            customers
                .Where(new Age18PlusCustomerSpecification().ToExpression())
                .Count()
                .ShouldBe(6);

            customers
                .Where(new BalanceCustomerSpecification(10000, 30000).ToExpression())
                .Count()
                .ShouldBe(2);

            customers
                .Where(new PremiumCustomerSpecification().ToExpression())
                .Count()
                .ShouldBe(3);
        }

        [Fact]
        public void IsSatisfiedByTests()
        {
            new PremiumCustomerSpecification().IsSatisfiedBy(new Customer("David", 49, 55000, "USA")).ShouldBeTrue();

            new PremiumCustomerSpecification().IsSatisfiedBy(new Customer("David", 49, 200, "USA")).ShouldBeFalse();
            new PremiumCustomerSpecification().IsSatisfiedBy(new Customer("David", 12, 55000, "USA")).ShouldBeFalse();
        }

        [Fact]
        public void CustomSpecificationCompositeTests()
        {
            customers
                .Where(new EuropeanCustomerSpecification().And(new Age18PlusCustomerSpecification()).ToExpression())
                .Count()
                .ShouldBe(4);

            customers
               .Where(new EuropeanCustomerSpecification().Not().And(new Age18PlusCustomerSpecification()).ToExpression())
               .Count()
               .ShouldBe(2);

            customers
                .Where(new Age18PlusCustomerSpecification().AndNot(new EuropeanCustomerSpecification()).ToExpression())
                .Count()
                .ShouldBe(2);
        }

        private class Customer
        {
            public string Name { get; private set; }

            public byte Age { get; private set; }

            public long Balance { get; private set; }

            public string Location { get; private set; }

            public Customer(string name, byte age, long balance, string location)
            {
                Name = name;
                Age = age;
                Balance = balance;
                Location = location;
            }
        }

        private class EuropeanCustomerSpecification : Specification<Customer>
        {
            public override Expression<Func<Customer, bool>> ToExpression()
            {
                return c => c.Location == "England" ||
                            c.Location == "Turkey" ||
                            c.Location == "German" ||
                            c.Location == "Netherlands" ||
                            c.Location == "Italy" ||
                            c.Location == "Belgium";
            }
        }

        private class Age18PlusCustomerSpecification : Specification<Customer>
        {
            public override Expression<Func<Customer, bool>> ToExpression()
            {
                return c => c.Age >= 18;
            }
        }

        private class BalanceCustomerSpecification : Specification<Customer>
        {
            public int MinBalance { get; }

            public int MaxBalance { get; }

            public BalanceCustomerSpecification(int minBalance, int maxBalance)
            {
                MinBalance = minBalance;
                MaxBalance = maxBalance;
            }

            public override Expression<Func<Customer, bool>> ToExpression()
            {
                return c => c.Balance >= MinBalance && c.Balance <= MaxBalance;
            }
        }

        private class PremiumCustomerSpecification : AndSpecification<Customer>
        {
            public PremiumCustomerSpecification()
                : base(new Age18PlusCustomerSpecification(), new BalanceCustomerSpecification(20000, int.MaxValue))
            {
            }
        }
    }
}
