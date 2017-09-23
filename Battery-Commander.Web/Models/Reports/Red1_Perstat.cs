using System.Collections.Generic;
using System.Linq;

namespace BatteryCommander.Web.Models.Reports
{
    public class Red1_Perstat
    {
        public IEnumerable<Soldier> Soldiers { get; set; } = Enumerable.Empty<Soldier>();

        public Row Assigned => new Row
        {
            Enlisted = Soldiers.Where(_ => _.IsEnlisted).Count(),
            Officer = Soldiers.Where(_ => _.IsOfficer).Count()
        };

        public Row Attached => new Row { };

        public Row Detached => new Row
        {
            Enlisted = Soldiers.Where(_ => _.IsEnlisted).Where(_ => _.Status == Soldier.SoldierStatus.Detached).Count(),
            Officer = Soldiers.Where(_ => _.IsOfficer).Where(_ => _.Status == Soldier.SoldierStatus.Detached).Count()
        };

        public Row Total => new Row
        {
            Enlisted = Assigned.Enlisted + Attached.Enlisted - Detached.Enlisted,
            Officer = Assigned.Officer + Attached.Officer - Detached.Officer
        };

        public Row PresentForDuty => new Row
        {
            Enlisted = Soldiers.Where(_ => _.IsEnlisted).Where(_ => _.Status == Soldier.SoldierStatus.PresentForDuty).Count(),
            Officer = Soldiers.Where(_ => _.IsOfficer).Where(_ => _.Status == Soldier.SoldierStatus.PresentForDuty).Count()
        };

        public Row WoundedInAction => new Row { };

        public Row KilledInAction => new Row { };

        public Row MissingInAction => new Row { };

        public Row OnLeaveOrPass => new Row
        {
            Enlisted = Soldiers.Where(_ => _.IsEnlisted).Where(_ => _.Status == Soldier.SoldierStatus.OnPass).Count(),
            Officer = Soldiers.Where(_ => _.IsOfficer).Where(_ => _.Status == Soldier.SoldierStatus.OnPass).Count()
        };

        public Row TDY => new Row
        {
            Enlisted = Soldiers.Where(_ => _.IsEnlisted).Where(_ => _.Status == Soldier.SoldierStatus.TDY).Count(),
            Officer = Soldiers.Where(_ => _.IsOfficer).Where(_ => _.Status == Soldier.SoldierStatus.TDY).Count()
        };

        public Row AWOL => new Row
        {
            Enlisted = Soldiers.Where(_ => _.IsEnlisted).Where(_ => _.Status == Soldier.SoldierStatus.AWOL).Count(),
            Officer = Soldiers.Where(_ => _.IsOfficer).Where(_ => _.Status == Soldier.SoldierStatus.AWOL).Count()
        };

        public Row RearDetachment => new Row
        {
            Enlisted = Soldiers.Where(_ => _.IsEnlisted).Where(_ => _.Status == Soldier.SoldierStatus.RearDetachment).Count(),
            Officer = Soldiers.Where(_ => _.IsOfficer).Where(_ => _.Status == Soldier.SoldierStatus.RearDetachment).Count()
        };

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