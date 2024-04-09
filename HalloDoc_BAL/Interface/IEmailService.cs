using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.Interface
{
    public interface IEmailService
    {

        public bool AddEmailLog(string subject, int requestId, string emailTemplet, string UserEmail, string ConfirmationNumber, bool IsEmailSent);


        public bool SendEmail(string email,string subject,string body);

    }
}
