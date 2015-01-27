using System;
using System.ComponentModel.DataAnnotations;

namespace BatteryCommander.Web.Models
{
    public class UserEditModel
    {
        public int Id { get; set; }

        // public Role Role { get; set; }

        [Required]
        [EmailAddress, DataType(DataType.EmailAddress)]
        [StringLength(50)]
        public String UserName { get; set; }

        [Required]
        public Boolean TwoFactorEnabled { get; set; }

        [DataType(DataType.PhoneNumber), Phone]
        [StringLength(20)]
        [Display(Name = "Mobile Phone")]
        public String PhoneNumber { get; set; }
    }
}