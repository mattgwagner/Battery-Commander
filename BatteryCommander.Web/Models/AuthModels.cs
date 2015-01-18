using System;
using System.ComponentModel.DataAnnotations;

namespace BatteryCommander.Web.Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public String Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public String Password { get; set; }

        [Display(Name = "Remember This Browser")]
        public Boolean RememberMe { get; set; }

        public String ReturnUrl { get; set; }
    }
}