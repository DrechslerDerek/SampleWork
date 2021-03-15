using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;

namespace BillingReports
{
    class CSVout
    {
        List<BillableMember> Members;
        string Date;
        public CSVout(List<BillableMember> members)
        {
            Members = members;
            Date = DateTime.Today.ToString();
            Date = Date.Substring(0, Date.IndexOf(" "));
            Date = Date.Replace(@"/", "-");

        }

        public void ProcessStart()
        {
            WriteCsv();
        }

        private void WriteCsv()
        {
            Console.WriteLine("writing....");
            string filepath = System.Environment.GetEnvironmentVariable("USERPROFILE") + @"\gulfchroniccare.com\LCH-Team Storage - Documents\TeamStorage\6 Insurance and Billing\MonthlyBillingReports\Billing SpreadSheet" + Date+".csv";

            using (StreamWriter writer = new StreamWriter(new FileStream(filepath, FileMode.Create, FileAccess.Write)))
            {
                writer.WriteLine("sep=,");
                writer.WriteLine(Header());
                foreach(BillableMember b in Members)
                {
                    string[] nameSplit = b.Name.Split(" ");
                    string[] address = b.Address.Split(",");

                    string[] codes = GetCodes(b);

                    writer.WriteLine(b.DateOfService + "," +
                        nameSplit[0] + ", ," +
                        nameSplit[1] + "," +
                        b.DOB + "," +
                        b.Phone + "," +
                        address[0] + "," +
                        address[1] + "," +
                        address[2] + "," +
                        address[3] + ", , ," +//null fields email + last4
                        b.Gender + "," +
                        b.MBI + "," +
                        b.SupplementalID + "," +
                        b.SupplementalPayer + ", ,E11.8," +
                    codes[0] + "," +
                    codes[1] + "," +
                    codes[2] + "," +
                    codes[3]);

                }
            }

        }

        private string[] GetCodes(BillableMember b)
        {
            string[] tmpCodes = new string[4];
            for(int i = 0; i<tmpCodes.Length; i++)
            {
                tmpCodes[i] = " ";
            }

            if(b.Code99453==true)
            {
                tmpCodes[0] = "X";
            }
            if (b.Code99454 == true)
            {
                tmpCodes[1] = "X";
            }
            if (b.Code99457 == true)
            {
                tmpCodes[2] = "X";
            }

            int cd458 = 0;
            for (int i = 0; i < 3; i++)
            {
                if (b.Code99458[i] == true)
                {
                    cd458++;
                }
            }
            tmpCodes[3]=cd458.ToString();
            return tmpCodes;
        }
        private string Header()
        {
            return "Date of Service,First Name,MI,Last Name,DOB,Phone Number,Address,City,State,Zip Code," +
                "Email,Last 4 of social,Gender,Medicare Number,MemberID,Insurance Name,Billing Notes,DX code," +
                "Monitoring codes - 99453,Monitoring codes - 99454,Monitoring codes - 99457,Monitoring codes - 99458";
        }

    }
}
