using System;
using System.Collections.Generic;
using System.Text;

namespace BillingReports
{
    public class BillableMember
    {
        public string Name { get; set; }
        public string DOB { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string MBI { get; set; }
        public string Last4 { get; set; }
        public string Gender { get; set; }
        public string DateOfService { get; set; }
        public string SupplementalPayer { get; set; }
        public string SupplementalID { get; set; }
        public bool Code99453 {get;set;} //setup, for onboarding todo
        public bool Code99454 { get; set; }//16 tests per month
        public bool Code99457 { get; set; } //first 20 minutes managed time
        public List<bool> Code99458 { get; set; } //additional 20 minutes of managed time

        public bool Equals(BillableMember obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else if(this.Name.Equals(obj.Name))
            {
                    return true;
            }
            else
            {
                return false;
            }
        }
    }
}
