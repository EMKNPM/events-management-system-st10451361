using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContractMaster.Web.Models
{
    public enum ServiceRequestStatus
    {
        [Display(Name = "Open")]
        Open = 0,

        [Display(Name = "In Progress")]
        InProgress = 1,

        [Display(Name = "Completed")]
        Completed = 2,

        [Display(Name = "Cancelled")]
        Cancelled = 3
    }

    public class ServiceRequest
    {
        [Key]
        public int ServiceRequestId { get; set; }

        [Required(ErrorMessage = "Contract is required")]
        [Display(Name = "Contract")]
        public int ContractId { get; set; }

        [ForeignKey("ContractId")]
        public virtual Contract? Contract { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500)]
        [Display(Name = "Service Description")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cost in USD is required")]
        [Range(0.01, 1000000, ErrorMessage = "Cost must be between $0.01 and $1,000,000")]
        [Display(Name = "Cost (USD)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostUSD { get; set; }

        [Display(Name = "Cost (ZAR)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostZAR { get; set; }

        [Required]
        [Display(Name = "Status")]
        public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Open;

        [Display(Name = "Created Date")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}