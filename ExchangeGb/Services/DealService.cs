using System;
using System.Collections.Generic;
using System.Linq;
using ExchangeGb.Models.Entities;
using ExchangeGb.Models.Repositories;

namespace ExchangeGb.Services
{
    /// <summary>
    /// Add sell order use case handler
    /// </summary>
    public interface IAddSellOrderUseCase
    {
        /// <summary>
        /// Add new sell order and handle deals
        /// </summary>
        /// <param name="sellOrder">Sell order</param>
        public void AddSellOrder(SellOrder sellOrder);
    }

    /// <summary>
    /// Add buy order use case handler
    /// </summary>
    public interface IAddBuyOrderUseCase
    {
        /// <summary>
        /// Add new Buy order and handle deals
        /// </summary>
        /// <param name="buyOrder">Buy order</param>
        public void AddBuyOrder(BuyOrder buyOrder);
    }

    /// <summary>
    /// Deals service
    /// </summary>
    public class DealService : IAddSellOrderUseCase, IAddBuyOrderUseCase
    {
        private readonly IBuyOrderRepository _buyOrderRepository;
        private readonly ISellOrderRepository _sellOrderRepository;
        private readonly IDealRepository _dealRepository;

        public DealService(
            IBuyOrderRepository buyOrderRepository,
            ISellOrderRepository sellOrderRepository,
            IDealRepository dealRepository)
        {
            _buyOrderRepository = buyOrderRepository;
            _sellOrderRepository = sellOrderRepository;
            _dealRepository = dealRepository;
        }

        public void AddSellOrder(SellOrder sellOrder)
        {
            var completeOrders = new List<BuyOrder>();
            var deals = new List<Deal>();
            var buyOrders = _buyOrderRepository.Orders
                .Where(o => o.Price >= sellOrder.Price)
                .OrderByDescending(o => o.Price);
            BuyOrder updatedBuyOrder = null;

            foreach (var buyOrder in buyOrders)
            {
                var dealQty = Math.Min(sellOrder.Qty, buyOrder.Qty);
                deals.Add(new Deal(buyOrder, sellOrder, buyOrder.Price, dealQty));

                buyOrder.Qty -= dealQty;
                if (buyOrder.Qty > 0)
                {
                    updatedBuyOrder = buyOrder;
                }
                else
                {
                    completeOrders.Add(buyOrder);
                }

                sellOrder.Qty -= dealQty;
                if (sellOrder.Qty <= 0)
                {
                    break;
                }
            }

            SaveOrderIfNotEmpty(_buyOrderRepository, updatedBuyOrder);
            SaveDeals(deals);
            SaveOrderIfNotEmpty(_sellOrderRepository, sellOrder);
            _buyOrderRepository.DeleteMany(completeOrders);
        }

        public void AddBuyOrder(BuyOrder buyOrder)
        {
            var completeOrders = new List<SellOrder>();
            var deals = new List<Deal>();
            var sellOrders = _sellOrderRepository.Orders
                .Where(o => o.Price <= buyOrder.Price)
                .OrderBy(o => o.Price);
            SellOrder updatedSellOrder = null;

            foreach (var sellOrder in sellOrders)
            {
                var dealQty = Math.Min(sellOrder.Qty, buyOrder.Qty);
                deals.Add(new Deal(buyOrder, sellOrder, sellOrder.Price, dealQty));

                sellOrder.Qty -= dealQty;
                if (sellOrder.Qty > 0)
                {
                    updatedSellOrder = sellOrder;
                }
                else
                {
                    completeOrders.Add(sellOrder);
                }

                buyOrder.Qty -= dealQty;
                if (buyOrder.Qty <= 0)
                {
                    break;
                }
            }

            SaveOrderIfNotEmpty(_sellOrderRepository, updatedSellOrder);
            SaveDeals(deals);
            SaveOrderIfNotEmpty(_buyOrderRepository, buyOrder);
            _sellOrderRepository.DeleteMany(completeOrders);
        }

        /// <summary>
        /// Save finished deals to repository
        /// </summary>
        /// <param name="deals">Collection of finished deals</param>
        private void SaveDeals(ICollection<Deal> deals)
        {
            if (deals.Count > 0)
            {
                _dealRepository.SaveRange(deals);
            }
        }

        /// <summary>
        /// Create or update order in repository if it is not null or empty (qty > 0)
        /// </summary>
        /// <param name="repository">Orders repository</param>
        /// <param name="order">The order to be saved</param>
        /// <typeparam name="T">Type of the order</typeparam>
        private void SaveOrderIfNotEmpty<T>(IOrderRepository<T> repository, T order) where T : OrderBase
        {
            if (order != null && order.Qty > 0)
            {
                repository.Save(order);
            }
        }
    }
}