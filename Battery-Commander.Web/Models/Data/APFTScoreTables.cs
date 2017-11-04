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
        private static IEnumerable<APFTAgeGroup> AgeGroups { get { return Enum.GetValues(typeof(APFTAgeGroup)).Cast<APFTAgeGroup>(); } }

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

        public static IEnumerable<Entry> Walk
        {
            get
            {
                return new[]
                {
                    new Entry { AgeGroup = APFTAgeGroup.Group_17_to_21, Gender = Gender.Male, Reps = (int)TimeSpan.FromMinutes(34).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_22_to_26, Gender = Gender.Male, Reps = (int)TimeSpan.FromMinutes(34.5).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_27_to_31, Gender = Gender.Male, Reps = (int)TimeSpan.FromMinutes(35).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_32_to_36, Gender = Gender.Male, Reps = (int)TimeSpan.FromMinutes(35.5).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_37_to_41, Gender = Gender.Male, Reps = (int)TimeSpan.FromMinutes(36).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_42_to_46, Gender = Gender.Male, Reps = (int)TimeSpan.FromMinutes(36.5).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_47_to_51, Gender = Gender.Male, Reps = (int)TimeSpan.FromMinutes(37).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_52_to_56, Gender = Gender.Male, Reps = (int)TimeSpan.FromMinutes(37.5).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_57_to_61, Gender = Gender.Male, Reps = (int)TimeSpan.FromMinutes(38).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_62_Plus, Gender = Gender.Male, Reps = (int)TimeSpan.FromMinutes(38.5).TotalSeconds },

                    new Entry { AgeGroup = APFTAgeGroup.Group_17_to_21, Gender = Gender.Female, Reps = (int)TimeSpan.FromMinutes(37).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_22_to_26, Gender = Gender.Female, Reps = (int)TimeSpan.FromMinutes(37.5).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_27_to_31, Gender = Gender.Female, Reps = (int)TimeSpan.FromMinutes(38).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_32_to_36, Gender = Gender.Female, Reps = (int)TimeSpan.FromMinutes(38.5).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_37_to_41, Gender = Gender.Female, Reps = (int)TimeSpan.FromMinutes(39).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_42_to_46, Gender = Gender.Female, Reps = (int)TimeSpan.FromMinutes(39.5).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_47_to_51, Gender = Gender.Female, Reps = (int)TimeSpan.FromMinutes(40).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_52_to_56, Gender = Gender.Female, Reps = (int)TimeSpan.FromMinutes(40.5).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_57_to_61, Gender = Gender.Female, Reps = (int)TimeSpan.FromMinutes(41).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_62_Plus, Gender = Gender.Female, Reps = (int)TimeSpan.FromMinutes(41.5).TotalSeconds }
                };
            }
        }

        public static IEnumerable<Entry> Bicycle
        {
            get
            {
                return new[]
                {
                    new Entry { AgeGroup = APFTAgeGroup.Group_17_to_21, Gender = Gender.Male, Reps = (int)TimeSpan.FromMinutes(24).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_22_to_26, Gender = Gender.Male, Reps = (int)TimeSpan.FromMinutes(24.5).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_27_to_31, Gender = Gender.Male, Reps = (int)TimeSpan.FromMinutes(25).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_32_to_36, Gender = Gender.Male, Reps = (int)TimeSpan.FromMinutes(25.5).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_37_to_41, Gender = Gender.Male, Reps = (int)TimeSpan.FromMinutes(26).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_42_to_46, Gender = Gender.Male, Reps = (int)TimeSpan.FromMinutes(27).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_47_to_51, Gender = Gender.Male, Reps = (int)TimeSpan.FromMinutes(28).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_52_to_56, Gender = Gender.Male, Reps = (int)TimeSpan.FromMinutes(30).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_57_to_61, Gender = Gender.Male, Reps = (int)TimeSpan.FromMinutes(31).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_62_Plus, Gender = Gender.Male, Reps = (int)TimeSpan.FromMinutes(32).TotalSeconds },

                    new Entry { AgeGroup = APFTAgeGroup.Group_17_to_21, Gender = Gender.Female, Reps = (int)TimeSpan.FromMinutes(25).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_22_to_26, Gender = Gender.Female, Reps = (int)TimeSpan.FromMinutes(25.5).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_27_to_31, Gender = Gender.Female, Reps = (int)TimeSpan.FromMinutes(26).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_32_to_36, Gender = Gender.Female, Reps = (int)TimeSpan.FromMinutes(26.5).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_37_to_41, Gender = Gender.Female, Reps = (int)TimeSpan.FromMinutes(27).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_42_to_46, Gender = Gender.Female, Reps = (int)TimeSpan.FromMinutes(28).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_47_to_51, Gender = Gender.Female, Reps = (int)TimeSpan.FromMinutes(30).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_52_to_56, Gender = Gender.Female, Reps = (int)TimeSpan.FromMinutes(32).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_57_to_61, Gender = Gender.Female, Reps = (int)TimeSpan.FromMinutes(33).TotalSeconds },
                    new Entry { AgeGroup = APFTAgeGroup.Group_62_Plus, Gender = Gender.Female, Reps = (int)TimeSpan.FromMinutes(34).TotalSeconds }
                };
            }
        }

        public static IEnumerable<Entry> Swim
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public class Entry
        {
            public APFTAgeGroup AgeGroup { get; set; }

            public Gender Gender { get; set; }

            // # of Push-ups/Sit-ups or Seconds on Run

            public int Reps { get; set; }

            public int Score { get; set; }
        }
    }
}