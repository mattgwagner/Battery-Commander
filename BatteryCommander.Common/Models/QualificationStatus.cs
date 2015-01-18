using System.ComponentModel.DataAnnotations;

namespace BatteryCommander.Common.Models
{
    public enum QualificationStatus
    {
        [Display(Name = "Unknown")]
        Unknown = 0,

        Pass = 1,

        Fail = 2,

        [Display(Name = "Not Applicable")]
        NotApplicable = 10
    }
}