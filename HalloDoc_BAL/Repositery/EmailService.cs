using HalloDoc_BAL.Interface;
using HalloDoc_DAL.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Net.Mail;
using System.Net;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.Repositery
{
    public class EmailService : IEmailService
    {


        public Object SendEmail(string uemail, string subject, string body)
        {

            var email = new MimeMessage();

            email.From.Add(MailboxAddress.Parse("yashu@gmail.com"));
            email.To.Add(MailboxAddress.Parse(uemail));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };


            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate("yashusoni003@gmail.com", "nkrr wbqn zbaw aezs");
            smtp.Send(email);
            smtp.Disconnect(true);

            // Create the MailMessage object
            //MailMessage mail = new MailMessage("yashusoni003@gmail.com", uemail, subject, body);

            //SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
            //{
            //    Port = 587,
            //    Credentials = new NetworkCredential("yashusoni003@gmail.com", "nkrr wbqn zbaw aezs"),
            //    EnableSsl = true,
            //};

            //try
            //{
            //    smtpClient.Send(mail);
            //    Console.WriteLine("Email sent successfully!");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"Error sending email: {ex.Message}");
            //}





            return smtp;






        }

    }
}
