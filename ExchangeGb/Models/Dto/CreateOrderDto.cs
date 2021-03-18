using System;
using System.ComponentModel.DataAnnotations;
using ExchangeGb.Models.Entities;

namespace ExchangeGb.Models.Dto
{
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "Количество должно быть задано")]
        [Display(Name = "Количество")]
        [Range(1, int.MaxValue, ErrorMessage = "Количество позиций должно быть больше нуля")]
        public int Qty { get; set; }

        [EmailAddress(ErrorMessage = "Неправильный формат Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Цена должна быть задана")]
        [Range(0.01, Double.MaxValue, ErrorMessage = "Цена должна быть больше нуля")]
        public decimal Price { get; set; }

        public BuyOrder ToBuyOrderEntity()
        {
            return new BuyOrder
            {
                Qty = Qty,
                Email = Email,
                Price = Price
            };
        }

        public SellOrder ToSellOrder()
        {
            return new SellOrder
            {
                Qty = Qty,
                Email = Email,
                Price = Price
            };
        }
    }
}