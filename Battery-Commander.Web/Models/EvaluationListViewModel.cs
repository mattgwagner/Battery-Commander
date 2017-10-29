using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Models
{
    public class EvaluationListViewModel
    {
        public IEnumerable<Evaluation> Evaluations { get; set; } = Enumerable.Empty<Evaluation>();
    }
}
