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
        public int Delinquent => Evaluations.Where(_ => _.IsDelinquent).Count();

        public int Due => Evaluations.Where(_ => !_.IsDelinquent).Where(_ => _.ThruDate < DateTime.Today).Count();

        [Display(Name = "Next 30")]
        public int Next30 => Evaluations.Where(_ => _.ThruDate > DateTime.Today).Where(_ => DateTime.Today.AddDays(30) < _.ThruDate).Count();

        [Display(Name = "Next 60")]
        public int Next60 => Evaluations.Where(_ => _.ThruDate > DateTime.Today.AddDays(30)).Where(_ => DateTime.Today.AddDays(60) <= _.ThruDate).Count();

        [Display(Name = "Next 90")]
        public int Next90 => Evaluations.Where(_ => _.ThruDate > DateTime.Today.AddDays(60)).Where(_ => DateTime.Today.AddDays(90) <= _.ThruDate).Count();
    }
}