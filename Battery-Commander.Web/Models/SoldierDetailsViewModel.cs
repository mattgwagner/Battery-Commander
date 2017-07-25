﻿using System;
using System.Collections.Generic;

namespace BatteryCommander.Web.Models
{
    public class SoldierDetailsViewModel
    {
        public virtual Soldier Soldier { get; set; }

        public ICollection<EvaluationViewModel> Evaluations { get; set; } = new List<EvaluationViewModel>();

        public ICollection<Soldier> Subordinates { get; set; } = new List<Soldier>();

        public class EvaluationViewModel
        {
            public Evaluation Evaluation { get; set; }

            public String Role { get; set; }
        }
    }
}