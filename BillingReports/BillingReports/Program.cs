using System;

namespace BillingReports
{
    class Program
    {
        static void Main(string[] args)
        {
            Connection databaseConnection = new Connection();

            string filePathTests = System.Environment.GetEnvironmentVariable("USERPROFILE")+ @"\gulfchroniccare.com\LCH-Team Storage - Documents\TeamStorage\12 Technology Department\Data\testsPerMonth.csv";
            string filePathTime = System.Environment.GetEnvironmentVariable("USERPROFILE") + @"\gulfchroniccare.com\LCH-Team Storage - Documents\TeamStorage\12 Technology Department\Data\timePerMonth.csv";

            BillingReport RPT = new BillingReport(filePathTests,filePathTime,databaseConnection.GlobalConnection);

            RPT.ProcessStart();
        }
    }
}
