using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using HalloDoc_BAL.Interface;
using HalloDoc_DAL.AdminViewModels;
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

        public ProviderController(IAdmindashboard dashboard, IAccount account, IAuthManager authManager, IEmailService emailService, IPatientReq patientReq, IProvider provider)
        {
            _dashboard = dashboard;
            _account = account;
            _emailService = emailService;
            _patientReq = patientReq;
            _provider = provider;
            _authManager = authManager;
        }

        [HttpGet]
        [Route("Admin/providerprofile/{id}",Name ="AdminProviderProfile")]
        [Route("Provider/Myprofile/{id}",Name ="ProviderProviderProfile")]
        public IActionResult ProviderProfile(int id)
        {

            if (_authManager.Authorize(HttpContext, 8) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            string token = HttpContext.Request.Cookies["jwt"];
            LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

            if(loggedInUser.Role == "Admin")
            {
                ViewData["RoleName"] = "Adm";
            }else if(loggedInUser.Role == "Physician")
            {
                ViewData["RoleName"] = "Phy";
            }



            ProviderProfileView providerProfileView = _provider.GetProviderData(id);

            providerProfileView.PhysicianId = id;

            return View(providerProfileView);
        }

        [HttpPost]
        public IActionResult EditProviderOnboardingData(ProviderProfileView providerProfileView,int id)
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
                bool response = _provider.EditOnBoardingData(providerProfileView.ProviderDocuments, id);

                if (response)
                {
                    TempData["ShowPositiveNotification"] = "Account Info Saved Successfully.";

                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Data Not Saved!";

                }

                return RedirectToAction("ProviderProfile", new { id = id });
            }
        }


        [HttpPost]

        public IActionResult SaveProviderAccountInfo(ProviderProfileView providerProfileView, int id)
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

                return RedirectToAction("ProviderProfile", new { id = id });
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


        public IActionResult SaveProviderInformation(ProviderProfileView providerProfileView, int id)
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

        public IActionResult SaveProviderMailingAndBillingInfo(ProviderProfileView providerProfileView, int id)
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

        public IActionResult SaveProviderProfileInfo(ProviderProfileView providerProfileView, int id)
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

                return RedirectToAction("ProviderMenu", "Admindashboard");
            }
        }

        [HttpGet]
        [Route("createprovideraccount")]
        public IActionResult CreateProviderAccountView()
        {

            if (_authManager.Authorize(HttpContext, 8) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            CreateProviderAccount createProviderAccoun = new CreateProviderAccount();

            List<Region> regions = _dashboard.GetAllRegions();

            List<Role> roles = _provider.GetAllRoles();

            List<SelectedRegions> selectedRegions = new List<SelectedRegions>();

            foreach(Region region in regions)
            {

                SelectedRegions selectedRegion = new SelectedRegions();

                selectedRegion.regionName = region.Name;
                selectedRegion.IsSelected = false;
                selectedRegion.regionId = region.Regionid;

                selectedRegions.Add(selectedRegion);

            }

            createProviderAccoun.Regions = selectedRegions;
            createProviderAccoun.roles = roles;
            createProviderAccoun.ProviderRegions = regions;

            return View("CreateProviderAccount", createProviderAccoun);
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
            else if (ModelState.IsValid)
            {

                bool IsEmailExists = _patientReq.IsEmailExistance(createProviderAccount.Email);

                if (IsEmailExists)
                {
                    TempData["ShowNegativeNotification"] = "Email is in use!";
                    return RedirectToAction("CreateProviderAccountView");
                }


                LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);
                string hashedPassword = _account.GetHashedPassword(createProviderAccount.Password);
                if (hashedPassword.IsNullOrEmpty())
                {
                    TempData["ShowNegativeNotification"] = "Something Went Wrong!";
                    return RedirectToAction("ProviderMenu", "Admindashboard");

                }
                createProviderAccount.Password = hashedPassword;
                bool response = _provider.CreateProviderAccount(createProviderAccount,loggedInUser.UserId);



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

            if (_authManager.Authorize(HttpContext, 4) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }



           

            return View();
        }


        public IActionResult GetAccountAccessTableView(int currentPage)
        {
            List<Role> roles = _provider.GetAllRoles();


            AccountAccessTable accountAccessTable = new AccountAccessTable()
            {
                TotalPages = (int)Math.Ceiling((double)roles.Count / 2),
                roles = roles.Skip(2 * (currentPage - 1)).Take(2).ToList(),
                currentPage = currentPage,
            };

            return PartialView("_AccountAccessTable", accountAccessTable);
        }

        public IActionResult CreateRole()
        {

            if (_authManager.Authorize(HttpContext, 4) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            return View();
        }


        public IActionResult GetAccessArea(int accountType, int roleId)
        {

            if (_authManager.Authorize(HttpContext, 4) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            List<AccessAreas> menus = _provider.GetAreaAccessByAccountType(accountType, roleId);

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


            if (ModelState.IsValid)
            {

                bool response = _provider.CreateRole(createRole, loggedInUser.UserId);

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

                return RedirectToAction("CreateRole", createRole);
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
                bool response = _provider.DeleteRole(roleId, loggedInUser.UserId);

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
            CreateAdminAccountModel createAdminAccount = new CreateAdminAccountModel();

            List<Region> regions = _dashboard.GetAllRegions();

            List<Role> roles = _provider.GetAllRoles();

            List<SelectedRegions> selectedRegions = new List<SelectedRegions>();

            foreach (Region region in regions)
            {

                SelectedRegions selectedRegion = new SelectedRegions();

                selectedRegion.regionName = region.Name;
                selectedRegion.IsSelected = false;
                selectedRegion.regionId = region.Regionid;

                selectedRegions.Add(selectedRegion);

            }

            createAdminAccount.SelectedRegions = selectedRegions;
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

                if (IsEmailExists)
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
                bool response = _provider.CreateAdminAccount(createAdminAccount, loggedInUser.UserId);



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

                return RedirectToAction("CreateAdminAccount", createAdminAccount);
            }
        }

        [HttpGet]
        [Route("editrole/{id}")]
        public IActionResult EditRoleView(int id)
        {

            if (_authManager.Authorize(HttpContext, 4) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            Role role = _provider.GetRoleById(id);

            CreateRole createRole = new CreateRole();

            createRole.Roleid = id;
            createRole.RoleName = role.Name;
            createRole.AccountType = role.Accounttype;
            createRole.AccessAreas = _provider.GetAreaAccessByAccountType(role.Accounttype, id);


            return View("EditRole", createRole);

        }


        [HttpPost]

        public IActionResult EditRole(CreateRole createRole, int id)
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
                bool response = _provider.EditRoleService(createRole, loggedInUser.UserId, id);



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

                return RedirectToAction("EditRoleView", new { id = id });
            }

        }

        [HttpGet]
        [Route("Admin/scheduling",Name ="AdminSchedule")]
     

        public IActionResult ProviderScheduling(int id)
        {

            if (_authManager.Authorize(HttpContext, 2) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            ShiftModel shift = new ShiftModel();

            List<Region> regions = _dashboard.GetAllRegions();
            shift.Today = DateTime.Now;
            shift.regions = regions;
            //shift.lastDate = DateTime.Now;

            return View("Scheduling", shift);
        }



        public IActionResult GetDayWiseShiftTable(int date, int month, int year, int regionId)
        {
            List<DayWisePhysicianShifts> dayWiseShifts = _provider.GetAllPhysicianDayWiseShifts(date, month, year, regionId);

            Physicianshifts shift = new Physicianshifts();

            shift.dayWiseShifts = dayWiseShifts;
            shift.lastDate = new DateTime(year, month, date);

            return PartialView("_DayWiseShiftTable", shift);
        }

        public IActionResult GetDayWiseAllShiftView(int date, int month, int year, int regionId)
        {
            List<ShiftInformation> dayWiseShifts = _provider.GetDayWiseAllShiftInformation(date, month, year, regionId);



            return PartialView("_AllShiftView", dayWiseShifts);
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
            else if (ModelState.IsValid)
            {

                if (_provider.IsValidShift(createShift))
                {
                    TempData["ShowNegativeNotification"] = "Shift Already Exists!";
                    return RedirectToAction("ProviderScheduling");

                }

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

        public IActionResult EditShift(ViewShift viewShift)
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

                //if (_provider.IsValidShift(createShift))
                //{
                //    TempData["ShowNegativeNotification"] = "Shift Already Exists!";
                //    return RedirectToAction("ProviderScheduling");

                //}

                LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);
                bool response = _provider.EditShiftService(viewShift, loggedInUser.UserId);

                if (response)
                {
                    TempData["ShowPositiveNotification"] = "Shift Edited Successfully.";

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


        public IActionResult RequestedShiftTableView(int regionId,int currentPage)
        {
            List<RequestedShiftDetails> details = _provider.GetRequestedShiftDetails(regionId);

            RequestShiftTable requestShiftTable = new RequestShiftTable()
            {
                regionId = regionId,
                TotalPages = (int)Math.Ceiling((double)details.Count / 2),
                requestedShiftDetails = details.Skip(2 * (currentPage - 1)).Take(2).ToList(),
                currentPage = currentPage,
            };

            return PartialView("_RequestedShiftTable", requestShiftTable);
        }


        public IActionResult ApproveShiftAction(RequestShiftTable requestShiftTable, int Val)
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

                if (Val == 1)
                {

                    bool response = _provider.ApproveShiftsService(requestShiftTable.requestedShiftDetails, loggedInUser.UserId);

                    if (response)
                    {
                        TempData["ShowPositiveNotification"] = "Shift Approved Successfully.";

                    }
                    else
                    {
                        TempData["ShowNegativeNotification"] = "Something Went Wrong!";

                    }
                }else if(Val == 2)
                {
                    bool response = _provider.DeleteRequestedShifts(requestShiftTable.requestedShiftDetails, loggedInUser.UserId);

                    if (response)
                    {
                        TempData["ShowPositiveNotification"] = "Shift Deleted Successfully.";

                    }
                    else
                    {
                        TempData["ShowNegativeNotification"] = "Something Went Wrong!";

                    }
                }


                return RedirectToAction("ProviderScheduling");
            }
            else
            {
                TempData["ShowNegativeNotification"] = "Not Valid Data!";

                return RedirectToAction("RequestedShiftView");
            }



        }



        public IActionResult DeleteShift(int ShiftDetailId)
        {
            if(ShiftDetailId == 0)
            {
                TempData["ShowNegativeNotification"] = "No Records Found";
            }

            string token = HttpContext.Request.Cookies["jwt"];
            LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);




            bool response = _provider.DeleteShiftService(ShiftDetailId, loggedInUser.UserId);

            if (response)
            {
                TempData["ShowPositiveNotification"] = "Shift Deleted Successfully.";
            }
            else
            {
                TempData["ShowNegativeNotification"] = "Something went wrong!";
            }

            return RedirectToAction("ProviderScheduling");
        }

        public IActionResult GetWeekWiseShiftTableView(int date, int month, int year, int regionId)
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
        public IActionResult GetMonthWiseShiftTableView(int date, int month, int year, int regionId)
        {
            List<MonthWisePhysicianShifts> MonthWiseshifts = _provider.GetAllPhysicianMonthWiseShifts(date, month, year, regionId);
            Physicianshifts shift = new Physicianshifts();
            shift.monthWiseShifts = MonthWiseshifts;

            shift.lastDate = new DateTime(year, month, date);

            return PartialView("_MonthWiseShiftTable", shift);
        }

        [HttpGet]
        [Route("UserAccess")]
        public IActionResult UserAccessView()
        {

            if (_authManager.Authorize(HttpContext, 15) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            List<Role> roles = _provider.GetAllRoles();

            UserAccessMenu userAccessMenu = new UserAccessMenu();

            userAccessMenu.roles = roles;

            return View("UserAccess", userAccessMenu);
        }


        public IActionResult GetUserAccessTableView(int roleId,int currentPage)
        {



            List<UserAccess> userAccesses = _provider.GetAllAspNetUsers(roleId);


            UserAccessTable userAccessTable = new UserAccessTable()
            {
                TotalPages = (int)Math.Ceiling((double)userAccesses.Count / 2),
                userAccesses = userAccesses.Skip(2 * (currentPage - 1)).Take(2).ToList(),
                currentPage = currentPage,
            };



            return PartialView("_UserAccessTable", userAccessTable);
        }


        public IActionResult RedirectToAspEditAccount(int account, int id)
        {
            if (account == 0)
            {
                return RedirectToAction("EditAdminProfile", "Admindashboard", new { id = id });
            }
            else if (account == 1)
            {
                return RedirectToAction("ProviderProfile", new { id = id });
            }
            else
            {
                return RedirectToAction("UserAccessView");
            }
        }


        public IActionResult GetVendorsView()
        {

            if (_authManager.Authorize(HttpContext, 10) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

                List<Healthprofessionaltype> healthprofessionaltypes = _provider.GetAllHealthProfessionalTypes();

            Vendors vendors = new Vendors()
            {

                healthprofessionaltypes = healthprofessionaltypes,
            };

            return View("Vendors", vendors);
        }

        public IActionResult GetVendorsTableView(string vendorName, int HealthProfessionId, int currentPage)
        {
            List<VendorList> vendorLists = _provider.GetVendorsData(vendorName,HealthProfessionId);

            VendorTable vendorTable = new VendorTable()
            {

                TotalPages = (int)Math.Ceiling((double)vendorLists.Count / 2),
                VendorList = vendorLists.Skip(2 * (currentPage - 1)).Take(2).ToList(),
                currentPage = currentPage,

            };

            return PartialView("_VendorTable", vendorTable);
        }

        public IActionResult EditVendorView(int id)
        {
            VendorDetails vendorDetails = _provider.EditVendorDetailsView(id);
            vendorDetails.VendorId = id;

            return View("EditVendor", vendorDetails);
        }

        public IActionResult EditVendorDetails(VendorDetails vendorDetails,int id)
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
                bool response = _provider.EditVendor(vendorDetails,id, loggedInUser.UserId);

                if (response)
                {
                    TempData["ShowPositiveNotification"] = "Vendor Edited Successfully.";

                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Something Went Wrong!";

                }

                return RedirectToAction("EditVendorView",new {id = id});
            }
            else
            {
                TempData["ShowNegativeNotification"] = "Not Valid Data!";

                return RedirectToAction("EditVendorView",  new {id=id});
            }
        }

        public IActionResult DeleteVendor(int id)
        {
            string token = HttpContext.Request.Cookies["jwt"];


            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired || token.IsNullOrEmpty())
            {

                TempData["ShowNegativeNotification"] = "Session timed out!";
                return RedirectToAction("Login", "Account");
            }
            else if(id!= 0)
            {

                bool response = _provider.DeleteVendorService(id);

                if (response)
                {
                    TempData["ShowPositiveNotification"] = "Vendor Deleted Successfully.";

                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Something Went Wrong!";

                }

                return RedirectToAction("GetVendorsView");
            }
            else
            {
                TempData["ShowNegativeNotification"] = "User Not Found!";
                return RedirectToAction("GetVendorsView");

            }

        }


        public IActionResult AddVendorView()
        {

            if (_authManager.Authorize(HttpContext, 10) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            VendorDetails vendorDetails = new VendorDetails();

            vendorDetails.regions = _dashboard.GetAllRegions();
            vendorDetails.healthprofessionaltypes = _provider.GetAllHealthProfessionalTypes();

            return View("AddVendor", vendorDetails);
        }

        public IActionResult AddVendor(VendorDetails vendorDetails)
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
                bool response = _provider.AddVendorService(vendorDetails);

                if (response)
                {
                    TempData["ShowPositiveNotification"] = "Vendor Added Successfully.";

                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Something Went Wrong!";

                }

                return RedirectToAction("GetVendorsView");
            }
            else
            {
                TempData["ShowNegativeNotification"] = "Not Valid Data!";

                return RedirectToAction("GetVendorsView");
            }
        }

        public IActionResult SearchRecordsView()
        {
            if (_authManager.Authorize(HttpContext, 3) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            return View("SearchRecords");
        }

        public IActionResult GetSearchRecordsTableView(int currentPage,int Status,string PatientName,int RequestType,DateTime FromDate,DateTime ToDate,string ProviderName,string Email,string Phone)
        {

            List<SearchRecords> searchRecords = _provider.GetFillteredSearchRecordsData( Status,  PatientName,  RequestType,  FromDate,  ToDate,  ProviderName,  Email,  Phone);

         

            SearchRecordsTable searchRecordsTable = new SearchRecordsTable()
            {
                TotalPages = (int)Math.Ceiling((double)searchRecords.Count / 5),
                SearchRecords = searchRecords.Skip(5 * (currentPage - 1)).Take(5).ToList(),
                currentPage = currentPage,
     
            };


            return PartialView("_SearchRecordsTable", searchRecordsTable);

        }



        public IActionResult DeleteRecord(int requestId)
        {

             if(requestId == 0)
            {
                TempData["ShowNegativeNotification"] = "No Record Found!";
                return RedirectToAction("SearchRecordsView");
            }


            bool response = _provider.DeleteRecordService(requestId);


            if (response)
            {
                TempData["ShowPositiveNotification"] = "Record Deleted Succesfully.";
            }
            else
            {
                TempData["ShowNegativeNotification"] = "Something went wrong!";
            }

            return RedirectToAction("SearchRecordsView", "Provider");
        }


        public IActionResult EmailLogsView()
        {
            if (_authManager.Authorize(HttpContext, 3) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            return View("EmailLogs");
        }
        public IActionResult GetEmailLogsTableView(int currentPage, string ReciverName, int RoleId ,string  EmailId, DateTime CreateDate,DateTime SentDate)
        {

            List<EmailLogs> emailLogs = _provider.GetFillteredEmailLogsData( ReciverName,  RoleId,   EmailId,  CreateDate,  SentDate);

           
            EmailLogsTable emailLogsTable = new EmailLogsTable()
            {
                TotalPages = (int)Math.Ceiling((double)emailLogs.Count / 2),
                EmailLogs = emailLogs.Skip(2 * (currentPage - 1)).Take(2).ToList(),
                currentPage = currentPage,
               
            };


            return PartialView("_EmailLogsTable", emailLogsTable);

        }

        public IActionResult PatientHistoryView()
        {
            if (_authManager.Authorize(HttpContext, 3) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            return View("PatientHistory");
        }


        public IActionResult GetPatientHistoryTableView(int currentPage,string FirstName, string LastName, string Email, string Phone)
        {


            List<PatientHistory> patientHistories = _provider.GetPatientHistoryData(FirstName, LastName, Email, Phone);


            PatientHistoryTable patientHistoryTable = new PatientHistoryTable()
            {
                TotalPages = (int)Math.Ceiling((double)patientHistories.Count / 3),
                patientHistories = patientHistories.Skip(3 * (currentPage - 1)).Take(3).ToList(),
                currentPage = currentPage,
        
            };
            


            return PartialView("_PatienthistoryTable", patientHistoryTable);
        }


        public IActionResult GetPatientExploreView(int id)
        {
            if (_authManager.Authorize(HttpContext, 3) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            

            if(id == 0 )
            {
                TempData["ShowNeagativeNotification"] = "Record not Found";

                return RedirectToAction("PatientHistoryView");

            }



            PatientExplreView patientExplreView = new PatientExplreView();

            patientExplreView.UserId = id;


            return View("PatientExplore",patientExplreView);
            


        }


        public IActionResult GetPatientExploreTableView(int currentPage,int UserId)
        {

            List<PatientExplore> patientExplores = _provider.GetpatientExploreData(UserId);

            PatientExploreTable patientHistoryTable = new PatientExploreTable()
            {
                TotalPages = (int)Math.Ceiling((double)patientExplores.Count / 2),
                patientRecords = patientExplores.Skip(2 * (currentPage - 1)).Take(2).ToList(),
                currentPage = currentPage,
                UserId = UserId,
            };

            return PartialView("_PatientExploredTable", patientHistoryTable);
        }


        public IActionResult BlockHistoryView()
        {


            if (_authManager.Authorize(HttpContext, 3) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            return View("BlockHistory");

        }


        public IActionResult GetBlockHistoryTableView(int currentPage,string Name,DateTime CreateDate,string Email,string Phone)
        {

            List<BlockHistories> patientHistories = _provider.GetBlockHistoriesData(Name, CreateDate, Email, Phone);


            BlockHistoryTable patientHistoryTable = new BlockHistoryTable()
            {
                TotalPages = (int)Math.Ceiling((double)patientHistories.Count / 2),
                BlockRecords = patientHistories.Skip(2 * (currentPage - 1)).Take(2).ToList(),
                currentPage = currentPage,
                
            };



            return PartialView("_blockHistoryTableView", patientHistoryTable);
        }


    }
}
