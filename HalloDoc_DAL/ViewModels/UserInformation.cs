using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.ViewModels
{
    public class UserInformation
    {

       public List<DashBoardRequests> UserRequests { get; set; }

       public  UserProfile User { get; set; }

        public List<Region> Regions { get; set; }

    }
}
