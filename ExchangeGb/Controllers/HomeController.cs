using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ExchangeGb.Models;
using ExchangeGb.Models.Repositories;

namespace ExchangeGb.Controllers
{
    /// <summary>
    /// Home page controller
    /// </summary>
    public class HomeController : Controller
    {
        private readonly IBuyOrderRepository _buyOrderRepository;
        private readonly ISellOrderRepository _sellOrderRepository;
        private readonly IDealRepository _dealRepository;

        public HomeController(IBuyOrderRepository buyOrderRepository, ISellOrderRepository sellOrderRepository,
            IDealRepository dealRepository)
        {
            _buyOrderRepository = buyOrderRepository;
            _sellOrderRepository = sellOrderRepository;
            _dealRepository = dealRepository;
        }

        /// <summary>
        /// Index page 
        /// </summary>
        public IActionResult Index()
        {
            var buyOrders = _buyOrderRepository.Orders.ToList();
            var sellOrders = _sellOrderRepository.Orders.ToList();
            var deals = _dealRepository.Deals.ToList();
            var viewModel = new MainViewModel
            {
                BuyOrders = buyOrders,
                SellOrders = sellOrders,
                Deals = deals
            };
            return View(viewModel);
        }

        /// <summary>
        /// Error page action
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}