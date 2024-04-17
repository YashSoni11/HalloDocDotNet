using HalloDoc_DAL.Models;
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
        [Required(ErrorMessage ="Symptoms is Required.")]
        [MaxLength(500,ErrorMessage ="Could Not Enter More Than 500 Charachters.")]
        public string Symptoms { get; set; }

        [Required(ErrorMessage = "FirstName is Required.")]
        [MaxLength(30,ErrorMessage ="Could Not Enter More Than 30 Charachters.")]
        [MinLength(2,ErrorMessage ="Minimum 2 Charachters is Required.")]
        [RegularExpression(@"^[a-zA-Z]+$",ErrorMessage ="Only Alphabets are Allowed.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Lastname is Required.")]
        [MaxLength(30, ErrorMessage = "Could Not Enter More Than 30 Charachters.")]
        [MinLength(2, ErrorMessage = "Minimum 2 Charachters is Required.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only Alphabets are Allowed.")]
        public string LastName { get; set; }


        [Required(ErrorMessage ="Birthdate is Required.")]
        public DateOnly BirthDate { get; set; }


        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage ="Please Enter Valid Email.")]
        public string Email { get; set; }

  
        [Required(ErrorMessage ="Password is required.")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "The password and confirm password do not match.")]
        [Required(ErrorMessage = "Confirmpassword is required.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage ="Phonenumber is required.")]
        [DataType(DataType.PhoneNumber)]
        [Phone(ErrorMessage ="Please Enter Valid Phonenumber")]
        public string Phonenumber { get; set; }

        
        public AddressModel Location { get; set; }

        public IFormFile? FileUpload { get; set; } = null;

        public string? Relation { get; set; } = null;


        public List<Region>? regions { get; set; }

        [Required(ErrorMessage = "Region is Required.")]

        public int RegionId { get; set; }

    }
}
