using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatteryCommander.Web.Models
{
    [ComplexType]
    public class SSD_Completion
    {
        public DateTime? Updated { get; set; } = DateTime.UtcNow;

        public Decimal SSD1 { get; set; }

        public Decimal SSD2 { get; set; }

        public Decimal SSD3 { get; set; }

        public Decimal SSD4 { get; set; }

        public Decimal SSD5 { get; set; }
    }
}