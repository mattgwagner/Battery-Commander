﻿using System;
using System.ComponentModel.DataAnnotations;

namespace BatteryCommander.Web.Models
{
    public partial class Soldier
    {
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? IwqQualificationDate { get; set; }

        public TimeSpan? IwqQualificationAge => (DateTime.Today - IwqQualificationDate);

        [Display(Name = "IWQ")]
        public Boolean IwqQualified => IwqQualificationAge.HasValue && IwqQualificationAge < TimeSpan.FromDays(Soldier.DaysPerYear);
    }

    public enum WeaponQualificationStatus : byte
    {
        // Not used yet

        Unqualified,

        Marksman,

        Sharpshooter,

        Expert
    }
}