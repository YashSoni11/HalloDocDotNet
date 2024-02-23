using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalloDoc_DAL.Models;
using HalloDoc_DAL.ViewModels;
using Microsoft.AspNetCore.Http;

namespace HalloDoc_BAL.Interface
{
    public interface IAccount
    {

       public Aspnetuser ValidateLogin(UserCred um);

         public User GetUserByAspNetId(string aspnetuserid);

        public Aspnetuser GetAspnetuserByEmail(string email);

        public Aspnetuser UpdateAspnetuserPassByEmail(string Email,string Newpassword);
        public List<DashBoardRequests> GetUserRequests(int userid);

        public User GetUserByUserId(int userid);

        public string GetHashedPassword(string password);


        public UserProfile UpdateUserByUserId(UserInformation um,int userId);

        public List<ViewDocument> GetDocumentsByRequestId(int requestid);

        public bool UploadFile(IFormFile file, int requestId);

        //public var GiveResetPasswordLinkObject(string resetid);
    }
}
