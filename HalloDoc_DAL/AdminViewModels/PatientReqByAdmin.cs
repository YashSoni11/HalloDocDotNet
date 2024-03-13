using HalloDoc_DAL.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.AdminViewModels
{
    public class PatientReqByAdmin
    {

      
        public string Notes { get; set; }

        [Required(ErrorMessage = "FirstName is Required.")]
        [MaxLength(30, ErrorMessage = "Could Not Enter More Than 30 Charachters.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only Alphabets are Allowed.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Lastname is Required.")]
        [MaxLength(30, ErrorMessage = "Could Not Enter More Than 30 Charachters.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only Alphabets are Allowed.")]
        public string LastName { get; set; }


        [Required(ErrorMessage = "Birthdate is Required.")]
        public DateOnly BirthDate { get; set; }


        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phonenumber is required.")]
        [DataType(DataType.PhoneNumber)]
        [Phone(ErrorMessage = "Please Enter Valid Phonenumber")]
        public string Phonenumber { get; set; }


        public AddressModel Location { get; set; }

        
    }
}
