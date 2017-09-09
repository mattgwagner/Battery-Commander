using System;
using System.ComponentModel.DataAnnotations;

namespace BatteryCommander.Web.Models
{
    public partial class Soldier
    {
        [DataType(DataType.Date)]
        public DateTime? DscaQualificationDate { get; set; }

        public TimeSpan? DscaQualificationAge => (DateTime.Today - DscaQualificationDate);

        [Display(Name = "DSCA Qualified?")]
        public Boolean DscaQualified => DscaQualificationAge.HasValue && DscaQualificationAge < TimeSpan.FromDays(365);
    }
}
