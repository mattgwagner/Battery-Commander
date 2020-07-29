using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BatteryCommander.Web.Models.Data
{
    public static class ACFTScoreTables
    {
        // Current Scoring Tables as of HQDA EXORD 219-18 for FY20

        public static IEnumerable<SprintDragCarryScoreRow> Data
        {
            get
            {
                using (var stream = typeof(ACFTScoreTables).GetTypeInfo().Assembly.GetManifestResourceStream($"Battery-Commander.Web.Models.Data.{nameof(ACFTScoreTables)}.csv"))
                using (var reader = new StreamReader(stream))
                {
                    string line = string.Empty;

                    while (!String.IsNullOrWhiteSpace(line = reader.ReadLine()))
                    {
                        if (line.Contains("3RM")) continue;

                        var items = line.Split(',');

                        if (!string.IsNullOrWhiteSpace(items[9]))
                        {
                            // 9 time stored as -335 representing 3m 35s
                            // 10 points

                            var m = int.Parse($"{items[9][1]}");
                            var s = int.Parse(items[9].Substring(2));
                            var p = int.Parse(items[10]);

                            yield return new SprintDragCarryScoreRow
                            {
                                // This is hacky AF

                                Duration = new TimeSpan(hours: 0, minutes: int.Parse($"{items[9][1]}"), seconds: int.Parse(items[9].Substring(2))),
                                Points = int.Parse(items[10])
                            };
                        }
                    }
                }
            }
        }

        public class SprintDragCarryScoreRow
        {
            public TimeSpan Duration { get; set; }

            public int Points { get; set; }
        }

        public static int MaximumDeadLift(int weight_in_lbs)
        {
            if (weight_in_lbs >= 340) return 100;

            if (weight_in_lbs >= 200)
            {
                // For every 10 #s between 200 and 340, add 2 points

                double diff = weight_in_lbs - 200;

                var rows = (int)Math.Ceiling(diff / 10);

                return 70 + (rows * 2);
            }

            if (weight_in_lbs >= 190) return 68;
            if (weight_in_lbs >= 180) return 65;
            if (weight_in_lbs >= 170) return 64;
            if (weight_in_lbs >= 160) return 63;
            if (weight_in_lbs >= 150) return 62;

            if (weight_in_lbs >= 140) return 60;
            if (weight_in_lbs >= 130) return 50;
            if (weight_in_lbs >= 120) return 40;
            if (weight_in_lbs >= 110) return 30;
            if (weight_in_lbs >= 100) return 20;
            if (weight_in_lbs >= 90) return 10;

            return 0;
        }

        public static int StandingPowerThrow(decimal distance_in_meters)
        {
            if (distance_in_meters <= 3.3m) return 0;

            return distance_in_meters switch
            {
                3.4m => 5,
                3.5m => 10,
                3.6m => 15,
                3.7m => 20,
                3.8m => 25,
                3.9m => 30,
                4.0m => 35,
                4.1m => 40,
                4.2m => 45,
                4.3m => 50,
                4.4m => 55,
                4.5m => 60,
                4.6m => 60,
                4.7m => 60,
                4.8m => 60,
                4.9m => 61,
                5.0m => 61,
                5.1m => 61,
                5.2m => 61,
                5.3m => 61,
                5.4m => 62,
                5.5m => 62,
                5.6m => 62,
                5.7m => 62,
                5.8m => 63,
                5.9m => 63,
                6.0m => 63,
                6.1m => 63,
                6.2m => 64,
                6.3m => 64,
                6.4m => 64,
                6.5m => 65,
                6.6m => 65,
                6.7m => 65,
                6.8m => 66,
                6.9m => 66,
                7.0m => 66,
                7.1m => 67,
                7.2m => 67,
                7.3m => 67,
                7.4m => 67,
                7.5m => 68,
                7.6m => 68,
                7.7m => 68,
                7.8m => 69,
                7.9m => 69,
                8.0m => 70,
                8.1m => 70,
                8.2m => 71,
                8.3m => 72,
                8.4m => 72,
                8.5m => 73,
                8.6m => 74,
                8.7m => 74,
                8.8m => 75,
                8.9m => 76,
                9.0m => 76,
                9.1m => 77,
                9.2m => 78,
                9.3m => 78,
                9.4m => 78,
                9.5m => 80,
                9.6m => 80,
                9.7m => 81,
                9.8m => 82,
                9.9m => 83,
                10.0m => 83,
                10.1m => 84,
                10.2m => 84,
                10.3m => 85,
                10.4m => 86,
                10.5m => 86,
                10.6m => 87,
                10.7m => 88,
                10.8m => 88,
                10.9m => 89,
                11.0m => 90,
                11.1m => 90,
                11.2m => 91,
                11.3m => 92,
                11.4m => 92,
                11.5m => 93,
                11.6m => 94,
                11.7m => 94,
                11.8m => 95,
                11.9m => 96,
                12.0m => 96,
                12.1m => 97,
                12.2m => 98,
                12.3m => 98,
                12.4m => 99,

                // 12.5m+ is 100 points

                _ => 100
            };
        }

        public static int HandReleasePushUps(int reps)
        {
            if (reps == 0) return 0;

            if (reps >= 60) return 100;

            if (reps >= 30) return 70 + (reps - 30);

            if (reps >= 10) return 10 + (reps - 10) * 2;

            return reps * 5 + 10;
        }

        public static int SprintDragCarry(TimeSpan duration)
        {
            return 
                Data
                .OrderBy(entry => entry.Duration)
                .Where(entry => duration <= entry.Duration)
                .Select(entry => entry.Points)
                .FirstOrDefault();
        }            

        public static int LegTuck(int reps)
        {
            if (reps == 0) return 0;

            return reps switch
            {
                1 => 60,
                2 => 62,
                3 => 65,
                4 => 68,
                5 => 70,
                6 => 72,
                7 => 74,
                8 => 76,
                9 => 78,
                10 => 80,
                11 => 82,
                12 => 84,
                13 => 86,
                14 => 88,
                15 => 90,
                16 => 92,
                17 => 94,
                18 => 96,
                19 => 98,
                _ => 100 // Anything above 19 is 100 PTS
            };
        }

        public static int TwoMileRun(TimeSpan duration)
        {
            if (duration >= new TimeSpan(0, 22, 48)) return 0;

            var top_score = new TimeSpan(0, minutes: 13, seconds: 30);

            // For every 9 seconds after the top time, we subtract 1 point

            if (duration < top_score) return 100;

            if (duration <= new TimeSpan(0, 18, 0))
            {
                var over_1330 = duration - top_score;

                var rows = (int)Math.Ceiling(over_1330.TotalSeconds / 9);

                return 100 - rows;
            }

            if (duration <= new TimeSpan(0, 18, 12)) return 69;
            if (duration <= new TimeSpan(0, 18, 24)) return 68;
            if (duration <= new TimeSpan(0, 18, 36)) return 67;
            if (duration <= new TimeSpan(0, 18, 48)) return 66;
            if (duration <= new TimeSpan(0, 19, 0)) return 65;

            if (duration <= new TimeSpan(0, 19, 24)) return 64;
            if (duration <= new TimeSpan(0, 19, 48)) return 63;
            if (duration <= new TimeSpan(0, 20, 12)) return 62;
            if (duration <= new TimeSpan(0, 20, 36)) return 61;
            if (duration <= new TimeSpan(0, 21, 0)) return 60;

            // TODO Implement 21:01 -> 22:48

            return 0;
        }
    }
}