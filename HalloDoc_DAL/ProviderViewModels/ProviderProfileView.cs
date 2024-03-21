using HalloDoc_DAL.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
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
       
      
        public IFormFile? ICA { get; set; }
        public IFormFile? BGCheck { get; set; }
        public IFormFile? HIPAACompliance { get; set; }
        public IFormFile? NDA { get; set; }
        public IFormFile? LicenseDoc { get; set; }
    }



    public class ProviderAccountInfo
    {
        public string? UserName { get; set; }

        public string? Password { get; set; }

        public int? Status { get; set; }

        public string? Role { get; set; }

        public List<Role> roles { get; set; }

    }


    public class ProviderInformation
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public List<Region>? Regions { get; set; }
        public string? PhoneNumber { get; set; }
        public string? MedicalLicenseNumber { get; set; }
        public string? NPINumber { get; set; }
        public string? SyncEmail { get; set; }
    }


    public class ProviderMailingAndBillingInfo
    {
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public int? StateId { get; set; }
        public string? StateName { get; set; }
        public string? Zip { get; set; }
        public string?  Phone { get; set; }

        public List<Region> regions { get; set; }
    } 


    public class ProviderProfileInfo
    {
        public string? BusinessName { get; set; }
        public string? BusinessWebsite { get; set; }
        public IFormFile? Photo { get; set; }
        public IFormFile? Signature { get; set; }

        public byte[]? SignatureImage { get; set; }

        public string? AdminNotes { get; set; }
    }


}
