using ContractMaster.API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContractMaster.API.Models
{
    public enum ContractStatus
    {
        [Display(Name = "Draft")]
        Draft = 0,

        [Display(Name = "Active")]
        Active = 1,

        [Display(Name = "Expired")]
        Expired = 2,

        [Display(Name = "On Hold")]
        OnHold = 3
    }

    public class Contract
    {
        [Key]
        public int ContractId { get; set; }

        [Required(ErrorMessage = "Client is required")]
        [Display(Name = "Client")]
        public int ClientId { get; set; }

        [ForeignKey("ClientId")]
        public virtual Client? Client { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "End date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; } = DateTime.Today.AddYears(1);

        [Required]
        [Display(Name = "Status")]
        public ContractStatus Status { get; set; } = ContractStatus.Draft;

        [Required(ErrorMessage = "Service level is required")]
        [StringLength(50)]
        [Display(Name = "Service Level")]
        public string ServiceLevel { get; set; } = string.Empty;

        [Display(Name = "Signed Agreement")]
        public string? SignedAgreementPath { get; set; }

        // Navigation property
        public ICollection<ServiceRequest>? ServiceRequests { get; set; }
    }
}