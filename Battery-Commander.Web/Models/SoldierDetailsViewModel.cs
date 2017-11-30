using System;
using System.Collections.Generic;
using System.Linq;

namespace BatteryCommander.Web.Models
{
    public class SoldierDetailsViewModel
    {
        public virtual Soldier Soldier { get; set; }

        public IEnumerable<EvaluationViewModel> Evaluations { get; set; } = Enumerable.Empty<EvaluationViewModel>();

        public IEnumerable<Soldier> Subordinates { get; set; } = Enumerable.Empty<Soldier>();

        public class EvaluationViewModel
        {
            public Evaluation Evaluation { get; set; }

            public String Role { get; set; }
        }
    }
}