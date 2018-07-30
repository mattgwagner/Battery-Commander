namespace BatteryCommander.Web.Models
{
    public enum EventStatus
    {
        /// <summary>
        /// Soldier either does not have a record for this event, or the time validity of the event has passed
        /// </summary>
        NotTested,

        /// <summary>
        /// Soldier has a passing record for this event within a valid period of time
        /// </summary>
        Passed,

        /// <summary>
        /// Soldier's most recent record for this event was a failure
        /// </summary>
        Failed
    }
}