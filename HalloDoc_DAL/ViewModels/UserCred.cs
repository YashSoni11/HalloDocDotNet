using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.ViewModels
{
    public  class UserCred
    {

        [EmailAddress(ErrorMessage = "Please Enter Valid Email.")]
        [DataType(DataType.EmailAddress)]

        public string Email { get; set; }

        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).{8,}$", ErrorMessage = "Not Valid Password!")]

        public string Password { get; set; }

       
    }



    public class CreateAccountModel
    {
        [EmailAddress(ErrorMessage = "Please Enter Valid Email.")]
        [DataType(DataType.EmailAddress)]

        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).{8,}$", ErrorMessage = "Not Valid Password!")]

        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Password Do Not Match!")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Confirmpassword is required.")]

        public string Confirmpassword { get; set; }
    }
}
