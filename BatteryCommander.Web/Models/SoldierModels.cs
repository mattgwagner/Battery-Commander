using BatteryCommander.Common.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace BatteryCommander.Web.Models
{
    public class SoldierEditModel
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public String LastName { get; set; }

        [Required, StringLength(50)]
        public String FirstName { get; set; }

        [Required]
        public Rank Rank { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ETSDate { get; set; }

        [Required]
        public SoldierStatus Status { get; set; }

        // TODO Position - PL, FDO, Section Chief, etc.

        [Required]
        public MOS MOS { get; set; }

        [Required]
        public Group Group { get; set; }

        public SoldierEditModel()
        {
            this.Rank = Rank.E1;
            this.Status = SoldierStatus.Active;
            this.MOS = MOS.Unknown;
            this.Group = Group.GhostGuns;
        }
    }
}