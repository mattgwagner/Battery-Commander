using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BatteryCommander.Web.Models
{
    public class EvaluationListViewModel
    {
        public IEnumerable<Evaluation> Evaluations { get; set; } = Enumerable.Empty<Evaluation>();

        [Display(Name = "Delinquent > 60 Days")]
        public int Delinquent => Evaluations.Where(_ => !_.IsCompleted).Where(_ => _.IsDelinquent).Count();

        public int Due => Evaluations.Where(_ => !_.IsCompleted).Where(_ => !_.IsDelinquent).Where(_ => _.ThruDate < DateTime.Today).Count();

        [Display(Name = "Next 30")]
        public int Next30 => Evaluations.Where(_ => !_.IsCompleted).Where(_ => 0 <= _.Delinquency.TotalDays && _.Delinquency.TotalDays <= 30).Count();

        [Display(Name = "Next 60")]
        public int Next60 => Evaluations.Where(_ => !_.IsCompleted).Where(_ => 30 < _.Delinquency.TotalDays && _.Delinquency.TotalDays <= 60).Count();

        [Display(Name = "Next 90")]
        public int Next90 => Evaluations.Where(_ => !_.IsCompleted).Where(_ => 60 < _.Delinquency.TotalDays && _.Delinquency.TotalDays <= 90).Count();
    }
}