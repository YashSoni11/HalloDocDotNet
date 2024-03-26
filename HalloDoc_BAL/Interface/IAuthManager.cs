using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.Interface
{
    public interface IAuthManager
    {
        public bool Authorize(HttpContext context,int RoleMenuId);

    }
}
