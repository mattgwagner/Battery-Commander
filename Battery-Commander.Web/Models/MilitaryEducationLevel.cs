using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatteryCommander.Web.Models
{
    public enum MilitaryEducationLevel : byte
    {
        Unknown,

        [Display(Name = "(AIT) Advanced Individual Training", ShortName = "AIT")]
        AIT = 1,

        [Display(Name = "(BLC) Basic Leader Course", ShortName = "BLC")]
        BLC = 2,

        [Display(Name = "(ALC) Advanced Leader Course", ShortName = "ALC")]
        ALC = 3,

        [Display(Name = "(SLC) Senior Leader Course", ShortName = "SLC")]
        SLC = 4,

        [Display(Name = "(MLC) Master Leader Course", ShortName = "MLC")]
        MLC = 5,

        [Display(Name = "(SMA) Sergeants Major Academy", ShortName = "SMA")]
        SMC = 6,

        [Display(Name = "(CST) Cadet Summer Training", ShortName = "CST")]
        CST = 7,

        [Display(Name = "(BOLC) Basic Officer Leaders Course", ShortName = "BOLC")]
        BOLC = 10,

        [Display(Name = "(CCC) Captains Career Course", ShortName = "CCC")]
        CCC = 11,

        [Display(Name = "(ILE) Intermediate Level Education", ShortName = "ILE")]
        ILE = 12,

        [Display(Name = "(AOC) Advanced Operations Course", ShortName = "AOC")]
        AOC = 13,

        [Display(Name = "(AWC) Army War College", ShortName = "AWC")]
        AWC = 14,

        [Display(Name = "(WOBC) Warrant Officer Basic Course", ShortName = "WOCS")]
        WOCS = 20,

        [Display(Name = "(WOAC) Warrant Officer Advanced Course", ShortName = "WOAC")]
        WOAC = 21,

        [Display(Name = "(WOILE) Warrant Intermediate Level Education", ShortName = "WOILE")]
        WOILE= 22,

        [Display(Name = "(WOSSE) Warrant Senior Service Education Course", ShortName = "WOSSE")]
        WOSSE = 23
    }

    public static class EducationExtensions
    {
        public static MilitaryEducationLevel RequiredEducation(this Rank rank)
        {
            switch (rank)
            {
                // Do Warrant Officers have requirements? Yes, https://usacac.army.mil/organizations/cace/wocc/courses and https://mil.wa.gov/asset/5ba421df6ca19

                case Rank.WO1:
                    return MilitaryEducationLevel.WOCS;

                case Rank.WO2:
                    return MilitaryEducationLevel.WOAC;

                case Rank.WO3:
                    return MilitaryEducationLevel.WOILE;

                case Rank.WO4:
                case Rank.WO5:
                    return MilitaryEducationLevel.WOSSE;

                case Rank.E5:
                    return MilitaryEducationLevel.BLC;

                case Rank.E6:
                    return MilitaryEducationLevel.ALC;

                case Rank.E7:
                    return MilitaryEducationLevel.SLC;

                case Rank.E8:
                case Rank.E8_MSG:
                    return MilitaryEducationLevel.MLC;

                case Rank.E9:
                    return MilitaryEducationLevel.SMC;

                case Rank.CDT:
                    return MilitaryEducationLevel.CST;

                case Rank.O1:
                case Rank.O2:
                    return MilitaryEducationLevel.BOLC;

                case Rank.O3:
                    return MilitaryEducationLevel.CCC;

                case Rank.O4:
                    return MilitaryEducationLevel.ILE;

                case Rank.O5:
                    return MilitaryEducationLevel.AOC;

                case Rank.O6:
                    return MilitaryEducationLevel.AWC;

                default:
                    return MilitaryEducationLevel.AIT;
            }
        }
    }

    public partial class Soldier
    {
        /// <summary>
        /// The highest level of military education attained by the Soldier
        /// </summary>
        [Display(Name = "Education Level")]
        public MilitaryEducationLevel EducationLevel { get; set; } = MilitaryEducationLevel.Unknown;

        [NotMapped]
        public virtual Boolean IsEducationComplete => Rank.RequiredEducation() <= EducationLevel;
    }
}