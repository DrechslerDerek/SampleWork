using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BillingReports
{
    class TestPerMonthReader
    {
        string FilePath;
        public List<BillableMember> Members;
        public TestPerMonthReader(string path)
        {
            FilePath = path;
            Members = new List<BillableMember>();
        }


        public void ProcessStart()
        {
            ReadCSV();
        }

        private void ReadCSV()
        {
            List<string> testWithNames = new List<string>();
            using (TextFieldParser parser = new TextFieldParser(System.Environment.GetEnvironmentVariable("USERPROFILE") + @"\gulfchroniccare.com\LCH-Team Storage - Documents\TeamStorage\12 Technology Department\Data\testsPerMonth.csv"))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    //Processing row
                    string[] fields = parser.ReadFields();
                    testWithNames.Add(fields[1]+","+fields[8].Substring(0, 10));
                }
            }
            InitializeObjects(testWithNames);
        }

        private void InitializeObjects(List<string> testData)
        {
            List<string> namesToMakeObjects = new List<string>();
            foreach(string s in testData.Distinct())
            {
                namesToMakeObjects.Add(s.Substring(0, s.IndexOf(",")));
            }

            var query = namesToMakeObjects .GroupBy(s => s)
                .Select(g => new { Name = g.Key, Count = g.Count() });

            foreach (var result in query)
            {
                bool isTesting = false;
                if(result.Count>15)
                {
                    isTesting = true;
                }
                BillableMember ghost = new BillableMember
                {
                    Name = result.Name,
                    Code99454 = isTesting

                };
                Members.Add(ghost);
            }

        }

    }
}
