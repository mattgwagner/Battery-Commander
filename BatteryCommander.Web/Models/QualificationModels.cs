﻿using BatteryCommander.Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BatteryCommander.Web.Models
{
    public class QualificationEditModel
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public String Name { get; set; }

        [DataType(DataType.MultilineText)]
        public String Description { get; set; }

        [Display(Name = "Parent Task (if applicable)")]
        public int? ParentTaskId { get; set; }

        public IEnumerable<SelectListItem> PossibleParentQualifications { get; set; }
    }

    public class BulkQualificationUpdateModel
    {
        [Required]
        public int QualificationId { get; set; }

        [Required]
        public int SoldierId { get; set; }

        public Soldier Soldier { get; set; }

        [Required]
        public QualificationStatus Status { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime QualificationDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ExpirationDate { get; set; }

        [StringLength(200)]
        public String Comments { get; set; }

        public BulkQualificationUpdateModel()
        {
            this.Status = QualificationStatus.Unknown;
            this.QualificationDate = DateTime.Today;
        }
    }
}