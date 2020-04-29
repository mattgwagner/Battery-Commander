using BatteryCommander.Web.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Models
{
    public class Weapon : IValidatableObject
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UnitId { get; set; }

        public virtual Unit Unit { get; set; }

        [Required(AllowEmptyStrings = false), StringLength(10)]
        public String AdminNumber { get; set; }

        [Required(AllowEmptyStrings = false), StringLength(50)]
        public String Serial { get; set; }

        [Display(Name = "Optic Serial")]
        [StringLength(50)]
        public String OpticSerial { get; set; }

        public WeaponOptic OpticType { get; set; } = WeaponOptic.CCO;

        public Soldier Assigned { get; set; }

        public int? AssignedId { get; set; }

        public WeaponType Type { get; set; } = WeaponType.M4;

        public enum WeaponOptic : byte
        {
            None = 0,

            CCO = 1,

            ACOG = 2
        }

        public enum WeaponType : byte
        {
            [Display(Name = "Rifle, 5.56mm, M4")]
            M4 = 0,

            M9 = 1,

            M240B = 2,

            M249 = 3,

            M320 = 4
        }

        [NotMapped]
        public virtual String StockNumber
        {
            get
            {
                switch (Type)
                {
                    case WeaponType.M4:
                        return "1005-01-231-0973";

                    // TODO The rest of the descriptions
                    default:
                        return String.Empty;
                }
            }
        }

        public virtual Task<byte[]> GenerateReceipt()
        {
            return PDFService.Generate_DA3749(new Services.Forms.EquipmentReceipt
            {
                Name = $"{Assigned?.LastName} {Assigned?.FirstName}",
                Grade = (Services.Forms.Rank)Assigned?.Rank,
                From = "Arms Room",
                ReceiptNumber = $"{Type}-{AdminNumber}",
                SerialNumber = Serial,
                Unit = Unit.Name,
                Description = Type.DisplayName(),
                StockNumber = StockNumber
            });
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (String.IsNullOrWhiteSpace(AdminNumber)) yield return new ValidationResult("Admin Number must be populated", new[] { nameof(AdminNumber) });
        }
    }
}