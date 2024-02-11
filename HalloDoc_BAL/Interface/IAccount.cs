using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalloDoc_DAL.ViewModels;

namespace HalloDoc_BAL.Interface
{
    public interface IAccount
    {

       public  Object ValidateLogin(UserCred um);
      
    }
}
