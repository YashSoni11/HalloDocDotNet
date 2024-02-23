using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace HalloDoc_DAL.ViewModels
{
    public  class PatientReq
    {
        [Required(ErrorMessage ="nakh ne bhai")]
        public string Symptoms { get; set; }

        [Required(ErrorMessage = "nakh ne bhai")]
        public string FirstName { get; set; }


        public string LastName { get; set; }


        public DateOnly BirthDate { get; set; }


        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage ="Email is Required!")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public string Phonenumber { get; set; }

        public AddressModel Location { get; set; }

        public IFormFile FileUpload { get; set; }

        public string Relation { get; set; }
    }
}
