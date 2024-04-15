using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.ViewModels
{
    public class LoggedInUser
    {

        public int UserId { get; set; } 

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Email { get; set; }

        public string Role { get; set; } 

        public int AspnetRole { get; set; }  

    }
}
