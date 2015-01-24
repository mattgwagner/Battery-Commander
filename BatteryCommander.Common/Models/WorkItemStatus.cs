using System.ComponentModel.DataAnnotations;

namespace BatteryCommander.Common.Models
{
    public enum WorkItemStatus : byte
    {
        Unknown = 0,

        // TODO Waiting?

        // TODO Assigned?

        Completed = 10
    }
}