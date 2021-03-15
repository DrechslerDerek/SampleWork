using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BillingReports
{
    class TimePerMonthReader
    {
        string FilePath;
        public List<BillableMember> Members;

        public TimePerMonthReader(string path)
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
            using (TextFieldParser parser = new TextFieldParser(System.Environment.GetEnvironmentVariable("USERPROFILE") + @"\gulfchroniccare.com\LCH-Team Storage - Documents\TeamStorage\12 Technology Department\Data\timePerMonth.csv"))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    //Processing row
                    string[] fields = parser.ReadFields();
                    testWithNames.Add(fields[1] + "," + fields[9]);
                }
            }
            InitializeObjects(testWithNames);
        }

        private void InitializeObjects(List<string> testData)
        {
            Dictionary<string, int> membersTime = new Dictionary<string, int>();
            foreach (string s in testData.Skip(1))
            {

                string name = s.Substring(0, s.IndexOf(","));
                try
                {
                    int timeInterval = Int32.Parse(s.Substring(s.IndexOf(",") + 1));
                    if (membersTime.ContainsKey(name))
                    {
                        int concatTime = membersTime[name];
                        membersTime.Remove(name);
                        membersTime.Add(name, concatTime + timeInterval);
                    }
                    else
                    {
                        membersTime.Add(name, timeInterval);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message+" in: "+this.GetType());
                }
            }

            foreach (KeyValuePair<string, int> entry in membersTime)
            {

                List<bool> additional20 = new List<bool>();

                bool first20 = false;
                if (entry.Value > 1200)
                {
                    first20 = true;
                }
                if(entry.Value > 2400)
                {
                    additional20.Add(true);
                }
                else
                {
                    additional20.Add(false);
                }
                if (entry.Value > 3600)
                {
                    additional20.Add(true);
                }
                else
                {
                    additional20.Add(false);
                }
                if (entry.Value > 4800)
                {
                    additional20.Add(true);
                }
                else
                {
                    additional20.Add(false);
                }

                BillableMember ghost = new BillableMember
                {
                    Name = entry.Key,
                    Code99457 = first20,
                    Code99458 = additional20
                };
                Members.Add(ghost);
               
            }

        }
    }
}
