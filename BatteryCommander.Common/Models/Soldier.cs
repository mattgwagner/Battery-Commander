using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatteryCommander.Common.Models
{
    public class Soldier
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, StringLength(50)]
        public String LastName { get; set; }

        [Required, StringLength(50)]
        public String FirstName { get; set; }

        [Required]
        public Rank Rank { get; set; }

        public SoldierStatus Status { get; set; }

        // TODO Position - PL, FDO, Section Chief, etc.

        // TODO MOS & Duty MOSQ'd

        public Soldier()
        {
            this.Rank = Rank.E1;
            this.Status = SoldierStatus.Active;
        }
    }
}