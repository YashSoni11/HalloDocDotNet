using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.AdminViewModels
{
    public class SendAgreement
    {

        public string requestId { get; set; }
          public int Requestor { get; set; }


        [Required(ErrorMessage = "Phonenumber is required.")]
        [DataType(DataType.PhoneNumber)]
        [Phone(ErrorMessage = "Please Enter Valid Phonenumber")]
        public string Phonenumber { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email.")]
        public string Email { get; set; }   
    }
}
