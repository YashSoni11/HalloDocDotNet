using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.AdminViewModels
{
    public class AdminAssignCase
    {

      public   List<Region> Regions { get; set; }

        public List<Physician>  Physicians { get; set; }

        public string SelectedRegionName { get; set; }

        public string SelectedPhycisianId { get; set; }

        public string Description { get; set; }

        public int RequestId { get; set; }

         
    }
}
