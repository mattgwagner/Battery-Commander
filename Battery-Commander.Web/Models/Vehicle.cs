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

        // Notes, What's broken about it?

        [Required]
        public VehicleStatus Status { get; set; } = VehicleStatus.FMC;

        [Required, StringLength(10)]
        public String Bumper { get; set; }

        [Required]
        public VehicleType Type { get; set; } = VehicleType.HMMWV;

        // public String Registration { get; set; }

        // public String Serial { get; set; }

        [Required]
        public int Seats { get; set; } = 2;

        // TroopCapacity?

        // Chalk Order?

        // LIN?

        // Fuel Card? Towbar? Water Buffalo?

        // Fuel Level?

        // Driver, A-Driver, Passengers, Assigned Section?

        // Has JBC-P?

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