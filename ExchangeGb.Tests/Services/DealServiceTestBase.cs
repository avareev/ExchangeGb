using System;
using System.Threading.Tasks;
using ExchangeGb.Data;
using ExchangeGb.Models.Repositories;
using ExchangeGb.Tests.TestUtils;

namespace ExchangeGb.Tests.Services
{
    public abstract class DealServiceTestBase : IAsyncDisposable
    {
        private readonly TestExchangeDbContextProvider _exchangeDbContextProvider;
        protected ExchangeDbContext DbContext { get; }
        protected IBuyOrderRepository BuyOrderRepository { get; }
        protected ISellOrderRepository SellOrderRepository { get; }
        protected IDealRepository DealRepository { get; }

        protected DealServiceTestBase()
        {
            {
                var dbContextTask = TestExchangeDbContextProvider.GetInstanceAsync();
                dbContextTask.Wait();
                _exchangeDbContextProvider = dbContextTask.Result;

                DbContext = _exchangeDbContextProvider.GetDbContext();
                BuyOrderRepository = new BuyOrderRepository(DbContext);
                SellOrderRepository = new SellOrderRepository(DbContext);
                DealRepository = new DealRepository(DbContext);
            }
        }

        public ValueTask DisposeAsync()
        {
            DbContext.Dispose();
            return _exchangeDbContextProvider.DisposeAsync();
        }
    }
}