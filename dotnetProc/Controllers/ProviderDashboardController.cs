using HalloDoc_BAL.Interface;
using HalloDoc_DAL.AdminViewModels;
using HalloDoc_DAL.Models;
using HalloDoc_DAL.ProviderViewModels;
using HalloDoc_DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace dotnetProc.Controllers
{
    public class ProviderDashboardController : Controller
    {

        private readonly IAdmindashboard _dashboard;
        private readonly IAccount _account;
        private readonly IEmailService _emailService;
        private readonly IPatientReq _patientReq;
        private readonly IProvider _provider;
        private readonly IAuthManager _authManager;
        private readonly IProviderDashboard _providerDashboard;

        public ProviderDashboardController(IProviderDashboard providerDashboard,IAdmindashboard dashboard, IAccount account, IAuthManager authManager, IEmailService emailService, IPatientReq patientReq, IProvider provider)
        {
            _dashboard = dashboard;
            _account = account;
            _emailService = emailService;
            _patientReq = patientReq;
            _provider = provider;
            _authManager = authManager;
            _providerDashboard = providerDashboard;
        }




        public IActionResult Dashboard()
        {


            string token = HttpContext.Request.Cookies["jwt"];

            if (token == null)
            {
                TempData["ShowNegativeNotification"] = "Something went wrong!";
                return RedirectToAction("Login", "Account");
            }
            else
            {

                LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);
                string adminname = _dashboard.GetAdminUsername(loggedInUser.UserId);
                Response.Cookies.Append("UserName", adminname);


                List<DashboardRequests> dashboardRequests = _providerDashboard.GetAllProviderRequests(loggedInUser.UserId);
                RequestTypeCounts requestTypeCounts = _dashboard.GetAllRequestsCount(dashboardRequests);


                ProviderDashboard providerDashboard = new ProviderDashboard
                {

                    RequestTypeCounts = requestTypeCounts,

                };




                return View("Dashboard", providerDashboard);
            }

            


        }
        public IActionResult GetStatuswiseProviderRequests(string[] StatusArray, int currentPage)
        {
            //int statusint = int.Parse(status);

            string token = HttpContext.Request.Cookies["jwt"];

            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired)
            {
                TempData["ShowNegativeNotification"] = "You need to login!";
                return Json(new { code = 401 });
            }
            else
            {
                LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

                List<DashboardRequests> dashboardRequests = _providerDashboard.GetStatuswiseProviderRequestsService(StatusArray,loggedInUser.UserId);
               

                ProvideRequestTable provideRequestTable = new ProvideRequestTable
                {


                    TotalPages = (int)Math.Ceiling((double)dashboardRequests.Count / 2),
                    Requests = dashboardRequests.Skip(2 * (currentPage - 1)).Take(2).ToList(),
                    currentPage = currentPage,
                    Searchstring = "",
                    RequestType = 0,

                };

                return PartialView("_ProviderRequestTable", provideRequestTable);

            }


        }


        public IActionResult GetRequestorTypeWiseProviderRequests(int type, string[] StatusArray, string Name, int currentPage)
        {
            string token = HttpContext.Request.Cookies["jwt"];

            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired)
            {
                return Json(new { code = 401 });
            }
            else
            {

                int IntType = type;
                //if (type != null)
                //{
                //    IntType = int.Parse(type);
                //}
                LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

                List<DashboardRequests> dashboardRequests = _providerDashboard.GetProviderRequestsFromRequestorTypeService(IntType, StatusArray, Name,loggedInUser.UserId);
              

                ProvideRequestTable provideRequestTable = new ProvideRequestTable
                {


                    TotalPages = (int)Math.Ceiling((double)dashboardRequests.Count / 2),
                    Requests = dashboardRequests.Skip(2 * (currentPage - 1)).Take(2).ToList(),
                    currentPage = currentPage,
                    Searchstring = Name,
                    RequestType = IntType,


                };

                return PartialView("_ProviderRequestTable", provideRequestTable);
            }
        }


        public IActionResult AcceptRequest(int requestId)
        {

                  string token = HttpContext.Request.Cookies["jwt"];

                LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

            bool response = _providerDashboard.AcceptRequest(requestId, loggedInUser.UserId);

            if (response)
            {
                TempData["ShowPositiveNotification"] = "Request Accepted Successfully";
            }
            else
            {
                TempData["ShowNegativeNotification"] = "Somthing Went Wrong";
            }

            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        //[Route("ProviderDashboard/Viewrequest/{requestid}")]
        public IActionResult ViewRequest(int requestid)
        {
            return RedirectToRoute("ProviderViewCase", new { requestid = requestid });
        }

        [HttpGet]
        public IActionResult GetRequestNotes(int id)
        {


            return RedirectToRoute("ProviderviewNotes",new {id=id});
        }

        [HttpGet]

        public IActionResult ViewUploads(int id)
        {
            return RedirectToRoute("ProviderViewUploads", new { id = id });
        }


        [HttpGet]
        public IActionResult SendOrder(string id)
        {
            return RedirectToRoute("ProviderSendOrder", new {id=id});
        }

        [HttpGet]

        public IActionResult EncounterForm(string id)
        {
            return RedirectToRoute("ProviderEncounterForm", new { id = id });
        }

        [HttpGet]

        public IActionResult PatientReqByAdmin()
        {
            return RedirectToRoute("ProviderPatientReq");
        }

        [HttpPost]

        public IActionResult SaveTypeOfCare(int RequestId,bool HouseCall) 
        {


            bool response = _providerDashboard.SaveTypeOfCareService(RequestId, HouseCall);


            if (response)
            {
                TempData["ShowPositiveNotification"] = "Care Selected Successfully.";
            }
            else
            {
                TempData["ShowNegativeNotification"] = "Somthing Went Wrong";
            }

            return RedirectToAction("Dashboard");
        }


        [HttpPost]

        public IActionResult PostHouseCallAction(int id)
        {


            bool response = _providerDashboard.HouseCallActionService(id);

            if (response)
            {
                TempData["ShowPositiveNotification"] = "Moved To Conclude Sate Successfully.";
            }
            else
            {
                TempData["ShowNegativeNotification"] = "Somthing Went Wrong";
            }

            return RedirectToAction("Dashboard");


        }

        [HttpGet]
        [Route("Provider/ConcludeCare/{id}")]
        public IActionResult ConcludeCareView(int id)
        {
            

            ConcludeCare concludeCare = _providerDashboard.GetConcludeCareDetails(id);

            return View("ConcludeCare",concludeCare);
        }


        [HttpPost]

        public IActionResult PostConcludeCare(ConcludeCare concludeCare, int id)
        {

            string token = HttpContext.Request.Cookies["jwt"];

            LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

            bool response = _providerDashboard.ConcludeCareService(concludeCare, id,loggedInUser.UserId,loggedInUser.Role);


            if(response)
            {
                TempData["ShowPositiveNotification"] = "Request Concluded Successfully.";
            }
            else
            {
                TempData["ShowNeagativeNotification"] = "Something Went Wrong!";
            }

            return RedirectToAction("Dashboard");
        }


        [HttpGet]

        public IActionResult ProviderProfile()
        {
            string token = HttpContext.Request.Cookies["jwt"];

            LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

            return RedirectToRoute("ProviderProviderProfile", new { id = loggedInUser.UserId });
        }


        [HttpGet]
        [Route("Provider/MySchedule")]
        public IActionResult ProviderScheduling()
        {
            ShiftModel shift = new ShiftModel();

          
            shift.Today = DateTime.Now;
         
            //shift.lastDate = DateTime.Now;

            return View("ProviderScheduling", shift);
        }


    

        public IActionResult GetMonthWiseProviderShiftTableView(int date, int month, int year)
        {
            string token = HttpContext.Request.Cookies["jwt"];

            LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);


            if(loggedInUser.Role == "Admin")
            {
                ViewData["RoleName"] = "Adm";
            }
            else
            {
                ViewData["RoleName"] = "Phy";
            }

            List<MonthWisePhysicianShifts> MonthWiseshifts = _providerDashboard.GetPhysicianMonthWiseShifts(date, month, year, loggedInUser.UserId);
            Physicianshifts shift = new Physicianshifts();
            shift.monthWiseShifts = MonthWiseshifts;

            shift.lastDate = new DateTime(year, month, date);

            return PartialView("_ProviderMonthWiseShiftTable", shift);
        }

        public IActionResult GetProviderCreateShiftView()
        {

            string token = HttpContext.Request.Cookies["jwt"];

            LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);


            CreateShift createShift = new CreateShift();

            createShift.regions = _providerDashboard.GetAllPhysicianRegions(loggedInUser.UserId);
           


            return PartialView("_ProviderCreateShiftModal", createShift);
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

    }
}
