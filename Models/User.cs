using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeletingDataService.Models
{
    public class User
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [DataType(DataType.Date)]
        public DateTime AddedTime { get; set; }

        public int? ProfileID { get; set; }

        public virtual Profile Profile { get; set; }

        public string FullName
        {
            get { return Name + " " + Surname; }
        }
    }
}