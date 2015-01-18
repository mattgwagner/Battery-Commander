using System.ComponentModel.DataAnnotations;

namespace BatteryCommander.Common.Models
{
    public enum WorkItemStatus
    {
        [Display(Name = "Unknown")]
        Unknown = 0,

        // TODO Waiting?

        // TODO Assigned?

        Completed = 10
    }
}