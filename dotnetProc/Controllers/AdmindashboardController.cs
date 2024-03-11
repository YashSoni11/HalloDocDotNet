﻿using Microsoft.AspNetCore.Mvc;
using HalloDoc_BAL.Interface;
using HalloDoc_DAL.Models;
using HalloDoc_DAL.ViewModels;
using HalloDoc_DAL.AdminViewModels;
using Newtonsoft.Json;
using HalloDoc_BAL.Repositery;
using System.IdentityModel.Tokens.Jwt;
using System.Globalization;



namespace dotnetProc.Controllers
{
    public class AdmindashboardController : Controller
    {



        private readonly IAdmindashboard _dashboard;
        private readonly IAccount _account;
        private readonly IEmailService _emailService;

        public AdmindashboardController(IAdmindashboard dashboard, IAccount account, IEmailService emailService)
        {
            _dashboard = dashboard;
            _account = account;
            _emailService = emailService;
        }


        public IActionResult Index()
        {
            return View();
        }



        [AuthManager("Admin")]
        [HttpGet]
        [Route("Admindashboard/Dashboard")]
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
                TempData["UserName"] = adminname;

                List<DashboardRequests> requests = _dashboard.GetAllRequests();
                RequestTypeCounts requestTypeCounts = _dashboard.GetAllRequestsCount(requests);
                List<Region> regions = _dashboard.GetAllRegions();

                AdminDashboard adminDashboard = new AdminDashboard
                {
                    Requests = requests,
                    RequestTypeCounts = requestTypeCounts,
                    Regions = regions,
                };




