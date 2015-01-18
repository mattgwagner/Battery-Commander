using Microsoft.AspNet.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatteryCommander.Common.Models
{
    public class AppUser : IUser<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Role Role { get; set; }

        [Required]
        [EmailAddress, DataType(DataType.EmailAddress)]
        [StringLength(50)]
        [Index(IsUnique = true)]
        public String UserName { get; set; }

        [Required]
        public Boolean EmailAddressConfirmed { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public String Password { get; set; }

        [DataType(DataType.Password)]
        public String SecurityStamp { get; set; }

        [Required]
        public Boolean TwoFactorEnabled { get; set; }

        [DataType(DataType.PhoneNumber), Phone]
        [StringLength(20)]
        [Display(Name = "Mobile Phone")]
        public String PhoneNumber { get; set; }

        public Boolean PhoneNumberConfirmed { get; set; }

        [Required]
        public int AccessFailedCount { get; set; }

        public DateTime? LockoutEndDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime LastUpdated { get; set; }

        public AppUser()
        {
            this.Created = DateTime.UtcNow;
            this.LastUpdated = DateTime.UtcNow;

            // Generate a unique security stamp on create
            this.SecurityStamp = Guid.NewGuid().ToString();
        }
    }
}