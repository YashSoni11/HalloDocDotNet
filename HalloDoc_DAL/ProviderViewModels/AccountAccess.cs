using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.ProviderViewModels
{
    public class AccountAccessTable
    {
        public int rowsPerPage { get; set; }

        public int TotalPages { get; set; }

        public int currentPage { get; set; }
        public List<Role> roles { get; set; }
    }
}
