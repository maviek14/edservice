using System.ComponentModel.DataAnnotations;

namespace PlatformService.Models
{
    public enum DeviceType
    {
        PC, Laptop, Monitor, Phone, Charger, Other
    }
    public class Device
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [Display(Name = "Device Type")]
        public DeviceType Type { get; set; }

        [Required]
        [Display(Name = "Device Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Device Description")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        public string Description { get; set; }

        [Display(Name = "Device Picture")]
        [DisplayFormat(NullDisplayText = "There is no picture for this device", ApplyFormatInEditMode = true)]
        public string PictureName { get; set; }

        public virtual Profile Profile { get; set; }

        public int? ProfileID { get; set; }
    }
}