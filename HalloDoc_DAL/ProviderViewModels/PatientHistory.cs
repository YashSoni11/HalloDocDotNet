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
}
