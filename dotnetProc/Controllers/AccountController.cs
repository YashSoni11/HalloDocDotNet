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
using Microsoft.AspNetCore.Http.HttpResults;


namespace dotnetProc.Controllers
{
    public class AccountController : Controller
    {



        private readonly IAccount _account;
        private readonly IEmailService _emailService;
        private readonly IPatientReq _patientReq;
        public readonly IJwtServices _jwtServices;
        public AccountController(IAccount account, IEmailService emailService, IPatientReq patientReq, IJwtServices jwtServices)
        {


            _account = account;
            _emailService = emailService;
            _patientReq = patientReq;
            _jwtServices = jwtServices;
        }

        #region CreateAccountActions
        [HttpGet]
        [Route("Createaccount/{createid}")]
        public IActionResult Createaccount(int createid)
        {

            ViewData["RequestId"] = createid;




            if (_account.IsRequestBelongsToUser(createid))
            {
                TempData["IsValidResetLink"] = "Not a Valid Link.";

                return RedirectToAction("Error", "Account");
            }

            return View();

        }


        [HttpPost]
        [Route("Createaccount/{createid}")]
        public IActionResult Createaccount(UserCred user, int RequestId)
        {

            if (RequestId == 0)
            {
                TempData["ShowNegativeNotification"] = "No Record Found!";
                return RedirectToAction("Createaccount", "Account");
            }


            if (ModelState.IsValid)
            {


                bool IsEmailExists = _patientReq.IsEmailExistance(user.Email);

                if (IsEmailExists)
                {
                    TempData["ShowNegativeNotification"] = "Account is in use!";
                    return RedirectToAction("Createaccount", "Account", new { createid = RequestId });
                }

                if (user.Password == user.Confirmpassword)
                {
                    Requestclient requestclient = _patientReq.GetRequestByIdAndEmail(RequestId, user.Email);

                    if (requestclient == null)
                    {
                        TempData["ShowNegativeNotification"] = "No Record Found!";
                        return RedirectToAction("Createaccount", "Account", new { createid = RequestId });
                    }


                    PatientReq patientReq = new PatientReq
                    {

                        FirstName = requestclient.Firstname,
                        LastName = requestclient.Lastname,
                        Email = requestclient.Email,
                        //Password = user.Password,
                        Phonenumber = requestclient.Phonenumber,

                    };

                    AddressModel addressModel = new AddressModel
                    {
                        State = (int)requestclient.Regionid,
                        Street = requestclient.Street,
                        City = requestclient.City,
                        ZipCode = requestclient.Zipcode
                    };


                    Aspnetuser aspnetuser = _patientReq.AddAspNetUser(patientReq, user.Password);

                    int userid = _patientReq.AddUser(aspnetuser.Id, patientReq, addressModel);

                    Request request = _patientReq.UpdateRequestByRequestId(requestclient.Requestid, userid);





                    TempData["ShowPositiveNotification"] = "Account Created Successfully.";


                    return RedirectToAction("Login", "Account");

                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Password Do not Match!";
                    return RedirectToAction("Createaccount", "Account");
                }
            }
            else
            {
                TempData["ShowNegativeNotification"] = "Not Valid Data!";
                return RedirectToAction("Createaccount", "Account");
            }

        }
        #endregion


