using BatteryCommander.Web.Models.Data;
using BatteryCommander.Web.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BatteryCommander.Web.Models
{
    public enum SSD : byte
    {
        SSD_1,

        SSD_2,

        SSD_3,

        SSD_4,

        SSD_5
    }


    public partial class Soldier
    {
        public virtual ICollection<SSDSnapshot> SSDSnapshots { get; set; }

        public virtual SSDStatusModel SSDStatus
        {
            get
            {
                return new SSDStatusModel
                {
                    AsOf = 
                        SSDSnapshots
                        .Select(snapshot => snapshot.AsOf)
                        .Max(),

                    SSD_1 =
                        SSDSnapshots
                        .OrderByDescending(snapshot => snapshot.AsOf)
                        .Where(snapshot => snapshot.SSD == SSD.SSD_1)
                        .Select(snapshot => (decimal?)snapshot.PerecentComplete)
                        .FirstOrDefault(),

                    SSD_2 =
                        SSDSnapshots
                        .OrderByDescending(snapshot => snapshot.AsOf)
                        .Where(snapshot => snapshot.SSD == SSD.SSD_2)
                        .Select(snapshot => (decimal?)snapshot.PerecentComplete)
                        .FirstOrDefault(),

                    SSD_3 =
                        SSDSnapshots
                        .OrderByDescending(snapshot => snapshot.AsOf)
                        .Where(snapshot => snapshot.SSD == SSD.SSD_3)
                        .Select(snapshot => (decimal?)snapshot.PerecentComplete)
                        .FirstOrDefault(),

                    SSD_4 =
                        SSDSnapshots
                        .OrderByDescending(snapshot => snapshot.AsOf)
                        .Where(snapshot => snapshot.SSD == SSD.SSD_4)
                        .Select(snapshot => (decimal?)snapshot.PerecentComplete)
                        .FirstOrDefault(),

                    SSD_5 =
                        SSDSnapshots
                        .OrderByDescending(snapshot => snapshot.AsOf)
                        .Where(snapshot => snapshot.SSD == SSD.SSD_5)
                        .Select(snapshot => (decimal?)snapshot.PerecentComplete)
                        .FirstOrDefault()
                };
            }
        }

        public class SSDSnapshot
        {
            public DateTimeOffset AsOf { get; set;} = DateTimeOffset.UtcNow;

            public SSD SSD { get; set; } = SSD.SSD_1;

            public decimal PerecentComplete { get; set; }
        }

        public class SSDStatusModel
        {
            public DateTimeOffset AsOf { get; set; }

            // Humanized time since

            public decimal? SSD_1 { get; set; }

            public decimal? SSD_2 { get; set; }

            public decimal? SSD_3 { get; set; }

            public decimal? SSD_4 { get; set; }

            public decimal? SSD_5 { get; set; }
        }
    }
}