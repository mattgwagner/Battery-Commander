using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Models
{
    public class SUTA
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Soldier -> Unit, Grade

        [Required]
        public int SoldierId { get; set; }

        public virtual Soldier Soldier { get; set; }

        // The reason to request missing IDT

        public String Reasoning { get; set; }

        // Type training and location(s) to be performed

        // Time/Date to be performed ILO scheduled established training assembly - hour(s), date(s), For Training Assembly - Date

        public Boolean FirstLineSupervisorApproval { get; set; }

        public Boolean PlatoonSergeantApproval { get; set; }

        public Boolean CommanderApproval { get; set; }

        public SUTAStatus Status { get; set; } = SUTAStatus.Created;

        // Track Status/Workflow, Who Approved and When

        // Track Comments/Questions

        public enum SUTAStatus
        {
            Created, Submitted, Approved, Scheduled, Completed
        }

        // SUTA Request Process:
        // 1. SM submits their concern/request through their chain-of-command
        // 2. Chain of command helps the Soldier think through mitigation processes -- try to find a work-around
        // 3. Soldier contacts the FTS to coordinate ILO dates
        // 4. Soldier submits a SUTA request form through CoC
        // 5. CoC signs off and submits through Battery Commander/1SG and CC's the FTS
        // 6. The only approval authority is the Battery Commander
    }
}
