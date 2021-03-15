using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BillingReports
{
    class MergeTestAndTime
    {
        List<BillableMember> Members;
        List<BillableMember> MembersTests;
        List<BillableMember> MembersTimes;
        List<BillableMember> DataBaseMembers;
        MySqlConnection Connection;
        Dictionary<string, string> States;

        public MergeTestAndTime(List<BillableMember> membersTest, List<BillableMember> membersTime, MySqlConnection con)
        {
            Members = new List<BillableMember>();
            DataBaseMembers = new List<BillableMember>();
            MembersTests = membersTest;
            MembersTimes = membersTime;
            Connection = con;
            States = new Dictionary<string, string>();
            IntializeSatesDict();

        }

        public void ProcessStart()
        {
            ExtractDBAll();
            ObjectMerge();
            int i = 0;
            foreach(BillableMember b in Members)
            {
                i++;
                Console.WriteLine(i+". "+b.Name);
            }
        }

        public List<BillableMember> GetFinalMembersList()
        {
            return Members;
        }

        private void ObjectMerge()
        {
            foreach (BillableMember b in DataBaseMembers)
            {
                
                BillableMember memTimeIndividual = MembersTimes.Find(x => x.Name.Equals(b.Name));

                BillableMember memTestIndividual = MembersTests.Find(t => t.Name.Equals(b.Name));
                if (memTimeIndividual != null && memTestIndividual != null)
                {
                    BillableMember ghost = new BillableMember
                    {
                        Name = b.Name,
                        Code99453 = b.Code99453,
                        Code99454 = memTestIndividual.Code99454,                      
                        Code99457 = memTimeIndividual.Code99457,
                        Code99458 = memTimeIndividual.Code99458,
                        DOB = b.DOB,
                        Phone = b.Phone,
                        Address = b.Address,
                        MBI = b.MBI,
                        Gender = b.Gender,
                        DateOfService = b.DateOfService,
                        SupplementalPayer = b.SupplementalPayer,
                        SupplementalID = b.SupplementalID
                    };
                    Members.Add(ghost);
                }
                else if(memTimeIndividual != null)
                {
                    BillableMember ghost = new BillableMember
                    {
                        Name = b.Name,
                        Code99453 = b.Code99453,
                        Code99454 = false,
                        Code99457 = memTimeIndividual.Code99457,
                        Code99458 = memTimeIndividual.Code99458,
                        DOB = b.DOB,
                        Phone = b.Phone,
                        Address = b.Address,
                        MBI = b.MBI,
                        Gender = b.Gender,
                        DateOfService = b.DateOfService,
                        SupplementalPayer = b.SupplementalPayer,
                        SupplementalID = b.SupplementalID
                    };
                    Members.Add(ghost);
                }
                else if(memTestIndividual != null)
                {
                    List<bool> false99458List = new List<bool>
                    {
                        false,
                        false,
                        false
                    };

                    BillableMember ghost = new BillableMember
                    {
                        Name = b.Name,
                        Code99453 = b.Code99453,
                        Code99454 = memTestIndividual.Code99454,
                        Code99457 = false,
                        Code99458 = false99458List,
                        DOB = b.DOB,
                        Phone = b.Phone,
                        Address = b.Address,
                        MBI = b.MBI,
                        Gender = b.Gender,
                        DateOfService = b.DateOfService,
                        SupplementalPayer = b.SupplementalPayer,
                        SupplementalID = b.SupplementalID
                    };
                    Members.Add(ghost);
                }
                else if (memTimeIndividual == null && memTestIndividual == null)
                {
                    List<bool> false99458List = new List<bool>
                    {
                        false,
                        false,
                        false
                    };

                    BillableMember ghost = new BillableMember
                    {
                        Name = b.Name,
                        Code99453 = b.Code99453,
                        Code99454 = false,
                        Code99457 = false,
                        Code99458 = false99458List,
                        DOB = b.DOB,
                        Phone = b.Phone,
                        Address = b.Address,
                        MBI = b.MBI,
                        Gender = b.Gender,
                        DateOfService = b.DateOfService,
                        SupplementalPayer = b.SupplementalPayer,
                        SupplementalID = b.SupplementalID
                    };
                    Members.Add(ghost);
                }
            }
        }

        private void ExtractDBAll()
        {
            Connection.Open();
            string sql = "SELECT * FROM member_data INNER JOIN member_process ON member_data.MBI=member_process.MBI WHERE Onboarded = 'Yes' and Cancel='No'";
            using var query = new MySqlCommand(sql, Connection);
            using MySqlDataReader rdr = query.ExecuteReader();

            while (rdr.Read())
            {
                DataBaseMembers.Add(CreateModel(
                    rdr["Name"].ToString(),
                    rdr["DOB"].ToString(),
                    rdr["HomePhone"].ToString(),
                    rdr["StreetAddress"].ToString().Replace(",", "") + " " + rdr["StreetAddress2"].ToString().Replace(",",""),
                    rdr["City"].ToString().Replace(",", ""),
                    rdr["State"].ToString().Replace(",", ""),
                    rdr["Zip"].ToString(),
                    rdr["MBI"].ToString(),
                    rdr["Gender"].ToString(),
                    rdr["SubmissionDate"].ToString(),
                    rdr["SupplementalPayer"].ToString(),
                    rdr["SupplementalNumber"].ToString()
                    ));
            }
            rdr.Close();
            Connection.Close();

        }

        //in the future this will go of onboard date, for now its just sub date
        private BillableMember CreateModel(string name, string dob, string phone, string street,string city,string state,string zip, string mbi, string gender, string dos, string payer, string id)
        {
            dob = FormatDOB(dob);
            BillableMember temp = new BillableMember
            {
                Name = name,
                DOB = dob,
                Phone = phone.Replace("-",""),
                Address = street+","+city + "," + StateLookup(state) + "," + zip,
                MBI = mbi,
                Gender = gender,
                DateOfService = dos,
                SupplementalPayer = payer,
                SupplementalID = id,
                Code99453 = IfOnboardedThisMonth(dos)
            };
            return temp;
        }

        private bool IfOnboardedThisMonth(string date)
        {
            try
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                CultureInfo enUS = new CultureInfo("en-US");
                date = FormatDate(date);
                var onboardedDate = new DateTime();
                onboardedDate = DateTime.ParseExact(date.Trim(), "MM/dd/yyyy", new CultureInfo("en-US"));
                DateTime compare = new DateTime(2020, 10, 22, 0, 0, 0);
                int compareValue = DateTime.Compare( compare,onboardedDate);
                //Console.WriteLine(compareValue);
                //Console.WriteLine(compare+":"+onboardedDate);
                if (compareValue <= 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private string FormatDate(string date)
        {
            string[] dateArray = date.Split("/");
            try { 
                
                if (dateArray[0].Length < 2)
                {
                    dateArray[0] = "0" + dateArray[0];
                }
                if (dateArray[1].Length < 2)
                {
                    dateArray[1] = "0" + dateArray[1];
                }
            }catch(Exception ex)
            {
                Console.WriteLine(date + " threw: " + ex.Message);
            }
            return dateArray[0] + "/"+dateArray[1] + "/" + dateArray[2];
        }
        private string FormatDOB(string dob)
        {
            if(dob.Substring(0,dob.IndexOf("/")).Length<2)
            {
                dob = "0" + dob;
                return dob;
            }else
            {
                return dob;
            }
        }

        private string StateLookup(string state)
        {

            if(state.Length>2)
                return States[state.Trim()];
            else
                return state.ToUpper();
        }

        private void IntializeSatesDict()
        {
            States.Add("Alabama","AL");
            States.Add("Alaska","AK");
            States.Add("Arizona", "AZ");
            States.Add("Arkansas","AR");
            States.Add("California", "CA");
            States.Add("Colorado", "CO");
            States.Add("Connecticut", "CT");
            States.Add("Delaware", "DE");
            States.Add("District of Columbia", "DC");
            States.Add("Florida", "FL");
            States.Add("Georgia", "GA");
            States.Add("Hawaii","HI");
            States.Add("Idaho", "ID");
            States.Add("Illinois","IL");
            States.Add("Indiana", "IN");
            States.Add("Iowa", "IA");
            States.Add("Kansas", "KS");
            States.Add("Kentucky", "KY");
            States.Add("Louisiana", "LA");
            States.Add("Maine", "ME");
            States.Add("Maryland", "MD");
            States.Add("Massachusetts", "MA");
            States.Add("Michigan", "MI");
            States.Add("Minnesota", "MN");
            States.Add("Mississippi", "MS");
            States.Add("Missouri", "MO");
            States.Add("Montana", "MT");
            States.Add("Nebraska", "NE");
            States.Add("Nevada", "NV");
            States.Add("New Hampshire", "NH");
            States.Add("New Jersey", "NJ");
            States.Add("New Mexico", "NM");
            States.Add("New York", "NY");
            States.Add("North Carolina", "NC");
            States.Add("ND", "North Dakota");
            States.Add("Ohio", "OH");
            States.Add("Oklahoma", "OK");
            States.Add("Oregon", "OR");
            States.Add("Pennsylvania", "PA");
            States.Add("Rhode Island", "RI");
            States.Add("South Carolina", "SC");
            States.Add("South Dakota", "SD");
            States.Add("Tennessee", "TN");
            States.Add("Texas", "TX");
            States.Add("Utah", "UT");
            States.Add("Vermont", "VT");
            States.Add("Virginia", "VA");
            States.Add("Washington", "WA");
            States.Add("West Virginia", "WV");
            States.Add("Wisconsin", "WI");
            States.Add("Wyoming", "WY");

        }
    }
}