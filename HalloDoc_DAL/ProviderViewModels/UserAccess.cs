using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.ProviderViewModels
{

    public class UserAccessMenu
    {
        public List<Role> roles { get; set; }


    }


    public class UserAccessTable
    {
        public int rowsPerPage { get; set; }

        public int TotalPages { get; set; }

        public int currentPage { get; set; }

        public List<UserAccess> userAccesses { get; set; }
    }
    public class UserAccess
    {

         public int UserId { get; set; }
            public int AccountType { get; set; }

            public string AccountName { get; set; }

            public string PhoneNumber { get; set; }

            public int status { get; set; }

            public int OpenRequest { get; set; }

        public int roleId { get; set; }


    }
}
