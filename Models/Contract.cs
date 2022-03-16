using System;
using System.ComponentModel.DataAnnotations;

namespace PlatformService.Models
{
    public enum ContractStatus
    {
        Taken, Available, Completed
    }
    public class Contract
    {
        [Key]
        public int ID { get; set; }
        
        [Required]
        [Display(Name = "Contract Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Contract Description")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        public string Description { get; set; }

        [Display(Name = "Creation Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreatedTime { get; set; }

        [Display(Name = "Completion Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(NullDisplayText = "The contract has not been completed yet", DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? CompletedTime { get; set; }

        [Required]
        [Display(Name = "Contract Price")]
        public float Price { get; set; }

        [Required]
        [Display(Name = "Contract Status")]
        public ContractStatus Status { get; set; }

        public int DeviceID { get; set; }

        public virtual Device Device { get; set; }

        public int PrincipalID { get; set; }

        [Display(Name = "Principal Username")]
        public virtual Profile Principal { get; set; }

        [DisplayFormat(NullDisplayText = "The contract has not been taken yet.", ApplyFormatInEditMode = true)]
        public int? MandatoryID { get; set; }

        [Display(Name = "Mandatory Username")]
        [DisplayFormat(NullDisplayText = "The contract has not been taken yet.", ApplyFormatInEditMode = true)]
        public virtual Profile Mandatory { get; set; }
    }
}