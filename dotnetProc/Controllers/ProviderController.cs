﻿using DocumentFormat.OpenXml.Bibliography;
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

           if(_authManager.Authorize(HttpContext, 4) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }



            List<Role> roles = _provider.GetAllRoles();

            return View(roles);
        }


        public IActionResult CreateRole()
        {

            if (_authManager.Authorize(HttpContext, 4) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            return View();
        }


        public IActionResult GetAccessArea(int accountType,int roleId)
        {

            if (_authManager.Authorize(HttpContext, 4) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            List<AccessAreas> menus = _provider.GetAreaAccessByAccountType(accountType,roleId);

            CreateRole createRole = new CreateRole();   
            createRole.AccessAreas = menus;
            createRole.AccountType = accountType;

            return PartialView("_AccountControlArea", createRole);
        }


        public IActionResult PostCreateRole(CreateRole createRole)
        {

            if (_authManager.Authorize(HttpContext, 4) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

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

            if (_authManager.Authorize(HttpContext, 4) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }


            string token = HttpContext.Request.Cookies["jwt"];


            bool istokenExpired = _account.IsTokenExpired(token);

            if (roleId != 0)
            {

                LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);
                bool response = _provider.DeleteRole(roleId,loggedInUser.UserId);

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

        [HttpGet]
        [Route("createadmin")]
        public IActionResult CreateAdminAccount()
        {
            CreateAdminAccountModel createAdminAccount =  new CreateAdminAccountModel();

            List<Region> regions = _dashboard.GetAllRegions();

            List<Role> roles = _provider.GetAllRoles();

            createAdminAccount.Regions = regions;
            createAdminAccount.roles = roles;

            return View(createAdminAccount);
        }


        [HttpPost]

        public IActionResult PostCreateAdminAccount(CreateAdminAccountModel createAdminAccount)
        {
            string token = HttpContext.Request.Cookies["jwt"];


            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired || token.IsNullOrEmpty())
            {

                TempData["ShowNegativeNotification"] = "Session timed out!";
                return RedirectToAction("Login", "Account");
            }
            else if (ModelState.IsValid)
            {
                bool IsEmailExists = _patientReq.IsEmailExistance(createAdminAccount.Email);

                if(IsEmailExists)
                {
                    TempData["ShowNegativeNotification"] = "Account Already Exists!";
                    return RedirectToAction("CreateAdminAccount", createAdminAccount);
                }

                string hashedPassword = _account.GetHashedPassword(createAdminAccount.Password);
                if (hashedPassword.IsNullOrEmpty())
                {
                    TempData["ShowNegativeNotification"] = "Something Went Wrong!";
                    return RedirectToAction("ProviderMenu", "Admindashboard");

                }
                createAdminAccount.Password = hashedPassword;
                LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);
                bool response = _provider.CreateAdminAccount(createAdminAccount,loggedInUser.UserId);



                if (response)
                {
                    TempData["ShowPositiveNotification"] = "Account Created Successfully.";

                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Something Went Wrong!";

                }

                return RedirectToAction("CreateAdminAccount");
            }
            else
            {
                TempData["ShowNegativeNotification"] = "Not Valid Data!";

                return RedirectToAction("CreateAdminAccount",createAdminAccount);
            }
        }

        [HttpGet]
        [Route("editrole/{id}")]
        public IActionResult EditRoleView(int id)
        {

            Role role = _provider.GetRoleById(id);

            CreateRole createRole = new CreateRole();

            createRole.Roleid = id;
            createRole.RoleName = role.Name;
            createRole.AccountType = role.Accounttype;
            createRole.AccessAreas = _provider.GetAreaAccessByAccountType(role.Accounttype,id);


            return View("EditRole", createRole);

        }


        [HttpPost]
        
        public IActionResult EditRole(CreateRole createRole ,int id) 
        {

            string token = HttpContext.Request.Cookies["jwt"];


            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired || token.IsNullOrEmpty())
            {

                TempData["ShowNegativeNotification"] = "Session timed out!";
                return RedirectToAction("Login", "Account");
            }
            else if (ModelState.IsValid)
            {
                
                LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);
                bool response = _provider.EditRoleService(createRole, loggedInUser.UserId,id);



                if (response)
                {
                    TempData["ShowPositiveNotification"] = "Role Edited Successfully.";

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

                return RedirectToAction("EditRoleView", new {id = id});
            }

        }

        [HttpGet]
        [Route("scheduling")]

        public IActionResult ProviderScheduling()
        {

       

            ShiftModel shift = new ShiftModel();

            List<Region> regions = _dashboard.GetAllRegions();
            shift.Today = DateTime.Now;
            shift.regions = regions;
            //shift.lastDate = DateTime.Now;

            return View("Scheduling", shift);
        }


 
        public IActionResult GetDayWiseShiftTable(int date,int month,int year,int regionId)
        {
            List<DayWisePhysicianShifts> dayWiseShifts = _provider.GetAllPhysicianDayWiseShifts(date,month,year, regionId);

            Physicianshifts shift = new Physicianshifts();

            shift.dayWiseShifts = dayWiseShifts;
            shift.lastDate = new DateTime(year,month,date);

            return PartialView("_DayWiseShiftTable", shift);
        }


        public IActionResult GetCreateShiftView()
        {

            List<Region> regions = _dashboard.GetAllRegions();
            List<Physician> physicians = _provider.GetPhysicinForShiftsByRegionService(0);

            CreateShift createShift = new CreateShift();

            createShift.regions = regions;
            createShift.physicians = physicians;


            return PartialView("_ShiftModal", createShift);
        }
        public IActionResult CreateShift(CreateShift createShift)
        {

            string token = HttpContext.Request.Cookies["jwt"];


            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired || token.IsNullOrEmpty())
            {

                TempData["ShowNegativeNotification"] = "Session timed out!";
                return RedirectToAction("Login", "Account");
            }
            else if (!ModelState.IsValid)
            {

                LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);
                bool response = _provider.CreateShiftService(createShift, loggedInUser.UserId);

                if (response)
                {
                    TempData["ShowPositiveNotification"] = "Shift Created Successfully.";

                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Something Went Wrong!";

                }

                return RedirectToAction("ProviderScheduling");
            }
            else
            {
                TempData["ShowNegativeNotification"] = "Not Valid Data!";

                return RedirectToAction("ProviderScheduling");
            }

           

            
        }


        public IActionResult RequestedShiftView()
        {
            RequestedShiftModal requestedShiftModal = new RequestedShiftModal();


            List<Region> regions = _dashboard.GetAllRegions();

           


            requestedShiftModal.regions = regions;
            

            return View("RequestedShift", requestedShiftModal);
        }


        public IActionResult RequestedShiftTableView(int regionId)
        {
            List<RequestedShiftDetails> details = _provider.GetRequestedShiftDetails(regionId);

            return PartialView("_RequestedShiftTable", details);
        }


        public IActionResult ApproveShiftAction(List<RequestedShiftDetails> requestedShiftDetails)
        {

            string token = HttpContext.Request.Cookies["jwt"];


            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired || token.IsNullOrEmpty())
            {

                TempData["ShowNegativeNotification"] = "Session timed out!";
                return RedirectToAction("Login", "Account");
            }
            else if (!ModelState.IsValid)
            {

                LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);
                bool response = _provider.ApproveShiftsService(requestedShiftDetails,loggedInUser.UserId);


                if (response)
                {
                    TempData["ShowPositiveNotification"] = "Shift Approved Successfully.";

                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Something Went Wrong!";

                }

                return RedirectToAction("ProviderScheduling");
            }
            else
            {
                TempData["ShowNegativeNotification"] = "Not Valid Data!";

                return RedirectToAction("RequestedShiftView");
            }



        }

        public IActionResult GetWeekWiseShiftTableView(int date,int month,int year,int regionId)
        {


            List<WeekWisePhysicianShifts> weekWiseShifts = _provider.GetAllPhysicianWeekWiseShifts(date, month, year, regionId);

            Physicianshifts shift = new Physicianshifts();

            shift.weekWiseShifts = weekWiseShifts;

            shift.StartOfWeek = new DateTime(year, month, date);
            shift.lastDate = new DateTime(year, month, date).AddDays(7);

            return PartialView("_WeekWiseShiftTable", shift);
        }



        public IActionResult GetViewShiftModel(int shiftdetailId)
        {
            List<Region> regions = _dashboard.GetAllRegions();
            List<Physician> physicians = _provider.GetPhysicinForShiftsByRegionService(0);

            ViewShift viewShift = _provider.GetShiftDetailsById(shiftdetailId);

        

            viewShift.regions = regions;
            viewShift.physicians = physicians;


            return PartialView("_ViewShiftModel", viewShift);
        }
        public IActionResult GetMonthWiseShiftTableView(int date,int month,int year,int regionId)
        {
            List<MonthWisePhysicianShifts> MonthWiseshifts = _provider.GetAllPhysicianMonthWiseShifts(date, month, year, regionId);
            Physicianshifts shift = new Physicianshifts();
            shift.monthWiseShifts = MonthWiseshifts;

            shift.lastDate = new DateTime(year, month, date);

            return PartialView("_MonthWiseShiftTable", shift);
        }
    }
}
