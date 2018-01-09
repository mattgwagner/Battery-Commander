using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatteryCommander.Web.Models
{
    public class Weapon
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UnitId { get; set; }

        public virtual Unit Unit { get; set; }

        [Required, StringLength(10)]
        public String AdminNumber { get; set; }

        [Required, StringLength(50)]
        public String Serial { get; set; }

        [Display(Name = "Optic Serial")]
        [StringLength(50)]
        public String OpticSerial { get; set; }

        public WeaponOptic OpticType { get; set; } = WeaponOptic.CCO;

        public Soldier Assigned { get; set; }

        public int? AssignedId { get; set; }

        public WeaponType Type { get; set; } = WeaponType.M4;

        public enum WeaponOptic : byte
        {
            None = 0,

            CCO = 1,

            ACOG = 2
        }

        public enum WeaponType : byte
        {
            // TODO Add Full Display Description, i.e. Rifle, 5.56mm M16A2

            M4 = 0,

            M9 = 1,

            M240B = 2,

            M249 = 3,

            M320 = 4
        }
    }
}