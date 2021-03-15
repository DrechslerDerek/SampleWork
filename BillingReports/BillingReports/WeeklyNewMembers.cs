using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace BillingReports
{
    class WeeklyNewMembers
    {
        public string ReportPath;
        public MySqlConnection DataBase;

        public WeeklyNewMembers(MySqlConnection con)
        {
            DataBase = con;
            ReportPath = "";
        }

        public void ProcessStart(string path)
        {
            while (true)
            {
                Console.WriteLine("1) Generate new report\n2) Read uploaded report\nq) quit");
                string c = Console.ReadLine();
                if (c == "1")
                {
                    GenerateWeeklyNewMembers(path);
                    FilePath();
                    break;
                }
                else if (c == "2")
                {
                    FindNewFile();
                    break;
                }
                else if (c == "q")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid Choice!");
                }
            }
        }
        //makes a spreadsheet of new members per week, displays all ins info
        public void GenerateWeeklyNewMembers(string fileOutputPath)
        {
            ArrayList listOfNewMembers = ReadMembers();

            string date = GetDate().ToShortDateString().Replace(@"\", ".");
            date = date.Replace('/','_');

            fileOutputPath = @"C:\Users\Derek-LCH\gulfchroniccare.com\LCH-Team Storage - Documents\TeamStorage\6 Insurance and Billing\NewMembersWeekly\";
            fileOutputPath += "NewMembers" + date+ ".csv";

            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileOutputPath, true))
                {
                    file.WriteLine("Name,Phone,DOB,Address,City,State,Zip,MBI,Supplemental Payer,Supplementat ID,BIN,Group");
                    foreach(string s in listOfNewMembers)
                    {
                        file.WriteLine(s);
                        Console.WriteLine(s);
                    }

                }
                Console.WriteLine("New members excel file created!");
                ReportPath = "NewMembers" + date + ".csv";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        //gets file path of new xlsx file generated
        public void FilePath()
        {

        }

        //searches for file automatically uploaded
        public void FindNewFile()
        {

        }

        private DateTime GetDate()
        {
            return DateTime.Now;
        }

        private ArrayList ReadMembers()
        {
            DateTime date = GetDate();
            DataBase.Open();

            string sql = "SELECT members.SubmissionDate, members.FirstName, members.LastName,members.Phone,members.DOB,members.StreetAddress,members.City," +
                "members.State,members.Zip,members.MedicareNumber,members.OtherInsurance,members.InsuranceID,members.BIN,members.Group " +
                            "FROM members " +
                        "INNER JOIN membersqual ON members.MedicareNumber = membersqual.MedicareNumber WHERE Prescribed = 'Yes';";

            using var query = new MySqlCommand(sql, DataBase);
            using MySqlDataReader rdr = query.ExecuteReader();

            ArrayList returnedQuery = new ArrayList();
            while (rdr.Read())
            {
                string dateString = rdr["SubmissionDate"].ToString();
                var submissionDate = DateTime.Parse(dateString);
                var difference = date.Subtract(submissionDate);
                if (difference.TotalDays < 8.00)
                {
                    string line = "";
                    line += rdr["FirstName"].ToString().Trim() + " ";
                    line += rdr["LastName"].ToString().Trim() + ",";
                    line += rdr["Phone"].ToString().Trim() + ",";
                    line += rdr["DOB"].ToString().Trim() + ",";
                    line += rdr["StreetAddress"].ToString().Trim() + ",";
                    line += rdr["City"].ToString().Trim() + ",";
                    line += rdr["State"].ToString().Trim() + ",";
                    line += rdr["Zip"].ToString().Trim() + ",";
                    line += rdr["MedicareNumber"].ToString().Trim() + ",";
                    line += rdr["OtherInsurance"].ToString().Trim() + ",";

                    line += rdr["InsuranceID"].ToString().Trim() + ",";
                    line += rdr["BIN"].ToString().Trim() + ",";
                    line += rdr["Group"].ToString().Trim();
                    returnedQuery.Add(line);
                    Console.WriteLine(line);

                }
            }
            rdr.Close();
            DataBase.Close();
            return returnedQuery;
        }
    }
}
