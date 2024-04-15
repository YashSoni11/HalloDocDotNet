using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.ProviderViewModels
{

    public class SearchRecordsForm
    {
        public string requestStatus { get; set; }   

        public string Patientname { get; set; }

        public string RequestType { get; set; }

        public DateTime FromDateOfServive { get; set; }

        public DateTime ToDateOfService { get; set; }   

        public string ProviderName { get; set; }    

        public string Email { get; set; }   

        public string PhoneNumber { get; set; }
    }
    public class SearchRecordsTable
    {
        public int rowsPerPage { get; set; }

        public int TotalPages { get; set; }

        public int currentPage { get; set; }
        public List<SearchRecords> SearchRecords { get; set; }
    }
    public class SearchRecords
    {

        public int RequestId { get; set; }
        public string PatientName { get; set; } 

        public string RequestorType { get; set; }   

        public DateTime? DateOfService { get; set; } 

        public DateTime? CloseCaseDate { get; set; } 

        public string Email { get; set; }

        public string PatientPhone { get; set; }    


        public string Address { get; set; }

        public string Zip { get; set; } 

        public string RequestStatus { get; set; }

        public string Physician { get; set; }   

        public string CanclledByProviderNote { get; set; }  

        public string Adminnote { get; set; }   

        public string PatientNote { get; set; }

        public string PhysicianNote { get; set; }


         
    }
}
