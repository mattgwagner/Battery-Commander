using BatteryCommander.Web.Models;
using iTextSharp.text.pdf;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Services
{
    public class PDFService
    {
        private static readonly HttpClient http = new HttpClient { };

        private static Forms.RedLeg_FormsClient forms_client => new Forms.RedLeg_FormsClient("https://forms.redleg.app", http);

        static PDFService()
        {
            // Who needs the owner password?

            PdfReader.AllowOpenWithFullPermissions = true;
        }

        public static async Task<byte[]> Generate_DA4856Async(Forms.Counseling model)
        {
            var result = await forms_client.DA4856Async(model);

            using var memory = new MemoryStream();

            await result.Stream.CopyToAsync(memory);

            return memory.ToArray();
        }

        public static async Task<byte[]> Generate_DA5500(ABCP model)
        {
            var result = await forms_client.DA5500Async(new Forms.ABCP
            {
                Soldier = new Forms.ABCP_Soldier
                {
                    FirstName = model.Soldier.FirstName,
                    MiddleName = model.Soldier.MiddleName,
                    LastName = model.Soldier.LastName,
                    Age = model.Soldier.AgeAsOf(model.Date),
                    Gender = (Forms.Gender)model.Soldier.Gender,
                    Rank = (Forms.Rank)model.Soldier.Rank
                },
                Date = model.Date,
                Height = (double)model.Height,
                Weight = model.Weight,
                Measurements =
                    model
                    .Measurements
                    .Select(measurement => new Forms.Measurement
                    {
                        Neck = measurement.Neck,
                        Waist = measurement.Waist
                    })
                    .ToList()
            });

            using var memory = new MemoryStream();

            await result.Stream.CopyToAsync(memory);

            return memory.ToArray();
        }

        public static byte[] Generate_DA3749(EquipmentReceipt model)
        {
            const String prefix = "form1[0].Page1[0]";

            using (var stream = typeof(Program).GetTypeInfo().Assembly.GetManifestResourceStream("Battery-Commander.Web.Models.Data.DA3749.pdf"))
            using (var output = new MemoryStream())
            {
                var reader = new PdfReader(stream);
                var stamper = new PdfStamper(reader, output);

                var form = stamper.AcroFields;

#if DEBUG
                foreach (DictionaryEntry de in form.Fields)
                {
                    Console.WriteLine($"{de.Key}");
                }
#endif

                // Update the form fields as appropriate -- 4 per page, _B, _C, _D

                form.SetField($"{prefix}.UNIT[0]", $"{model.Unit}");
                form.SetField($"{prefix}.RECEIPT[0]", $"{model.ReceiptNumber}");
                form.SetField($"{prefix}.STOCK[0]", $"{model.StockNumber}");
                form.SetField($"{prefix}.SERIAL[0]", $"{model.SerialNumber}");
                form.SetField($"{prefix}.DESCRIPT[0]", $"{model.Description}");
                form.SetField($"{prefix}.FROM[0]", $"{model.From}");
                form.SetField($"{prefix}.NAME[0]", $"{model.Name}");
                form.SetField($"{prefix}.GRADE[0]", $"{model.Grade?.ShortName()}");

                stamper.Close();

                return output.ToArray();
            }
        }

        public static async Task<byte[]> Generate_DA5501(ABCP model)
        {
            var result = await forms_client.DA5501Async(new Forms.ABCP
            {
                Soldier = new Forms.ABCP_Soldier
                {
                    FirstName = model.Soldier.FirstName,
                    MiddleName = model.Soldier.MiddleName,
                    LastName = model.Soldier.LastName,
                    Age = model.Soldier.AgeAsOf(model.Date),
                    Gender = (Forms.Gender)model.Soldier.Gender,
                    Rank = (Forms.Rank)model.Soldier.Rank
                },
                Date = model.Date,
                Height = (double)model.Height,
                Weight = model.Weight,
                Measurements =
                    model
                    .Measurements
                    .Select(measurement => new Forms.Measurement
                    {
                        Hips = measurement.Hips,
                        Neck = measurement.Neck,
                        Waist = measurement.Waist
                    })
                    .ToList()
            });

            using var memory = new MemoryStream();

            await result.Stream.CopyToAsync(memory);

            return memory.ToArray();
        }

        public class EquipmentReceipt
        {
            public String Unit { get; set; }

            public String ReceiptNumber { get; set; }

            public String StockNumber { get; set; }

            public String SerialNumber { get; set; }

            public String Description { get; set; }

            public String From { get; set; }

            public String Name { get; set; }

            public Rank? Grade { get; set; }
        }
    }
}