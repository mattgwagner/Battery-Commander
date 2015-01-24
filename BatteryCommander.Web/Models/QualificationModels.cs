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
        public Qualification Qualification { get; set; }

        public IEnumerable<ModelRow> Rows { get; set; }

        public BulkQualificationUpdateModel()
        {
            this.Rows = Enumerable.Empty<ModelRow>();
        }

        public class ModelRow
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
            public DateTime QualificationDate { get; set; }

            [DataType(DataType.Date)]
            public DateTime? ExpirationDate { get; set; }

            public ModelRow()
            {
                this.Status = QualificationStatus.Unknown;
                this.QualificationDate = DateTime.Today;
            }
        }
    }
}