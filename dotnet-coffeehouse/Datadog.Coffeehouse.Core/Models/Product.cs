using System.ComponentModel.DataAnnotations;
using Datadog.Coffeehouse.Core.Enums;
using Datadog.Coffeehouse.Core.Interfaces;

namespace Datadog.Coffeehouse.Core.Models
{
    public class Product : IHasStringId
    {
        [Required]
        [StringLength(32, MinimumLength = 32)]
        public string Id { get; set; }

        [Required]
        [Range(1, 1000)]
        public ProductType ProductType { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [Range(0.01, 10000.0)]
        public double UnitCost { get; set; }
    }
}
