using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Humanizer;

namespace BatteryCommander.Web.Models
{
    public class SUTA
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int SoldierId { get; set; }

        public virtual Soldier Soldier { get; set; }

        // The reason to request missing IDT

        public String Reasoning { get; set; }

        [Display(Name = "Mitigation Plan")]
        public String MitigationPlan { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public int SupervisorId { get; set; }

        public virtual Soldier Supervisor { get; set; }

        [Display(Name = "Supervisor Signature")]
        public String SupervisorSignature { get; set; }

        public DateTime? SupervisorSignedAt { get; set; }

        [Display(Name = "1SG Signature")]
        public String FirstSergeantSignature { get; set; }

        public DateTime? FirstSergeantSignedAt { get; set; }

        [Display(Name = "CDR Signature")]
        public String CommanderSignature { get; set; }

        public DateTime? CommanderSignedAt { get; set; }

        // TODO Type training and location(s) to be performed

        // TODO Time/Date to be performed ILO scheduled established training assembly - hour(s), date(s), For Training Assembly - Date

        // TODO Maybe we make status flip based on the signatures?

        public SUTAStatus Status { get; set; } = SUTAStatus.Created;

        public enum SUTAStatus
        {
            Created, Approved, Scheduled, Completed
        }

        // SUTA Request Process:
        // 1. SM submits their concern/request through their chain-of-command
        // 2. Chain of command helps the Soldier think through mitigation processes -- try to find a work-around
        // 3. Soldier contacts the FTS to coordinate ILO dates
        // 4. Soldier submits a SUTA request form through CoC
        // 5. CoC signs off and submits through Battery Commander/1SG and CC's the FTS
        // 6. The only approval authority is the Battery Commander

        // Track Status/Workflow, Who Approved and When

        // Track Comments/Questions

        [Display(Name = "History")]
        public virtual ICollection<Event> Events { get; set; } = new List<Event>();

        [Table(name: "SUTA_Events")]
        public class Event
        {
            // Could represent a state transition or a manual comment added, perhaps we need to flag that?

            [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; }

            public int SUTAId { get; set; }

            public virtual SUTA SUTA { get; set; }

            [DataType(DataType.DateTime)]
            public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

            public String TimestampHumanized => Timestamp.Humanize();

            public string Author { get; set; }

            public string Message { get; set; }

            public override string ToString() => $"{Author}: {Message}";
        }
    }
}
