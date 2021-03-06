﻿using Humanizer;
using System;
using System.Collections.Generic;
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

        public int? Occupancy => Passengers?.Count + (DriverId.HasValue ? 1 : 0) + (A_DriverId.HasValue ? 1 : 0);

        [Display(Name = "Capacity")]
        public int TotalCapacity => Seats + TroopCapacity;

        public virtual ICollection<Passenger> Passengers { get; set; }

        public VehicleChalk Chalk { get; set; } = VehicleChalk.Unknown;

        [Display(Name = "Order of March")]
        public int OrderOfMarch { get; set; } = 0;

        public String Notes { get; set; }

        public String GoogleSearchUrl => String.IsNullOrWhiteSpace(Nomenclature) ? String.Empty : $"https://www.google.com/search?q={Nomenclature}";

        public DateTimeOffset LastUpdate { get; set; } = DateTimeOffset.UtcNow;

        [NotMapped, Display(Name = "Last Updated")]
        public String LastUpdateHumanized => (LastUpdate - DateTime.UtcNow).Humanize(maxUnit: Humanizer.Localisation.TimeUnit.Day);

        public class Passenger
        {
            public int VehicleId { get; set; }

            public virtual Vehicle Vehicle { get; set; }

            public int SoldierId { get; set; }

            public virtual Soldier Soldier { get; set; }
        }

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

        public enum VehicleChalk : byte
        {
            [Display(Name = "")]
            Unknown = 0,

            [Display(Name = "Chalk 1")]
            Chalk_1 = 1,

            [Display(Name = "Chalk 2")]
            Chalk_2 = 2,

            [Display(Name = "Chalk 3")]
            Chalk_3 = 3,

            [Display(Name = "Chalk 4")]
            Chalk_4 = 4,

            [Display(Name = "Chalk 5")]
            Chalk_5
        }
    }
}