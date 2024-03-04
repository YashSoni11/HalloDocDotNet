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


       public  List<DashboardRequests> Requests { get; set; }

        public List<Region> Regions { get; set; }   

        public string Searchstring { get; set; }


        public RequestTypeCounts RequestTypeCounts { get; set; }

        public Requestclient requestclient { get; set; }

        public RequestNotes requestnotes { get; set; }  

       
    }
}
