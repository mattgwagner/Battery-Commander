using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BatteryCommander.Web.Models
{
    public enum Rank : byte
    {
        [Display(Name = "Cadet", ShortName = "CDT")]
        CDT = 0,

        [Display(Name = "Private", ShortName = "PV1")]
        E1 = 1,

        [Display(Name = "Private (PV2)", ShortName = "PV2")]
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

        [Display(Name = "Sergeant Major", ShortName = "SGM")]
        E9 = 9,

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
        O6 = 15,

        [Display(Name = "Master Sergeant", ShortName = "MSG")]
        E8_MSG = 16,

        [Display(Name = "Warrant Officer", ShortName = "WO1")]
        WO1 = 17,

        [Display(Name = "Chief Warrant Officer", ShortName = "WO2")]
        WO2 = 18,

        [Display(Name = "Chief Warrant Officer", ShortName = "WO3")]
        WO3 = 19,

        [Display(Name = "Chief Warrant Officer", ShortName = "WO4")]
        WO4 = 20,

        [Display(Name = "Chief Warrant Officer", ShortName = "WO5")]
        WO5 = 21,

        [Display(Name = "Corporal", ShortName = "CPL")]
        E4_CPL = 22
    }

    public partial class Soldier
    {
        [Required]
        public Rank Rank { get; set; } = Rank.E1;

        [NotMapped]
        public String RankHumanized => Rank.ShortName();

        [NotMapped]
        public Boolean IsEnlisted => Rank.IsEnlisted();

        [NotMapped]
        public Boolean IsNCO => Rank.IsNCO();

        [NotMapped]
        public Boolean IsOfficer => Rank.IsOfficer();

        [NotMapped]
        public Boolean IsCadet => Rank.CDT == Rank;
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

        public static IEnumerable<Rank> Officers => All().Where(rank => rank.IsOfficer());

        public static IEnumerable<Rank> Enlisted => All().Where(rank => rank.IsEnlisted());

        public static IEnumerable<Rank> All()
        {
            foreach (Rank rank in Enum.GetValues(typeof(Rank)))
            {
                yield return rank;
            }
        }

        public static Boolean IsEnlisted(this Rank rank)
        {
            switch (rank)
            {
                case Rank.E1:
                case Rank.E2:
                case Rank.E3:
                case Rank.E4:
                case Rank.E4_CPL:
                case Rank.E5:
                case Rank.E6:
                case Rank.E7:
                case Rank.E8:
                case Rank.E8_MSG:
                case Rank.E9:
                    return true;

                default:
                    return false;
            }
        }

        public static Boolean IsNCO(this Rank rank)
        {
            switch (rank)
            {
                case Rank.E4_CPL:
                case Rank.E5:
                case Rank.E6:
                case Rank.E7:
                case Rank.E8:
                case Rank.E8_MSG:
                case Rank.E9:
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