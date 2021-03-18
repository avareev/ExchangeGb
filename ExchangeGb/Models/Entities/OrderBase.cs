using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExchangeGb.Models.Entities
{
    /// <summary>
    /// Base class for orders
    /// </summary>
    public abstract class OrderBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public decimal Price { get; set; }
        public int Qty { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}