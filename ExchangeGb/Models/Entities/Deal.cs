using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExchangeGb.Models.Entities
{
    /// <summary>
    /// Deal entity
    /// </summary>
    public class Deal
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime BuyOrderDate { get; set; }
        public DateTime SellOrderDate { get; set; }
        public decimal Price { get; set; }
        public int Qty { get; set; }
        public string SellerEmail { get; set; }
        public string BuyerEmail { get; set; }

        public Deal()
        {
        }

        public Deal(BuyOrder buyOrder, SellOrder sellOrder, decimal price, int qty)
        {
            BuyOrderDate = buyOrder.CreatedAt;
            SellOrderDate = sellOrder.CreatedAt;
            Price = price;
            Qty = qty;
            SellerEmail = sellOrder.Email;
            BuyerEmail = buyOrder.Email;
        }
    }
}