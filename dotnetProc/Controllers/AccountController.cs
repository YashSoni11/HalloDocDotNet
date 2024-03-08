using Microsoft.AspNetCore.Mvc;
using HalloDoc_DAL.ViewModels;
using HalloDoc_DAL.Context;
using HalloDoc_DAL.Models;
using Microsoft.EntityFrameworkCore;
using HalloDoc_BAL.Interface;
using static HalloDoc_BAL.Repositery.PatientRequest;
using Newtonsoft.Json;
using System.Globalization;
using HalloDoc_BAL.Repositery;
using System.IdentityModel.Tokens.Jwt;


namespace dotnetProc.Controllers
{
    public class AccountController : Controller
    {



        private readonly IAccount _account;
        private readonly IEmailService _emailService;
        private readonly IPatientReq _patientReq;
        public readonly IJwtServices _jwtServices;
        public AccountController(IAccount account, IEmailService emailService,IPatientReq patientReq,IJwtServices jwtServices)
        {


            _account = account;
            _emailService = emailService;
            _patientReq = patientReq;
            _jwtServices = jwtServices;
        }


        [HttpGet]
        [Route("Createaccount/{createid}")]
        public IActionResult Createaccount(string createid)
        {

        
            return View();
            
        }


        [HttpPost]
        [Route("Createaccount/{createid}")]
        public IActionResult Createaccount(UserCred user)
        {





            if (user.Password == user.Confirmpassword)
            {
                Requestclient requestclient = _patientReq.GetRequestClientByEmail(user.Email);



                PatientReq patientReq = new PatientReq
                {

                    FirstName = requestclient.Firstname,
                    LastName = requestclient.Lastname,
                    Email = requestclient.Email,
                    Password = user.Password,
                    Phonenumber = requestclient.Phonenumber,

                };

                AddressModel addressModel = new AddressModel
                {
                    State = requestclient.State,
                    Street = requestclient.Street,
                    City = requestclient.City,
                    ZipCode = requestclient.Zipcode
                };


                Aspnetuser aspnetuser = _patientReq.AddAspNetUser(patientReq, user.Password);

                int userid = _patientReq.AddUser(aspnetuser.Id, patientReq, addressModel);

                Request request = _patientReq.UpdateRequestByRequestId(requestclient.Requestid,userid);

            

            



                return RedirectToAction("Login", "Account");

            }
            else
            {
                TempData["ErrorPassword"] = "Password Do not Match!";
                return RedirectToAction("Createaccount", "Account");
            }

        }

        [HttpGet]

