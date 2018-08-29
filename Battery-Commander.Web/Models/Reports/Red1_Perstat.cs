using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BatteryCommander.Web.Models.Reports
{
    public class Red1_Perstat : Report
    {
        public Red1_Perstat(Unit unit) : base(unit)
        {
            // Nothing to do here
        }

        public override ReportType Type => ReportType.Perstat;

        [Display(Name = "Personnel Assigned")]
        public Row Assigned => new Row
        {
            Enlisted = Unit.Soldiers.Where(_ => _.IsEnlisted).Count(),
            Warrant = Unit.Soldiers.Where(_ => _.IsWarrant).Count(),
            Officer = Unit.Soldiers.Where(_ => _.IsOfficer || _.IsCadet).Count()
        };

        [Display(Name = "Personnel Attached")]
        public Row Attached => new Row { };

        [Display(Name = "Personnel Detached")]
        public Row Detached => new Row
        {
            Enlisted = Unit.Soldiers.Where(_ => _.IsEnlisted).Where(_ => _.Status == Soldier.SoldierStatus.Detached).Count(),
            Warrant = Unit.Soldiers.Where(_ => _.IsWarrant).Where(_ => _.Status == Soldier.SoldierStatus.Detached).Count(),
            Officer = Unit.Soldiers.Where(_ => _.IsOfficer || _.IsCadet).Where(_ => _.Status == Soldier.SoldierStatus.Detached).Count()
        };

        [Display(Name = "Total Personnel")]
        public Row Total => new Row
        {
            Enlisted = Assigned.Enlisted + Attached.Enlisted - Detached.Enlisted,
            Warrant = Assigned.Warrant + Attached.Warrant - Detached.Warrant,
            Officer = Assigned.Officer + Attached.Officer - Detached.Officer
        };

        [Display(Name = "Total Present for Duty")]
        public Row PresentForDuty => new Row
        {
            Enlisted = Unit.Soldiers.Where(_ => _.IsEnlisted).Where(_ => _.Status == Soldier.SoldierStatus.PresentForDuty).Count() + Attached.Enlisted,
            Warrant = Unit.Soldiers.Where(_ => _.IsWarrant).Where(_ => _.Status == Soldier.SoldierStatus.PresentForDuty).Count() + Attached.Warrant,
            Officer = Unit.Soldiers.Where(_ => _.IsOfficer || _.IsCadet).Where(_ => _.Status == Soldier.SoldierStatus.PresentForDuty).Count() + Attached.Officer
        };

        [Display(Name = "Wounded in Action")]
        public Row WoundedInAction => new Row { };

        [Display(Name = "Killed in Action")]
        public Row KilledInAction => new Row { };

        [Display(Name = "Missing in Action")]
        public Row MissingInAction => new Row { };

        [Display(Name = "Personnel On Leave / Pass")]
        public Row OnLeaveOrPass => new Row
        {
            Enlisted = Unit.Soldiers.Where(_ => _.IsEnlisted).Where(_ => _.Status == Soldier.SoldierStatus.OnPass).Count(),
            Warrant = Unit.Soldiers.Where(_ => _.IsWarrant).Where(_ => _.Status == Soldier.SoldierStatus.OnPass).Count(),
            Officer = Unit.Soldiers.Where(_ => _.IsOfficer || _.IsCadet).Where(_ => _.Status == Soldier.SoldierStatus.OnPass).Count()
        };

        [Display(Name = "Personnel TDY")]
        public Row TDY => new Row
        {
            Enlisted = Unit.Soldiers.Where(_ => _.IsEnlisted).Where(_ => _.Status == Soldier.SoldierStatus.TDY).Count(),
            Warrant = Unit.Soldiers.Where(_ => _.IsWarrant).Where(_ => _.Status == Soldier.SoldierStatus.TDY).Count(),
            Officer = Unit.Soldiers.Where(_ => _.IsOfficer || _.IsCadet).Where(_ => _.Status == Soldier.SoldierStatus.TDY).Count()
        };

        [Display(Name = "Personnel AWOL")]
        public Row AWOL => new Row
        {
            Enlisted = Unit.Soldiers.Where(_ => _.IsEnlisted).Where(_ => _.Status == Soldier.SoldierStatus.AWOL).Count(),
            Warrant = Unit.Soldiers.Where(_ => _.IsWarrant).Where(_ => _.Status == Soldier.SoldierStatus.AWOL).Count(),
            Officer = Unit.Soldiers.Where(_ => _.IsOfficer || _.IsCadet).Where(_ => _.Status == Soldier.SoldierStatus.AWOL).Count()
        };

        [Display(Name = "Rear Detachment")]
        public Row RearDetachment => new Row
        {
            Enlisted = Unit.Soldiers.Where(_ => _.IsEnlisted).Where(_ => _.Status == Soldier.SoldierStatus.RearDetachment).Count(),
            Warrant = Unit.Soldiers.Where(_ => _.IsWarrant).Where(_ => _.Status == Soldier.SoldierStatus.RearDetachment).Count(),
            Officer = Unit.Soldiers.Where(_ => _.IsOfficer || _.IsCadet).Where(_ => _.Status == Soldier.SoldierStatus.RearDetachment).Count()
        };

        [Display(Name = "Replacements Requested")]
        public Row ReplacementsRequested => new Row { };

        public class Row
        {
            public int Officer { get; set; }

            public int Enlisted { get; set; }

            public int Warrant { get; set; }

            public int Total => Officer + Enlisted + Warrant;
        }
    }
}