using System;
using System.Collections.Generic;
using System.Linq;
using ExchangeGb.Models.Entities;
using ExchangeGb.Services;
using ExchangeGb.Tests.TestUtils;
using Xunit;

namespace ExchangeGb.Tests.Services
{
    public class DealServiceAddSellOrderUseCaseTest : DealServiceTestBase
    {
        public DealServiceAddSellOrderUseCaseTest() : base()
        {
        }

        [Fact]
        public void CanAddSellToEmptyDb()
        {
            IAddSellOrderUseCase sellOrderUseCase =
                new DealService(BuyOrderRepository, SellOrderRepository, DealRepository);

            var sellOrder1 = new SellOrder
            {
                Email = "test1@example.com",
                Price = 24.13m,
                Qty = 5
            };
            var sellOrder2 = new SellOrder
            {
                Email = "test2@example.com",
                Price = 25m,
                Qty = 10
            };
            sellOrderUseCase.AddSellOrder(sellOrder1);
            sellOrderUseCase.AddSellOrder(sellOrder2);

            var dbSellOrders = DbContext.SellOrders.ToList();
            Assert.Equal(2, dbSellOrders.Count);

            Assert.True(dbSellOrders[0].Id > 0);
            Assert.True(dbSellOrders[1].Id > 0);
            Assert.True(DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(15)) < dbSellOrders[0].CreatedAt);
            Assert.Equal(24.13m, dbSellOrders[0].Price);
            Assert.Equal(25m, dbSellOrders[1].Price);
            Assert.Equal(5, dbSellOrders[0].Qty);
            Assert.Equal(10, dbSellOrders[1].Qty);
            Assert.Equal("test1@example.com", dbSellOrders[0].Email);
            Assert.Equal("test2@example.com", dbSellOrders[1].Email);
        }

        [Theory]
        [MemberData(nameof(SellOrderTestData))]
        public void CanAddSellOrder(TestDbState initialState, TestDbState resultState, SellOrder sellOrder)
        {
            initialState.Apply(DbContext);

            IAddSellOrderUseCase sellOrderUseCase =
                new DealService(BuyOrderRepository, SellOrderRepository, DealRepository);
            sellOrderUseCase.AddSellOrder(sellOrder);

            resultState.AssertState(DbContext);
        }

        public static IEnumerable<object[]> SellOrderTestData()
        {
            yield return new object[]
            {
                TestDbState.Create()
                    .AddOrder(new BuyOrder {Price = 43m, Qty = 10, Email = "test1@a.com"})
                    .AddOrder(new BuyOrder {Price = 35m, Qty = 15, Email = "test2@a.com"})
                    .AddOrder(new BuyOrder {Price = 31m, Qty = 20, Email = "test3@a.com"})
                    .AddOrder(new BuyOrder {Price = 20m, Qty = 20, Email = "test4@a.com"}),
                TestDbState.Create()
                    .AddOrder(new SellOrder {Price = 33m, Qty = 75, Email = "seller@a.com"})
                    .AddOrder(new BuyOrder {Price = 31m, Qty = 20, Email = "test3@a.com"})
                    .AddOrder(new BuyOrder {Price = 20m, Qty = 20, Email = "test4@a.com"})
                    .AddDeal(new Deal
                    {
                        BuyerEmail = "test1@a.com", SellerEmail = "seller@a.com", Price = 43m, Qty = 10,
                        
                    })
                    .AddDeal(new Deal
                    {
                        BuyerEmail = "test2@a.com", SellerEmail = "seller@a.com", Price = 35m, Qty = 15,
                        
                    }),
                new SellOrder {Price = 33m, Qty = 100, Email = "seller@a.com"}
            };
        }
    }
}