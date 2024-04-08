using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.ProviderViewModels
{

    public class Vendors
    {
        public List<Healthprofessionaltype> healthprofessionaltypes { get; set; }
    }
    public class VendorTable
    {

        public int rowsPerPage { get; set; }

        public int TotalPages { get; set; }

        public int currentPage { get; set; }
        public List<VendorList> VendorList {get; set;}
    } 

    public class VendorList
    {
        public int VendorId { get; set; }
        public string Profession { get; set; }  

        public  string BusinessName { get; set; }

        public string Email { get; set; }

        public string FaxNumber { get; set; }

        public string PhoneNumber { get; set; } 

        public string BusinessContact { get; set; }


    }
    public class VendorDetails
    {
        public int? VendorId { get; set; }

        public List<Healthprofessionaltype>? healthprofessionaltypes { get; set; }

        public List<Region>? regions { get; set; }


        public string? Profession { get; set; }

        public int ProfessionId { get; set; }

        public string BusinessName { get; set; }

        public string Email { get; set; }

        public string FaxNumber { get; set; }

        public string PhoneNumber { get; set; }

        public string BusinessContact { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public int regionId { get; set; }

        public string? regionName { get; set; }

        public string ZipCode { get; set; }
    }
}
