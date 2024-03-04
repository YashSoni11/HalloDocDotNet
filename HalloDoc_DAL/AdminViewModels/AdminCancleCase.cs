using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.AdminViewModels
{
    public class AdminCancleCase
    {

         public string Reason { get; set; }

        public string AdditionalNotes { get; set; }

        public  int requestId { get; set; }

        public string PatientName { get; set; }
    }
}
