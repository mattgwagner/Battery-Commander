using System.ComponentModel.DataAnnotations;

namespace BatteryCommander.Common.Models
{
    public enum SecurityClearance
    {
        [Display(ShortName = "N")]
        None,

        [Display(ShortName = "S")]
        Secret,

        [Display(Name = "Top Secret", ShortName = "TS")]
        TopSecret
    }
}