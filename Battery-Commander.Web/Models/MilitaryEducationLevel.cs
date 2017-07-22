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

        [Display(Name = "(SMC) Sergeants Major Course", ShortName = "SMC")]
        SMC = 6,

        [Display(Name = "(BOLC) Basic Officer Leaders Course", ShortName = "BOLC")]
        BOLC = 10,

        [Display(Name = "(CCC) Captains Career Course", ShortName = "CCC")]
        CCC = 11
    }

    public static class EducationExtensions
    {
        public static MilitaryEducationLevel RequiredEducation(this Rank rank)
        {
            switch (rank)
            {
                case Rank.E5:
                    return MilitaryEducationLevel.BLC;

                case Rank.E6:
                    return MilitaryEducationLevel.ALC;

                case Rank.E7:
                    return MilitaryEducationLevel.SLC;

                case Rank.E8:
                    return MilitaryEducationLevel.MLC;

                case Rank.O1:
                case Rank.O2:
                    return MilitaryEducationLevel.BOLC;

                case Rank.O3:
                    return MilitaryEducationLevel.CCC;

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