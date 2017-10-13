using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BatteryCommander.Web.Models.Reports
{
    public class Red1_Perstat
    {
        public IEnumerable<Soldier> Soldiers { get; set; } = Enumerable.Empty<Soldier>();

        [Display(Name = "Personnel Assigned")]
        public Row Assigned => new Row
        {
            Enlisted = Soldiers.Where(_ => _.IsEnlisted).Count(),
            Officer = Soldiers.Where(_ => _.IsOfficer).Count()
        };

        [Display(Name = "Personnel Attached")]
        public Row Attached => new Row { };

        [Display(Name = "Personnel Detached")]
        public Row Detached => new Row
        {
            Enlisted = Soldiers.Where(_ => _.IsEnlisted).Where(_ => _.Status == Soldier.SoldierStatus.Detached).Count(),
            Officer = Soldiers.Where(_ => _.IsOfficer).Where(_ => _.Status == Soldier.SoldierStatus.Detached).Count()
        };

        [Display(Name = "Total Personnel")]
        public Row Total => new Row
        {
            Enlisted = Assigned.Enlisted + Attached.Enlisted - Detached.Enlisted,
            Officer = Assigned.Officer + Attached.Officer - Detached.Officer
        };

        [Display(Name = "Total Present for Duty")]
        public Row PresentForDuty => new Row
        {
            Enlisted = Soldiers.Where(_ => _.IsEnlisted).Where(_ => _.Status == Soldier.SoldierStatus.PresentForDuty).Count(),
            Officer = Soldiers.Where(_ => _.IsOfficer).Where(_ => _.Status == Soldier.SoldierStatus.PresentForDuty).Count()
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
            Enlisted = Soldiers.Where(_ => _.IsEnlisted).Where(_ => _.Status == Soldier.SoldierStatus.OnPass).Count(),
            Officer = Soldiers.Where(_ => _.IsOfficer).Where(_ => _.Status == Soldier.SoldierStatus.OnPass).Count()
        };

        [Display(Name = "Personnel TDY")]
        public Row TDY => new Row
        {
            Enlisted = Soldiers.Where(_ => _.IsEnlisted).Where(_ => _.Status == Soldier.SoldierStatus.TDY).Count(),
            Officer = Soldiers.Where(_ => _.IsOfficer).Where(_ => _.Status == Soldier.SoldierStatus.TDY).Count()
        };

        [Display(Name = "Personnel AWOL")]
        public Row AWOL => new Row
        {
            Enlisted = Soldiers.Where(_ => _.IsEnlisted).Where(_ => _.Status == Soldier.SoldierStatus.AWOL).Count(),
            Officer = Soldiers.Where(_ => _.IsOfficer).Where(_ => _.Status == Soldier.SoldierStatus.AWOL).Count()
        };

        [Display(Name = "Rear Detachment")]
        public Row RearDetachment => new Row
        {
            Enlisted = Soldiers.Where(_ => _.IsEnlisted).Where(_ => _.Status == Soldier.SoldierStatus.RearDetachment).Count(),
            Officer = Soldiers.Where(_ => _.IsOfficer).Where(_ => _.Status == Soldier.SoldierStatus.RearDetachment).Count()
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