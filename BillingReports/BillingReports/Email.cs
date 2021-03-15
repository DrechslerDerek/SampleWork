using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;
using MailKit.Security;
using MailKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using System.IO;
using MimeKit.Utils;
using System.Reflection;

namespace BillingReports
{

    class Email
    {
        private readonly string[] _EmailAddress = { "brandy@evergreen.com" ,"mrockwell@livecarehealth.com"};
        private string FileName;
        private bool IsWeekly;
        private bool IsMonthly;
        public Email(string path,bool isWeekly, bool isMonthly)
        {
            FileName = path;
            IsMonthly = isMonthly;
            IsWeekly = isWeekly;
        }

        public void AutoEmail()
        {


            using (var smtpClient = new SmtpClient())
            {
                var mailMessage = new MimeMessage();
                if (IsWeekly == true)
                {
                    mailMessage = WeeklyNewMembersReport();
                }
                if(IsMonthly == true)
                {
                    mailMessage = MonthlyBillingReport();
                }
                try
                {
                    
                    smtpClient.Connect("smtp.office365.com", 587, false);
                    smtpClient.Authenticate("mqd@livecarehealth.com", "MQD801@@");
                    smtpClient.Send(mailMessage);
                    smtpClient.Disconnect(true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Bad Email, email failed to send");
                }
            }
            Console.WriteLine("Email sent to billing!");
        }

        private MimeMessage WeeklyNewMembersReport()
        {
            var builder = new BodyBuilder();
            builder.HtmlBody = string.Format(@"<div><h3>Weekly new members with insurance numbers, see attached</h3>
            <h3>This message was auto generated, reply to ddrechsler@livecarehealth.com or mrockwell@livecarehealth.com</h3></div>");
            builder.Attachments.Add(@"C:\Users\Derek-LCH\gulfchroniccare.com\LCH-Team Storage - Documents\TeamStorage\6 Insurance and Billing\NewMembersWeekly\" + FileName);


            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress("Live Care Health", "mqd@livecarehealth.com"));
            mailMessage.To.Add(new MailboxAddress("New Member Weekly Report", _EmailAddress[1]));
            mailMessage.Subject = "New Member Weekly Report";
            mailMessage.Body = builder.ToMessageBody();

            return mailMessage;
        }

        private MimeMessage MonthlyBillingReport()
        {
            var builder = new BodyBuilder();
            builder.HtmlBody = string.Format(@"<div><h3>Monthly billing report, see attached</h3>
            <h3>This message was auto generated, reply to ddrechsler@livecarehealth.com or mrockwell@livecarehealth.com</h3></div>");
            builder.Attachments.Add(@"C: \Users\Derek - LCH\gulfchroniccare.com\LCH - Team Storage - Documents\TeamStorage\6 Insurance and Billing\MonthlyBillingReports\" + FileName);


            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress("Live Care Health", "mqd@livecarehealth.com"));
            mailMessage.To.Add(new MailboxAddress("Monthly Billing Report", _EmailAddress[1]));
            mailMessage.Subject = "Monthly Billing Report";
            mailMessage.Body = builder.ToMessageBody();

            return mailMessage;
        }


    }
}
