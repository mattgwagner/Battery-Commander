using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatteryCommander.Web.Models
{
    public enum EventStatus
    {
        // Soldier either does not have a record for this event, or the time validity of the event has passed
        NotTested,

        // Soldier has a passing record for this event within a valid period of time
        Passed,

        // Soldier's most recent record for this event was a failure
        Failed
    }
}
        
