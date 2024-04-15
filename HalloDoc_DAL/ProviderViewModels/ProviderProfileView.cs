using HalloDoc_DAL.AdminViewModels;
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
    public class ProviderProfileView
    {
        public int? PhysicianId { get; set; }
      
       public ProviderAccountInfo? AccountInfo { get; set; }

        public ProviderMailingAndBillingInfo? ProviderMailingAndBillingInfo { get; set; }

        public ProviderProfileInfo? ProviderProfileInfo { get; set; }    

        public ProviderInformation? ProviderInformation { get; set; }
       
        public ProviderDocuments? ProviderDocuments { get; set; }
    }



    public class ProviderAccountInfo
    {
        [Required(ErrorMessage = "Username is Required.")]
        [MaxLength(30, ErrorMessage = "Could Not Enter More Than 30 Charachters.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only Alphabets are Allowed.")]
        public string? UserName { get; set; }

        public string? Password { get; set; }

        [Required(ErrorMessage = "Status is Required.")]
        public int? Status { get; set; }

        [Required(ErrorMessage = "Role is Required.")]
        public string? Role { get; set; }

        public List<Role> roles { get; set; }

    }


    public class ProviderInformation
    {
        [Required(ErrorMessage = "FirstName is Required.")]
        [MaxLength(30, ErrorMessage = "Could Not Enter More Than 30 Charachters.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only Alphabets are Allowed.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Lastname is Required.")]
        [MaxLength(30, ErrorMessage = "Could Not Enter More Than 30 Charachters.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only Alphabets are Allowed.")]
        public string? LastName { get; set; }


        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email.")]
        public string? Email { get; set; }
        public List<Region>? Regions { get; set; }

        [Required(ErrorMessage = "Phonenumber is required.")]
        [DataType(DataType.PhoneNumber)]
        [Phone(ErrorMessage = "Please Enter Valid Phonenumber")]
        public string? PhoneNumber { get; set; }

        public string? MedicalLicenseNumber { get; set; }
        public string? NPINumber { get; set; }
        public string? SyncEmail { get; set; }

        public List<SelectedRegions> SelectedRegions { get; set; }

    }


    public class ProviderMailingAndBillingInfo
    {

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
        public string? Zip { get; set; }
        public string?  Phone { get; set; }

        public List<Region> regions { get; set; }
    } 


    public class ProviderProfileInfo
    {
        [Required(ErrorMessage = "Businessname is Required.")]
        public string? BusinessName { get; set; }

        [Required(ErrorMessage = "Businesswensite is Required.")]
        public string? BusinessWebsite { get; set; }
        public IFormFile? Photo { get; set; }
        public IFormFile? Signature { get; set; }

        public byte[]? SignatureImage { get; set; }

        public string? AdminNotes { get; set; }
    }


    public class ProviderDocuments
    {
        public IFormFile ContractAgrrement { get; set; }
        public bool IsContractAggreeMent { get; set; }

        public IFormFile? BackgroundCheck { get; set; }
        public bool? IsBackGroundCheck { get; set; }



        public IFormFile HIPAA { get; set; }
        public bool IsHipaa { get; set; }

        public IFormFile NonDisclouser { get; set; }
        public bool IsNonDisClouser { get; set; }
    }
 
}
