using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.ViewModels
{
    public class ConcieargeModel
    {

        public CmnInformation concieargeInformation { get; set; }

        public PatientReq PatinentInfo { get; set; }

        public AddressModel concieargeLocation { get; set; }

        public string propertyName { get; set; }

        public int patientRoomNo { get; set; }

    }
}
