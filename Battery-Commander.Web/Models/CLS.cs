using System;
using System.ComponentModel.DataAnnotations;

namespace BatteryCommander.Web.Models
{
    public partial class Soldier
    {
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? ClsQualificationDate { get; set; }

        public TimeSpan? ClsQualificationAge => (DateTime.Today - ClsQualificationDate);

        /// <summary>
        /// While a CLS certification is technically permanent, soldiers in Priority 1 units (actively-deploying brigade combat teams, for example)
        /// must retake the course once a year to retain their certification.
        /// </summary>
        [Display(Name = "CLS")]
        public Boolean ClsQualified => ClsQualificationAge.HasValue && ClsQualificationAge < TimeSpan.FromDays(Soldier.DaysPerYear);
    }
}