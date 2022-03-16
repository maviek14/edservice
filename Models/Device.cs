using System.ComponentModel.DataAnnotations;

namespace DeletingDataService.Models
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
        public DeviceType Type { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public string PictureName { get; set; }

        public virtual Profile Profile { get; set; }

        public int? ProfileID { get; set; }
    }
}