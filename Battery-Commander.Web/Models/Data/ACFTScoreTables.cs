using System;

namespace BatteryCommander.Web.Models.Data
{
    public static class ACFTScoreTables
    {
        // Current Scoring Tables as of HQDA EXORD 219-18 for FY20

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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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