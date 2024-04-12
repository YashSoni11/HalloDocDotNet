using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.ProviderViewModels
{
    public class ConcludeCare
    {

        public int RequestId { get; set; }


        public string PatientName { get; set; } 


        public bool IsFinilize { get; set; }

        public IFormFile file { get; set; }

        public string ProviderNotes { get; set; }

    }
}
