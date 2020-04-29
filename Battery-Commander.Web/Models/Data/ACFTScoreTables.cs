using System;

namespace BatteryCommander.Web.Models.Data
{
    public static class ACFTScoreTables
    {
        // Current Scoring Tables as of HQDA EXORD 219-18 for FY20

        public static int MaximumDeadLift(int weight_in_lbs)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}