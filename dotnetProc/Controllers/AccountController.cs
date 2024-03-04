using Microsoft.AspNetCore.Mvc;
using HalloDoc_DAL.ViewModels;
using HalloDoc_DAL.Context;
using HalloDoc_DAL.Models;
using Microsoft.EntityFrameworkCore;
using HalloDoc_BAL.Interface;
using static HalloDoc_BAL.Repositery.PatientRequest;
using Newtonsoft.Json;
using System.Globalization;

namespace dotnetProc.Controllers
{
    public class AccountController : Controller
    {



        private readonly IAccount _account;
        private readonly IEmailService _emailService;
        private readonly IPatientReq _patientReq;

        public AccountController(IAccount account, IEmailService emailService,IPatientReq patientReq)
        {


            _account = account;
            _emailService = emailService;
            _patientReq = patientReq;
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


        public IActionResult Login()
        {
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

                    TempData["Error"] = "Invalid Attributes";

                    return View();

                }
                else
                {

                    User user = _account.GetUserByAspNetId(aspuser.Id);

                    HttpContext.Session.SetInt32("LoginId", user.Userid);

                    HttpContext.Session.SetString("UserName", user.Firstname);

                    return RedirectToAction("DashBoard", "Account");
                }



            //}



            //return View("Login", um);




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


        [HttpGet]
        public IActionResult DashBoard()
        {


            int LoginId = (int)HttpContext.Session.GetInt32("LoginId");

            User user = _account.GetUserByUserId(LoginId);


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




            List<DashBoardRequests> userRequests = _account.GetUserRequests(LoginId);
            List<Region> regions = _account.GetAllRegions();

            UserInformation userinfo = new UserInformation
            {
                UserRequests = userRequests,
                User = userProfile,
                Regions = regions

            };

            TempData["UserName"] = HttpContext.Session.GetString("UserName");

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



    }
}
