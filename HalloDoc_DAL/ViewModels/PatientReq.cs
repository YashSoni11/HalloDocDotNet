﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace HalloDoc_DAL.ViewModels
{
    public  class PatientReq
    {

        public string Symptoms { get; set; }

        public string FirstName { get; set; }


        public string LastName { get; set; }


        public DateOnly BirthDate { get; set; }


        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage ="Email is Required!")]
        public string Email { get; set; }


        public string Phonenumber { get; set; }

        public AddressModel Location { get; set; }


    }
}
