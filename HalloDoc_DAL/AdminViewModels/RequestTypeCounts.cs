using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.AdminViewModels
{
    public  class RequestTypeCounts
    {


        public int Newrequestcount { get; set; }

        public int Unpaidrequestcount { get; set; }


        public int Activerequestcount { get; set; }

        public int Concluderequestcount { get; set; }

        public int Tocloserequestcount { get; set; }

        public int Pendingrequestcount { get; set; }


    }
}
