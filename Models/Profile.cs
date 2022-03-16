using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeletingDataService.Models
{
    public class Profile
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string UserName { get; set; }

        public virtual List<Contract> Contracts { get; set; }

        public virtual List<Device> Devices { get; set; }
    }
}