using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace BillingReports
{
    class BillingReport
    {
        MySqlConnection Connection;
        public string MonthlyReportPathTest;
        public string MonthlyReportPathTime;
        public List<BillableMember> Members;
        public BillingReport(string pathTest, string pathTime, MySqlConnection con)
        {
            MonthlyReportPathTest = pathTest;
            MonthlyReportPathTime = pathTime;
            Connection = con;
            Members = new List<BillableMember>();
        }

        public void ProcessStart()
        {
            TestPerMonthReader csvTestingValues = new TestPerMonthReader(MonthlyReportPathTest);
            TimePerMonthReader csvTimeValues = new TimePerMonthReader(MonthlyReportPathTime);
            MergeTestAndTime mergedMembers = new MergeTestAndTime(csvTestingValues.Members,csvTimeValues.Members,Connection);

            csvTestingValues.ProcessStart();
            csvTimeValues.ProcessStart();
            mergedMembers.ProcessStart() ;

            CSVout writeToFile = new CSVout(mergedMembers.GetFinalMembersList());
            writeToFile.ProcessStart();
        }


    }

}
