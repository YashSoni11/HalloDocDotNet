using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.ProviderViewModels
{

    public class PatientHistoryTable
    {
        public int rowsPerPage { get; set; }

        public int TotalPages { get; set; }

        public int currentPage { get; set; }
        public List<PatientHistory> patientHistories { get; set; }
    }
    public class PatientHistory
    {

        public  string FirstName { get; set; }  

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; } 

        public int UserId { get; set; }

    }


    public class PatientExplreView
    {
        public int UserId { get; set; }
    }

    public class PatientExploreTable
    {
        public int rowsPerPage { get; set; }

        public int TotalPages { get; set; }

        public int currentPage { get; set; }
        public List<PatientExplore> patientRecords { get; set; }

        public int UserId { get; set; }

        public int RequestId { get; set; }
    }


    public class PatientExplore
    {
        public string? PatientName { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? Confirmation { get; set; }

        public string? Providername { get; set; }

        public DateTime? ConcludeDate { get; set; }

        public int?   status { get; set; }

        public int RequestId { get; set; }
        public bool? IsFinalized { get; set; }

    }
        
}
