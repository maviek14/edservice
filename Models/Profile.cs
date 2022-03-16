using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlatformService.Models
{
    public class Profile
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [Display(Name = "Username")]
        [DisplayFormat(NullDisplayText = "The contract has not been taken yet.", ApplyFormatInEditMode = true)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        public string UserName { get; set; }

        public virtual List<Contract> Contracts { get; set; }

        public virtual List<Device> Devices { get; set; }
    }
}