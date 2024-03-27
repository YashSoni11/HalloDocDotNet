using HalloDoc_DAL.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.ProviderViewModels
{
    public class CreateAdminAccountModel
    {


        [Required(ErrorMessage = "Username is Required.")]
        [MaxLength(30, ErrorMessage = "Could Not Enter More Than 30 Charachters.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only Alphabets are Allowed.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Password is Required.")]
        public string? Password { get; set; }

   

        [Required(ErrorMessage = "Role is Required.")]
        public int? Role { get; set; }

        public List<Role> roles { get; set; }

        [Required(ErrorMessage = "FirstName is Required.")]
        [MaxLength(30, ErrorMessage = "Could Not Enter More Than 30 Charachters.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only Alphabets are Allowed.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Lastname is Required.")]
        [MaxLength(30, ErrorMessage = "Could Not Enter More Than 30 Charachters.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only Alphabets are Allowed.")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Email is Required.")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "ConfirmEmail is Required.")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email.")]
        public string? ConfirmEmail { get; set; }

        public List<Region>? Regions { get; set; }

        [Required(ErrorMessage = "Phonenumber is required.")]
        [DataType(DataType.PhoneNumber)]
        [Phone(ErrorMessage = "Please Enter Valid Phonenumber")]
        public string? Phone { get; set; }

     

        [Required(ErrorMessage = "Address is required.")]
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }

        [Required(ErrorMessage = "City is Required.")]
        [MaxLength(30, ErrorMessage = "Could Not Enter More Than 30 Charachters.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only Alphabets are Allowed.")]
        public string? City { get; set; }


        [Required(ErrorMessage = "State is Required.")]
        public int? StateId { get; set; }


        public string? StateName { get; set; }

        [Required(ErrorMessage = "Zipcode is Required.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "ZIP code must be exactly 6 digits.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "ZIP code must be exactly 6 digits.")]
        public string? Zip { get; set; }

        [Required(ErrorMessage = "Phonenumber is required.")]
        [DataType(DataType.PhoneNumber)]
        [Phone(ErrorMessage = "Please Enter Valid Phonenumber")]
        public string? AltPhone { get; set; }


    }
}
