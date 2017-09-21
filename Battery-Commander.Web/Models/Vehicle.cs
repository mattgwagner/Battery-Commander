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

        [Required]
        public VehicleType Type { get; set; } = VehicleType.HMMWV;

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

        // TroopCapacity?

        // Chalk Order?

        // LIN?

        // Fuel Card? Towbar? Water Buffalo?

        // Fuel Level?

        // Driver, A-Driver, Passengers, Assigned Section?

        // Has JBC-P?

        // Location?
        
        public String Notes { get; set; }

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
    }
}