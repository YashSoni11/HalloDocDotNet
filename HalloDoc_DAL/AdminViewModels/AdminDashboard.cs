using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.AdminViewModels
{
    public class AdminDashboard
    {
        public RequestTypeCounts RequestTypeCounts { get; set; }





    }


    public class RequestTable
    {


       public int adminid { get; set; }
        public List<DashboardRequests> Requests { get; set; }

        public List<Region> Regions { get; set; }

        public int regionId { get; set; }

        public string Searchstring { get; set; }

       public int RequestType { get; set; }

        public Requestclient requestclient { get; set; }

        public RequestNotes requestnotes { get; set; }

        public int rowsPerPage { get; set; }

        public int TotalPages { get; set; }

        public int currentPage { get; set; }
    }
  
}
