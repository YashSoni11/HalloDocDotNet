﻿using HalloDoc_BAL.Interface;
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
using AutoMapper;
using HalloDoc_DAL.Context;
using System.Collections;
using Org.BouncyCastle.Asn1.Ocsp;

namespace HalloDoc_BAL.Repositery
{
    public class EmailService : IEmailService
    {

        private readonly HalloDocContext _context;
       

        public EmailService(HalloDocContext context, IMapper mapper)
        {
            _context = context;
        }


        public bool AddEmailLog(string subject,int requestId, string emailTemplet, string UserEmail, string ConfirmationNumber, bool IsEmailSent)
        {

            try
            {

            Emaillog emaillog = new Emaillog();
            emaillog.Subjectname = subject;
            emaillog.Requestid = requestId;
            emaillog.Emailtemplate = emailTemplet;
            emaillog.Isemailsent = new BitArray(new[] { IsEmailSent});
            emaillog.Emailid = UserEmail;
            emaillog.Confirmationnumber = ConfirmationNumber;
                emaillog.Createdate = DateTime.Now;
                emaillog.Sentdate = DateTime.Now;
                emaillog.Action = 1;

            _context.Emaillogs.Add(emaillog);
            _context.SaveChanges();

            return true;
            }catch(Exception ex)
            {
                return false;
            }


        }

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
            message.From = new MailAddress("tatva.dotnet.yashsoni@outlook.com");
            message.Subject = subject;
            message.To.Add(new MailAddress(uemail));
            message.Body = body;
            message.IsBodyHtml = true;


            try
            {

                using (var smtpClient = new SmtpClient("smtp.office365.com"))
                {
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new NetworkCredential("tatva.dotnet.yashsoni@outlook.com", "hursiqxmvunkqxnk");
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
