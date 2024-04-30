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

        public string GetHashedPassword(string password);

        public  bool ValidatePassword(string password);

        public Aspnetuser ValidateLogin(UserCred um);

        #region Get User,Admin,PhysicianByAspNetId Services
        public User GetUserByAspNetId(string aspnetuserid);

        public Admin GetAdminByAspNetId(string aspnetuserid);

        public Physician GetPhysicianByAspNetId(string aspnetuserid);
        #endregion
        public int StoreResetid(DateTime expirationtime, string email);


        #region Get AspNetUser By ResetId,Email
        public Token GetTokenByTokenId(int tokenId);

        public Aspnetuser GetAspnetuserByEmail(string email);
        #endregion


        public Aspnetuser UpdateAspnetuserPassByEmail(string Email, string Newpassword, int tokenId);
        public List<DashBoardRequests> GetUserRequests(int userid);

        public User GetUserByUserId(int userid);



        public bool UpdateUserByUserId(UserInformation um,int userId);

        public List<ViewDocument> GetDocumentsByRequestId(int requestid);

        public bool UploadFile(IFormFile file, int requestId, string Role, int UserId);

        public Object GiveResetPasswordLinkObject(string resetid);

        public List<Region> GetAllRegions();

        //public int GetAspNetUserRoleById(string aspnetuserid);

        public LoggedInUser GetLoggedInUserFromJwt(string token);

        public bool IsTokenExpired(string token);

        public string GetAspNetRolesByAspNetId(string id);

        public bool IsRequestBelongsToUser(int id);
    }
}
