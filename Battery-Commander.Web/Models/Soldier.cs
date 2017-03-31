using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatteryCommander.Web.Models
{
    public class Soldier
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, StringLength(50)]
        [Display(Name = "Last Name")]
        public String LastName { get; set; }

        [Required, StringLength(50)]
        [Display(Name = "First Name")]
        public String FirstName { get; set; }

        [Required]
        public Rank Rank { get; set; } = Rank.E1;

        public String DoDId { get; set; }

        [DataType(DataType.EmailAddress)]
        public String MilitaryEmail { get; set; }

        [DataType(DataType.EmailAddress)]
        public String CivilianEmail { get; set; }

        // Status - Active, Inactive

        // Unit

        // Position

        // Security Clearance

        // MOS - Duty MOSQ'd?

        // ETS Date

        // Education Level Completed
    }
}