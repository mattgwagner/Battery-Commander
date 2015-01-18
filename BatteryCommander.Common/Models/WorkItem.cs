using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatteryCommander.Common.Models
{
    public class WorkItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public String Description { get; set; }

        public int? AssigneeId { get; set; }

        public virtual Soldier Assignee { get; set; }

        [Required]
        public WorkItemStatus Status { get; set; }

        public WorkItem()
        {
            this.Status = WorkItemStatus.Unknown;
        }
    }
}