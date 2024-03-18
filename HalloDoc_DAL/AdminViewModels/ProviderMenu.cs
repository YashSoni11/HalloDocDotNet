using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.AdminViewModels
{
    public class ProviderMenu
    {

         public bool IsNoificationOn { get; set; }

        public string Name { get; set; }    

        public string Role { get; set; }    

        public string OnCallStatus { get; set; }

        public int status { get; set; }

        public int ProviderId { get; set; }
    }

    public class Providers
    {
        public List<ProviderMenu> providers { get; set; }

        public List<Region> regions { get; set; }
    }

    public class ProviderList
    {
        public List<ProviderMenu> providers { get; set; }

        public string rr { get; set; }  
    }
}
