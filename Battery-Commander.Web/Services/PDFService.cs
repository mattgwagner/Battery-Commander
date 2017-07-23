using BatteryCommander.Web.Models;
using iTextSharp.text.pdf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BatteryCommander.Web.Services
{
    public class PDFService
    {
        static PDFService()
        {
            // Who needs the owner password?

            PdfReader.AllowOpenWithFullPermissions = true;
        }

        public static byte[] Generate_DA4856(Counseling model)
        {
            const String prefix = "form1[0]";

            using (var stream = typeof(Program).GetTypeInfo().Assembly.GetManifestResourceStream("BatteryCommander.Web.Models.Data.DA4856.pdf"))
            using (var output = new MemoryStream())
            {
                var reader = new PdfReader(stream);
                var stamper = new PdfStamper(reader, output);

                var form = stamper.AcroFields;

                //form1[0].Page1[0].Name_Title_Counselor[0]
                //form1[0].Page1[0].Key_Points_Disscussion[0]
                //form1[0].Page1[0].Name[0]
                //form1[0].Page1[0].Date_Counseling[0]
                //form1[0].Page1[0].Rank_Grade[0]
                //form1[0].Page2[0].Leader_Responsibilities[0]
                //form1[0].Page1[0].Purpose_Counseling[0]
                //form1[0].Page1[0].Organization[0]
                //form1[0].Page2[0].Plan_Action[0]

                // Update the form fields as appropriate

                form.SetField($"{prefix}.Page1[0].Name[0]", model.Name);
                form.SetField($"{prefix}.Page1[0].Name_Title_Counselor[0]", model.Counselor);
                form.SetField($"{prefix}.Page1[0].Key_Points_Disscussion[0]", model.KeyPointsOfDiscussion);
                form.SetField($"{prefix}.Page1[0].Date_Counseling[0]", model.Date.ToString("yyyy-MM-dd"));
                form.SetField($"{prefix}.Page1[0].Rank_Grade[0]", model.Rank.DisplayName());
                form.SetField($"{prefix}.Page2[0].Leader_Responsibilities[0]", model.LeadersResponsibilities);
                form.SetField($"{prefix}.Page1[0].Purpose_Counseling[0]", model.Purpose);
                form.SetField($"{prefix}.Page1[0].Organization[0]", model.Organization);
                form.SetField($"{prefix}.Page2[0].Plan_Action[0]", model.PlanOfAction);

                stamper.Close();

                return output.ToArray();
            }
        }

        public static byte[] Generate_DA5500(ABCP model)
        {
            const String prefix = "form1[0].Page1[0]";

            using (var stream = typeof(Program).GetTypeInfo().Assembly.GetManifestResourceStream("BatteryCommander.Web.Models.Data.DA5500.pdf"))
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

                // Update the form fields as appropriate

                form.SetField($"{prefix}.NAME[0]", $"{model.Soldier.LastName}, {model.Soldier.FirstName} {model.Soldier.MiddleName?.ToCharArray().FirstOrDefault()}");
                form.SetField($"{prefix}.RANK[0]", $"{model.Soldier.Rank.ShortName()}");

                form.SetField($"{prefix}.HEIGHT[0]", $"{model.Height}");
                form.SetField($"{prefix}.WEIGHT[0]", $"{model.Weight}");
                form.SetField($"{prefix}.AGE[0]", $"{model.Soldier.AgeAsOf(model.Date)}");

                form.SetField($"{prefix}.DATE_A[0]", $"{model.Date:yyyyMMdd}");
                form.SetField($"{prefix}.DATE_B[0]", $"{model.Date:yyyyMMdd}");

                var q = new Queue<String>(new[] { "FIRST", "SCND", "THIRD" });

                foreach (var measurement in model.Measurements)
                {
                    // Abdomen

                    var m = q.Dequeue();

                    form.SetField($"{prefix}.{m}_A[0]", $"{measurement.Neck}");
                    form.SetField($"{prefix}.{m}_B[0]", $"{measurement.Waist}");
                }

                // Check

                if (model.RequiresTape)
                {
                    form.SetField($"{prefix}.IS[0]", model.IsPassing ? "1" : "0");
                    form.SetField($"{prefix}.ISNOT[0]", model.IsPassing ? "0" : "2");
                }

                form.SetField($"{prefix}.AVE_B[0]", $"{model.WaistAverage}");
                form.SetField($"{prefix}.AVE_A[0]", $"{model.NeckAverage}");
                form.SetField($"{prefix}.AVE_C[0]", $"{model.NeckAverage}");
                form.SetField($"{prefix}.AVE_D[0]", $"{model.WaistAverage}");
                form.SetField($"{prefix}.AVE_E[0]", $"{model.CircumferenceValue}");
                form.SetField($"{prefix}.AVE_F[0]", $"{model.Height}");
                form.SetField($"{prefix}.AVE_G[0]", $"{model.BodyFatPercentage}%");

                form.SetField($"{prefix}.REMRKS[0]", $@"
                    Soldier's Actual Weight: {model.Weight} lbs
                    Screening Table Weight: {model.Screening_Weight} lbs
                    {(model.RequiresTape ? "OVER " : "UNDER")} {(Math.Abs(model.Screening_Weight - model.Weight))} lbs

                    Soldier's Actual Body Fat %: {model.BodyFatPercentage}%
                    Authorized Body Fat %: {model.MaximumAllowableBodyFat}%

                    Individual is {(model.IsPassing ? "" : "not")} in compliance with Army standards.
                ");

                stamper.Close();

                return output.ToArray();
            }
        }

        public static byte[] Generate_DA5501(ABCP model)
        {
            const String prefix = "form1[0]";

            using (var stream = typeof(Program).GetTypeInfo().Assembly.GetManifestResourceStream("BatteryCommander.Web.Models.Data.DA5501.pdf"))
            using (var output = new MemoryStream())
            {
                var reader = new PdfReader(stream);
                var stamper = new PdfStamper(reader, output);

                var form = stamper.AcroFields;

                // Update the form fields as appropriate

#if DEBUG
                foreach (DictionaryEntry de in form.Fields)
                {
                    Console.WriteLine($"{de.Key}");
                }
#endif

                // Update the form fields as appropriate

                form.SetField($"{prefix}.NAME[0]", $"{model.Soldier.LastName} {model.Soldier.FirstName}");
                form.SetField($"{prefix}.RANK[0]", $"{model.Soldier.Rank.ShortName()}");

                form.SetField($"{prefix}.HEIGHT[0]", $"{model.Height}");
                form.SetField($"{prefix}.WEIGHT[0]", $"{model.Weight}");
                form.SetField($"{prefix}.AGE[0]", $"{model.Soldier.AgeAsOf(model.Date)}");

                form.SetField($"{prefix}.DATE[0]", $"{model.Date:yyyyMMdd}");
                form.SetField($"{prefix}.DATE_B[0]", $"{model.Date:yyyyMMdd}");

                // form.SetField($"{prefix}.Page1[0].Name[0]", model.Name);

                //form1[0].Page1[0].NECK_A[0] / B / C

                var q = new Queue<String>(new[] { "A", "B", "C" });

                foreach (var measurement in model.Measurements)
                {
                    // Abdomen

                    var m = q.Dequeue();

                    form.SetField($"{prefix}.NECK_{m}[0]", $"{measurement.Neck}");
                    form.SetField($"{prefix}.ARM_{m}[0]", $"{measurement.Waist}");
                    form.SetField($"{prefix}.HIP_{m}[0]", $"{measurement.Hips}");
                }

                form.SetField($"{prefix}.AVE_NECK[0]", $"{model.NeckAverage}");
                form.SetField($"{prefix}.AVE_ARM[0]", $"{model.WaistAverage}");
                form.SetField($"{prefix}.AVE_HIP[0]", $"{model.HipAverage}");

                //form1[0].Page1[0].H_FACTR[0]
                //form1[0].Page1[0].N_FACTR[0]
                //form1[0].Page1[0].F_FACTR[0]
                //form1[0].Page1[0].WE_FACTR[0]
                //form1[0].Page1[0].HE_FACTR[0]

                //form1[0].Page1[0].TOT_A[0]

                //form1[0].Page1[0].APPRVD[0]
                //form1[0].Page1[0].BODY_FAT[0]

                form.SetField($"{prefix}.REMRKS[0]", $@"
                    AUTHORIZED BODY FAT IS: {model.MaximumAllowableBodyFat}%
                         TOTAL BODY FAT IS: {model.BodyFatPercentage}%

                    SOLDIER {(model.IsPassing ? "MEETS" : "DOES NOT MEET")} ARMY STANDARDS
                ");

                // Check

                form.SetField($"{prefix}.IS[0]", model.IsPassing ? "1" : "0");
                form.SetField($"{prefix}.ISNOT[0]", model.IsPassing ? "0" : "2");

                stamper.Close();

                return output.ToArray();
            }
        }

        public class Counseling
        {
            public String Name { get; set; }

            public Rank Rank { get; set; }

            public DateTime Date { get; set; } = DateTime.Today;

            public String Organization { get; set; }

            public String Counselor { get; set; }

            public String Purpose { get; set; }

            public String KeyPointsOfDiscussion { get; set; }

            public String PlanOfAction { get; set; }

            public String LeadersResponsibilities { get; set; }
        }
    }
}