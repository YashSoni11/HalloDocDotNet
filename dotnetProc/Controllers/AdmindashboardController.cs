using Microsoft.AspNetCore.Mvc;
using HalloDoc_BAL.Interface;
using HalloDoc_DAL.Models;
using HalloDoc_DAL.ViewModels;
using HalloDoc_DAL.AdminViewModels;
using Newtonsoft.Json;
using HalloDoc_BAL.Repositery;
using System.IdentityModel.Tokens.Jwt;
using System.Globalization;
using Microsoft.IdentityModel.Tokens;
using ClosedXML;
using ClosedXML.Excel;
using System.Security.Principal;
using DocumentFormat.OpenXml.Office2010.Excel;
using HalloDoc_DAL.ProviderViewModels;




namespace dotnetProc.Controllers
{
    public class AdmindashboardController : Controller
    {



        private readonly IAdmindashboard _dashboard;
        private readonly IAccount _account;
        private readonly IEmailService _emailService;
        private readonly IPatientReq _patientReq;
        private readonly IProvider _provider;
        private readonly IAuthManager _authManager;

        public AdmindashboardController(IProvider provider, IAdmindashboard dashboard, IAccount account, IEmailService emailService, IPatientReq patientReq, IAuthManager authManager)
        {
            _dashboard = dashboard;
            _account = account;
            _emailService = emailService;
            _patientReq = patientReq;
            _authManager = authManager;
            _provider = provider;
        }


        public IActionResult Index()
        {
            return View();
        }



        #region AdminDashboardActions
        [HttpGet]
        [Route("Admindashboard/Dashboard")]
        public IActionResult Dashboard()
        {
            if (_authManager.Authorize(HttpContext, 6) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            string token = HttpContext.Request.Cookies["jwt"];

            if (token == null)
            {
                TempData["ShowNegativeNotification"] = "Something went wrong!";
                return RedirectToAction("Login", "Account");
            }
            else
            {

                LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);
                string adminname = _dashboard.GetAdminUsername(loggedInUser.UserId, loggedInUser.Role);
                Response.Cookies.Append("UserName", adminname);


                List<DashboardRequests> dashboardRequests = _dashboard.GetAllRequests();
                RequestTypeCounts requestTypeCounts = _dashboard.GetAllRequestsCount(dashboardRequests);


                AdminDashboard adminDashboard = new AdminDashboard
                {

                    RequestTypeCounts = requestTypeCounts,

                };




                return View(adminDashboard);
            }
        }
        public IActionResult GetStatuswiseRequests(string[] StatusArray, int currentPage)
        {
            //int statusint = int.Parse(status);

            if (_authManager.Authorize(HttpContext, 9) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            string token = HttpContext.Request.Cookies["jwt"];

            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired)
            {
                TempData["ShowNegativeNotification"] = "You need to login!";
                return Json(new { code = 401 });
            }
            else
            {
                List<DashboardRequests> dashboardRequests = _dashboard.GetStatuswiseRequests(StatusArray);
                List<Region> regions = _dashboard.GetAllRegions();

                RequestTable adminDashboard = new RequestTable
                {


                    TotalPages = (int)Math.Ceiling((double)dashboardRequests.Count / 2),
                    Requests = dashboardRequests.Skip(2 * (currentPage - 1)).Take(2).ToList(),
                    currentPage = currentPage,
                    Searchstring = "",
                    Regions = regions,
                    regionId = 0,
                    RequestType = 0,

                };

                return PartialView("_Requeststable", adminDashboard);

            }


        }
        public IActionResult GetRequestorTypeWiseRequests(int type, string[] StatusArray, string region, string Name, int currentPage)
        {

            if (_authManager.Authorize(HttpContext, 9) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

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

                List<DashboardRequests> dashboardRequests = _dashboard.GetRequestsFromRequestorType(IntType, StatusArray, region, Name);
                List<Region> regions = _dashboard.GetAllRegions();

                RequestTable adminDashboard = new RequestTable
                {


                    TotalPages = (int)Math.Ceiling((double)dashboardRequests.Count / 2),
                    Requests = dashboardRequests.Skip(2 * (currentPage - 1)).Take(2).ToList(),
                    currentPage = currentPage,
                    regionId = region == null ? 0 : int.Parse(region),
                    Regions = regions,
                    Searchstring = Name,
                    RequestType = IntType,


                };

                return PartialView("_Requeststable", adminDashboard);
            }
        }
        #endregion

        #region ProviderLocationsAction
        [HttpGet]
        [Route("providerlocation")]
        public IActionResult ProviderLocations()
        {
            if (_authManager.Authorize(HttpContext, 17) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            List<Physicianlocation> physicianlocations = _dashboard.GetAllPhysicianlocation();


            return View("providerlocation", physicianlocations);
        }
        #endregion


        #region RequestNotesActions
        [HttpGet]
        [Route("Admin/Viewnotes/{id}", Name = "AdminviewNotes")]
        [Route("Provider/Viewnotes/{id}", Name = "ProviderviewNotes")]
        public IActionResult GetRequestNotes(int id)
        {

            string token = HttpContext.Request.Cookies["jwt"];


            LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);


            if (loggedInUser.Role != "Physician" &&  _authManager.Authorize(HttpContext, 9) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            if (loggedInUser.Role == "Physician")
            {
                if (_provider.IsRequestBelongToProvider(loggedInUser.UserId, id) == false)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                ViewData["RoleName"] = "Phy";

            }
            else if (loggedInUser.Role == "Admin")
            {
                if (_dashboard.IsRequestExist(id) == false)
                {

                    return RedirectToAction("Error", "Account");
                }
                ViewData["RoleName"] = "Adm";
            }




            RequestNotes requestNotes = _dashboard.GetNotesFromRequestId(id);

            NotesView notes = new NotesView
            {
                requestnotes = requestNotes,
                requestId = id
            };

            return View("ViewNotes", notes);
        }

        [HttpPost]
        public IActionResult SaveNotesChanges(NotesView notesView, int id)
        {
            string token = HttpContext.Request.Cookies["jwt"];

            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired || token.IsNullOrEmpty())
            {
                TempData["ShowNegativeNotification"] = "You need to login";
                return RedirectToAction("Login", "Account");
            }
            else
            {
                LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

                int newrequestid = id;

                bool response = _dashboard.SaveNotesChanges(notesView.AdditionalNotes, newrequestid, loggedInUser.Role, loggedInUser.UserId);

                if (response)
                {
                    TempData["ShowPositiveNotification"] = "Chnages Saved Successfully.";
                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Data Not Saved!";
                }

                if (loggedInUser.Role == "Admin")
                {
                    return RedirectToRoute("AdminviewNotes", new { id = newrequestid });

                }
                else
                {
                    return RedirectToRoute("ProviderviewNotes", new { id = newrequestid });

                }

            }

        }
        #endregion


