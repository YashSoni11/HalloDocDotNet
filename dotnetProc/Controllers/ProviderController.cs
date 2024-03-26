using DocumentFormat.OpenXml.Office2010.Excel;
using HalloDoc_BAL.Interface;
using HalloDoc_DAL.Models;
using HalloDoc_DAL.ProviderViewModels;
using HalloDoc_DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;


namespace dotnetProc.Controllers
{
    public class ProviderController : Controller
    {


        private readonly IAdmindashboard _dashboard;
        private readonly IAccount _account;
        private readonly IEmailService _emailService;
        private readonly IPatientReq _patientReq;
        private readonly IProvider _provider;
        private readonly IAuthManager _authManager;

        public ProviderController(IAdmindashboard dashboard, IAccount account,IAuthManager authManager, IEmailService emailService, IPatientReq patientReq,IProvider provider)
        {
            _dashboard = dashboard;
            _account = account;
            _emailService = emailService;
            _patientReq = patientReq;
            _provider = provider;
            _authManager = authManager;
        }

        [HttpGet]
        [Route("providerprofile/{id}")]
        public IActionResult ProviderProfile(int id)
        {
            ProviderProfileView providerProfileView = _provider.GetProviderData(id);

            providerProfileView.PhysicianId = id;

            return View(providerProfileView);
        }


        [HttpPost]

        public IActionResult SaveProviderAccountInfo(ProviderProfileView providerProfileView,int id)
        {
            string token = HttpContext.Request.Cookies["jwt"];


            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired || token.IsNullOrEmpty())
            {
                TempData["ShowNegativeNotification"] = "Session timed out!";
                return RedirectToAction("Login", "Account");
            }
            else
            {
                bool response = _provider.SaveProviderAccountInfo(providerProfileView.AccountInfo, id);

                if (response)
                {
                    TempData["ShowPositiveNotification"] = "Account Info Saved Successfully.";

                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Data Not Saved!";

                }

                return RedirectToAction("ProviderProfile",new { id = id});
            }

        }

