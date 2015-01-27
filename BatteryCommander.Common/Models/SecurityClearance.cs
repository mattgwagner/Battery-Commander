using System.ComponentModel.DataAnnotations;

namespace BatteryCommander.Common.Models
{
    public enum SecurityClearance
    {
        None,

        Secret,

        [Display(Name = "Top Secret")]
        TopSecret
    }
}