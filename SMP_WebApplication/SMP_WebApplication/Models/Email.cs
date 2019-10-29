using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace SMP_WebApplication.Models
{
    public class Email
    {
        static string email = "smptislindia@gmail.com", password = "group122019"; // site admin

        public static bool Send(string email_to, string subject, string body)
        {

            MailMessage msg = new MailMessage();
            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();

            try
            {
                msg.Subject = subject;
                msg.Body = body;

                msg.From = new MailAddress(email);
                msg.To.Add(email_to);
                msg.IsBodyHtml = true;
                client.Host = "smtp.gmail.com";
                System.Net.NetworkCredential basicauthenticationinfo = new System.Net.NetworkCredential(email, password);
                client.Port = 587;
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = basicauthenticationinfo;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(msg);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}