        #region ViewCaseActions
        [HttpGet]
        [Route("Admindashboard/Viewrequest/{requestid}", Name = "AdminViewCase")]
        [Route("ProviderDashboard/Viewrequest/{requestid}", Name = "ProviderViewCase")]
        public IActionResult ViewRequest(int requestid)
        {

            string token = HttpContext.Request.Cookies["jwt"];


            LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

            if (loggedInUser.Role != "Physician"  &&  _authManager.Authorize(HttpContext, 9) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }


            if (loggedInUser.Role == "Physician")
            {
                if (_provider.IsRequestBelongToProvider(loggedInUser.UserId, requestid) == false)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                ViewData["RoleName"] = "Phy";
            }
            else if (loggedInUser.Role == "Admin")
            {
                if(_dashboard.IsRequestExist(requestid) == false) {

                    return RedirectToAction("Error", "Account");
                }

                ViewData["RoleName"] = "Adm";
            }



            ClientRequest requestclient = _dashboard.GetUserInfoFromRequestId(requestid);
            requestclient.RequestId = requestid;

            return View(requestclient);

        }

        [HttpPost]

        public IActionResult PostViewRequest(ClientRequest clientRequest, string requestId)
        {
            int newrequestid = int.Parse(requestId);

           

            ClientRequest clientRequest1 = _dashboard.UpdateClientRequest(clientRequest, newrequestid);

            if(clientRequest1 != null)
            {
                TempData["ShowPostiveNotification"] = "Case Updated Successfully.";
            }
            else
            {
                TempData["ShowNegativeNotification"] = "Something went wrong!";
            }

            return RedirectToRoute("AdminViewCase", new { requestid = newrequestid});
        }
        #endregion


        #region CancleCaseActions
        public IActionResult GetCancleCaseView(string id)
        {

            string token = HttpContext.Request.Cookies["jwt"];

            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired)
            {
                return Json(new { code = 401 });
            }
            else
            {

                int newrequestid = int.Parse(id);

                string name = _dashboard.GetPatientName(newrequestid);
                List<Casetag> casetags = _dashboard.GetAllCaseTags();

                AdminCancleCase adminCancleCase = new AdminCancleCase
                {
                    PatientName = name,
                    requestId = newrequestid,
                    reasons = casetags,
                };

                return PartialView("_CancleCaseModel", adminCancleCase);
            }
        }

        [HttpPost]
        public IActionResult PostCancleRequest(AdminCancleCase adminCancleCase, int requestId)
        {
            string token = HttpContext.Request.Cookies["jwt"];

            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired)
            {
                //TempData["ShowNegativeNotification"] = "You need to login!";

                return RedirectToAction("Login", "Account", new { message = "You need to Login!" });
            }
            else if (ModelState.IsValid)
            {

                LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);
                bool response = _dashboard.UpdateRequestToClose(adminCancleCase, requestId, loggedInUser.UserId);

