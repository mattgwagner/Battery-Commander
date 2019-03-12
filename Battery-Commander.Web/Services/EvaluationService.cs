using Microsoft.EntityFrameworkCore;
using Stateless;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BatteryCommander.Web.Models
{
    public class EvaluationService
    {
        public static IQueryable<Evaluation> Filter(Database db, Query query)
        {
            IQueryable<Evaluation> evaluations =
                db
                .Evaluations
                .Include(evaluation => evaluation.Ratee)
                .Include(evaluation => evaluation.Rater)
                .Include(evaluation => evaluation.SeniorRater)
                .Include(evaluation => evaluation.Reviewer)
                .Include(evaluation => evaluation.Events)
                .AsQueryable();

            if (query.Delinquent.HasValue)
            {
                evaluations = evaluations.Where(evaluation => evaluation.IsDelinquent == query.Delinquent);
            }

            if (query.Complete.HasValue)
            {
                evaluations = evaluations.Where(evaluation => evaluation.IsCompleted == query.Complete);
            }

            if (query.Unit.HasValue)
            {
                evaluations = evaluations.AsEnumerable().Where(evaluation =>
                {
                    if (evaluation.Ratee.UnitId == query.Unit) return true;

                    if (evaluation.Rater.UnitId == query.Unit) return true;

                    if (evaluation.SeniorRater.UnitId == query.Unit) return true;

                    if (evaluation.Reviewer?.UnitId == query.Unit) return true;

                    return false;
                })
                .AsQueryable();
            }

            return
                evaluations
                .OrderBy(evaluation => evaluation.ThruDate);
        }

        public class Query
        {
            public Boolean? Delinquent { get; set; }

            public Boolean? Complete { get; set; }

            public int? Unit { get; set; }
        }
    }

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

            [Display(Name = "Rater Signature Completed")]
            Rater_Signed,

            [Display(Name = "Senior Rater Signature Completed")]
            Senior_Rater_Signed,

            [Display(Name = "Rated Solider Signature Completed")]
            Soldier_Signed,

            [Display(Name = "Reviewer Signature Completed")]
            Reviewer_Signed,

            [Display(Name = "Signatures Completed")]
            Signed,

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
                    .PermitIf(Trigger.Senior_Rater_Completed, EvaluationStatus.At_1SG_Review, () => !ReviewerId.HasValue);

                machine.Configure(EvaluationStatus.At_Reviewer)
                    .Permit(Trigger.Reviewer_Completed, EvaluationStatus.At_1SG_Review)
                    .Permit(Trigger.Return_to_Rater, EvaluationStatus.At_Rater);

                machine.Configure(EvaluationStatus.At_1SG_Review)
                    .Permit(Trigger.Return_to_Rater, EvaluationStatus.At_Rater)
                    .Permit(Trigger.Internal_Review_Completed, EvaluationStatus.Ready_for_Signatures);

                machine.Configure(EvaluationStatus.Ready_for_Signatures)
                    .Permit(Trigger.Return_to_Rater, EvaluationStatus.At_Rater)
                    .Permit(Trigger.Rater_Signed, EvaluationStatus.Pending_Senior_Rater_Signature);

                machine.Configure(EvaluationStatus.Pending_Senior_Rater_Signature)
                    .PermitIf(Trigger.Senior_Rater_Signed, EvaluationStatus.Pending_SM_Signature);

                machine.Configure(EvaluationStatus.Pending_SM_Signature)
                    .PermitIf(Trigger.Soldier_Signed, EvaluationStatus.Pending_Reviewer_Signature, () => ReviewerId.HasValue)
                    .PermitIf(Trigger.Soldier_Signed, EvaluationStatus.Pending_HQDA_Submission, () => !ReviewerId.HasValue);

                machine.Configure(EvaluationStatus.Pending_Reviewer_Signature)
                    .PermitIf(Trigger.Reviewer_Signed, EvaluationStatus.Pending_HQDA_Submission);

                machine.Configure(EvaluationStatus.Pending_HQDA_Submission)
                    .Permit(Trigger.Submitted_to_Hqda, EvaluationStatus.Submitted_to_HQDA)
                    .Permit(Trigger.Accepted_to_iPerms, EvaluationStatus.Accepted_to_iPerms);

                machine.Configure(EvaluationStatus.Submitted_to_HQDA)
                    .Permit(Trigger.Return_to_Rater, EvaluationStatus.At_Rater)
                    .Permit(Trigger.Accepted_to_iPerms, EvaluationStatus.Accepted_to_iPerms);

                return machine;
            }
        }
    }
}