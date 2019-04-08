using System.ComponentModel.DataAnnotations;
using Datadog.Coffeehouse.Core.Interfaces;

namespace Datadog.Coffeehouse.Core.Models
{
    public class User : IHasStringId
    {
        [Required]
        [StringLength(32, MinimumLength = 32)]
        public string Id { get; set; }

        [StringLength(32, MinimumLength = 32)]
        public string CompanyId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        public long CreatedOn { get; set; }
    }
}
