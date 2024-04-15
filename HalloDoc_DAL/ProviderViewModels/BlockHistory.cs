using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.ProviderViewModels
{
    public class BlockHistory
    {


    }



    public class BlockHistoryTable
    {
        public int rowsPerPage { get; set; }

        public int TotalPages { get; set; }

        public int currentPage { get; set; }
        public List<BlockHistories> BlockRecords { get; set; }

       
    }


    public class BlockHistories
    {


        public string PatientName { get; set; }

         public string PhoneNumber { get; set; }


        public string Email { get; set; }

        public DateTime CratedAr { get; set; }

        public string BlockNotes { get; set; }

        public bool IsActive { get; set; }


    }

}
