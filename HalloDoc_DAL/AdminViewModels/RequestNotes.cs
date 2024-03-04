using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.AdminViewModels
{
    public class RequestNotes
    {

         public string? TranferNotes { get; set; }

        public string? AdminNotes { get; set; }

        public string? PhysicianNotes { get; set; }

        public string? AddtionalNotes { get; set; }

        public DateOnly?  Tranferdate { get; set; }


    }
}
