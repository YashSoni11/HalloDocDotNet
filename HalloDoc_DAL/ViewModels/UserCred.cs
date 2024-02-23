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


        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        //[Compare("Password", ErrorMessage = "Password Do Not Match!")]
        [DataType(DataType.Password)]
        public string Confirmpassword { get; set; }
    }
}
