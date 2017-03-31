using BatteryCommander.Web.Models.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BatteryCommander.Web.Models
{
    public class ABCP
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int SoldierId { get; set; }

        public virtual Soldier Soldier { get; set; }

        [Required, DataType(DataType.Date), Column(TypeName = "date")]
        public DateTime Date { get; set; } = DateTime.Today;

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

        public Boolean RequiresTape => Screening_Weight < Weight;

        [NotMapped]
        public ICollection<Measurement> Measurements
        {
            get { return JsonConvert.DeserializeObject<List<Measurement>>(MeasurementsJson); }
            set { MeasurementsJson = JsonConvert.SerializeObject(value); }
        }

        public Double WaistAverage => Measurements.Select(_ => _.Waist).Average();

        public Double NeckAverage => Measurements.Select(_ => _.Neck).Average();

        public Double HipAverage => Measurements.Select(_ => _.Hips).Average();

        public String MeasurementsJson { get; set; } = String.Empty;

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

        public Boolean IsPassingTape => BodyFatPercentage <= MaximumAllowableBodyFat;

        public class Measurement
        {
            [Range(0, 50)]
            public Double Waist { get; set; }

            [Range(0, 50)]
            public Double Neck { get; set; }

            [Range(0, 50)]
            public Double Hips { get; set; }
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