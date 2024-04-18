using HalloDoc_BAL.Interface;
using HalloDoc_DAL.Context;
using HalloDoc_DAL.Models;
using HalloDoc_DAL.ViewModels;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace HalloDoc_BAL.Repositery
{
    public class Account : IAccount
    {
        private readonly HalloDocContext _context;

      public  Account(HalloDocContext context)
        {
             _context = context;
        }

        public enum Months
        {
            January = 1,
            February,
            March,
            April,
            May,
            June,
            July,
            August,
            September,
            October,
            November,
            December
        }


        public enum Status
        {
            Unassigned = 1,
            Unpaid,
            MDEnRoute,
            MDOnSite,
            Conclude,
            Cancelled,
            CancelledByPatient,
            Closed,
            Accepted,
            Clear,
        }

        public Object GiveResetPasswordLinkObject(string resetid)
        {


            string subject = "Password Reset";

            string resetLink = "https://localhost:7008/ResetPassword/" + resetid;

            string body = "Please click on <a asp-route-id='" + resetid + "' href='" + resetLink + "'+>ResetPassword</a> to reset your password";

            var resetLinkObj = new
            {
                subject,
                resetLink,
                body
            };

            return resetLinkObj;


        }
        public string GetAspNetRolesByAspNetId(string id)
        {
            return _context.Aspnetuserroles.Where(q=>q.Userid == id).Select(q=>q.Roleid).FirstOrDefault();
        }


        public string GetHashedPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return "";
            }

            using (SHA256 hash = SHA256.Create())
            {
                byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2")); // Convert byte to hexadecimal string
                }

                return builder.ToString();
            }

        }

        public int StoreResetid( DateTime expirationtime, string email)
        {
            try
            {

            string AspNetId = _context.Aspnetusers.Where(q => q.Email == email).Select(q=>q.Id).FirstOrDefault();
                int tokenId = _context.Tokens.OrderByDescending(q => q.Createdate).Select(q=>q.Tokenid).FirstOrDefault();

            if (!string.IsNullOrEmpty(AspNetId))
            {
                Token token = new Token();
                token.Tokenid = tokenId+1;
                token.UserId = AspNetId;
                token.Tokenexpire = expirationtime;
                token.Createdate = DateTime.Now;
                token.Isused = false;

            
            _context.Tokens.Add(token);
            _context.SaveChanges();


                return token.Tokenid;
            }

                return 0;
            }catch(Exception ex)
            {
                return 0;
            }

        }

        //public int GetAspNetUserRoleById(string aspnetuserid)
        //{
             
        //      int role = _context.Aspnetuserroles.Where(q=>q.Userid == aspnetuserid).Select(r=>r.Roleid).FirstOrDefault();

        //      return role;
        //}


        public LoggedInUser GetLoggedInUserFromJwt(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return new LoggedInUser();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            int userId = int.Parse(jwtToken.Claims.FirstOrDefault(claim => claim.Type == "UserId").Value);
            string Firstname = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "Firstname").Value;
            string Role = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "Role").Value;
            int  AspNetRole = int.Parse(jwtToken.Claims.FirstOrDefault(claim => claim.Type == "RoleId").Value);

            LoggedInUser loggedInUser = new LoggedInUser()
            {
                UserId = userId,
                Firstname = Firstname,
                Role = Role,
                AspnetRole = AspNetRole,
            };

            return loggedInUser;
        }

        public bool IsTokenExpired(string token)
        {

            

            if (string.IsNullOrEmpty(token))
            {
           
                return true;
            }

           
            var tokenHandler = new JwtSecurityTokenHandler();
            var decodedToken = tokenHandler.ReadJwtToken(token);

            
            if (!decodedToken.Payload.TryGetValue("exp", out object expirationObj))
            {
                return true;
            }

           
            long expirationTimeUnix = Convert.ToInt64(expirationObj);
            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expirationTimeUnix).UtcDateTime;

         
            if (expirationTime <= DateTime.UtcNow)
            {
                return true;
            }

            return false;
        }


        public Token GetTokenByTokenId(int tokenId)
        {
            Token token = _context.Tokens.Where(q => q.Tokenid == tokenId && q.Isused == false).FirstOrDefault();

            return token;
        }

        public Aspnetuser ValidateLogin(UserCred um)
        {

            string pass = GetHashedPassword(um.Password);

            Aspnetuser user = _context.Aspnetusers.FirstOrDefault(u => um.Email == u.Email && pass == u.Passwordhash);

            return user;
        }

        public Aspnetuser GetAspnetuserByEmail(string email)
        {

            Aspnetuser aspnetuser = _context.Aspnetusers.FirstOrDefault(u => u.Email == email);

            return aspnetuser;

        }
        public User GetUserByAspNetId(string id)
        {
             
            User user  = _context.Users.FirstOrDefault(u=>u.Aspnetuserid == id);

            return user;

        }

        public Admin GetAdminByAspNetId(string aspnetuserid)
        {
            Admin user = _context.Admins.FirstOrDefault(u => u.Aspnetuserid == aspnetuserid);

            return user;
        }

        public Physician GetPhysicianByAspNetId(string aspnetuserid)
        {
            return _context.Physicians.FirstOrDefault(q => q.Aspnetuserid == aspnetuserid);
        }


        public List<Region> GetAllRegions()
        {
            List<Region> regions = _context.Regions.ToList();

            return regions;
        }

        public User GetUserByUserId(int  userId)
        {
            User user = _context.Users.FirstOrDefault(u => u.Userid == userId);
 

      
            return user;
        }

        public Aspnetuser UpdateAspnetuserPassByEmail(string aspnetId, string Newpassword,int tokenId)
        {
            Aspnetuser aspnetuser = _context.Aspnetusers.FirstOrDefault(u => u.Id == aspnetId);
            Token token = _context.Tokens.FirstOrDefault(q=>q.Tokenid == tokenId);

            if (aspnetuser != null)
            {
                aspnetuser.Passwordhash = GetHashedPassword(Newpassword);
                aspnetuser.Modifieddate = DateTime.Now;
                token.Isused = true;

                _context.Aspnetusers.Update(aspnetuser);
                _context.SaveChanges();

            }

            return aspnetuser;
        }

        public List<DashBoardRequests> GetUserRequests(int userid)
        {

            List<DashBoardRequests> requests = _context.Requests.Where(u => u.Userid == userid).Select(r => new DashBoardRequests
            {
                requestid = r.Requestid,
                createDate = r.Createddate,
                //Status = Enum.GetName(typeof(Status),r.Status),
                Status = "UnAssigned",
                totalDocuments = _context.Requestwisefiles.Where(u => u.Requestid == r.Requestid).Count(),
                providerName = _context.Physicians.FirstOrDefault(u => u.Physicianid == r.Physicianid).Firstname,
            }).ToList();




            //var data = (from r in _context.Requests
            //            join rc in _context.Requestwisefiles on r.Requestid equals rc.Requestid
            //            join p in _context.Physicians on r.Physicianid equals p.Physicianid into prJoin
            //            from p in prJoin.DefaultIfEmpty()
            //            where r.Isdeleted[0] == false && r.Userid == userid
            //            select new
            //            {
            //                Request = r,
            //                Requestwisefiles = rc,
            //                Physicians = p,     
            //            }).ToList();



            //List<DashBoardRequests> requests1 = data.Select(q => new DashBoardRequests {
            //    requestid = q.Request.Requestid,
            //    createDate = q.Request.Createddate,
            //    //Status = Enum.GetName(typeof(Status),r.Status),
            //    Status = "UnAssigned",
            //    totalDocuments = q.Requestwisefiles.Where(u => u.Requestid == r.Requestid).Count(),
            //    providerName = _context.Physicians.FirstOrDefault(u => u.Physicianid == r.Physicianid).Firstname,

            //});


            return requests;
        }


        public  UserProfile UpdateUserByUserId(UserInformation um,int UserId)
        {

        

            User curUser = _context.Users.FirstOrDefault(u => u.Userid == UserId);

            if(curUser != null)
            {


                curUser.Firstname = um.User.Firstname;
                curUser.Lastname = um.User.Lastname;
                curUser.Email = um.User.Email;
                curUser.Mobile = um.User.Phonnumber;
                curUser.Intdate = um.User.Birthdate.Day;
                curUser.Intyear = um.User.Birthdate.Year;
                curUser.Strmonth = ((Months)um.User.Birthdate.Month).ToString();
                curUser.Street = um.User.Address.Street;
                curUser.City = um.User.Address.City;
                curUser.State =   _context.Regions.Where(q=>q.Regionid ==  um.User.Address.State).Select(q=>q.Name).FirstOrDefault();
                curUser.Zipcode = um.User.Address.ZipCode;

                 _context.Users.Update(curUser);
                _context.SaveChanges();

                AddressModel newAddress = new AddressModel
                {
                    State = um.User.Address.State,
                    City = um.User.Address.City,
                    Street = um.User.Address.Street,
                    ZipCode = um.User.Address.ZipCode,
                };

                UserProfile updatedUser = new UserProfile
                {
                    Firstname = um.User.Firstname,
                    Lastname = um.User.Lastname,
                    Email = um.User.Email,
                    Birthdate = um.User.Birthdate,
                    Phonnumber = um.User.Phonnumber,
                    Address = newAddress,

                };
            return updatedUser;

            }
            return new UserProfile();
        }


        public List<ViewDocument> GetDocumentsByRequestId(int requestId)
        {

            List<ViewDocument> documents = _context.Requestwisefiles.Where(u => u.Requestid == requestId && u.Isdeleted != true).Select(r => new ViewDocument
            {
                filename = r.Filename,
                uploader = _context.Requests.FirstOrDefault(u => u.Requestid == requestId).Firstname,
                uploadDate = r.Createddate
            }).ToList();

             return documents;
        }

        public bool UploadFile(IFormFile file, int requestid,string Role,int UserId)
        {

            string path = "";
            bool iscopied = false;

            Requestwisefile requestwisefile = new Requestwisefile
            {
                Requestid = requestid,
                Filename = file.FileName,
                Doctype = 1,
                Createddate = DateTime.Now,
                Isdeleted = false,

            };

            if(Role == "Admin")
            {
                requestwisefile.Adminid = UserId;
            }else if(Role == "Physician")
            {
                requestwisefile.Physicianid = UserId;
            }


            try
            {
                if (file.Length > 0)
                {
                    string filename = file.FileName;
                    path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory())) + "\\wwwroot\\Upload";
                    using (var filestream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }
                    _context.Requestwisefiles.Add(requestwisefile);
                    _context.SaveChanges();
                    iscopied = true;
                }
                else
                {
                    iscopied = false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return iscopied;



        }

    }
}
