using System.Collections.Generic;
using ExchangeGb.Models.Entities;
using ExchangeGb.Services;
using ExchangeGb.Tests.TestUtils;
using Xunit;

namespace ExchangeGb.Tests.Services
{
    public class DealServiceAddBuyOrderUseCaseTest : DealServiceTestBase
    {
        [Theory]
        [MemberData(nameof(BuyOrderTestData))]
        public void CanAddBuyOrder(TestDbState initialState, TestDbState resultState, BuyOrder buyOrder)
        {
            initialState.Apply(DbContext);

            IAddBuyOrderUseCase sellOrderUseCase =
                new DealService(BuyOrderRepository, SellOrderRepository, DealRepository);
            sellOrderUseCase.AddBuyOrder(buyOrder);

            resultState.AssertState(DbContext);
        }

        public static IEnumerable<object[]> BuyOrderTestData()
        {
            yield return new object[]
            {
                TestDbState.Create(),
                TestDbState.Create()
                    .AddOrder(new BuyOrder {Price = 33m, Qty = 100, Email = "b@a.com"}),
                new BuyOrder {Price = 33m, Qty = 100, Email = "b@a.com"}
            };

            yield return new object[]
            {
                TestDbState.Create()
                    .AddOrder(new SellOrder {Price = 45m, Qty = 5, Email = "s1@a.com"})
                    .AddOrder(new SellOrder {Price = 50m, Qty = 100, Email = "s2@a.com"})
                    .AddOrder(new SellOrder {Price = 55m, Qty = 5, Email = "s3@a.com"}),
                TestDbState.Create()
                    .AddOrder(new SellOrder {Price = 50m, Qty = 90, Email = "s2@a.com"})
                    .AddOrder(new SellOrder {Price = 55m, Qty = 5, Email = "s3@a.com"})
                    .AddDeal(new Deal {BuyerEmail = "b1@a.com", SellerEmail = "s1@a.com", Price = 45m, Qty = 5})
                    .AddDeal(new Deal {BuyerEmail = "b1@a.com", SellerEmail = "s2@a.com", Price = 50m, Qty = 10}),
                new BuyOrder {Price = 50m, Qty = 15, Email = "b1@a.com"}
            };

            yield return new object[]
            {
                TestDbState.Create()
                    .AddOrder(new SellOrder {Price = 35m, Qty = 5, Email = "s1@a.com"})
                    .AddOrder(new SellOrder {Price = 30m, Qty = 10, Email = "s2@a.com"}),
                TestDbState.Create()
                    .AddOrder(new BuyOrder {Price = 50m, Qty = 100, Email = "b1@a.com"})
                    .AddDeal(new Deal {BuyerEmail = "b1@a.com", SellerEmail = "s1@a.com", Price = 35m, Qty = 5})
                    .AddDeal(new Deal {BuyerEmail = "b1@a.com", SellerEmail = "s2@a.com", Price = 30m, Qty = 10}),
                new BuyOrder {Price = 50m, Qty = 115, Email = "b1@a.com"}
            };
        }
    }
}