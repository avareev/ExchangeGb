using System;
using System.Linq;
using ExchangeGb.Models.Dto;
using ExchangeGb.Models.Entities;
using ExchangeGb.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeGb.Controllers
{
    /// <summary>
    /// Orders controller. Handles new buy and sell orders
    /// </summary>
    public class OrderController : Controller
    {
        private readonly IAddSellOrderUseCase _addSellOrderUseCase;
        private readonly IAddBuyOrderUseCase _addBuyOrderUseCase;

        public OrderController(IAddSellOrderUseCase addSellOrderUseCase, IAddBuyOrderUseCase addBuyOrderUseCase)
        {
            _addSellOrderUseCase = addSellOrderUseCase;
            _addBuyOrderUseCase = addBuyOrderUseCase;
        }

        /// <summary>
        /// Create new sell order and handle deals
        /// </summary>
        /// <param name="createOrderDto">Sell order data</param>
        [HttpPost("sell")]
        public IActionResult CreateSellOrder(CreateOrderDto createOrderDto)
        {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                    .SelectMany(e => e.Errors)
                    .Select(err => err.ErrorMessage);
                return BadRequest(String.Join(";", errorMessages));
            }

            SellOrder sellOrder = createOrderDto.ToSellOrder();
            _addSellOrderUseCase.AddSellOrder(sellOrder);
            return Redirect("/");
        }

        /// <summary>
        /// Create new buy order and handle deals
        /// </summary>
        /// <param name="createOrderDto">Buy order data</param>
        [HttpPost("buy")]
        public IActionResult CreateBuyOrder(CreateOrderDto createOrderDto)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = string.Join(";", ModelState.Values.SelectMany(e => e.Errors.ToString()));
                return BadRequest(errorMessage);
            }

            BuyOrder buyOrder = createOrderDto.ToBuyOrderEntity();
            _addBuyOrderUseCase.AddBuyOrder(buyOrder);
            return Redirect("/");
        }
    }
}