        #region LoginLogoutActions
        [HttpGet]
        public IActionResult Login(string message)
        {

            if (!string.IsNullOrEmpty(message))
            {
                TempData["ShowNegativeNotification"] = message;
            }

            string LoginMsg = HttpContext.Request.Cookies["LoginMsg"];

            if (!string.IsNullOrEmpty(LoginMsg) && LoginMsg == "true")
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


            Aspnetuser aspuser = _account.ValidateLogin(um);



            if (aspuser == null)
            {

                TempData["ShowNegativeNotification"] = "Invalid Credentialse!";

                return View();

            }
            else
            {
                string userRole = _account.GetAspNetRolesByAspNetId(aspuser.Id); ;
                //int userRole = _account.GetAspNetUserRoleById(aspuser.Id);

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

                    if (admin != null)
                    {

                        LoggedInUser loggedInUser = new LoggedInUser();

                        loggedInUser.UserId = admin.Adminid;
                        loggedInUser.Firstname = admin.Firstname;
                        loggedInUser.AspnetRole = (int)admin.Roleid;
                        loggedInUser.Role = userRole;


                        var jwtToken = _jwtServices.GenerateJWTAuthetication(loggedInUser);

                        Response.Cookies.Append("jwt", jwtToken);





                        TempData["UserName"] = loggedInUser.Firstname;
                        TempData["ShowPositiveNotification"] = "Logged In Successfully";
                        return RedirectToAction("Dashboard", "Admindashboard");

                    }
                    else
                    {


                        Physician physician = _account.GetPhysicianByAspNetId(aspuser.Id);
                        if (physician != null)
                        {
                            LoggedInUser loggedInUser = new LoggedInUser();

                            loggedInUser.UserId = physician.Physicianid;
                            loggedInUser.Firstname = physician.Firstname;
                            loggedInUser.AspnetRole = (int)physician.Roleid;
                            loggedInUser.Role = userRole;


                            var jwtToken = _jwtServices.GenerateJWTAuthetication(loggedInUser);

                            Response.Cookies.Append("jwt", jwtToken);





                            TempData["UserName"] = loggedInUser.Firstname;
                            TempData["ShowPositiveNotification"] = "Logged In Successfully";
                            return RedirectToAction("Dashboard", "ProviderDashboard");
                        }
                        else
                        {

                            TempData["ShowNegativeNotification"] = "Something went wrong!";

                            return View();
                        }

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
        #endregion


        #region ForgotPassActions
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




            DateTime expiretime = DateTime.Now.AddMinutes(60);

            int tokenId = _account.StoreResetid(expiretime, um.Email);


            string subject = "Password Reset";

            string resetLink = "https://localhost:7008/ResetPassword/" + tokenId;

            string body = "Please click on <a asp-route-id='" + tokenId + "' href='" + resetLink + "'+>ResetPassword</a> to reset your password";


            _emailService.SendEmail(um.Email, subject, body);

            return RedirectToAction("Login", "Account");
        }
        #endregion



        #region ResetPassActions
        [HttpGet]
        [Route("ResetPassword/{resetid}")]
        public IActionResult ResetPassword(int resetid)
        {



            Token ResetToken = _account.GetTokenByTokenId(resetid);




            if (ResetToken == null)
            {

                TempData["IsValidResetLink"] = "Not a Valid Link.";

                return RedirectToAction("Error", "Account");
            }
            else if (DateTime.Now > ResetToken.Tokenexpire)
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

        public IActionResult ResetPassword(ForgotPassword fr, int resetid)
        {


            Token ResetToken = _account.GetTokenByTokenId(resetid);

            if (ResetToken == null)
            {
                TempData["IsValidResetLink"] = "Not a Valid Link!";
                return RedirectToAction("Error", "Account");
            }
            else if (fr.NewPassword != fr.ConfirmPassword)
            {
                TempData["ShowNegativeNotification"] = "Passwords Do Not Match!";

                return View();


            }
            else
            {
                Aspnetuser user = _account.UpdateAspnetuserPassByEmail(ResetToken.Userid, fr.NewPassword, ResetToken.Tokenid);
                TempData["ShowPositiveNotification"] = "Passwords Updated Successfully!";

            }



            return RedirectToAction("Login", "Account");

        }
        #endregion


        #region UserDashboardActions
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
                State = user.Regionid == null ? 0 : (int)user.Regionid,
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
                Regions = regions,
                StateName = user.State
            };





            return View(userinfo);
        }



        [HttpPost]
        public IActionResult PostDashBoard(UserInformation Um)
        {


            var token = Request.Cookies["jwt"];

            LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);


            bool response = _account.UpdateUserByUserId(Um, loggedInUser.UserId);


            if (response)
            {

                TempData["ShowPositiveNotification"] = "User Updated Successfully.";
            }
            else
            {
                TempData["ShowNegativeNotification"] = "Not Valid Operation!";
            }



            return RedirectToAction("Dashboard");

        }

        [HttpGet]
        [Route("/Account/ViewDocuments/{id}")]
        public IActionResult ViewDocuments(int id)
        {
            List<ViewDocument> docs = _account.GetDocumentsByRequestId(id);


            Documents documents = new Documents
            {
                ViewDocuments = docs,
                FormFile = null,
                requestId = id
            };


            return View(documents);
        }


        [HttpPost]
        public IActionResult PostViewDocuments(Documents docs, int id)
        {

            var token = Request.Cookies["jwt"];

            LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

            foreach (IFormFile file in docs.FormFile)
            {

                bool response = _account.UploadFile(file, id, loggedInUser.Role, loggedInUser.UserId);
            }




            return RedirectToAction("ViewDocuments", new { id = id });


        }
        #endregion

        #region ErrorAccessActions
        [HttpGet]
        [Route("Accessdenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        [Route("Account/Error")]
        public IActionResult Error()
        {
            return View();
        }
        #endregion


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

    }
}
