using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.AdminViewModels
{
    public class DashboardRequests
    {

        public string Username { get; set; }

        public int Requestid { get; set; }

        public DateTime Birthdate { get; set; }

        public string Requestor { get; set; }

        public DateTime Requestdate { get; set; }

        public string Phone { get; set; }

        public string RequestorPhone { get; set; }


        public string Address { get; set; }


        public string Notes { get; set; }


        public int status { get; set; }

        public int Requesttype { get; set; }

        public string Requestortype { get; set; }

        public string PhysicianName { get; set; }

        public int? RegionId { get; set; }

        public int RequestTypeId { get; set; }

    }
}
