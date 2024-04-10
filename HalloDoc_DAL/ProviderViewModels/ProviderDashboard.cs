using HalloDoc_DAL.AdminViewModels;
using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.ProviderViewModels
{
    public class ProviderDashboard
    {
        public RequestTypeCounts RequestTypeCounts { get; set; }
    }


    public class ProvideRequestTable
    {
        public List<DashboardRequests> Requests { get; set; }

        public string Searchstring { get; set; }

        public int RequestType { get; set; }

        public int rowsPerPage { get; set; }

        public int TotalPages { get; set; }

        public int currentPage { get; set; }
    }
}
