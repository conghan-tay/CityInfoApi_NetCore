using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.Services
{
    public class LocalMailServices : IMailService
    {
        //private string _mailTo = "admin@mycompany.com";
        //private string _mailFrom = "noreply@mycompany.com";

        private string _mailTo = Startup.Configuration["mailSettings:mailToAddress"];
        private string _mailFrom = Startup.Configuration["mailSettings:mailFromAddress"];

        public void Send(string subject, string message)
        {
            Debug.WriteLine($"Mail from {_mailFrom} to {_mailTo}, with LocalMailServices");
            Debug.WriteLine($"Subject:  {subject} ");
            Debug.WriteLine($"Message {message} ");
        }
    }
}
