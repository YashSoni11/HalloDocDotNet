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

       public List<Request> UserRequests { get; set; }

       public  User User { get; set; }

    }
}
