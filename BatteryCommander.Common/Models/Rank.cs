using System.ComponentModel.DataAnnotations;

namespace BatteryCommander.Common.Models
{
    public enum Rank
    {
        [Display(Name = "PVT")]
        E1 = 1,

        [Display(Name = "PV2")]
        E2 = 2,

        [Display(Name = "PFC")]
        E3 = 3,

        [Display(Name = "SPC")]
        E4 = 4,

        [Display(Name = "SGT")]
        E5 = 5,

        [Display(Name = "SSG")]
        E6 = 6,

        [Display(Name = "SFC")]
        E7 = 7,

        [Display(Name = "1SG")]
        E8 = 8,

        Cadet = 9,

        [Display(Name = "2LT")]
        O1 = 10,

        [Display(Name = "1LT")]
        O2 = 11,

        [Display(Name = "CPT")]
        O3 = 12,

        [Display(Name = "MAJ")]
        O4 = 13,

        [Display(Name = "LTC")]
        O5 = 14,

        [Display(Name = "COL")]
        O6 = 15
    }
}