        public IActionResult ResetProviderPassword(string Password, int id)
        {
            string token = HttpContext.Request.Cookies["jwt"];


            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired || token.IsNullOrEmpty())
            {
                TempData["ShowNegativeNotification"] = "Session timed out!";
                return RedirectToAction("Login", "Account");
            }
            else
            {
                string hashedPassword = _account.GetHashedPassword(Password);
                bool response = _provider.ResetProviderAccountPassword(hashedPassword, id);

                if (response)
                {
                    TempData["ShowPositiveNotification"] = "Password Changed  Successfully.";

                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Something Went Wrong";

                }

                return RedirectToAction("ProviderProfile", new { id = id });
            }
        }


        public IActionResult SaveProviderInformation(ProviderProfileView providerProfileView ,int id)
        {
            string token = HttpContext.Request.Cookies["jwt"];


            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired || token.IsNullOrEmpty())
            {
                TempData["ShowNegativeNotification"] = "Session timed out!";
                return RedirectToAction("Login", "Account");
            }
            else
            {
                bool response = _provider.SaveProviderInformation(providerProfileView.ProviderInformation, id);

                if (response)
                {
                    TempData["ShowPositiveNotification"] = "Physician Information Saved Successfully.";

                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Data Not Saved!";

                }

                return RedirectToAction("ProviderProfile", new { id = id });
            }
        }

        public IActionResult SaveProviderMailingAndBillingInfo(ProviderProfileView providerProfileView ,int id)
        {
            string token = HttpContext.Request.Cookies["jwt"];


            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired || token.IsNullOrEmpty())
            {
                TempData["ShowNegativeNotification"] = "Session timed out!";
                return RedirectToAction("Login", "Account");
            }
            else
            {
                bool response = _provider.SaveProviderMailingAndBillingInfo(providerProfileView.ProviderMailingAndBillingInfo, id);

                if (response)
                {
                    TempData["ShowPositiveNotification"] = "Mailing&Billing Information Saved Successfully.";

                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Data Not Saved!";

                }

                return RedirectToAction("ProviderProfile", new { id = id });
            }
        }

        public IActionResult SaveProviderProfileInfo(ProviderProfileView providerProfileView ,int id)
        {
            string token = HttpContext.Request.Cookies["jwt"];


            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired || token.IsNullOrEmpty())
            {
                TempData["ShowNegativeNotification"] = "Session timed out!";
                return RedirectToAction("Login", "Account");
            }
            else
            {
                bool response = _provider.SaveProviderProfileInfo(providerProfileView.ProviderProfileInfo, id);

                if (response)
                {
                    TempData["ShowPositiveNotification"] = "Profile Information Saved Successfully.";

                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Data Not Saved!";

                }

                return RedirectToAction("ProviderProfile", new { id = id });
            }
        }

        public IActionResult DeleteProviderAccount(int id) 
        {
            string token = HttpContext.Request.Cookies["jwt"];


            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired || token.IsNullOrEmpty())
            {
                TempData["ShowNegativeNotification"] = "Session timed out!";
                return RedirectToAction("Login", "Account");
            }
            else
            {
                bool response = _provider.DeleteProviderAccount(id);

                if (response)
                {
                    TempData["ShowPositiveNotification"] = "Account Deleted Successfully.";

                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Data Not Saved!";

                }

                return RedirectToAction("ProviderMenu","Admindashboard");
            }
        }

        [HttpGet]
        [Route("createprovideraccount")]
        public IActionResult CreateProviderAccountView()
        {

            CreateProviderAccount createProviderAccoun =  new CreateProviderAccount();

            List<Region> regions = _dashboard.GetAllRegions();

            List<Role> roles = _provider.GetAllRoles();

            createProviderAccoun.Regions = regions;
            createProviderAccoun.roles = roles;

            return View("CreateProviderAccount",createProviderAccoun);
        }


        [HttpPost]
        
        public IActionResult CreateProviderAccount(CreateProviderAccount createProviderAccount)
        {
            string token = HttpContext.Request.Cookies["jwt"];


            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired || token.IsNullOrEmpty())
            {

                TempData["ShowNegativeNotification"] = "Session timed out!";
                return RedirectToAction("Login", "Account");
            }
            else if(ModelState.IsValid)
            {


                string hashedPassword = _account.GetHashedPassword(createProviderAccount.Password);
                if (hashedPassword.IsNullOrEmpty())
                {
                    TempData["ShowNegativeNotification"] = "Something Went Wrong!";
                    return RedirectToAction("ProviderMenu", "Admindashboard");

                }
                createProviderAccount.Password = hashedPassword;
                bool response = _provider.CreateProviderAccount(createProviderAccount);



                if (response)
                {
                    TempData["ShowPositiveNotification"] = "Account Created Successfully.";

                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Something Went Wrong!";

                }

                return RedirectToAction("ProviderMenu", "Admindashboard");
            }
            else
            {
                TempData["ShowNegativeNotification"] = "Not Valid Data!";

                return RedirectToAction("CreateProviderAccountView");
            }
        }


        [HttpGet]
        [Route("accountaccess")]

        public IActionResult AccountAccess()
        {

            bool response = _authManager.Authorize(HttpContext, 4);

            List<Role> roles = _provider.GetAllRoles();

            return View(roles);
        }


        public IActionResult CreateRole()
        {
            return View();
        }


        public IActionResult GetAccessArea(int accountType)
        {

            List<AccessAreas> menus = _provider.GetAreaAccessByAccountType(accountType);

            CreateRole createRole = new CreateRole();   
            createRole.AccessAreas = menus;

            return PartialView("_AccountControlArea", createRole);
        }


        public IActionResult PostCreateRole(CreateRole createRole)
        {
            string token = HttpContext.Request.Cookies["jwt"];

             LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);


            if (ModelState.IsValid) {

                bool response = _provider.CreateRole(createRole,loggedInUser.UserId);

                if (response)
                {
                    TempData["ShowPositiveNotification"] = "Role Created Successfully.";
                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Something Went Wrong!";
                }

                return RedirectToAction("AccountAccess");
            }
            else
            {
                TempData["ShowNegativeNotification"] = "Not Valid Data!";

                return RedirectToAction("CreateRole",createRole);
            }

        }


        public IActionResult DeletRole(int roleId)
        {
            if(roleId != 0)
            {

                bool response = _provider.DeleteRole(roleId);

                if (response)
                {
                    TempData["ShowPositiveNotification"] = "Role Deleted Successfully.";
                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Something Went Wrong";
                }

                return RedirectToAction("AccountAccess");
            }
            else
            {
                TempData["ShowNegativeNotification"] = "Role Not Exists!";

                return RedirectToAction("AccountAccess");


            }
        }
    }
}
