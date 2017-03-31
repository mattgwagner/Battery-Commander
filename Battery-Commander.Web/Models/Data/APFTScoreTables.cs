namespace BatteryCommander.Web.Models.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public class APFTScoreTables
    {
        private static IEnumerable<AgeGroup> AgeGroups { get { return Enum.GetValues(typeof(AgeGroup)).Cast<AgeGroup>(); } }

        public static IEnumerable<Entry> PushUps
        {
            get
            {
                using (var stream = typeof(APFTScoreTables).GetTypeInfo().Assembly.GetManifestResourceStream($"BatteryCommander.Web.Models.Data.{nameof(PushUps)}.csv"))
                using (var reader = new StreamReader(stream))
                {
                    string line = string.Empty;

                    while (!String.IsNullOrWhiteSpace(line = reader.ReadLine()))
                    {
                        if (line.Contains("Gender") || line.Contains("Reps")) continue;

                        var items = new Queue(line.Split(','));

                        var reps = Convert.ToInt32(items.Dequeue());

                        foreach (var group in AgeGroups)
                        {
                            foreach (var gender in new[] { Gender.Male, Gender.Female })
                            {
                                yield return new Entry
                                {
                                    Event = Event.PushUp,
                                    Gender = gender,
                                    AgeGroup = group,
                                    Reps = reps,
                                    Score = Convert.ToInt32(items.Dequeue())
                                };
                            }
                        }
                    }
                }
            }
        }

        public static IEnumerable<Entry> SitUps
        {
            get
            {
                using (var stream = typeof(APFTScoreTables).GetTypeInfo().Assembly.GetManifestResourceStream($"BatteryCommander.Web.Models.Data.{nameof(SitUps)}.csv"))
                using (var reader = new StreamReader(stream))
                {
                    string line = string.Empty;

                    while (!String.IsNullOrWhiteSpace(line = reader.ReadLine()))
                    {
                        if (line.Contains("Reps")) continue;

                        var items = new Queue(line.Split(','));

                        var reps = Convert.ToInt32(items.Dequeue());

                        foreach (var group in AgeGroups)
                        {
                            var score = Convert.ToInt32(items.Dequeue());

                            foreach (var gender in new[] { Gender.Male, Gender.Female })
                            {
                                yield return new Entry
                                {
                                    Event = Event.SitUp,
                                    Gender = gender,
                                    AgeGroup = group,
                                    Reps = reps,
                                    Score = score
                                };
                            }
                        }
                    }
                }
            }
        }

        public static IEnumerable<Entry> Run
        {
            get
            {
                using (var stream = typeof(APFTScoreTables).GetTypeInfo().Assembly.GetManifestResourceStream($"BatteryCommander.Web.Models.Data.{nameof(Run)}.csv"))
                using (var reader = new StreamReader(stream))
                {
                    string line = string.Empty;

                    while (!String.IsNullOrWhiteSpace(line = reader.ReadLine()))
                    {
                        if (line.Contains("Gender") || line.Contains("Time")) continue;

                        var items = new Queue(line.Split(','));

                        var raw = $"{items.Dequeue()}".Split(':');

                        var time = new TimeSpan(0, Convert.ToInt32(raw[0]), Convert.ToInt32(raw[1]));

                        foreach (var group in AgeGroups)
                        {
                            foreach (var gender in new[] { Gender.Male, Gender.Female })
                            {
                                yield return new Entry
                                {
                                    Event = Event.Run,
                                    Gender = gender,
                                    AgeGroup = group,
                                    Reps = (int)time.TotalSeconds,
                                    Score = Convert.ToInt32(items.Dequeue())
                                };
                            }
                        }
                    }
                }
            }
        }

        public class Entry
        {
            public AgeGroup AgeGroup { get; set; }

            public Gender Gender { get; set; }

            public Event Event { get; set; }

            // # of Push-ups/Sit-ups or Seconds on Run

            public int Reps { get; set; }

            public int Score { get; set; }
        }
    }
}