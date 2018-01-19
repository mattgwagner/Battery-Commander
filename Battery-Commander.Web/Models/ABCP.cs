using BatteryCommander.Web.Models.Data;
using BatteryCommander.Web.Services;
using Humanizer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BatteryCommander.Web.Models
{
    public partial class Soldier
    {
        public virtual ICollection<ABCP> ABCPs { get; set; }

        public virtual ABCP LastAbcp => ABCPs?.OrderByDescending(abcp => abcp.Date).FirstOrDefault();

        public virtual int ConsecutiveAbcpFailures
        {
            get
            {
                var count = 0;

                if (ABCPs?.Any() == true)
                {
                    foreach (var abcp in ABCPs?.OrderByDescending(_ => _.Date))
                    {
                        if (abcp.IsPassing) break;

                        count++;
                    }
                }

                return count;
            }
        }

        public virtual EventStatus AbcpStatus
        {
            get
            {
                if (LastAbcp?.IsValid == true)
                {
                    // TODO This doesn't take into account soldiers who passed tape but are still on the ABCP program

                    if (LastAbcp?.IsPassing == true) return EventStatus.Passed;

                    if (LastAbcp?.IsPassing == false) return EventStatus.Failed;
                }

                return EventStatus.NotTested;
            }
        }
    }

    public class ABCP : IValidatableObject
    {
        /// <summary>
        /// Measurements are considered invalid if they're greater than 1 inch apart
        /// </summary>
        public const int MAX_DIFFERNCE = 1;

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int SoldierId { get; set; }

        public virtual Soldier Soldier { get; set; }

        [Required, DataType(DataType.Date), Column(TypeName = "date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        public DateTime ValidThru => Date.AddDays(Soldier.DaysPerYear);

        public Boolean IsValid => DateTime.Today <= ValidThru;

        public int? Age => Soldier?.AgeAsOf(Date);

        /// <summary>
        /// Recorded to nearest half inch when used for body fat percentage calculations
        /// </summary>
        [Required, Range(0, 90)]
        public Decimal Height { get; set; }

        /// <summary>
        /// Recorded to the nearest pound for all usage
        /// </summary>
        [Required, Range(0, 400)]
        public int Weight { get; set; }

        public ABCPAgeGroup AgeGroup
        {
            get
            {
                var Age = Soldier?.AgeAsOf(Date);

                if (Age <= 20) return ABCPAgeGroup.Group_17_to_20;
                if (Age <= 27) return ABCPAgeGroup.Group_21_to_27;
                if (Age <= 39) return ABCPAgeGroup.Group_28_to_39;

                return ABCPAgeGroup.Group_40_Plus;
            }
        }

        [Display(Name = "Screening Weight")]
        public Decimal Screening_Weight
        {
            get
            {
                return ABCPScoreTables
                    .ScreeningWeights
                    .Where(_ => _.AgeGroup == AgeGroup)
                    .Where(_ => _.Gender == Soldier?.Gender)
                    .Where(_ => _.Height == Math.Round(Height, decimals: 0, mode: MidpointRounding.AwayFromZero))
                    .Select(_ => _.Weight)
                    .SingleOrDefault();
            }
        }

        [Display(Name = "Requires Taping?")]
        public Boolean RequiresTape => Screening_Weight < Weight;

        [NotMapped]
        public ICollection<Measurement> Measurements
        {
            get { return JsonConvert.DeserializeObject<List<Measurement>>(MeasurementsJson); }
            set { MeasurementsJson = JsonConvert.SerializeObject(value); }
        }

        [NotMapped]
        public Boolean AreMeasurementsValid
        {
            // Returns false if any measurement is greater than 1 inch away from another measurement

            get
            {
                foreach (var measurement in Measurements)
                {
                    if (Measurements.Any(check => Math.Abs(check.Waist - measurement.Waist) > MAX_DIFFERNCE)) return false;
                    if (Measurements.Any(check => Math.Abs(check.Neck - measurement.Neck) > MAX_DIFFERNCE)) return false;
                    if (Measurements.Any(check => Math.Abs(check.Hips - measurement.Hips) > MAX_DIFFERNCE)) return false;
                }

                return true;
            }
        }

        public Double WaistAverage => Average_To_Half(Measurements.Select(_ => _.Waist));

        public Double NeckAverage => Average_To_Half(Measurements.Select(_ => _.Neck));

        public Double HipAverage => Average_To_Half(Measurements.Select(_ => _.Hips));

        public String MeasurementsJson { get; set; } = "[]";

        [Display(Name = "Circumference Value")]
        public Double CircumferenceValue
        {
            get
            {
                switch (Soldier?.Gender)
                {
                    case Gender.Female:
                        return (WaistAverage + HipAverage) - NeckAverage;

                    case Gender.Male:
                    default:
                        return WaistAverage - NeckAverage;
                }
            }
        }

        [Display(Name = "Calculated Body Fat %")]
        public Double BodyFatPercentage
        {
            get
            {
                switch (Soldier?.Gender)
                {
                    case Gender.Female:
                        return Math.Round(((163.205 * Math.Log10(WaistAverage + HipAverage - NeckAverage)) - (97.684 * Math.Log10((Double)Height)) - 78.387));

                    case Gender.Male:
                    default:
                        return Math.Round((86.010 * Math.Log10(WaistAverage - NeckAverage)) - (70.041 * Math.Log10((Double)Height)) + 36.76);
                }
            }
        }

        [Display(Name = "Max Body Fat %")]
        public Double MaximumAllowableBodyFat
        {
            get
            {
                return
                    ABCPScoreTables
                    .MaxAllowablePercentages
                    .Where(_ => _.Gender == Soldier?.Gender)
                    .Where(_ => _.AgeGroup == AgeGroup)
                    .Select(_ => _.Maximum)
                    .SingleOrDefault();
            }
        }

        [Display(Name = "Is Passing Tape?")]
        public Boolean IsPassingTape => RequiresTape && Measurements.Any() && BodyFatPercentage <= MaximumAllowableBodyFat;

        [Display(Name = "Is Passing?")]
        public Boolean IsPassing => !RequiresTape || IsPassingTape;

        /// <summary>
        /// Returns true if the Soldier made satisfactory progress since the last ABCP weigh-in
        /// </summary>
        [NotMapped, Display("Is Satisfactory Progress?")]
        public Boolean IsSatisfactory
        {
            get
            {
                var delta_weight = Weight - Previous.Weight;
                var delta_bodyfat = BodyFatPercentage - Previous.BodyFatPercentage;

                return (delta_weight <= -3 || delta_bodyfat <= -1);
            }
        }

        public virtual ABCP Previous =>
            Soldier
                .ABCPs
                .Where(abcp => abcp.Date < Date)
                .OrderByDescending(abcp => abcp.Date)
                .FirstOrDefault();

        public class Measurement
        {
            [Range(0, 50)]
            public Double Waist { get; set; }

            [Range(0, 50)]
            public Double Neck { get; set; }

            [Range(0, 50)]
            public Double Hips { get; set; }
        }

        public byte[] GenerateWorksheet()
        {
            switch (Soldier?.Gender)
            {
                case Gender.Female:
                    return PDFService.Generate_DA5501(this);

                case Gender.Male:
                default:
                    return PDFService.Generate_DA5500(this);
            }
        }

        public byte[] GenerateCounseling()
        {
            var consecutive_failures = 1;

            foreach (var abcp in Soldier.ABCPs.Where(_ => _.Date < Date).OrderByDescending(_ => _.Date))
            {
                if (abcp.IsPassing) break;

                consecutive_failures++;
            }

            return PDFService.Generate_DA4856(new PDFService.Counseling
            {
                Name = $"{Soldier.LastName}, {Soldier.FirstName}",
                Rank = Soldier.Rank,
                Organization = $"{Soldier.Unit.Name}",
                Date = Date,

                Purpose = $@"
Failure to meet weight and tape standard per AR 600-9.
{(Previous == null ? "Enrollment into the ABCP program." : "")}
Height: {Height}in
{(Previous != null ? $"Previous Weight: {Previous.Weight}lbs" : "")}
Weight: {Weight}lbs
Authorized Weight: {Screening_Weight}lbs
MAW Body Fat: {MaximumAllowableBodyFat}%
{(Previous != null ? $"Previous Body Fat: {Previous.BodyFatPercentage}%" : "")}
Current Body Fat: {BodyFatPercentage}%
",

                KeyPointsOfDiscussion = $@"You failed to meet weight and tape standards as per AR 600-9 for the {consecutive_failures.Ordinalize()} time on {Date:yyyyMMdd}.

A flag has been initiated against you for failing to meet weight and tape standards as per AR 600-9, AR 600-8-2, and/or NGR 600-200 chapter 7, as appropriate.
(1) Soldiers who are flagged for weight control normally are not eligible to receive awards or attend schools IAW AR 600-8-2. The only exception to receive    awards is that the commander may submit a waiver permitting the Soldier to be recommended for and receive awards when the award is for valor, heroism, or length of service.

You are REQUIRED to be weighed in and taped monthly to monitor your progress by The Army Body Composition Program Officer and NCO IAW AR 600-9 Chapter 3 Para 9, it is your responsibility to ensure you get weighted monthly.

IAW AR 600-9 Chapter 3 Para 12
Satisfactory progress in the ABCP is defined as a monthly weight loss of either 3 to 8 pounds or 1 percent body fat.

A Soldier enrolled in the ABCP is considered to be failing the program if:
(1) He or she exhibits less than satisfactory progress on two consecutive monthly ABCP assessments; or
(2) After 6 months in the ABCP he or she still exceeds body fat standards, and exhibits less than satisfactory progress for three or more (nonconsecutive) monthly ABCP assessments.

You will be informed, in writing, that a bar to reenlistment, separation action, or a transfer to the IRR is being initiated under the following applicable regulation(s): AR 135–175; AR 135–178; AR 600–8–24 (see eliminations and miscellaneous types of separations); AR 601–280; AR 635–200; AR 140–10; National Guard Regulation (NGR) (AR) 600–5; NGR 600–101; NGR 600–200; or NGR 635–100.

You will be removed administratively from the ABCP as soon as the body fat standard is achieved. Soldiers that meet the screening table weight must remain in the ABCP program until they no longer exceed the required body fat standard.

IAW AR 600-9 Chapter 3 Para 14
If a Soldier again exceeds the body fat standard within 12 months after release from the ABCP, a DA Form 268 will be initiated on the Soldier.
(1) Commander will initiate administrative action up to and including separation and discharge

If, after 12 months but less than 36 months from the date of release from the ABCP, it is determined that a Soldier again exceeds the body fat standard, a DA Form 268 will be initiated on the Soldier.
(1) Solider will have 90 days to meet the standards
",

                PlanOfAction = $@"
Continue to monitor your weight loss every week (choose a day each week)

Continue to develop and improve your diet and exercise plan, utilize the Battery PT Plan to assist you.

Individualized Plan of Action from Soldier below.
",

                LeadersResponsibilities = $@"
Identify reasons for failure and help overcome
Evaluate nutrition and PT training program for effectiveness
Discuss ways to develop a better, lasting, diet
Encourage and support
",

                Assessment = $@"
{(Previous == null ? String.Empty : $"{(IsSatisfactory ? "SATISFACTORY" : "UNSATISFACTORY")} progress for the month: {Weight - Previous.Weight}lbs, {BodyFatPercentage - Previous.BodyFatPercentage}% body fat.")}
"
            });
        }

        private static Double Average_To_Half(IEnumerable<Double> values)
        {
            if (values.Any())
            {
                return Math.Round(values.Average() * 2, MidpointRounding.AwayFromZero) / 2;
            }

            return 0;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Date > DateTime.Today) yield return new ValidationResult("Cannot select a date after today", new[] { nameof(Date) });
        }
    }

    public enum ABCPAgeGroup : byte
    {
        [Display(Name = "17-20")]
        Group_17_to_20,

        [Display(Name = "21-27")]
        Group_21_to_27,

        [Display(Name = "28-39")]
        Group_28_to_39,

        [Display(Name = "40+")]
        Group_40_Plus,
    }
}