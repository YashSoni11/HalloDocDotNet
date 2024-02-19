﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.ViewModels
{
    public class ForgotPassword
    {

        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match!")]

        public string ConfirmPassword { get; set; }

    }


    public class UserEmail
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

    }
}
