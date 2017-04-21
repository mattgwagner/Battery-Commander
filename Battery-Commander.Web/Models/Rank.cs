using System;
using System.ComponentModel.DataAnnotations;

namespace BatteryCommander.Web.Models
{
    public enum Rank : byte
    {
        [Display(Name = "Private", ShortName = "PVT")]
        E1 = 1,

        [Display(Name = "Private", ShortName = "PV2")]
        E2 = 2,

        [Display(Name = "Private First Class", ShortName = "PFC")]
        E3 = 3,

        [Display(Name = "Specialist", ShortName = "SPC")]
        E4 = 4,

        [Display(Name = "Sergeant", ShortName = "SGT")]
        E5 = 5,

        [Display(Name = "Staff Sergeant", ShortName = "SSG")]
        E6 = 6,

        [Display(Name = "Sergeant First Class", ShortName = "SFC")]
        E7 = 7,

        [Display(Name = "First Sergeant", ShortName = "1SG")]
        E8 = 8,

        [Display(Name = "Cadet", ShortName = "CDT")]
        Cadet = 9,

        [Display(Name = "Second Lieutenant", ShortName = "2LT")]
        O1 = 10,

        [Display(Name = "First Lieutenant", ShortName = "1LT")]
        O2 = 11,

        [Display(Name = "Captain", ShortName = "CPT")]
        O3 = 12,

        [Display(Name = "Major", ShortName = "MAJ")]
        O4 = 13,

        [Display(Name = "Lieutenant Colonel", ShortName = "LTC")]
        O5 = 14,

        [Display(Name = "Colonel", ShortName = "COL")]
        O6 = 15
    }

    public static class RankExtensions
    {
        public static Rank Parse(String value)
        {
            Rank result = Rank.E1;

            if (!Enum.TryParse(value, ignoreCase: true, result: out result))
            {
                // Couldn't parse rank strictly by string

                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    if (String.Equals(rank.DisplayName(), value, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return rank;
                    }

                    if (String.Equals(rank.ShortName(), value, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return rank;
                    }
                }
            }

            return result;
        }

        public static Boolean IsNCO(this Rank rank)
        {
            switch (rank)
            {
                case Rank.E5:
                case Rank.E6:
                case Rank.E7:
                case Rank.E8:
                    return true;

                default:
                    return false;
            }
        }

        public static Boolean IsOfficer(this Rank rank)
        {
            switch (rank)
            {
                case Rank.O1:
                case Rank.O2:
                case Rank.O3:
                case Rank.O4:
                case Rank.O5:
                case Rank.O6:
                    return true;

                default:
                    return false;
            }
        }
    }
}