using ExchangeGb.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExchangeGb.Data
{
    /// <summary>
    /// Application database context
    /// </summary>
    public class ExchangeDbContext : DbContext
    {
        public ExchangeDbContext(DbContextOptions<ExchangeDbContext> options) : base(options)
        {
        }
        
        public DbSet<Deal> Deals { get; set; }
        public DbSet<BuyOrder> BuyOrders { get; set; }
        public DbSet<SellOrder> SellOrders { get; set; }
        
    }
}