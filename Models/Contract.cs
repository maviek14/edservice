using System;
using System.ComponentModel.DataAnnotations;

namespace DeletingDataService.Models
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
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedTime { get; set; }

        [DataType(DataType.Date)]
        public DateTime? CompletedTime { get; set; }

        [Required]
        public float Price { get; set; }

        [Required]
        public ContractStatus Status { get; set; }

        public int DeviceID { get; set; }

        public virtual Device Device { get; set; }

        public int PrincipalID { get; set; }

        public virtual Profile Principal { get; set; }

        public int? MandatoryID { get; set; }

        public virtual Profile Mandatory { get; set; }
    }
}