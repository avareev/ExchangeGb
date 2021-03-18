using System.Collections.Generic;
using ExchangeGb.Models.Entities;

namespace ExchangeGb.Models
{
    /// <summary>
    /// Main page view model
    /// </summary>
    public class MainViewModel
    {
        /// <summary>
        /// Pending sell orders
        /// </summary>
        public IEnumerable<SellOrder> SellOrders { get; set; }
        /// <summary>
        /// Pending buy orders
        /// </summary>
        public IEnumerable<BuyOrder> BuyOrders { get; set; }
        /// <summary>
        /// Finished deals
        /// </summary>
        public IEnumerable<Deal> Deals { get; set; }
    }
}