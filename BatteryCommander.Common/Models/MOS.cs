using System.ComponentModel.DataAnnotations;

namespace BatteryCommander.Common.Models
{
    public enum MOS
    {
        Unknown = 0,

        [Display(Name = "13A Field Artillery Officer")]
        FA_13A = 131,

        [Display(Name = "13B Cannon Crewmember")]
        FA_13B = 132,

        [Display(Name = "13D Field Artillery Tactical Data Systems Specialist")]
        FA_13D = 134,

        [Display(Name = "13E Cannon Fire Direction Specialist")]
        FA_13E = 135,

        [Display(Name = "13F Fire Support Specialist")]
        FA_13F = 136,

        [Display(Name = "13Z Field Artillery Senior Sergeant")]
        FA_13Z = 139,

        [Display(Name = "25U Signal Support Systems Specialist")]
        SC_25U = 258,

        [Display(Name = "92Y Unit Supply Specialist")]
        SY_92Y = 928,

        Other = 1000,
    }
}