        public IActionResult Login(string message)
        {

            if(!string.IsNullOrEmpty(message))
            {
                TempData["ShowNegativeNotification"] = message;
            }

            string LoginMsg = HttpContext.Request.Cookies["LoginMsg"];

            if(!string.IsNullOrEmpty(LoginMsg) && LoginMsg == "true")
            {
                TempData["ShowNegativeNotification"] = "You need to login!";
                HttpContext.Response.Cookies.Delete("LoginMsg");
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Login(UserCred um)
        {


            //if (ModelState.IsValid)
            //{
                //var user  =  await _context.Aspnetusers.FirstOrDefaultAsync(u=>um.Email == u.Email && um.Password == u.Passwordhash);

                Aspnetuser aspuser = _account.ValidateLogin(um);
               


                if (aspuser == null)
                {

                    TempData["ShowNegativeNotification"] = "Invalid Credentialse!";

                    return View();

                }
                else
                {
                        
                   string userRole = _account.GetAspNetUserRoleById(aspuser.Id);

                    User user = _account.GetUserByAspNetId(aspuser.Id);


                     if (user != null)
                     {


                        LoggedInUser loggedInUser = new LoggedInUser();

                       loggedInUser.UserId = user.Userid;
                       loggedInUser.Firstname = user.Firstname;
                       loggedInUser.Role = userRole;


                         var jwtToken = _jwtServices.GenerateJWTAuthetication(loggedInUser);

                         Response.Cookies.Append("jwt", jwtToken);

                        TempData["UserName"] = loggedInUser.Firstname;
                        TempData["ShowPositiveNotification"] = "Logged In Successfully";
                    return RedirectToAction("DashBoard", "Account");
                     }
                     else
                     {
                         Admin admin = _account.GetAdminByAspNetId(aspuser.Id);

                         if(admin != null)
                         {

                           LoggedInUser loggedInUser = new LoggedInUser();

                           loggedInUser.UserId = admin.Adminid;
                           loggedInUser.Firstname = admin.Firstname;
                           loggedInUser.Role = userRole;


                            var jwtToken = _jwtServices.GenerateJWTAuthetication(loggedInUser);

                             Response.Cookies.Append("jwt", jwtToken);




                         
                           TempData["UserName"] = loggedInUser.Firstname;
                           TempData["ShowPositiveNotification"] = "Logged In Successfully";
                           return RedirectToAction("Dashboard", "Admindashboard");

                         }
                         else
                         {
                          TempData["ShowNegativeNotification"] = "Something went wrong!";

                          return View();

                         }

                     }
                    



                }
               
                

        }


        public IActionResult Logout()
        {


            Response.Cookies.Delete("jwt");
            TempData["ShowPositiveNotification"] = "Logged Out Successfully";

            return RedirectToAction("Login");
        }

        [HttpGet]
        [Route("/Account/ForgotPass")]
        public IActionResult ForgotPass()
        {
            return View();
        }

        [HttpPost]
        [Route("/Account/ForgotPass")]

        public IActionResult ForgotPass(UserEmail um)
        {

            string resetid = Guid.NewGuid().ToString();

            string hashedresetid = _account.GetHashedPassword(resetid);

            DateTime expiretime = DateTime.Now.AddMinutes(60);

             _account.StoreResetid(hashedresetid, expiretime,um.Email);


            string subject = "Password Reset";

            string resetLink = "https://localhost:7008/ResetPassword/" + resetid;

            string body = "Please click on <a asp-route-id='" + resetid + "' href='" + resetLink + "'+>ResetPassword</a> to reset your password";


            _emailService.SendEmail(um.Email, subject, body);

            return RedirectToAction("Login", "Account");
        }


        [HttpGet]
        [Route("Account/Error")]
        public IActionResult Error()
        {
            return View();
        }

        [HttpGet]
        [Route("ResetPassword/{resetid}")]
        public IActionResult ResetPassword(string resetid)
        {

            string hashedresetid = _account.GetHashedPassword(resetid);

          Aspnetuser aspnetuser = _account.GetAspnetuserByResetId(hashedresetid);




            if (aspnetuser == null)
            {

                TempData["IsValidResetLink"] = "Not a Valid Link.";

                return RedirectToAction("Error", "Account");
            }
            else if (DateTime.Now > aspnetuser.Toekenexpire)
            {
                TempData["IsValidResetLink"] = "Your Link is expired.";

                return RedirectToAction("Error", "Account");
            }
            else
            {
                return View();
            }



        }


        [HttpPost]
        [Route("ResetPassword/{resetid}")]

        public IActionResult ResetPassword(ForgotPassword fr,string resetid)
        {
             string hashedresetid = _account.GetHashedPassword(resetid);

            Aspnetuser aspnetuser = _account.GetAspnetuserByResetId(hashedresetid);

            if (aspnetuser == null)
            {
                TempData["IsValidResetLink"] = "Not a Valid Link!";
                return RedirectToAction("Error", "Account");
            }
            else if(fr.NewPassword != fr.ConfirmPassword)
            {
                TempData["ErrorPassword"] = "Passwords Do Not Match!";

                return View();


            }
            else
            {
                   Aspnetuser user = _account.UpdateAspnetuserPassByEmail(aspnetuser.Email, fr.NewPassword);

            }



            return RedirectToAction("Login", "Account");

        }




        //public IActionResult GiveResetLink(string email)
        //{

        //    string resetid = Guid.NewGuid().ToString();

        //    HttpContext.Session.SetString("resetid", resetid);

        //    string subject = "Password Reset";

        //    string resetLink = "https://localhost:7008/ResetPassword/" + resetid;

        //    string body = "Please click on <a asp-route-id='"+resetid+"' href='"+resetLink+"'+>ResetPassword</a> to reset your password";

        //    _emailService.SendEmail(email, subject, body);

        //    return RedirectToAction("Login", "Account");

        //}

        [AuthManager("Patient")]
        [HttpGet]
        public IActionResult DashBoard(string message)
        {

            if (!string.IsNullOrEmpty(message))
            {
                TempData["ShowNegativeNotification"] = message;
            }


            var token = Request.Cookies["jwt"];

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            int userId = int.Parse(jwtToken.Claims.FirstOrDefault(claim => claim.Type == "UserId").Value);
            string Firstname = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "Firstname").Value;

            User user = _account.GetUserByUserId(userId);


            AddressModel address = new AddressModel
            {
                State = user.State,
                City = user.City,
                Street = user.Street,
                ZipCode = user.Zipcode
            };

            UserProfile userProfile = new UserProfile
            {
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Birthdate = new DateTime((user.Intyear ?? 0) == 0 ? 1 : (int)(user.Intyear ?? 0), DateTime.ParseExact(user.Strmonth, "MMMM", CultureInfo.InvariantCulture).Month, (user.Intdate ?? 0) == 0 ? 1 : (int)(user.Intdate ?? 0)),
                Email = user.Email,
                Phonnumber = user.Mobile,
                Address = address,

            };




            List<DashBoardRequests> userRequests = _account.GetUserRequests(userId);
            List<Region> regions = _account.GetAllRegions();

            UserInformation userinfo = new UserInformation
            {
                UserRequests = userRequests,
                User = userProfile,
                Regions = regions

            };

          



            return View(userinfo);
        }



        [HttpPost]
        public IActionResult DashBoard(UserInformation Um)
        {


            int LoginId = (int)HttpContext.Session.GetInt32("LoginId");


            UserProfile newUser = _account.UpdateUserByUserId(Um, LoginId);



            List<DashBoardRequests> userRequests = _account.GetUserRequests(LoginId);

          

            UserInformation userinfo = new UserInformation
            {
                UserRequests = userRequests,
                User = newUser,
              

            };





            return View(userinfo);

        }

        [HttpGet]
        [Route("/Account/ViewDocuments/{id}")]
        public IActionResult ViewDocuments(int id)
        {
            List<ViewDocument> docs = _account.GetDocumentsByRequestId(id);


            Documents documents = new Documents
            {
                ViewDocuments = docs,
                FormFile = null
            };


            return View(documents);
        }


        [HttpPost]
        public IActionResult ViewDocuments(Documents docs)
        {

            string path = HttpContext.Request.Path;

            string[] paths = path.Split('/');

            int requestId = int.Parse(paths[paths.Length - 1]);


            //bool response = _account.UploadFile(docs.FormFile, requestId);

            

            return RedirectToAction("ViewDocuments",requestId);

             
        }


        [HttpGet]
        [Route("Accessdenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }


    }
}
