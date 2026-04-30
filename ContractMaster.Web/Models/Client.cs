using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace ContractMaster.Web.Models
{
    public class Client
    {
        [Key]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Client name is required")]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Client Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact details are required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Name = "Contact Email")]
        public string ContactDetails { get; set; } = string.Empty;

        [Required(ErrorMessage = "Region is required")]
        [StringLength(50)]
        [Display(Name = "Region")]
        public string Region { get; set; } = string.Empty;

        // Navigation property
        public ICollection<Contract>? Contracts { get; set; }
    }
}