using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.ProviderViewModels
{



    public class EmailLogsTable
    {

        public int rowsPerPage { get; set; }

        public int TotalPages { get; set; }

        public int currentPage { get; set; }

        public List<EmailLogs> EmailLogs { get; set; }
    }

    public class EmailLogs
    {

        public string Recipient { get; set; }   

        public int Action { get; set; }  

        public string RoleName { get; set; }    

        public string Email { get; set; }   

        public DateTime CreateDate { get; set; }    

        public DateTime SentDate { get; set; }  

        public bool Sent { get; set; }    

        public string ConfirmationNumber { get; set; }  



    }
}
