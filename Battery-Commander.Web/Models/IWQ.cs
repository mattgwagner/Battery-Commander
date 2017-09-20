using System;
using System.ComponentModel.DataAnnotations;

namespace BatteryCommander.Web.Models
{
    public partial class Soldier
    {
        //[DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        //public DateTime? IwqQualificationDate { get; set; }

        //public TimeSpan? IwqQualificationAge => (DateTime.Today - IwqQualificationDate);

        //[Display(Name = "DSCA Qualified?")]
        //public Boolean IwqQualified => IwqQualificationAge.HasValue && IwqQualificationAge < TimeSpan.FromDays(365);
    }

    public enum Weapon : byte
    {
        M4,

        M9,

        M240B,

        M249,

        M320
    }

    public enum WeaponQualificationStatus : byte
    {
        Unqualified,

        Marksman,

        Sharpshooter,

        Expert
    }
}
