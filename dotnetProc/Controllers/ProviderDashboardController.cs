using HalloDoc_BAL.Interface;
using HalloDoc_DAL.AdminViewModels;
using HalloDoc_DAL.ProviderViewModels;
using HalloDoc_DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
       
    }
}
