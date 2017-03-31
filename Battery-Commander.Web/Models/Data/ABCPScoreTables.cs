using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BatteryCommander.Web.Models.Data
{
    public class ABCPScoreTables
    {
        private static IEnumerable<ABCPAgeGroup> AgeGroups { get { return Enum.GetValues(typeof(ABCPAgeGroup)).Cast<ABCPAgeGroup>(); } }

        public static IEnumerable<Entry> ScreeningWeights
        {
            get
            {
                using (var stream = typeof(ABCPScoreTables).GetTypeInfo().Assembly.GetManifestResourceStream($"BatteryCommander.Web.Models.Data.{nameof(ABCP)}.csv"))
                using (var reader = new StreamReader(stream))
                {
                    string line = string.Empty;

                    while (!String.IsNullOrWhiteSpace(line = reader.ReadLine()))
                    {
                        if (line.Contains("Gender") || line.Contains("Height")) continue;

                        var items = new Queue(line.Split(','));

                        var height = Convert.ToInt32(items.Dequeue());

                        foreach (var group in AgeGroups)
                        {
                            foreach (var gender in new[] { Gender.Male, Gender.Female })
                            {
                                yield return new Entry
                                {
                                    Gender = gender,
                                    AgeGroup = group,
                                    Height = height,
                                    Weight = Convert.ToInt32(items.Dequeue())
                                };
                            }
                        }
                    }
                }
            }
        }

        public class Entry
        {
            public Gender Gender { get; set; }

            public ABCPAgeGroup AgeGroup { get; set; }

            public int Height { get; set; }

            public int Weight { get; set; }
        }
    }
}