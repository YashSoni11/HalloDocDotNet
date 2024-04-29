using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.AdminViewModels
{
    public class AdminProfile
    {
      

       public AccountAndAdministratorInfo? accountInfo { get; set; }

        public MailingAndBillingInfo? mailingAndBillingInfo { get; set;}

        public int adminId { get; set; }
    }


    public class AccountAndAdministratorInfo
    {
        public string? AspnetAdminid { get; set; }

        [Required(ErrorMessage = "Username is Required.")]
        [MaxLength(30, ErrorMessage = "Could Not Enter More Than 30 Charachters.")]
        public string? Username { get; set; }

        //[Required(AllowEmptyStrings = true)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Status is Required.")]
        public int? Status { get; set; }

        [Required(ErrorMessage = "Role is Required.")]
        public int SelectedRoleId { get; set; }

        public string? RoleName { get; set; }

        public List<Role>? roles { get; set; }

        [Required(ErrorMessage = "FirstName is Required.")]
        [MaxLength(30, ErrorMessage = "Could Not Enter More Than 30 Charachters.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only Alphabets are Allowed.")]
        public string? Firstname { get; set; }

        [Required(ErrorMessage = "Lastname is Required.")]
        [MaxLength(30, ErrorMessage = "Could Not Enter More Than 30 Charachters.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only Alphabets are Allowed.")]
        public string? Lastname { get; set; }


        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email.")]
        public string? Email { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email.")]
        public string? Confirmationemail { get; set; }

        [Required(ErrorMessage = "Phonenumber is required.")]
        [DataType(DataType.PhoneNumber)]
        [Phone(ErrorMessage = "Please Enter Valid Phonenumber")]
        public string? Phone { get; set; }


        public List<SelectedRegions>? SelectedRegions { get; set; }    
    }

    public class MailingAndBillingInfo
    {
        [Required(ErrorMessage = "Address is required.")]
        public string? Address1 { get; set; }

        public string? Address2 { get; set; }

        [Required(ErrorMessage = "City is Required.")]
        [MaxLength(30, ErrorMessage = "Could Not Enter More Than 30 Charachters.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only Alphabets are Allowed.")]
        public string? City { get; set; }

        [Required(ErrorMessage = "State is Required.")]
        [MaxLength(30, ErrorMessage = "Could Not Enter More Than 30 Charachters.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only Alphabets are Allowed.")]
        public string? State { get; set; }

        public List<Region>? regions { get; set; }


        public string? Zip { get; set; }

        public string? AltPhone { get; set; }
    }

    public class SelectedRegions
    {
        public int? regionId { get; set; }

        public string? regionName { get; set; }

        public bool IsSelected { get; set; }
    }
}
