using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalloDoc_DAL.Models;
using HalloDoc_DAL.ViewModels;

namespace HalloDoc_BAL.Interface
{
    public interface IAccount
    {

       public Aspnetuser ValidateLogin(UserCred um);

    public User GetUserByAspNetId(string aspnetuserid);


        public List<Request> GetUserRequests(int userid);

        public User GetUserByUserId(int userid);

        public string GetHashedPassword(string password);


        public User UpdateUserByUserId(UserInformation um,int userId);


    }
}
