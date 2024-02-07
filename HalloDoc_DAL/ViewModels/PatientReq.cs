using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 

namespace HalloDoc_DAL.ViewModels
{
    public  class PatientReq
    {

        public string Symptoms { get; set; }

        public string FirstName { get; set; }


        public string LastName { get; set; }


        public DateOnly BirthDate { get; set; }

        public string Email { get; set; }


        public string Phonenumber { get; set; }

        public AddressModel Location { get; set; }


    }
}
