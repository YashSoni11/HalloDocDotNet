using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.AdminViewModels
{
    public class SendLink
    {

        public string requestId { get; set; }

        [Required(ErrorMessage = "FirstName is Required.")]
        [MaxLength(30, ErrorMessage = "Could Not Enter More Than 30 Charachters.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only Alphabets are Allowed.")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Lastname is Required.")]
        [MaxLength(30, ErrorMessage = "Could Not Enter More Than 30 Charachters.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only Alphabets are Allowed.")]
        public string Lastname { get; set; }

        //[Required(ErrorMessage = "Phonenumber is required.")]
        //[DataType(DataType.PhoneNumber)]
        //[Phone(ErrorMessage = "Please Enter Valid Phonenumber")]
        //public string Phonenumber { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email.")]
        public string Email { get; set; }
    }
}
