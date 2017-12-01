using Stateless;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BatteryCommander.Web.Models
{
    public partial class Evaluation
    {
        public enum Trigger : byte
        {
            // These don't map to anything in storage, so the specific ints don't matter

            [Display(Name = "Rater Complete")]
            Rater_Completed,

            [Display(Name = "Senior Rater Completed")]
            Senior_Rater_Completed,

            [Display(Name = "Reviewer Completed")]
            Reviewer_Completed,

            [Display(Name = "Return to Rater (Changes Required)")]
            Return_to_Rater,

            [Display(Name = "Admin/1SG Review Completed")]
            Internal_Review_Completed,

            [Display(Name = "Signatures Completed")]
            Signed,

            [Display(Name = "Sent to S1 for Review")]
            Sent_To_S1,

            [Display(Name = "S1 Review Completed")]
            S1_Review_Completed,

            [Display(Name = "Submitted to HQDA")]
            Submitted_to_Hqda,

            [Display(Name = "Accepted to iPerms")]
            Accepted_to_iPerms
        }

        public virtual IEnumerable<Trigger> Available_Transitions => Machine.PermittedTriggers;

        public virtual void Transition(Trigger trigger) => Machine.Fire(trigger);

        protected virtual StateMachine<EvaluationStatus, Trigger> Machine
        {
            get
            {
                var machine = new StateMachine<EvaluationStatus, Trigger>(() => Status, value => Status = value);

                machine.Configure(EvaluationStatus.At_Rater)
                    .Permit(Trigger.Rater_Completed, EvaluationStatus.At_Senior_Rater);

                machine.Configure(EvaluationStatus.At_Senior_Rater)
                    .Permit(Trigger.Return_to_Rater, EvaluationStatus.At_Rater)
                    .PermitIf(Trigger.Senior_Rater_Completed, EvaluationStatus.At_Reviewer, () => ReviewerId.HasValue)
                    .PermitIf(Trigger.Senior_Rater_Completed, EvaluationStatus.Pending_Internal_Review, () => !ReviewerId.HasValue);

                machine.Configure(EvaluationStatus.At_Reviewer)
                    .Permit(Trigger.Reviewer_Completed, EvaluationStatus.Pending_Internal_Review)
                    .Permit(Trigger.Return_to_Rater, EvaluationStatus.At_Rater);

                machine.Configure(EvaluationStatus.Pending_Internal_Review)
                    .Permit(Trigger.Return_to_Rater, EvaluationStatus.At_Rater)
                    .Permit(Trigger.Internal_Review_Completed, EvaluationStatus.Ready_for_Signatures);

                machine.Configure(EvaluationStatus.Ready_for_Signatures)
                    .Permit(Trigger.Return_to_Rater, EvaluationStatus.At_Rater)
                    .Permit(Trigger.Signed, EvaluationStatus.Pending_HQDA_Submission);

                //machine.Configure(EvaluationStatus.Pending_S1_Review)
                    //.Permit(Trigger.Return_to_Rater, EvaluationStatus.At_Rater)
                    //.Permit(Trigger.S1_Review_Completed, EvaluationStatus.Pending_HQDA_Submission);

                machine.Configure(EvaluationStatus.Pending_HQDA_Submission)
                    .Permit(Trigger.Return_to_Rater, EvaluationStatus.At_Rater)
                    .Permit(Trigger.Submitted_to_Hqda, EvaluationStatus.Submitted_to_HQDA);
                
                machine.Configure(EvaluationStatus.Submitted_to_HQDA)
                    .Permit(Trigger.Return_to_Rater, EvaluationStatus.At_Rater)
                    .Permit(Trigger.Accepted_to_iPerms, EvaluationStatus.Accepted_to_iPerms);

                return machine;
            }
        }
    }
}
