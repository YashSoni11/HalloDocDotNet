﻿using Microsoft.AspNetCore.Mvc;
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
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace dotnetProc.Controllers
{
    public class AdmindashboardController : Controller
    {



        private readonly IAdmindashboard _dashboard;
        private readonly IAccount _account;
        private readonly IEmailService _emailService;
        private readonly IPatientReq _patientReq;

        public AdmindashboardController(IAdmindashboard dashboard, IAccount account, IEmailService emailService, IPatientReq patientReq)
        {
            _dashboard = dashboard;
            _account = account;
            _emailService = emailService;
            _patientReq = patientReq;
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

        public IActionResult PostCancleAgreement(CancleAgreement cancleAgreement, string requestId)
        {
            bool response = _dashboard.CancleAgrrementByRequstId(cancleAgreement, requestId);

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
                if (ModelState.IsValid)
                {
                    string subject = "Request Link";


                    string link = "https://localhost:7008/Home/patientform/";

                    string body = "Dear" + " " + sendLink.Firstname + " " + sendLink.Lastname + ",<br/>";

                    body += "Please click on <a href='" + link + "'+>Request Link</a> to submit request ";


                    bool response = _emailService.SendEmail(sendLink.Email, subject, body);

                    if (response)
                    {
                        TempData["ShowPositiveNotification"] = "Link Sent Successfully.";
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

        [HttpGet]
        [Route("Admindashboard/encounterform/{id}")]
        public IActionResult EncounterForm(string id)
        {

            Encounterform encounterform1 = _dashboard.GetEncounterFormByRequestId(id);

            if (encounterform1 == null)
            {
                encounterform1 = new Encounterform();
                encounterform1.Requestid = int.Parse(id);

            }

            return View(encounterform1);
        }

        [HttpPost]

        public IActionResult SaveEncounterForm(Encounterform encounterform, string requestId)
        {


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

            return RedirectToAction("EncounterForm", new { id = requestId });
        }


        [HttpPost]
        public IActionResult DownLoadForm(string requestid)
        {


            Encounterform encounterForm = _dashboard.GetEncounterFormByRequestId(requestid);

            byte[] pdfBytes = _dashboard.GeneratePdf(encounterForm);

            return File(pdfBytes, "application/pdf", "encounterform.pdf");

        }


        public IActionResult GetEncounterFormStatus(string requestId)
        {
            int newid = int.Parse(requestId);

            bool? response = _dashboard.IsEncounterFormFinlized(newid);

            return Json(new { isfinelized = response });
        }

        [AuthManager("Admin")]
        [HttpGet]
        [Route("closecase/{id}")]

        public IActionResult CloseCase(string id)
        {

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

        [AuthManager("Admin")]
        [HttpGet]
        [Route("Admindashboard/reqbyadmin")]
        public IActionResult PatientReqByAdmin()
        {
            return View("CreateRequestbyAdmin");
        }

        [HttpPost]
        public IActionResult PatientFormByAdmin(PatientReqByAdmin patientReqByAdmin)
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

             
                bool isemailblocked = _patientReq.IsEmailBlocked(patientReqByAdmin.Email);
                bool IsPhoneBlocked = _patientReq.IsPhoneBlocked(patientReqByAdmin.Phonenumber);
                bool IsregionAvailable = _patientReq.IsRegionAvailable(patientReqByAdmin.Location.State);



                if (isemailblocked == true || IsPhoneBlocked || IsregionAvailable == false)
                {

                      TempData["isemailblocked"] = "Account with this email is blocked.";



                    if (IsPhoneBlocked)
                    {
                        TempData["IsPhoneBlocked"] = "Account With This Number Is Blocked.";
                    }

                    if (IsregionAvailable == false)
                    {
                        TempData["IsRegionAvailable"] = "Region is not available.";
                    }

                    return View("CreateRequestbyAdmin", patientReqByAdmin);
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

                LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

                Request patientRequest = _patientReq.AddRequest(patientInfo, loggedInUser.UserId, "Patient", patientReqByAdmin.Location.State);

                bool response = _patientReq.AddRequestClient(pr, patientRequest.Requestid, patientReqByAdmin.Location);



                if (response == false || patientRequest == null)
                {
                    TempData["ShowNegativeNotification"] = "Something Went  Wrong!";
                }
                else
                {
                    TempData["ShowPositiveNotification"] = "Request Submited Successfully.";
                }

                return RedirectToAction("Dashboard", "Admindashboard");


            }
            else
            {
                return RedirectToAction("PatientReqByAdmin");
            }
        }

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


                string[] header = { "Name", "Date of Birth", "Requestor", "Phonenumber", "Address", "Notes" };

                using (var package = new XLWorkbook())
                {

                    var worksheet = package.Worksheets.Add("Sheet1");
                    int row = 1;
                    for (int i = 0; i < header.Length; i++)
                    {
                        worksheet.Cell(row, i + 1).Value = header[i];
                    }

                    row++;
                    int j = 0;
                    foreach (DashboardRequests req in dashboardRequests)
                    {

                        worksheet.Cell(row, j + 1).Value = req.Username;

                        j++;

                        worksheet.Cell(row, j + 1).Value = req.Birthdate;

                        j++;

                        worksheet.Cell(row, j + 1)  .Value = req.Requestor;
                        j++;
                        worksheet.Cell(row, j + 1).Value = req.Phone + "/" + req.RequestorPhone;
                        j++;
                        worksheet.Cell(row, j + 1).Value = req.Address;
                        j++;
                        worksheet.Cell(row, j + 1).Value = req.Notes;

                        j = 0;
                        row++;




                    }

                    byte[] fileBytes;
                    using (var stream = new MemoryStream())
                    {
                        package.SaveAs(stream);
                        stream.Seek(0, SeekOrigin.Begin);

                        fileBytes = stream.ToArray();
                        string fileName = Guid.NewGuid().ToString() + ".xlsx";
                        string filePath =  Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory())) + "\\wwwroot\\Upload";

                        string path = Path.Combine(filePath, fileName); 

                        System.IO.File.WriteAllBytes(path, fileBytes);
                       
                        string fileUrl = path;

                        return Json(new { Url = "https://localhost:7008/Upload/"+fileName });
                  
                    }
                }

                //bool response = _dashboard.ExportExcelForCurrentPage(dashboardRequests);

                
            }
        }

        [AuthManager("Admin")]
        [HttpGet]
        [Route("myprofile")]
        public IActionResult AdminProfile()
        {

            string token = HttpContext.Request.Cookies["jwt"];

            LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

            AdminProfile adminProfile = _dashboard.GetAdminProfileData(loggedInUser.UserId);


            return View("Adminprofile",adminProfile);  
        }

        [HttpPost]
        public IActionResult ResetAdminPassword(string AdminPassword)
        {
            string token = HttpContext.Request.Cookies["jwt"];

            bool istokenExpired = _account.IsTokenExpired(token);

            if (istokenExpired || token.IsNullOrEmpty())
            {
                TempData["ShowNegativeNotification"] = "Session timed out!";
                return RedirectToAction("Login", "Account");
            }
            else if(AdminPassword.IsNullOrEmpty())
            {
                TempData["ShowNegativeNotification"] = "Not Valid Password!";
                return RedirectToAction("AdminProfile");

            }
            else
            {
                LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

                string hashedPassword = _account.GetHashedPassword(AdminPassword); 

                bool response = _dashboard.ResetAdminPassword(loggedInUser.UserId,hashedPassword);

                if (response)
                {
                    TempData["ShowPositiveNotification"] = "Password Changed Successfully.";
                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Somthing went wrong!";
                }

                return RedirectToAction("AdminProfile");
            }
        }

        [HttpPost]
        public IActionResult SaveAdminAccountInformation(AdminProfile ap)
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
                LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

                bool response = _dashboard.SaveAdminAccountInfo(ap,loggedInUser.UserId);

                if(response)
                {
                    TempData["ShowPositiveNotification"] = "Account Information Saved Successfully.";

                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Data Not Saved!";

                }
                return RedirectToAction("AdminProfile");


            }
            else
            {
                TempData["ShowNegativeNotification"] = "Somthing went wrong!";

                return RedirectToAction("AdminProfile");
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
            else if (ModelState.IsValid)
            {
                LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

                bool response = _dashboard.SaveAdminMailingAndBillingInfo(ap, loggedInUser.UserId);

                if (response)
               {
                    TempData["ShowPositiveNotification"] = "Mailing & Billig Info Saved Successfully.";

                }
                else
                {
                    TempData["ShowNegativeNotification"] = "Data Not Saved!";

                }
                return RedirectToAction("AdminProfile");


            }
            else
            {
                TempData["ShowNegativeNotification"] = "Somthing went wrong!";

                return RedirectToAction("AdminProfile");
            }
        }
    }
}
