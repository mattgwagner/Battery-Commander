using BatteryCommander.Web.Models.Data;
using BatteryCommander.Web.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BatteryCommander.Web.Models
{
    public partial class Soldier
    {
        public virtual ICollection<APFT> APFTs { get; set; }

        public virtual APFT LastApft => APFTs?.OrderByDescending(apft => apft.Date).FirstOrDefault();

        public virtual EventStatus ApftStatus
        {
            get
            {
                if (LastApft?.IsValid == true)
                {
                    if (LastApft?.IsPassing == true) return EventStatus.Passed;

                    if (LastApft?.IsPassing == false) return EventStatus.Failed;
                }

                return EventStatus.NotTested;
            }
        }
    }

    public class APFT : IValidatableObject
    {
        private const int MinimumPerEvent = 60;

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int SoldierId { get; set; }

        public virtual Soldier Soldier { get; set; }

        // Grader?

        [Required, DataType(DataType.Date), Column(TypeName = "date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        public DateTime ValidThru => Date.AddDays(Soldier.DaysPerYear);

        public Boolean IsValid => DateTime.Today <= ValidThru;

        public int? Age => Soldier?.AgeAsOf(Date);

        public APFTAgeGroup AgeGroup
        {
            get
            {
                if (Age <= 21) return APFTAgeGroup.Group_17_to_21;
                if (Age <= 26) return APFTAgeGroup.Group_22_to_26;
                if (Age <= 31) return APFTAgeGroup.Group_27_to_31;
                if (Age <= 36) return APFTAgeGroup.Group_32_to_36;
                if (Age <= 41) return APFTAgeGroup.Group_37_to_41;
                if (Age <= 46) return APFTAgeGroup.Group_42_to_46;
                if (Age <= 51) return APFTAgeGroup.Group_47_to_51;
                if (Age <= 56) return APFTAgeGroup.Group_52_to_56;
                if (Age <= 61) return APFTAgeGroup.Group_57_to_61;

                return APFTAgeGroup.Group_62_Plus;
            }
        }

        [Range(0, 200)]
        public int PushUps { get; set; }

        [Range(0, 200)]
        public int SitUps { get; set; }

        [Display(Name = "Aerobic Event")]
        public Event AerobicEvent { get; set; } = Event.Run;

        public Boolean IsAlternateAerobicEvent => AerobicEvent != Event.Run;

        [Range(0, int.MaxValue)]
        public int RunSeconds { get; set; }

        [NotMapped]
        public TimeSpan Run
        {
            get { return TimeSpan.FromSeconds(RunSeconds); }
            set { RunSeconds = (int)value.TotalSeconds; }
        }

        [Display(Name = "PushUp Score")]
        public int PushUpScore
        {
            get
            {
                return
                    APFTScoreTables
                    .PushUps
                    .OrderByDescending(row => row.Reps)
                    .Where(row => row.AgeGroup == AgeGroup)
                    .Where(row => row.Gender == Soldier?.Gender)
                    .Where(row => row.Reps <= PushUps)
                    .Select(row => row.Score)
                    .FirstOrDefault();
            }
        }

        [Display(Name = "SitUp Score")]
        public int SitUpScore
        {
            get
            {
                return
                    APFTScoreTables
                    .SitUps
                    .OrderByDescending(row => row.Reps)
                    .Where(row => row.AgeGroup == AgeGroup)
                    .Where(row => row.Gender == Soldier?.Gender)
                    .Where(row => row.Reps <= SitUps)
                    .Select(row => row.Score)
                    .FirstOrDefault();
            }
        }

        [Display(Name = "Run Score")]
        public int RunScore
        {
            get
            {
                switch (AerobicEvent)
                {
                    // Alternate aerobic events are pass/fail. To clear the other logical checks, just return 60 here if they meet standards.

                    case Event.Bike_Moving:
                    case Event.Bike_Stationary:
                        return APFTScoreTables.Bicycle.Where(row => row.AgeGroup == AgeGroup).Where(row => row.Gender == Soldier?.Gender).Select(row => row.Reps).SingleOrDefault() >= RunSeconds ? MinimumPerEvent : 0;

                    case Event.Walk:
                        return APFTScoreTables.Walk.Where(row => row.AgeGroup == AgeGroup).Where(row => row.Gender == Soldier?.Gender).Select(row => row.Reps).SingleOrDefault() >= RunSeconds ? MinimumPerEvent : 0;

                    case Event.Swim:
                        return APFTScoreTables.Swim.Where(row => row.AgeGroup == AgeGroup).Where(row => row.Gender == Soldier?.Gender).Select(row => row.Reps).SingleOrDefault() >= RunSeconds ? MinimumPerEvent : 0;

                    case Event.Run:
                    default:
                        return
                            APFTScoreTables
                            .Run
                            .OrderBy(row => row.Reps)
                            .Where(row => row.AgeGroup == AgeGroup)
                            .Where(row => row.Gender == Soldier?.Gender)
                            .Where(row => row.Reps >= Run.TotalSeconds)
                            .Select(row => row.Score)
                            .FirstOrDefault();
                }
            }
        }

        [Display(Name = "Total")]
        public int TotalScore => PushUpScore + SitUpScore + RunScore;

        [Display(Name = "Is Passing?")]
        public Boolean IsPassing => new[] { PushUpScore, SitUpScore, RunScore }.All(s => s >= MinimumPerEvent);

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Date > DateTime.Today) yield return new ValidationResult("Cannot select a date after today", new[] { nameof(Date) });

            if (Run > TimeSpan.FromHours(1)) yield return new ValidationResult("Cannot enter run time greater than one hour", new[] { nameof(Run) });
        }

        public byte[] GenerateCounseling()
        {
            return PDFService.Generate_DA4856(new PDFService.Counseling
            {
                Name = $"{Soldier.LastName}, {Soldier.FirstName}",
                Rank = Soldier.Rank,
                Organization = $"{Soldier.Unit.Name}",
                Date = Date,

                Purpose = $@"This counseling is for the failure of a record APFT on {Date:yyyy-MM-dd}. This counseling also outlines the actions that must be taken in the event of continuous APFT failures in accordance with AR 135-187, and AR 350-1, and AR 600-8-19 in order to increase Soldier awareness.

You achieved the following scores:

Push Ups(reps/score): {PushUps}/{PushUpScore}
Sit Ups(reps/score): {SitUps}/{SitUpScore}
{AerobicEvent.DisplayName()}(time/score): {Run}/{RunScore}

Total: {TotalScore}",

                KeyPointsOfDiscussion = @"
1. In the event of a first time record APFT failure, paragraph G-9m of Appendix G in AR 350-1 states:

- Soldiers who fail a record APFT for the first time or fail to take a record APFT within the required period will be flagged in accordance with AR 600–8–2. In the event of a record test failure, commanders may allow Soldiers to retake the test as soon as the Soldier and the commander feel the Soldier is ready. Soldiers without a medical profile will be retested no later than 90 days following the initial APFT failure. Reserve component Soldiers not on active duty and without a medical profile will be tested no later than 180 days following the initial APFT failure.

2. In addition, a first time record APFT failure may also result in reduction of rank, as outlined in Chapter 10 of AR 600-8-19:-10–5.

Policy Inefficiency is a demonstration of characteristics that shows that the person cannot perform duties and responsibilities of the grade and MOS. Inefficiency may also include any act or conduct that clearly shows that the Soldier lacks those abilities and qualities normally required and expected of an individual of that grade and experience. CDRs may consider misconduct, including conviction by civil court, as bearing on inefficiency. A Soldier may be reduced under this authority for longstanding unpaid personal debts that he or she has not made a reasonable attempt to pay.

3. Soldiers will be counseled and given time to a correct deficiencies. Paragraph 2-4b of AR 135-178 states:

-b. Counseling. When a Soldier’s conduct or performance approaches the point where a continuation of such conduct or performance would warrant initiating separation action for one of the reasons in paragraph a, above, the Soldier will be counseled by a responsible person about his or her deficiencies at least once before initiating separation action. Additional formal counseling is discretionary; however, the Soldier’s counseling or personnel records must establish that the Soldier was afforded a reasonable opportunity to overcome these deficiencies. Such factors as the length of time that has elapsed since the prior counseling, the Soldier’s conduct and performance during that period, and the commander’s assessment of the Soldier’s potential for becoming a fully satisfactory Soldier, should be considered.

4. If a Soldier fails to correct deficiencies after all above actions are taken, discharge procedures will be enacted. Paragraph 9-2e of AR 135-178 states:

-e. Initiation of discharge proceedings is required for Soldiers without medical limitations who have two consecutive failures of the Army Physical Fitness Test, or who are eliminated for cause from Noncommissioned Officer Education System courses, unless the responsible commander chooses to impose a bar to reenlistment in accordance with AR 140–111, or NGR 600–200."
            });
        }
    }

    public enum Event : byte
    {
        //PushUp,

        //SitUp,

        Run = 0,

        [Display(Name = "800-Yard-Swim Test")]
        Swim = 1,

        [Display(Name = "2.5-Mile Walk Test")]
        Walk = 2,

        [Display(Name = "6.2-Mile Bicycle Test")]
        Bike_Moving = 3,

        [Display(Name = "6.2-Mile Stationary-Cycle Ergometer Test")]
        Bike_Stationary = 4
    }

    public enum APFTAgeGroup : byte
    {
        [Display(Name = "17-21")]
        Group_17_to_21,

        [Display(Name = "22-26")]
        Group_22_to_26,

        [Display(Name = "27-31")]
        Group_27_to_31,

        [Display(Name = "32-36")]
        Group_32_to_36,

        [Display(Name = "37-41")]
        Group_37_to_41,

        [Display(Name = "42-46")]
        Group_42_to_46,

        [Display(Name = "47-51")]
        Group_47_to_51,

        [Display(Name = "52-56")]
        Group_52_to_56,

        [Display(Name = "57-61")]
        Group_57_to_61,

        [Display(Name = "62+")]
        Group_62_Plus
    }
}