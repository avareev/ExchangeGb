using System.Collections.Generic;
using System.Linq;
using ExchangeGb.Data;
using ExchangeGb.Models.Entities;

namespace ExchangeGb.Models.Repositories
{
    /// <summary>
    /// Deals data repository
    /// </summary>
    public interface IDealRepository
    {
        /// <summary>
        /// Actual deals data
        /// </summary>
        public IQueryable<Deal> Deals { get; }

        /// <summary>
        /// Save many deals
        /// </summary>
        public void SaveRange(IEnumerable<Deal> deals);
    }

    public class DealRepository : IDealRepository
    {
        private readonly ExchangeDbContext _context;

        public DealRepository(ExchangeDbContext context)
        {
            _context = context;
        }

        public IQueryable<Deal> Deals => _context.Deals;

        public void SaveRange(IEnumerable<Deal> deals)
        {
            _context.Deals.AddRange(deals);
            _context.SaveChanges();
        }
    }
}