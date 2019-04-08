using System.ComponentModel.DataAnnotations;
using Datadog.Coffeehouse.Core.Enums;
using Datadog.Coffeehouse.Core.Interfaces;

namespace Datadog.Coffeehouse.Core.Models
{
    public class Order : IHasStringId
    {
        [Required]
        [StringLength(32, MinimumLength = 32)]
        public string Id { get; set; }

        [StringLength(32, MinimumLength = 32)]
        public string UserId { get; set; }

        [Required]
        public OrderStatus Status { get; set; }

        public long CreatedOn { get; set; }
    }
}
