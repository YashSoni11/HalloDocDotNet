using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.AdminViewModels
{
    public class RequestNotes
    {

         public List<TransferNotes>? TranferNotes { get; set; }

        public string? AdminNotes { get; set; }

        public string? PhysicianNotes { get; set; }

        public string? AddtionalNotes { get; set; }

       


    }

    public class TransferNotes
    {
        public int? PhysicianId { get; set;}

        public string? PhysicinName { get; set;}    

        public string? TrasferPhysicianName { get; set; }

        public int? AdminId { get; set;}

        public string? AdminName { get; set;}   

        public DateTime TransferedDate { get; set;}

        public string? Description { get; set;}

        public bool? IsTransferToAdmin { get; set; }
    }
}
