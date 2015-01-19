using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatteryCommander.Common.Models
{
    public class Alert
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // TODO Recipient(s)

        // TODO Sender

        // TODO Send Status?

        [Required, StringLength(140)]
        public String Message { get; set; }

        [Required]
        public DateTime SendDateUtc { get; set; }
    }
}