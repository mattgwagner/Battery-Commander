using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatteryCommander.Web.Models
{
    public class Vehicle
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UnitId { get; set; }

        public virtual Unit Unit { get; set; }

        [Required]
        public VehicleStatus Status { get; set; } = VehicleStatus.FMC;

        public Boolean FMC => VehicleStatus.FMC == Status;

        [Required]
        public VehicleType Type { get; set; } = VehicleType.HMMWV;

        [Required]
        public VehicleLocation Location { get; set; } = VehicleLocation.HS;

        [NotMapped]
        public Boolean Available => Status == VehicleStatus.FMC && Location == VehicleLocation.HS;

        // Bumper, Registration, and Serial should be UNIQUE -- configured in Database.OnModelCreating

        [Required, StringLength(10)]
        public String Bumper { get; set; }

        [StringLength(10)]
        public String Registration { get; set; }

        [StringLength(50)]
        public String Serial { get; set; }

        [StringLength(20)]
        public String Nomenclature { get; set; }

        [StringLength(10)]
        public String LIN { get; set; }

        [Required]
        public int Seats { get; set; } = 2;

        [Display(Name = "Fuel Card")]
        public Boolean HasFuelCard { get; set; }

        [Display(Name = "Tow Bar")]
        public Boolean HasTowBar { get; set; }

        [Display(Name = "JBC-P")]
        public Boolean HasJBCP { get; set; }

        public Soldier Driver { get; set; }

        public int? DriverId { get; set; }

        [Display(Name = "A-Driver")]
        public Soldier A_Driver { get; set; }

        public int? A_DriverId { get; set; }

        [Display(Name = "Troop Capacity")]
        public int TroopCapacity { get; set; }

        [Display(Name = "Capacity")]
        public int TotalCapacity => Seats + TroopCapacity;

        // Passengers

        // Chalk, Order of March?

        public String Notes { get; set; }

        public String GoogleSearchUrl => String.IsNullOrWhiteSpace(Nomenclature) ? String.Empty : $"https://www.google.com/search?q={Nomenclature}";

        public enum VehicleType : byte
        {
            HMMWV = 0,

            MTV = 1
        }

        public enum VehicleStatus : byte
        {
            Unknown = 0,

            FMC = 1,

            NMC = byte.MaxValue
        }

        public enum VehicleLocation : byte
        {
            [Display(Name = "Home Station")]
            HS,

            [Display(Name = "Forward Support Company")]
            FSC,

            [Display(Name = "Field Maintenance Shop")]
            FMS,

            [Display(Name = "Combined Support Maintenance Shop")]
            CSMS,

            [Display(Name = "Maneuver Area Training Equipment Site")]
            MATES,

            Other = byte.MaxValue
        }
    }
}