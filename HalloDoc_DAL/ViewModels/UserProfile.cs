using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.ViewModels
{
    public  class UserProfile
    {

         public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Email { get; set; }

        public string Phonnumber { get; set; }

        public AddressModel Address { get; set; }

        public DateOnly Birthdate { get; set; }
    }
}
