using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.AdminViewModels
{
    public class AdminBlockCase
    {
        public int RequestId { get; set; }
        public string PatientName { get; set; }

        public string ReasonForBlocking { get; set; }
    }
}
