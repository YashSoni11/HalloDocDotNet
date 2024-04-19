using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.ViewModels
{
    public class DashBoardRequests
    {
        public int requestid { get; set; }

        public DateTime createDate { get; set; }

        public string? Status { get; set; }

        public int totalDocuments { get; set; }

        public string providerName { get; set; }


    }
}
