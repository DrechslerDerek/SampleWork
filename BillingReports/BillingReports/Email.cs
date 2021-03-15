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
        private readonly string[] _EmailAddress = {};
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



    }
}
