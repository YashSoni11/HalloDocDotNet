using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.ProviderViewModels
{
    public class RequestedShiftModal
    {

         public List<Region> regions { get; set; }

         public List<RequestedShiftDetails> requestedShiftDetails { get; set; }
    }

    public class RequestShiftTable
    {
        public int rowsPerPage { get; set; }

        public int TotalPages { get; set; }

        public int currentPage { get; set; }
        public int regionId { get; set; }
        public List<RequestedShiftDetails> requestedShiftDetails { get; set; }

    }

    public class RequestedShiftDetails
    {
        public bool IsSelected { get; set; }

        public int ShiftDetailId { get; set; }

        public string PhysicianName { get; set; }

        public DateTime Day { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public string Region { get; set; }
    }
}