                if (response)
                {
                    TempData["ShowPositiveNotification"] = "Request Canclled Succesfully.";
                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Something went wrong!";
                }
                return RedirectToAction("Dashboard", "Admindashboard");
            }
            else
            {
                TempData["ShowNegativeNotification"] = "Not Valid Data!";
                return RedirectToAction("Dashboard", "Admindashboard");

            }
        }
        #endregion

        #region AdminProfile
        [HttpGet]
        [Route("myprofile")]
        public IActionResult AdminProfile()
        {

            if (_authManager.Authorize(HttpContext, 5) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            string token = HttpContext.Request.Cookies["jwt"];

            LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);


            AdminProfile adminProfile = _dashboard.GetAdminProfileData(loggedInUser.UserId);

            adminProfile.adminId = loggedInUser.UserId;


            return View("Adminprofile", adminProfile);
        }

        [HttpGet]
        [Route("editadmin/{id}")]
        public IActionResult EditAdminProfile(int id)
        {
            AdminProfile adminProfile = _dashboard.GetAdminProfileData(id);
            adminProfile.adminId = id;

            return View("Adminprofile", adminProfile);
        }

        [HttpPost]
        public IActionResult ResetAdminPassword(string AdminPassword, int adminId)
        {
            string token = HttpContext.Request.Cookies["jwt"];

            bool istokenExpired = _account.IsTokenExpired(token);



            if (istokenExpired || token.IsNullOrEmpty())
            {
                TempData["ShowNegativeNotification"] = "Session timed out!";
                return RedirectToAction("Login", "Account");
            }
            else if (AdminPassword.IsNullOrEmpty())
            {
                LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);
                TempData["ShowNegativeNotification"] = "Not Valid Password!";

                if (loggedInUser.UserId == adminId)
                {

                    return RedirectToAction("AdminProfile");
                }
                else
                {


                    return RedirectToAction("EditAdminProfile", new { id = adminId });
                }


            }
            else
            {
                LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);
                string hashedPassword = _account.GetHashedPassword(AdminPassword);

                bool response = _dashboard.ResetAdminPassword(loggedInUser.UserId, hashedPassword);

                if (response)
                {
                    TempData["ShowPositiveNotification"] = "Password Changed Successfully.";
                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Somthing went wrong!";
                }


                if (loggedInUser.UserId == adminId)
                {

                    return RedirectToAction("AdminProfile");
                }
                else
                {


                    return RedirectToAction("EditAdminProfile", new { id = adminId });
                }
            }
        }

        [HttpPost]
        public IActionResult SaveAdminAccountInformation(AdminProfile ap)
        {
            string token = HttpContext.Request.Cookies["jwt"];

            if (ModelState.IsValid)
            {

            }

            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired || token.IsNullOrEmpty())
            {
                TempData["ShowNegativeNotification"] = "Session timed out!";
                return RedirectToAction("Login", "Account");
            }
            else
            {
                LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

                if (TryValidateModel(ap.accountInfo))
                {



                    bool IsEmailExists = _patientReq.IsValidAccountEmail(ap.accountInfo.Email,ap.adminId,loggedInUser.Role);

                    if (IsEmailExists == false)
                    {
                        TempData["ShowNegativeNotification"] = "Account with this email already exsist!";
                        return loggedInUser.UserId == ap.adminId ? RedirectToAction("AdminProfile") : RedirectToAction("EditAdminProfile", new { id = ap.adminId });


                    }


                    bool response = _dashboard.SaveAdminAccountInfo(ap, ap.adminId);

                    if (response)
                    {
                        TempData["ShowPositiveNotification"] = "Account Information Saved Successfully.";

                    }
                    else
                    {
                        TempData["ShowNegativeNotification"] = "Data Not Saved!";

                    }
                    return loggedInUser.UserId == ap.adminId ? RedirectToAction("AdminProfile") : RedirectToAction("EditAdminProfile", new { id = ap.adminId });


                }
                else
                {

                    TempData["ShowNegativeNotification"] = "Somthing went wrong!";

                    return loggedInUser.UserId == ap.adminId ? RedirectToAction("AdminProfile") : RedirectToAction("EditAdminProfile", new { id = ap.adminId });


                }


            }

        }


        [HttpPost]
        public IActionResult SaveMailingAndBillingInformation(AdminProfile ap)
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
                LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

                if (TryValidateModel(ap.mailingAndBillingInfo))
                {

                    bool response = _dashboard.SaveAdminMailingAndBillingInfo(ap, ap.adminId);
                    if (response)
                    {
                        TempData["ShowPositiveNotification"] = "Mailing & Billig Info Saved Successfully.";

                    }
                    else
                    {
                        TempData["ShowNegativeNotification"] = "Data Not Saved!";

                    }


                    return loggedInUser.UserId == ap.adminId ? RedirectToAction("AdminProfile") : RedirectToAction("EditAdminProfile", new { id = ap.adminId });

                }

                else
                {
                    TempData["ShowNegativeNotification"] = "Somthing went wrong!";

                    return loggedInUser.UserId == ap.adminId ? RedirectToAction("AdminProfile") : RedirectToAction("EditAdminProfile", new { id = ap.adminId });

                }
            }


        }
        #endregion

        #region ProviderMenuActions
        [HttpGet]
        [Route("providers")]
        public IActionResult ProviderMenu()
        {
            if (_authManager.Authorize(HttpContext, 8) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            List<Region> regions = _dashboard.GetAllRegions();

            Providers providers1 = new Providers()
            {

                regions = regions
            };

            return View(providers1);
        }

        [HttpPost]

        public IActionResult SaveProviderChanges(Providers pr)
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
                bool response = _dashboard.SaveProviderChanges(pr.providers);

                if (response)
                {
                    TempData["ShowPositiveNotification"] = "Data Saved Successfully.";

                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Data Not Saved!";

                }

                return RedirectToAction("ProviderMenu");
            }
        }
        public IActionResult GetPhysicianByRegion(int regionId)
        {

            string token = HttpContext.Request.Cookies["jwt"];

            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired)
            {
                return Json(new { code = 401 });
            }
            else
            {
                List<Physician> physicians = new List<Physician>();

                if (regionId == 0)
                {
                    physicians = _dashboard.GetAllPhysician();
                }
                else
                {
                    physicians = _dashboard.FilterPhysicianByRegion(regionId);
                }

                JsonSerializerSettings settings = new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                };

                string response = JsonConvert.SerializeObject(physicians, settings);

                return Json(response);
            }

        }

        public IActionResult GetProvidersByRegions(int regionId, int currentPage)
        {
            string token = HttpContext.Request.Cookies["jwt"];

            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired)
            {
                return Json(new { code = 401 });
            }
            else
            {
                List<ProviderMenu> physicians = _dashboard.FilterProviderByRegions(regionId);


                ProviderList providers = new ProviderList()
                {

                    TotalPages = (int)Math.Ceiling((double)physicians.Count / 2),
                    providers = physicians.Skip(2 * (currentPage - 1)).Take(2).ToList(),
                    currentPage = currentPage,
                };

                return PartialView("_ProviderTable", providers);
            }

        }
        #endregion



        #region AssignCaseActions
        public IActionResult GetAssginCaseView(int requestId)
        {
            string token = HttpContext.Request.Cookies["jwt"];

            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired)
            {
                return Json(new { code = 401 });
            }
            else
            {

                List<Region> regions = _dashboard.GetAllRegions();

                List<Physician> physicians = _dashboard.GetAllPhysician();

                AdminAssignCase adminAssignCase = new AdminAssignCase()
                {
                    Regions = regions,
                    Physicians = physicians,
                    RequestId = requestId
                };

                return PartialView("_AssignCaseModal", adminAssignCase);
            }
        }


        public IActionResult PostAssignCase(AdminAssignCase adminAssignCase, int requestId)
        {

            string token = HttpContext.Request.Cookies["jwt"];

            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired || token.IsNullOrEmpty())
            {

                return RedirectToAction("Login", "Account", new { message = "You need to Login!" });
            }
            else if (ModelState.IsValid)
            {
                bool IsAssigned = _dashboard.IsRequestAssigned(requestId);

                if(IsAssigned)
                {
                    TempData["ShowNegativeNotification"] = "Request Already Assigned!";
                    return RedirectToAction("Dashboard", "Admindashboard");


                }
                LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

                bool response = _dashboard.AssignRequest(adminAssignCase, requestId,loggedInUser.UserId);


                if (response)
                {
                    TempData["ShowPositiveNotification"] = "Request Assigned Succesfully.";
                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Something went wrong!";
                }
                return RedirectToAction("Dashboard", "Admindashboard");
            }
            else
            {
                TempData["ShowNegativeNotification"] = "Not Valid Data!";
                return RedirectToAction("Dashboard", "Admindashboard");

            }


        }
        #endregion


        #region BlockCaseActions
        public IActionResult GetBlockCaseView(AdminBlockCase adminBlockCase, string requestId)
        {
            string token = HttpContext.Request.Cookies["jwt"];

            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired)
            {
                return Json(new { code = 401 });
            }
            else
            {
                int newrequestid = int.Parse(requestId);

                string name = _dashboard.GetPatientName(newrequestid);

                AdminBlockCase adminBlockCase1 = new AdminBlockCase
                {
                    PatientName = name,
                    RequestId = newrequestid,
                };

                return PartialView("_BlockCaseModal", adminBlockCase1);
            }
        }

        public IActionResult PostBlockCase(AdminBlockCase adminBlockCase, int requestId)
        {

            string token = HttpContext.Request.Cookies["jwt"];

            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired || token.IsNullOrEmpty())
            {

                return RedirectToAction("Login", "Account", new { message = "You need to Login!" });
            }
            else if (ModelState.IsValid)
            {

                LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

                Request request = _dashboard.BlockRequest(adminBlockCase, requestId,loggedInUser.UserId);


                if (request != null)
                {
                    TempData["ShowPositiveNotification"] = "Request Blocked Succesfully.";
                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Something went wrong!";
                }
                return RedirectToAction("Dashboard", "Admindashboard");
            }
            else
            {
                TempData["ShowNegativeNotification"] = "Not Valid Data!";
                return RedirectToAction("Dashboard", "Admindashboard");

            }




        }
        public IActionResult UnBlockRequest(int RequestId)
        {

            if (RequestId == 0)
            {
                TempData["ShowNegativNotification"] = "No Data Found!";
            }

            bool response = _dashboard.UnblockRequestService(RequestId);


            if (response)
            {
                TempData["ShowPositiveNotification"] = "Request Unblocked Successfully.";
            }
            else
            {
                TempData["ShowNegativeNotification"] = "Something wen wrong!";
            }


            return RedirectToAction("BlockHistoryView", "Provider");
        }
        #endregion


        #region ViewDocumentsActions
        [HttpGet]
        [Route("Admin/Viewdocuments/{id}", Name = "AdminViewUploads")]
        [Route("Provider/Viewdocuments/{id}", Name = "ProviderViewUploads")]
        public IActionResult ViewUploads(int id)
        {

            string token = HttpContext.Request.Cookies["jwt"];
                LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);
            if(loggedInUser.Role != "Physician" &&  _authManager.Authorize(HttpContext, 9) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }


            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired)
            {
                TempData["ShowNegativeNotification"] = "You need to login!";
                return RedirectToAction("Login", "Account");
            }
            else
            {

                if (loggedInUser.Role == "Physician")
                {
                    if (_provider.IsRequestBelongToProvider(loggedInUser.UserId, id) == false)
                    {
                        return RedirectToAction("AccessDenied", "Account");
                    }
                    ViewData["RoleName"] = "Phy";
                }
                else if (loggedInUser.Role == "Admin")
                {
                    if (_dashboard.IsRequestExist(id) == false)
                    {

                        return RedirectToAction("Error", "Account");
                    }
                    ViewData["RoleName"] = "Adm";
                }
                TempData["requestId"] = id;

                List<ViewDocument> docs = _dashboard.GetDocumentsByRequestId(id);
                string ConfirmationNumber = _dashboard.GetConfirmationNumber(id);
                string PatientName = _dashboard.GetPatientName(id);

                Documents documents = new Documents
                {
                    requestId = id,
                    ViewDocuments = docs,
                    FormFile = null,
                    ConfirmationNumber = ConfirmationNumber,
                    PatientName = PatientName,
                };


                return View(documents);
            }
        }


        [HttpPost]
        public IActionResult UploadDocuments(Documents docs, int id)
        {

            //string path = HttpContext.Request.Path;

            //string[] paths = path.Split('/');

            //int requestId = int.Parse(paths[paths.Length - 1]);


            string token = HttpContext.Request.Cookies["jwt"];
            LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);


            bool response = true;

            for (int i = 0; i < docs.FormFile.Count; i++)
            {

                response &= _account.UploadFile(docs.FormFile[i], id, loggedInUser.Role, loggedInUser.UserId);

            }

            if (loggedInUser.Role == "Admin")
            {
                return RedirectToRoute("AdminViewUploads", new { id = id });

            }
            else
            {
                return RedirectToRoute("ProviderViewUploads", new { id = id });

            }




        }

        [HttpPost]
        [Route("Admindashboard/Deletedocuments/{id}")]
        public IActionResult DeleteFileById(int id)
        {
            _dashboard.DeleteFile(id);

            int requestid = (int)TempData["requestId"];

            return RedirectToAction("ViewUploads", new { id = requestid });


        }


        [Route("Admindashboard/DeleteAllDoc")]
        [HttpPost]

        public IActionResult DeleteAllFiles(string[] deleteIds)
        {

            var array = JsonConvert.DeserializeObject<string[]>(deleteIds[0]);

            int[] newarray = new int[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                newarray[i] = int.Parse(array[i]);
            }


            _dashboard.DeleteAllFiles(newarray);

            int requestid = (int)TempData["requestId"];

            return RedirectToAction("ViewUploads", new { id = requestid });

        }
        public IActionResult SendDocumentViaMail(string DocFiles, int id)
        {

            dynamic Data = JsonConvert.DeserializeObject(DocFiles);




            bool response = _dashboard.SendDocumentsViaEmail(Data);

            string Confirmationnumber = _dashboard.GetConfirmationNumber(id);

            string UserMail = Data["UserMail"];

            bool reponse1 = _emailService.AddEmailLog("Documents", id, "For Sending Attachments to Patient", UserMail, Confirmationnumber, response);




            if (response == true)
            {
                TempData["ShowPositiveNotification"] = "Documents sent successfully.";

            }
            else
            {
                TempData["ShowNegativeNotification"] = "Somthing went wrong!";
            }

            return RedirectToAction("ViewUploads", new { id = id });

        }
        #endregion




        #region TransferCaseActions
        public IActionResult GetTransferCaseView(string id)
        {
            string token = HttpContext.Request.Cookies["jwt"];

            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired)
            {
                return Json(new { code = 401 });
            }
            else
            {


                List<Region> regions = _dashboard.GetAllRegions();

                List<Physician> physicians = _dashboard.GetAllPhysician();

                AdminAssignCase adminAssignCase = new AdminAssignCase()
                {
                    Regions = regions,
                    Physicians = physicians,
                    RequestId = int.Parse(id),

                };

                return PartialView("_TransferCaseModal", adminAssignCase);
            }


        }

        public IActionResult PostTransferCase(AdminAssignCase adminAssignCase, int requestId)
        {

            string token = HttpContext.Request.Cookies["jwt"];

            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired)
            {

                return RedirectToAction("Login", "Account", new { message = "You need to Login!" });
            }
            else if (ModelState.IsValid)
            {
                LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

                bool response = _dashboard.TransferRequest(adminAssignCase, requestId, loggedInUser.UserId);

                if (response)
                {
                    TempData["ShowPositiveNotification"] = "Request Transfered Successfully.";

                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Something Went wrong!";

                }
                return RedirectToAction("Dashboard", "Admindashboard");

            }
            else
            {
                TempData["ShowNegativeNotification"] = "Not Valid Data!";
                return RedirectToAction("Dashboard", "Admindashboard");
            }
        }

        #endregion


        #region ClearCaseActions
        [HttpPost]
        public IActionResult GetClearCaseModal(string id)
        {
            string token = HttpContext.Request.Cookies["jwt"];

            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired)
            {
                return Json(new { code = 401 });
            }
            else
            {

                AdminAssignCase adminAssignCase = new AdminAssignCase()
                {

                    RequestId = int.Parse(id),

                };

                return PartialView("_ClearCaseModal", adminAssignCase);
            }
        }

        [HttpPost]
        public IActionResult ClearRequest(string requestId)
        {
            string token = HttpContext.Request.Cookies["jwt"];
            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired)
            {
                return Json(new { code = 401 });
            }
            else
            {
                LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

                bool response = _dashboard.ClearCaseByRequestid(requestId, loggedInUser.UserId);

                if (response == true)
                {

                    TempData["ShowPositiveNotification"] = "Request Cleared Successfully.";
                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Something went wrong!";
                }

                return RedirectToAction("Dashboard");

            }

        }
        #endregion


        #region AgreementActions
        [HttpGet]
        [Route("Reviewagreement/{id}")]
        public IActionResult ReviewAgreement(int id)
        {


             

            return View();

        }

        public IActionResult GetCancleAgreementPopup(string id)
        {

            CancleAgreement cancleAgreement = new CancleAgreement()
            {
                requestId = id,

            };

            return PartialView("_CancleAgreementModal", cancleAgreement);
        }

        public IActionResult PostCancleAgreement(CancleAgreement cancleAgreement, string requestId)
        {


            string token = HttpContext.Request.Cookies["jwt"];

            LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

            bool response = _dashboard.CancleAgrrementByRequstId(cancleAgreement, requestId,loggedInUser.UserId);

            if (response)
            {
                TempData["ShowPositiveNotification"] = "Agreement canceled successfully.";
            }
            else
            {
                TempData["ShowNegativeNotification"] = "Somthing went wrong!";
            }

            return RedirectToAction("Login", "Account");
        }

        public IActionResult PostAgreeAgrement(string requestId)
        {
            bool response = _dashboard.AgreeAgreementByRequestId(requestId);

            if (response)
            {
                TempData["ShowPositiveNotification"] = "Agreement accepeted successfully.";
            }
            else
            {
                TempData["ShowNegativeNotification"] = "Somthing went wrong!";
            }

            return RedirectToAction("Login", "Account");
        }


        public IActionResult GetSendAgreementModal(string requestId)
        {
            string token = HttpContext.Request.Cookies["jwt"];

            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired)
            {
                return Json(new { code = 401 });
            }
            else
            {
                SendAgreement sendAgreement = _dashboard.GetSendAgreementpopupInfo(requestId);

                return PartialView("_SendAgreementModal", sendAgreement);
            }



        }

        public IActionResult SendAgrrementLink(SendAgreement sendAgreement, string requestId)
        {

            string token = HttpContext.Request.Cookies["jwt"];
            LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);


            string subject = "Service Agreement";

            string link = "https://localhost:7008/Reviewagreement/" + requestId;

            string body = "Please click on <a asp-route-id='" + requestId + "' href='" + link + "'+>Agreement</a> to view agreement ";


            bool response = _emailService.SendEmail(sendAgreement.Email, subject, body);

            if (response)
            {
                TempData["ShowPositiveNotification"] = "Agreement Sent Successfully.";
            }
            else
            {
                TempData["ShowNegativeNotification"] = "Something went wrong!";
            }




            return loggedInUser.Role == "Admin" ? RedirectToAction("Dashboard") : RedirectToAction("Dashboard", "ProviderDashBoard");
        }
        #endregion


        #region SendLinkActions
        public IActionResult GetSendLinkView()
        {
            string token = HttpContext.Request.Cookies["jwt"];

            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired)
            {
                return Json(new { code = 401 });
            }
            else
            {
                return PartialView("_SendLink");
            }


        }

        [HttpPost]
        public IActionResult PostSendLink(SendLink sendLink)
        {
            string token = HttpContext.Request.Cookies["jwt"];

            bool istokenExpired = _account.IsTokenExpired(token);

            LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);
            if (istokenExpired)
            {
                return Json(new { code = 401 });
            }
            else
            {
                if (ModelState.IsValid)
                {
                    string subject = "Request Link";


                    string link = "https://localhost:7008/Home/patientform/";

                    string body = "Dear" + " " + sendLink.Firstname + " " + sendLink.Lastname + ",<br/>";

                    body += "Please click on <a href='" + link + "'+>Request Link</a> to submit request ";


                    bool response = _emailService.SendEmail(sendLink.Email, subject, body);


                    _emailService.AddEmailLog(subject, 0, "Link For Patient Form Requests", sendLink.Email, "", response);

                    if (response)
                    {
                        TempData["ShowPositiveNotification"] = "Link Sent Successfully.";
                    }
                    else
                    {
                        TempData["ShowNegativeNotification"] = "Somthing went wrong!";
                    }
                    if (loggedInUser.Role == "Admin")
                    {
                        return RedirectToAction("Dashboard");
                    }
                    else
                    {
                        return RedirectToAction("Dashboard", "ProviderDashboard");

                    }

                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Not Valid Information!";
                }

                if (loggedInUser.Role == "Admin")
                {
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    return RedirectToAction("Dashboard", "ProviderDashboard");

                }
            }


        }
        #endregion


        #region EncounterFormActions
        [HttpGet]
        [Route("Admin/encounterform/{id}", Name = "AdminEncounterForm")]
        [Route("Provider/encounterform/{id}", Name = "ProviderEncounterForm")]
        public IActionResult EncounterForm(int id)
        {

            string token = HttpContext.Request.Cookies["jwt"];

            LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

            if (loggedInUser.Role != "Physician"   &&  _authManager.Authorize(HttpContext, 9) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }


            if (loggedInUser.Role == "Admin")
            {
                if (_dashboard.IsRequestExist(id) == false)
                {

                    return RedirectToAction("Error", "Account");
                }
                ViewData["RoleName"] = "Adm";
            }
            else if (loggedInUser.Role == "Physician")
            {
                if (_provider.IsRequestBelongToProvider(loggedInUser.UserId, id) == false)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                ViewData["RoleName"] = "Phy";
            }


            Encounterform encounterform1 = _dashboard.GetEncounterFormByRequestId(id);

            if (encounterform1 == null)
            {
                encounterform1 = new Encounterform();
                encounterform1.Requestid = id;

            }

            return View(encounterform1);
        }

        [HttpPost]

        public IActionResult SaveEncounterForm(Encounterform encounterform, string requestId)
        {

            string token = HttpContext.Request.Cookies["jwt"];

            LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

            encounterform.Requestid = int.Parse(requestId);

            bool response = _dashboard.SaveEncounterForm(encounterform);

            if (response)
            {
                TempData["ShowPositiveNotification"] = "Data Saved Successfully.";
            }
            else
            {
                TempData["ShowNegativeNotification"] = "Something went wrong!";
            }

            if (loggedInUser.Role == "Admin")
            {
                return RedirectToRoute("AdminEncounterForm", new { id = requestId });
            }
            else
            {
                return RedirectToRoute("ProviderEncounterForm", new { id = requestId });
            }



        }


        [HttpPost]
        public IActionResult DownLoadForm(int requestid)
        {


            Encounterform encounterForm = _dashboard.GetEncounterFormByRequestId(requestid);

            byte[] pdfBytes = _dashboard.GeneratePdf(encounterForm);

            return File(pdfBytes, "application/pdf", "encounterform.pdf");

        }


        public IActionResult GetEncounterFormStatus(string requestId)
        {
            int newid = int.Parse(requestId);

            bool? response = _dashboard.IsEncounterFormFinlized(newid);
            int status = _dashboard.GetRequestStatusByRequestId(newid);

            return Json(new { isfinelized = response, status = status });
        }

        #endregion


        #region CloseCaseActions
        [HttpGet]
        [Route("closecase/{id}")]
        public IActionResult CloseCase(string id)
        {


            if (_authManager.Authorize(HttpContext, 9) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (_dashboard.IsRequestExist(int.Parse(id)) == false)
            {

                return RedirectToAction("Error", "Account");
            }

            CLoseCase closeCase = _dashboard.GetDataForCloseCaseByRequestId(id);
            closeCase.requestId = id;
            return View(closeCase);
        }

        [HttpPost]
        public IActionResult SaveCloseCaseData(CLoseCase closeCase, string requestId)
        {

            string token = HttpContext.Request.Cookies["jwt"];

            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired)
            {
                TempData["ShowNegativeNotification"] = "Session timed out!";
                return RedirectToAction("Login", "Account");
            }
            else
            {
                closeCase.requestId = requestId;
                bool response = _dashboard.SaveDataForCloseState(closeCase);

                if (response)
                {
                    TempData["ShowPositiveNotification"] = "Data Saved Successfully.";
                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Data Not Saved!";
                }
                return RedirectToAction("CloseCase", new { id = requestId });
            }

        }

        [HttpPost]

        public IActionResult PostCloseCase(string requestId)
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
                int newid = int.Parse(requestId);


                LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

                bool response = _dashboard.CloseCaseByRequestId(newid, loggedInUser.UserId);

                if (response)
                {
                    TempData["ShowPositiveNotification"] = "Request Closed Successfully.";
                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Something Went  Wrong!";
                }

                return RedirectToAction("Dashboard");
            }

        }

        #endregion

        #region PartnersPageActions

        [HttpGet]
        [Route("Admin/sendorder/{id}", Name = "AdminSendOrder")]
        [Route("Provider/sendorder/{id}", Name = "ProviderSendOrder")]
        public IActionResult SendOrder(int id)
        {

            string token = HttpContext.Request.Cookies["jwt"];

            bool istokenExpired = _account.IsTokenExpired(token);

                LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);
            if (istokenExpired)
            {
                TempData["ShowNegativeNotification"] = "You need to login!";
                return RedirectToAction("Login", "Account");
            }
          

            if (loggedInUser.Role != "Physician" && _authManager.Authorize(HttpContext, 12) == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (_dashboard.IsRequestExist(id) == false)
            {

                return RedirectToAction("Error", "Account");
            }


                if (loggedInUser.Role == "Physician")
                {
                    if (_provider.IsRequestBelongToProvider(loggedInUser.UserId, id) == false)
                    {
                        return RedirectToAction("AccessDenied", "Account");
                    }
                    ViewData["RoleName"] = "Phy";
                }
                else if (loggedInUser.Role == "Admin")
                {
                    ViewData["RoleName"] = "Adm";
                }

                List<Healthprofessionaltype> healthprofessionaltypes = _dashboard.GetOrderDetails();
                Order order = new Order();
                order.healthprofessionaltypes = healthprofessionaltypes;
                order.RequestId = id.ToString();

                return View(order);
            
        }


        [HttpPost]
        public IActionResult PostOrder(Order order, string id)
        {
            string token = HttpContext.Request.Cookies["jwt"];


            if (ModelState.IsValid)
            {

                int requestId = int.Parse(id);
                LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

                bool response = _dashboard.PostOrderById(requestId, order, loggedInUser.Role, loggedInUser.UserId);


                if (response == true)
                {
                    TempData["ShowPositiveNotification"] = "Order Sent Succesfully.";


                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Something Went Wrong!";


                }
                if (loggedInUser.Role == "Physician")
                {
                    return RedirectToRoute("ProviderSendOrder", new { id = id });
                }
                else
                {
                    return RedirectToRoute("AdminSendOrder", new { id = id });

                }

            }
            else
            {

                List<Healthprofessionaltype> healthprofessionaltypes = _dashboard.GetOrderDetails();
                order.healthprofessionaltypes = healthprofessionaltypes;
                return View("SendOrder", order);

            }

        }

        public IActionResult GetVendorsByProfession(int id)
        {



           

            List<Healthprofessional> healthprofessionals = _dashboard.GetHealthProfessionalsByProfessionId(id);

            string response = JsonConvert.SerializeObject(healthprofessionals);

            return Json(response);


        }

        public IActionResult GetVendorDetails(int id)
        {
            string token = HttpContext.Request.Cookies["jwt"];

            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired)
            {
                return Json(new { code = 401 });
            }
            else
            {
                if (id == 0)
                {

                    return Json(new { code = 403, msg = "Please Select Vendor!" });
                }
                else
                {

                    



                    Healthprofessional healthprofessional = _dashboard.GetVendorByVendorId(id);

                    return Json(healthprofessional);
                }
            }
        }
        #endregion

        #region RequestByAdminAndProviderActions
        [HttpGet]
        [Route("Admin/reqbyadmin", Name = "AdminPatientReq")]
        [Route("Provider/reqbyprovider", Name = "ProviderPatientReq")]
        public IActionResult PatientReqByAdmin()
        {
            string token = HttpContext.Request.Cookies["jwt"];

            if(string.IsNullOrEmpty(token))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

            List<Region> regions = _account.GetAllRegions();

            PatientReqByAdminModal patientReqByAdminModal = new PatientReqByAdminModal();

            patientReqByAdminModal.regions = regions;

            if (loggedInUser.Role == "Admin")
            {
                ViewData["RoleName"] = "Adm";
            }
            else if (loggedInUser.Role == "Physician")
            {

                ViewData["RoleName"] = "Phy";
            }

            return View("CreateRequestbyAdmin", patientReqByAdminModal);
        }

        [HttpPost]
        public IActionResult PatientFormByAdmin(PatientReqByAdminModal patientReqByAdmin)
        {

            string token = HttpContext.Request.Cookies["jwt"];

            bool istokenExpired = _account.IsTokenExpired(token);
            LoggedInUser loggedInUser = new LoggedInUser();

            if (istokenExpired || token.IsNullOrEmpty())
            {
                TempData["ShowNegativeNotification"] = "Session timed out!";
                return RedirectToAction("Login", "Account");
            }
            else if (ModelState.IsValid)
            {
                loggedInUser = _account.GetLoggedInUserFromJwt(token);

                bool isemailblocked = _patientReq.IsEmailBlocked(patientReqByAdmin.Email);
                bool IsPhoneBlocked = _patientReq.IsPhoneBlocked(patientReqByAdmin.Phonenumber);
                bool IsregionAvailable = _patientReq.IsRegionAvailable(patientReqByAdmin.Location.State);
                bool isEmailExists = _patientReq.IsEmailExistance(patientReqByAdmin.Email);



                if (isemailblocked == true || IsPhoneBlocked || IsregionAvailable == false || isEmailExists == false)
                {


                    if (isEmailExists == false)
                    {
                        TempData["ShowNegativeNotification"] = "Account does not exist!";

                    }
                    else if(isemailblocked){

                    TempData["ShowNegativeNotification"] = "Account with this email is blocked.";
                    }
                   else if (IsPhoneBlocked)
                    {
                        TempData["ShowNegativeNotification"] = "Account With This Number Is Blocked.";
                    }

                  else  if (IsregionAvailable == false)
                    {
                        TempData["ShowNegativeNotification"] = "Region is not available.";
                    }

                    return loggedInUser.Role == "Admin" ? RedirectToRoute("AdminPatientReq") : RedirectToRoute("ProviderPatientReq");
                }



                CmnInformation patientInfo = new CmnInformation
                {
                    FirstName = patientReqByAdmin.FirstName,
                    LastName = patientReqByAdmin.LastName,
                    Email = patientReqByAdmin.Email,
                    PhoneNumber = patientReqByAdmin.Phonenumber
                };

                AddressModel address = new AddressModel()
                {
                    City = patientReqByAdmin.Location.City,
                    State = patientReqByAdmin.Location.State,
                    Street = patientReqByAdmin.Location.Street,
                    ZipCode = patientReqByAdmin.Location.ZipCode,
                    RoomNo = patientReqByAdmin.Location.RoomNo
                };
                PatientReq pr = new PatientReq()
                {
                    FirstName = patientReqByAdmin.FirstName,
                    LastName = patientReqByAdmin.LastName,
                    Email = patientReqByAdmin.Email,
                    Phonenumber = patientReqByAdmin.Phonenumber,
                    BirthDate = patientReqByAdmin.BirthDate,
                    Location = address,
                    Symptoms = patientReqByAdmin.Notes
                };


                Request patientRequest = _patientReq.AddRequest(patientInfo, loggedInUser.UserId, "Patient", patientReqByAdmin.Location.State, loggedInUser.Role);

                bool response = _patientReq.AddRequestClient(pr, patientRequest.Requestid, patientReqByAdmin.Location);

                bool response1 = _dashboard.SaveNotesChanges(patientReqByAdmin.Notes, patientRequest.Requestid, "Admin", loggedInUser.UserId);

                if (response == false || patientRequest == null)
                {
                    TempData["ShowNegativeNotification"] = "Something Went  Wrong!";
                }
                else
                {
                    TempData["ShowPositiveNotification"] = "Request Submited Successfully.";
                }
                if (loggedInUser.Role == "Admin")
                {
                    return RedirectToAction("Dashboard", "Admindashboard");

                }
                else
                {
                    return RedirectToAction("Dashboard", "ProviderDashboard");
                }


            }
            else
            {
                return loggedInUser.Role == "Admin" ? RedirectToRoute("AdminPatientReq") : RedirectToRoute("ProviderPatientReq");
            }
        }
        #endregion

        #region ExportExcelActions
        public IActionResult ExportDataAsExcelFile(string type, string[] StatusArray, string region, string Name)
        {

            string token = HttpContext.Request.Cookies["jwt"];

            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired)
            {
                return Json(new { code = 401 });
            }
            else
            {

                int IntType = 0;
                if (type != null)
                {
                    IntType = int.Parse(type);
                }

                List<DashboardRequests> dashboardRequests = _dashboard.GetRequestsFromRequestorType(IntType, StatusArray, region, Name);


                if (dashboardRequests.Count == 0)
                {
                    TempData["ShowNegativeNotification"] = "No data found for exporting!";
                    return RedirectToAction("Dashboard");
                }

                string response = _dashboard.GetExcelFile(dashboardRequests);

                if (response != "")
                {
                    return Json(new { Url = response });
                }
                else
                {
                    return Json(new { code = 403 });
                }
                //bool response = _dashboard.ExportExcelForCurrentPage(dashboardRequests);


            }
        }

        public IActionResult GetExcelRecordStatusWise(string[] StatusArray)
        {
            string token = HttpContext.Request.Cookies["jwt"];

            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired)
            {
                TempData["ShowNegativeNotification"] = "You need to login!";
                return Json(new { code = 401 });
            }
            else
            {
                List<DashboardRequests> dashboardRequests = _dashboard.GetStatuswiseRequests(StatusArray);

                if (dashboardRequests.Count == 0)
                {
                    TempData["ShowNegativeNotification"] = "No data found for exporting!";
                    return RedirectToAction("Dashboard");
                }

                string response = _dashboard.GetExcelFile(dashboardRequests);


                if (response != "")
                {
                    return Json(new { Url = response });
                }
                else
                {
                    return Json(new { code = 403 });
                }


            }
        }

        #endregion





    }
}
