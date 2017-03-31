using BatteryCommander.Web.Models;
using iTextSharp.text.pdf;
using System;
using System.IO;
using System.Reflection;

namespace BatteryCommander.Web.Services
{
    public class PDFService
    {
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
                form.SetField($"{prefix}.Page1[0].Leader_Responsibilities[0]", model.Name);
                form.SetField($"{prefix}.Page1[0].Purpose_Counseling[0]", model.Purpose);
                form.SetField($"{prefix}.Page1[0].Organization[0]", model.Organization);
                form.SetField($"{prefix}.Page1[0].Plan_Action[0]", model.PlanOfAction);

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