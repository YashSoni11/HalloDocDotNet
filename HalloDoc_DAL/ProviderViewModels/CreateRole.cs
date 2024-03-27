using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.ProviderViewModels
{
    public class CreateRole
    {
        [Required(ErrorMessage ="Rolename is required")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only Alphabets are Allowed.")]
        public string RoleName { get; set; }

        [Required(ErrorMessage = "Accounttype is required")]
        public int AccountType { get; set; }

       public List<AccessAreas> AccessAreas { get; set; }

    public int? Roleid { get; set; }

    }

    public class AccessAreas
    {
        public string? AreaName { get; set; }

        public int AreaId { get; set; }

        public bool IsAreaSelected { get; set; }
    }
}
