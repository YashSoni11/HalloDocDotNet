using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace HalloDoc_DAL.ViewModels
{
    public class FamilyFriendModel
    {

        public CmnInformation FamilyFriendsIfo { get; set; }

        public PatientReq PatientInfo { get; set; }

        [Required(ErrorMessage ="Relation is required.")]
        public string Relation {get; set; } 

        public AddressModel? patientLocation { get; set; }

        public bool ValidEmail { get; set; }

    }
}
