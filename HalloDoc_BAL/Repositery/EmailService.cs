using HalloDoc_BAL.Interface;
using HalloDoc_DAL.Models;
//using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Net.Mail;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.Repositery
{
    public class EmailService : IEmailService
    {


        public bool SendEmail(string uemail, string subject, string body)
        {

            //var email = new MimeMessage();

            //email.From.Add(MailboxAddress.Parse("yashu@gmail.com"));
            //email.To.Add(MailboxAddress.Parse(uemail));
            //email.Subject = subject;
            //email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };


            //using var smtp = new SmtpClient();
            //smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            //smtp.Authenticate("yashusoni003@gmail.com", "nkrr wbqn zbaw aezs");
            //smtp.Send(email);
            //smtp.Disconnect(true);


            MailMessage message = new MailMessage();
            message.From = new MailAddress("yash.soni@etatvasoft.com");
            message.Subject = subject;
            message.To.Add(new MailAddress(uemail));
            message.Body = body;
            message.IsBodyHtml = true;


            try
            {

                using (var smtpClient = new SmtpClient("mail.etatvasoft.com"))
                {
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new NetworkCredential("yash.soni@etatvasoft.com", "kaX9Bjj8Sho");
                    smtpClient.EnableSsl = true;

                    smtpClient.Send(message);

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }



        }

    }
}
