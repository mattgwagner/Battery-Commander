using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Humanizer;
using Stateless;

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

        // TODO Type training and location(s) to be performed

        // TODO Time/Date to be performed ILO scheduled established training assembly - hour(s), date(s), For Training Assembly - Date

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

        public enum Trigger : byte
        {
            [Display(Name = "Supervisor Approved")]
            Supervisor_Approval,

            [Display(Name = "PLT Leadership Approved")]
            Platoon_Leadership_Approval,

            [Display(Name = "1SG Approved")]
            First_Sergeant_Approval,

            [Display(Name = "CDR Approved")]
            Commander_Approval,

            [Display(Name = "Returned to SM")]
            Returned_To_Solder,

            [Display(Name = "Rejected")]
            Reject
        }

        public virtual IEnumerable<Trigger> Available_Transitions => Machine.PermittedTriggers;

        public virtual void Transition(Trigger trigger) => Machine.Fire(trigger);

        protected virtual StateMachine<SUTAStatus, Trigger> Machine
        {
            get
            {
                var machine = new StateMachine<SUTAStatus, Trigger>(() => Status, value => Status = value);

                //machine.Configure(SUTAStatus.At_Rater)
                //    .Permit(Trigger.Rater_Completed, SUTAStatus.At_Senior_Rater);

                //machine.Configure(SUTAStatus.At_Senior_Rater)
                //    .Permit(Trigger.Return_to_Rater, SUTAStatus.At_Rater)
                //    .PermitIf(Trigger.Senior_Rater_Completed, SUTAStatus.At_Reviewer, () => ReviewerId.HasValue)
                //    .PermitIf(Trigger.Senior_Rater_Completed, SUTAStatus.At_1SG_Review, () => !ReviewerId.HasValue);

                //machine.Configure(SUTAStatus.At_Reviewer)
                //    .Permit(Trigger.Reviewer_Completed, SUTAStatus.At_1SG_Review)
                //    .Permit(Trigger.Return_to_Rater, SUTAStatus.At_Rater);

                //machine.Configure(SUTAStatus.At_1SG_Review)
                //    .Permit(Trigger.Return_to_Rater, SUTAStatus.At_Rater)
                //    .Permit(Trigger.Internal_Review_Completed, SUTAStatus.Ready_for_Signatures);

                //machine.Configure(SUTAStatus.Ready_for_Signatures)
                //    .Permit(Trigger.Return_to_Rater, SUTAStatus.At_Rater)
                //    .Permit(Trigger.Rater_Signed, SUTAStatus.Pending_Senior_Rater_Signature);

                //machine.Configure(SUTAStatus.Pending_Senior_Rater_Signature)
                //    .PermitIf(Trigger.Senior_Rater_Signed, EvaluationStatus.Pending_SM_Signature);

                //machine.Configure(SUTAStatus.Pending_SM_Signature)
                //    .PermitIf(Trigger.Soldier_Signed, EvaluationStatus.Pending_Reviewer_Signature, () => ReviewerId.HasValue)
                //    .PermitIf(Trigger.Soldier_Signed, EvaluationStatus.Pending_HQDA_Submission, () => !ReviewerId.HasValue);

                //machine.Configure(SUTAStatus.Pending_Reviewer_Signature)
                //    .PermitIf(Trigger.Reviewer_Signed, SUTAStatus.Pending_HQDA_Submission);

                //machine.Configure(SUTAStatus.Pending_HQDA_Submission)
                //    .Permit(Trigger.Submitted_to_Hqda, SUTAStatus.Submitted_to_HQDA)
                //    .Permit(Trigger.Accepted_to_iPerms, SUTAStatus.Accepted_to_iPerms);

                //machine.Configure(SUTAStatus.Submitted_to_HQDA)
                //    .Permit(Trigger.Return_to_Rater, SUTAStatus.At_Rater)
                //    .Permit(Trigger.Accepted_to_iPerms, SUTAStatus.Accepted_to_iPerms);

                return machine;
            }
        }
    }
}
