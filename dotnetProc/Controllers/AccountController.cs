using Microsoft.AspNetCore.Mvc;
using HalloDoc_DAL.ViewModels;
using HalloDoc_DAL.Context;
using HalloDoc_DAL.Models;
using Microsoft.EntityFrameworkCore;
using HalloDoc_BAL.Interface;
using static HalloDoc_BAL.Repositery.PatientRequest;

namespace dotnetProc.Controllers
{
    public class AccountController : Controller
    {



        private readonly IAccount _account;
        private readonly IEmailService _emailService;

        public AccountController(IAccount account, IEmailService emailService)
        {


            _account = account;
            _emailService = emailService;
        }


        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Login(UserCred um)
        {


            if (ModelState.IsValid)
            {
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



            }



            return View("Login", um);




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

            HttpContext.Session.SetString("resetid", resetid);
            HttpContext.Session.SetString("resetEmail", um.Email);

            string subject = "Password Reset";

            string resetLink = "https://localhost:7008/ResetPassword/" + resetid;

            string body = "Please click on <a asp-route-id='" + resetid + "' href='" + resetLink + "'+>ResetPassword</a> to reset your password";

            _emailService.SendEmail(um.Email, subject, body);

            return RedirectToAction("Login", "Account");
        }


        [HttpGet]
        [Route("ResetPassword/{resetid}")]
        public IActionResult ResetPassword(string resetid)
        {

            string ri = HttpContext.Session.GetString("resetid");
            

      
                if(ri == resetid)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }


        [HttpPost]
        [Route("ResetPassword/{resetid}")]

        public IActionResult ResetPassword(ForgotPassword fr)
        {

            string email = HttpContext.Session.GetString("resetEmail");

            if (fr.NewPassword != fr.ConfirmPassword)
            {
                TempData["ErrorPassword"] = "Passwords Do Not Match!";

                return View();
            }

            if (email != null)
            {

                Aspnetuser user = _account.UpdateAspnetuserPassByEmail(email, fr.NewPassword);

            }


            return RedirectToAction("Login", "Account");

        }




        public IActionResult GiveResetLink(string email)
        {

            string resetid = Guid.NewGuid().ToString();

            HttpContext.Session.SetString("resetid", resetid);

            string subject = "Password Reset";

            string resetLink = "https://localhost:7008/ResetPassword/" + resetid;

            string body = "Please click on <a asp-route-id='"+resetid+"' href='"+resetLink+"'+>ResetPassword</a> to reset your password";

            _emailService.SendEmail(email, subject, body);

            return RedirectToAction("Login", "Account");

        }


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
                Birthdate = new DateOnly(user.Intyear ?? 0, (int)(Months)Enum.Parse(typeof(Months), user.Strmonth), user.Intdate ?? 0),
                Email = user.Email,
                Phonnumber = user.Mobile,
                Address = address,

            };




            List<DashBoardRequests> userRequests = _account.GetUserRequests(LoginId);

            UserInformation userinfo = new UserInformation
            {
                UserRequests = userRequests,
                User = userProfile

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
                User = newUser

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


            bool response = _account.UploadFile(docs.FormFile, requestId);

            

            return RedirectToAction("ViewDocuments",requestId);

             
        }



    }
}