                return View(adminDashboard);
            }
        }


        [HttpPost]
        [Route("Admindashboard/Dashboard")]

        public IActionResult Dashboard(AdminDashboard adminDashboard)
        {


            if (adminDashboard.Searchstring != null)
            {
                List<DashboardRequests> requests = _dashboard.GetRequestsByUsername(adminDashboard.Searchstring);

                RequestTypeCounts requestTypeCounts = _dashboard.GetAllRequestsCount(requests);


                adminDashboard.Requests = requests;
                adminDashboard.RequestTypeCounts = requestTypeCounts;

                return View(adminDashboard);
            }
            else
            {
                List<DashboardRequests> requests = _dashboard.GetAllRequests();
                RequestTypeCounts requestTypeCounts = _dashboard.GetAllRequestsCount(requests);


                adminDashboard.Requests = requests;
                adminDashboard.RequestTypeCounts = requestTypeCounts;


                return View(adminDashboard);

            }



        }


        public IActionResult GetStatuswiseRequests(string[] StatusArray)
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
                List<DashboardRequests> dashboardRequests = _dashboard.GetStatuswiseRequests(StatusArray);

                AdminDashboard adminDashboard = new AdminDashboard { Requests = dashboardRequests };

                return PartialView("_Requeststable", adminDashboard);

            }


        }


        public IActionResult GetRequestorTypeWiseRequests(string type, string[] StatusArray, string region, string Name)
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

                AdminDashboard adminDashboard = new AdminDashboard { Requests = dashboardRequests };

                return PartialView("_Requeststable", adminDashboard);
            }
        }



        //[HttpPost]


        //public IActionResult GetRequestClientInfoFromRequestId(string id)
        //{

        //    int newrequestid = int.Parse(id);

        //    ClientRequest requestclient = _dashboard.GetUserInfoFromRequestId(newrequestid);


        //    return PartialView("_ViewRequest", requestclient);

        //}



        public IActionResult GetRequestNotes(string id)
        {
            int newrequestid = int.Parse(id);

            RequestNotes requestNotes = _dashboard.GetNotesFromRequestId(newrequestid);

            AdminDashboard adminDashboard = new AdminDashboard
            {
                requestnotes = requestNotes
            };

            return PartialView("_ViewNotes", adminDashboard);
        }

        [AuthManager("Admin")]
        [HttpGet]
        [Route("Admindashboard/Viewrequest/{requestid}")]
        public IActionResult ViewRequest(string requestid)
        {


            int newrequestid = int.Parse(requestid);

            ClientRequest requestclient = _dashboard.GetUserInfoFromRequestId(newrequestid);

            return View(requestclient);

        }

        [HttpPost]
        [Route("Admindashboard/Viewrequest/{requestid}")]
        public IActionResult ViewRequest(ClientRequest clientRequest, string requestid)
        {
            int newrequestid = int.Parse(requestid);

            ClientRequest clientRequest1 = _dashboard.UpdateClientRequest(clientRequest, newrequestid);

            return View(clientRequest1);
        }


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

                AdminCancleCase adminCancleCase = new AdminCancleCase
                {
                    PatientName = name,
                    requestId = newrequestid,
                };

                return PartialView("_CancleCaseModel", adminCancleCase);
            }
        }

        public IActionResult PostCancleRequest(AdminCancleCase adminCancleCase, int requestId)
        {
            string token = HttpContext.Request.Cookies["jwt"];

            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired)
            {
                //TempData["ShowNegativeNotification"] = "You need to login!";

                return RedirectToAction("Login", "Account", new { message = "You need to Login!" });
            }
            else
            {


                Request request = _dashboard.UpdateRequestToClose(adminCancleCase, requestId);

                return RedirectToAction("Dashboard", "Admindashboard");
            }
        }

        public IActionResult GetPhysicianByRegion(int regionId)
        {

            List<Physician> physicians = _dashboard.FilterPhysicianByRegion(regionId);


            string response = JsonConvert.SerializeObject(physicians);

            return Json(response);

        }

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
            Request request = _dashboard.AssignRequest(adminAssignCase, requestId);

            return RedirectToAction("Dashboard", "Admindashboard");
        }


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

            Request request = _dashboard.BlockRequest(adminBlockCase, requestId);

            return RedirectToAction("Dashboard", "Admindashboard");
        }

        [Route("Admindashboard/Viewdocuments/{id}")]
        [HttpGet]
        public IActionResult ViewUploads(int id)
        {
            string token = HttpContext.Request.Cookies["jwt"];

            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired)
            {
                TempData["ShowNegativeNotification"] = "You need to login!";
                return RedirectToAction("Login", "Account");
            }
            else
            {

                TempData["requestId"] = id;

                List<ViewDocument> docs = _dashboard.GetDocumentsByRequestId(id);


                Documents documents = new Documents
                {
                    //requestId = id,
                    ViewDocuments = docs,
                    FormFile = null
                };


                return View(documents);
            }
        }

        [Route("Admindashboard/Uploaddocuments/{id}")]
        [HttpPost]
        public IActionResult UploadDocuments(Documents docs, int id)
        {

            //string path = HttpContext.Request.Path;

            //string[] paths = path.Split('/');

            //int requestId = int.Parse(paths[paths.Length - 1]);



            bool response = true;

            for (int i = 0; i < docs.FormFile.Count; i++)
            {

                response &= _account.UploadFile(docs.FormFile[i], id);

            }



            return RedirectToAction("ViewUploads", new { id = id });


        }

        [Route("Admindashboard/Deletedocuments/{id}")]

        [HttpPost]
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


            //_dashboard.DeleteAllFiles(newarray);

            int requestid = (int)TempData["requestId"];

            return RedirectToAction("ViewUploads", new { id = requestid });

        }

        [Route("Admindashboard/sendorder/{id}")]
        [HttpGet]
        public IActionResult SendOrder(string id)
        {
            string token = HttpContext.Request.Cookies["jwt"];

            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired)
            {
                TempData["ShowNegativeNotification"] = "You need to login!";
                return RedirectToAction("Login", "Account");
            }
            else
            {


                List<Healthprofessionaltype> healthprofessionaltypes = _dashboard.GetOrderDetails();
                Order order = new Order();
                order.healthprofessionaltypes = healthprofessionaltypes;
                order.RequestId = id;

                return View(order);
            }
        }


        [Route("Admindashboard/postorder/{id}")]
        [HttpPost]

        public IActionResult PostOrder(Order order, string id)
        {

            if (ModelState.IsValid)
            {

                int requestId = int.Parse(id);

                bool response = _dashboard.PostOrderById(requestId, order);


                if (response == true)
                {
                    TempData["ShowPositiveNotification"] = "Order Sent Succesfully.";


                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Something Went Wrong!";


                }
                return RedirectToAction("Dashboard");
            }
            else
            {

                List<Healthprofessionaltype> healthprofessionaltypes = _dashboard.GetOrderDetails();
                order.healthprofessionaltypes = healthprofessionaltypes;
                return View("SendOrder", order);

            }

        }

        public IActionResult GetVendorsByProfession(string id)
        {

            int newid = int.Parse(id);

            List<Healthprofessional> healthprofessionals = _dashboard.GetHealthProfessionalsByProfessionId(newid);

            string response = JsonConvert.SerializeObject(healthprofessionals);

            return Json(response);


        }

        public IActionResult GetVendorDetails(string id)
        {
            int newid = int.Parse(id);

            Healthprofessional healthprofessional = _dashboard.GetVendorByVendorId(newid);

            return Json(healthprofessional);
        }



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

            LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

            Request request = _dashboard.TransferRequest(adminAssignCase, requestId, loggedInUser.UserId);

            if (request != null)
            {

                TempData["ShowPositiveNotification"] = "Request Transfered Successfully.";
            }
            else
            {
                TempData["ShowNegativeNotification"] = "Something Went wrong!";
            }
            return RedirectToAction("Dashboard", "Admindashboard");
        }


        public IActionResult SendDocumentViaMail(string DocFiles, string id)
        {

            dynamic Data = JsonConvert.DeserializeObject(DocFiles);



            bool response = _dashboard.SendDocumentsViaEmail(Data);


            if (response == true)
            {
                TempData["ShowPositiveNotification"] = "Documents sent successfully.";

            }
            else
            {
                TempData["ShowNegativeNotification"] = "Somthing went wrong!";
            }

            return RedirectToAction("ViewUploads", new { id = int.Parse(id) });

        }

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

        [HttpGet]
        [Route("Reviewagreement/{id}")]
        public IActionResult ReviewAgreement(string id)
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

        public IActionResult PostCancleAgreement(CancleAgreement cancleAgreement,string requestId)
        {
            bool response = _dashboard.CancleAgrrementByRequstId(cancleAgreement,requestId);

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

        public IActionResult SendAgrrementLink(SendAgreement sendAgreement,string requestId)
        {

            string subject = "Service Agreement";

            string link = "https://localhost:7008/Reviewagreement/" + requestId;

            string body = "Please click on <a asp-route-id='" + requestId + "' href='" + link + "'+>Agreement</a> to view agreement ";


           bool  response =  _emailService.SendEmail(sendAgreement.Email, subject, body);

            if(response)
            {
                TempData["ShowPositiveNotification"] = "Agreement Sent Successfully.";
            }
            else
            {
                TempData["ShowNegativeNotification"] = "Something went wrong!";
            }

            return RedirectToAction("Dashboard");
        }


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

            if (istokenExpired)
            {
                return Json(new { code = 401 });
            }
            else 
            { 
                 if(ModelState.IsValid)
                {
                    string subject = "Request Link";
                  

                    string link = "https://localhost:7008/Home/patientform/";

                    string body = "Dear" + " " + sendLink.Firstname + " " + sendLink.Lastname + ",/n";

                    body += "Please click on <a href='" + link + "'+>Agreement</a> to submit request ";


                    bool response = _emailService.SendEmail(sendLink.Email, subject, body);

                    if(response)
                    {
                        TempData["ShowPositiveNotifiction"] = "Link Sent Successfully.";
                    }
                    else
                    {
                        TempData["ShowNegativeNotification"] = "Somthing went wrong!";
                    }

                    return RedirectToAction("Dashboard");
                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Not Valid Information!";
                }

                    return RedirectToAction("Dashboard");
            }


        }


    }
}
