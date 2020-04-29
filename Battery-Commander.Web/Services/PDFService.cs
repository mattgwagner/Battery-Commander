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
            var result = await forms_client.GenerateDA4856Async(model);

            using var memory = new MemoryStream();

            await result.Stream.CopyToAsync(memory);

            return memory.ToArray();
        }

        public static async Task<byte[]> Generate_DA5500(ABCP model)
        {
            var result = await forms_client.GenerateDA5500Async(new Forms.ABCP
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
            var result = await forms_client.GenerateDA3749Async(new EquipmentReceipt
            {

            });

            using var memory = new MemoryStream();

            await result.Stream.CopyToAsync(memory);

            return memory.ToArray();
        }

        public static async Task<byte[]> Generate_DA5501(ABCP model)
        {
            var result = await forms_client.GenerateDA5501Async(new Forms.ABCP
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