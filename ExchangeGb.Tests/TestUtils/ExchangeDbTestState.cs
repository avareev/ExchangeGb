using System.Collections.Generic;
using System.Linq;
using ExchangeGb.Data;
using ExchangeGb.Models.Entities;
using Xunit;

namespace ExchangeGb.Tests.TestUtils
{
    public class TestDbState
    {
        private IList<SellOrder> SellOrders { get; }
        private IList<BuyOrder> BuyOrders { get; }
        private IList<Deal> Deals { get; }

        private TestDbState()
        {
            SellOrders = new List<SellOrder>();
            BuyOrders = new List<BuyOrder>();
            Deals = new List<Deal>();
        }

        public static TestDbState Create()
        {
            return new TestDbState();
        }

        public TestDbState AddOrder(SellOrder sellOrder)
        {
            SellOrders.Add(sellOrder);
            return this;
        }

        public TestDbState AddOrder(BuyOrder buyOrder)
        {
            BuyOrders.Add(buyOrder);
            return this;
        }

        public TestDbState AddDeal(Deal deal)
        {
            Deals.Add(deal);
            return this;
        }

        public void Apply(ExchangeDbContext dbContext)
        {
            dbContext.Deals.AddRange(Deals);
            dbContext.BuyOrders.AddRange(BuyOrders);
            dbContext.SellOrders.AddRange(SellOrders);
            dbContext.SaveChanges();
        }

        public void AssertState(ExchangeDbContext dbContext)
        {
            Assert.Equal(Deals.Count, dbContext.Deals.Count());
            Assert.Equal(SellOrders.Count, dbContext.SellOrders.Count());
            Assert.Equal(BuyOrders.Count, dbContext.BuyOrders.Count());

            var hasAllDeals = Deals.All(expectedDeal => dbContext.Deals.Any(actual => actual.Price == expectedDeal.Price
                && actual.Qty == expectedDeal.Qty
                && actual.BuyerEmail == expectedDeal.BuyerEmail
                && actual.SellerEmail == expectedDeal.SellerEmail));
            Assert.True(hasAllDeals, "Database should include all expected deals");
            
            var hasAllSellOrders = SellOrders.All(expectedSellOrder => dbContext.SellOrders.Any(actual =>
                expectedSellOrder.Email == actual.Email
                && expectedSellOrder.Price == actual.Price
                && expectedSellOrder.Qty == actual.Qty));
            Assert.True(hasAllSellOrders, "Database should include all expected sell orders");
            
            var hasAllBuyOrders = BuyOrders.All(expectedBuyOrder => dbContext.BuyOrders.Any(actual =>
                expectedBuyOrder.Email == actual.Email
                && expectedBuyOrder.Price == actual.Price
                && expectedBuyOrder.Qty == actual.Qty));
            Assert.True(hasAllBuyOrders, "Database should include all expected sell orders");
        }
    }
}