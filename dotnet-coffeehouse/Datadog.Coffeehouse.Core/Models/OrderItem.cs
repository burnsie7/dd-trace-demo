using System.ComponentModel.DataAnnotations;
using Datadog.Coffeehouse.Core.Interfaces;

namespace Datadog.Coffeehouse.Core.Models
{
    public class OrderItem : IHasStringId
    {
        [Required]
        [StringLength(32, MinimumLength = 32)]
        public string Id { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 32)]
        public string OrderId { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 32)]
        public string ProductId { get; set; }

        [Required]
        [Range(1, 1000)]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, 10000.0)]
        public double UnitCost { get; set; }
    }
}
