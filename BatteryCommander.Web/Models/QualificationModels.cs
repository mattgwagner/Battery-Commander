using BatteryCommander.Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BatteryCommander.Web.Models
{
    public class QualificationEditModel
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public String Name { get; set; }

        public String Description { get; set; }

        // TODO
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