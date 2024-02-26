using HalloDoc_BAL.Interface;
using HalloDoc_DAL.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
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



            return smtp;








        }

    }
}
