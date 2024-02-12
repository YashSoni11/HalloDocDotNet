using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.ViewModels
{
    public class BusinessReqModel
    {

        public CmnInformation BusinessInfo { get; set; }

        public string PropertyName { get; set; }

        public string CaseNumber { get; set; }

        public PatientReq PatientIfo { get; set; }  

        public AddressModel PatinentLocaiton { get; set; }



    }
}
