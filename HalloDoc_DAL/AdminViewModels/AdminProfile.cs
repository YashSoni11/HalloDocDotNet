using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.AdminViewModels
{
    public class AdminProfile
    {
        public string? AspnetAdminid { get; set; }
        public string? Username { get; set; }

        //[Required(AllowEmptyStrings = true)]
        public string? Password { get; set; }

        public int? Status { get; set; }

        public string? Role { get; set; }

        public List<Role>? roles { get; set; }

        public string? Firstname { get; set; }

        public string? Lastname { get; set; }

        public string? Email { get; set; }   

        public string? Confirmationemail { get; set; }   

        public string? Phone { get; set; }

        public string? Address1 { get; set; }

        public string? Address2 { get; set; }    

        public string? City { get; set; }

        public string? State { get; set; }

        public List<Region>? regions { get; set; }   

        public string? Zip { get; set; }

        public string? AltPhone { get; set; }

    }
}
