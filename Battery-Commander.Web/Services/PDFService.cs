using BatteryCommander.Web.Models;
using iTextSharp.text.pdf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

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

            using (var stream = typeof(Program).GetTypeInfo().Assembly.GetManifestResourceStream("Battery-Commander.Web.Models.Data.DA4856.pdf"))
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
                form.SetField($"{prefix}.Page2[0].Assessment[0]", model.Assessment);

                stamper.Close();

                return output.ToArray();
            }
        }

        public static byte[] Generate_DA5500(ABCP model)
        {
            const String prefix = "form1[0].Page1[0]";

            using (var stream = typeof(Program).GetTypeInfo().Assembly.GetManifestResourceStream("Battery-Commander.Web.Models.Data.DA5500.pdf"))
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

                var name = $"{model.Soldier.LastName}, {model.Soldier.FirstName} {model.Soldier.MiddleName?.ToCharArray().FirstOrDefault()}";

                var name_encoded = Encoding.Default.GetBytes(name);

                name = Encoding.ASCII.GetString(name_encoded).Replace('\0', ' ');

                form.SetField($"{prefix}.NAME[0]", name);
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

        public static byte[] Generate_DA5501(ABCP model)
        {
            const String prefix = "form1[0]";

            using (var stream = typeof(Program).GetTypeInfo().Assembly.GetManifestResourceStream("Battery-Commander.Web.Models.Data.DA5501.pdf"))
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

                form.SetField($"{prefix}.WE_FACTR[0]", $"{model.WaistAverage}");
                form.SetField($"{prefix}.HE_FACTR[0]", $"{model.Height}");
                form.SetField($"{prefix}.TOT_A[0]", $"{model.HipAverage + model.WaistAverage}");
                form.SetField($"{prefix}.H_FACTR[0]", $"{model.HipAverage}");
                form.SetField($"{prefix}.N_FACTR[0]", $"{model.NeckAverage}");
                form.SetField($"{prefix}.F_FACTR[0]", $"{model.HipAverage + model.WaistAverage - model.NeckAverage}");
                form.SetField($"{prefix}.HE_FACTR[0]", $"{model.Height}");

                form.SetField($"{prefix}.BODY_FAT[0]", $"{model.BodyFatPercentage}");

                //form1[0].Page1[0].APPRVD[0]

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

        public static byte[] Generate_Vendor_Request_Form(PurchaseOrder order)
        {
            using (var stream = typeof(Program).GetTypeInfo().Assembly.GetManifestResourceStream("Battery-Commander.Web.Models.Data.FLNG_FORM 49D_SAD_Vendor_Request_Form.pdf"))
            using (var output = new MemoryStream())
            {
                var reader = new PdfReader(stream);
                var stamper = new PdfStamper(reader, output);

                var form = stamper.AcroFields;

                // Update the form fields as appropriate

                form.SetField($"operation", order.Operation);

                form.SetField("Check Box18", String.IsNullOrWhiteSpace(order.Justification) ? "Off" : "Yes"); // Purchase of Equipment (Justification)
                form.SetField($"Justification", order.Justification);

                form.SetField($"RequestingUnit", order.Unit.Name);
                form.SetField($"CommandTF", order.Unit.CommandOrTaskForce);
                form.SetField($"UnitPhone", order.Unit.Phone);
                form.SetField($"UnitPOCCell", order.Unit.POC.PhoneNumber);
                form.SetField($"UnitPOC", order.Unit.POC.Name);

                form.SetField($"BusPOC", order.Vendor.POC.Name);
                form.SetField($"Position", order.Vendor.POC.Role);

                form.SetField($"VendorName", order.Vendor.Name);
                form.SetField($"BusPhone", order.Vendor.BusinessPhone);
                form.SetField($"FedID", order.Vendor.FedID);

                form.SetField($"PhysAdd", order.Vendor.PhysicalAddress.Line1);
                form.SetField($"City", order.Vendor.PhysicalAddress.City);
                form.SetField($"State", order.Vendor.PhysicalAddress.State);
                form.SetField($"Zip", order.Vendor.PhysicalAddress.ZipCode);

                form.SetField($"RemitTo", order.Vendor.RemitToAddress.Line1);
                form.SetField($"City2", order.Vendor.RemitToAddress.City);
                form.SetField($"State2", order.Vendor.RemitToAddress.State);
                form.SetField($"Zip2", order.Vendor.RemitToAddress.ZipCode);

                form.SetField($"Other", "");

                Func<PurchaseCategory, String> To_String = (category) => order.Category == category ? "Yes" : "Off";

                form.SetField("Check Box9", To_String(PurchaseCategory.Meals)); // Meals
                form.SetField("Check Box10", To_String(PurchaseCategory.Lodging)); // Lodging
                form.SetField("Check Box11", To_String(PurchaseCategory.OfficeSupplies)); // Office Supplies
                form.SetField("Check Box12", To_String(PurchaseCategory.Fuel)); // Fuel
                form.SetField("Check Box13", To_String(PurchaseCategory.Laundry)); // Laundry
                form.SetField("Check Box14", To_String(PurchaseCategory.RentalVehicles)); // Rental Vehicles
                form.SetField("Check Box15", To_String(PurchaseCategory.RepairParts)); // Repair Parts
                form.SetField("Check Box16", To_String(PurchaseCategory.EquipmentRental)); // Equipment Rental
                form.SetField("Check Box17", To_String(PurchaseCategory.Other)); // Other (Explain)

                stamper.Close();

                return output.ToArray();
            }
        }

        public class PurchaseOrder
        {
            public int Id { get; set; }

            public String Identifier { get; set; }

            public DateTime Date { get; set; } = DateTime.Today;

            public String Operation { get; set; } = "17-20 HURRICANE IRMA";

            public PurchaseCategory Category { get; set; } = PurchaseCategory.Meals;

            public PurchaseOrderUnit Unit { get; set; } = new PurchaseOrderUnit { };

            public OrderVendor Vendor { get; set; } = new OrderVendor { };

            public String Justification { get; set; } = "Hurricane Irma SAD lunch meal for 24 soldiers. Meal will be fed at Ocala for 1 day (9SEP17)";

            public class PurchaseOrderUnit
            {
                public String Name { get; set; } = "C BTRY 2-116TH FA";

                public String Phone { get; set; } = "3527321213";

                public String CommandOrTaskForce { get; set; } = "53 IBCT";

                public PointOfContact POC { get; set; } = new PointOfContact { };
            }

            public class OrderVendor
            {
                public String Name { get; set; }

                public String FedID { get; set; }

                public String BusinessPhone { get; set; }

                public PointOfContact POC { get; set; } = new PointOfContact { };

                public Address PhysicalAddress { get; set; } = new Address { };

                public Address RemitToAddress { get; set; } = new Address { };
            }

            public class PointOfContact
            {
                public String Name { get; set; }

                public String PhoneNumber { get; set; }

                public String Role { get; set; }
            }

            public class Address
            {
                public String Line1 { get; set; }

                public String City { get; set; }

                public String State { get; set; } = "FL";

                public String ZipCode { get; set; }
            }
        }

        public enum PurchaseCategory : byte
        {
            Meals,
            Lodging,
            OfficeSupplies,
            Fuel,
            Laundry,
            RentalVehicles,
            RepairParts,
            EquipmentRental,
            Other
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

            public String Assessment { get; set; }
        }
    }
}