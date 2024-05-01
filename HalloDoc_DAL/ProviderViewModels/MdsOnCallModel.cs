using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.ProviderViewModels
{
    public class MDsOnCallModel
    {


        public List<Physician> Physicians;

        public int RegionId { get; set; }

        public List<Physician>? OnCall { get; set; }

        public List<Physician>? OffDuty { get; set; }

    }


    public class ProvidersOnCallModel
    {

        public List<Region> regions;

         
    }
}
