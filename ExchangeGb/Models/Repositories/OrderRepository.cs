using System.Collections.Generic;
using System.Linq;
using ExchangeGb.Data;
using ExchangeGb.Models.Entities;

namespace ExchangeGb.Models.Repositories
{
    /// <summary>
    /// Orders data repository 
    /// </summary>
    /// <typeparam name="T">Sell or buy order class</typeparam>
    public interface IOrderRepository<T> where T : OrderBase
    {
        /// <summary>
        /// Get Orders
        /// </summary>
        public IQueryable<T> Orders { get; }
        
        /// <summary>
        /// Create or update order
        /// </summary>
        void Save(T order);
        
        /// <summary>
        /// Delete many orders 
        /// </summary>
        void DeleteMany(IEnumerable<T> orders);
    }

    /// <summary>
    /// Buy orders data repository
    /// </summary>
    public interface IBuyOrderRepository : IOrderRepository<BuyOrder>
    {
    }

    /// <summary>
    /// Sell orders data repository
    /// </summary>
    public interface ISellOrderRepository : IOrderRepository<SellOrder>
    {
    }

    public class BuyOrderRepository : IBuyOrderRepository
    {
        private readonly ExchangeDbContext _dbContext;

        public BuyOrderRepository(ExchangeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<BuyOrder> Orders => _dbContext.BuyOrders;

        public void Save(BuyOrder order)
        {
            var dbOrder = _dbContext.BuyOrders.FirstOrDefault(o => o.Id == order.Id);
            if (dbOrder != null)
            {
                dbOrder.Qty = order.Qty;
            }
            else
            {
                _dbContext.BuyOrders.Add(order);
            }

            _dbContext.SaveChanges();
        }

        public void DeleteMany(IEnumerable<BuyOrder> orders)
        {
            _dbContext.BuyOrders.RemoveRange(orders);
            _dbContext.SaveChanges();
        }
    }

    public class SellOrderRepository : ISellOrderRepository
    {
        private readonly ExchangeDbContext _dbContext;

        public SellOrderRepository(ExchangeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<SellOrder> Orders => _dbContext.SellOrders;

        public void Save(SellOrder order)
        {
            var dbOrder = _dbContext.SellOrders.FirstOrDefault(o => o.Id == order.Id);
            if (dbOrder != null)
            {
                dbOrder.Qty = order.Qty;
            }
            else
            {
                _dbContext.SellOrders.Add(order);
            }

            _dbContext.SaveChanges();
        }

        public void DeleteMany(IEnumerable<SellOrder> orders)
        {
            _dbContext.SellOrders.RemoveRange(orders);
            _dbContext.SaveChanges();
        }
    }
}