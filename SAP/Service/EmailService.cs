using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace SAP.Service
{
    public interface IEmailService
    {
        void SendMail(string toEmailAddress, string emailSubject, string emailMessage);
    }
    public class EmailService : IEmailService
    {
        public void SendMail(string toEmailAddress, string emailSubject, string emailMessage)
        {
            var message = new MailMessage();
            message.To.Add(toEmailAddress);
            message.IsBodyHtml = true;
            message.Subject = emailSubject;
            message.Body = emailMessage;

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Send(message);
            }
        }
    }
    
}