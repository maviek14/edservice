using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlatformService.Models
{
    public class User
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        public string Surname { get; set; }

        [Display(Name = "Added Time")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime AddedTime { get; set; }

        public int? ProfileID { get; set; }

        public virtual Profile Profile { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get { return Name + " " + Surname; }
        }
    }
}