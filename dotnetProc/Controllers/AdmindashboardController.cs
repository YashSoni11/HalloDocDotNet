using Microsoft.AspNetCore.Mvc;
using HalloDoc_BAL.Interface;
using HalloDoc_DAL.Models;
using HalloDoc_DAL.ViewModels;
using HalloDoc_DAL.AdminViewModels;
using Newtonsoft.Json;



namespace dotnetProc.Controllers
{
    public class AdmindashboardController : Controller
    {



        private readonly IAdmindashboard _dashboard;
        private readonly IAccount _account;

         public AdmindashboardController(IAdmindashboard dashboard, IAccount account)
        {
            _dashboard = dashboard;
            _account = account;
        }


        public IActionResult Index()
        {
            return View();
        }




        [HttpGet]
        [Route("Admindashboard/Dashboard")]
        public IActionResult Dashboard()
        {


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

            List<DashboardRequests> dashboardRequests = _dashboard.GetStatuswiseRequests(StatusArray);

            AdminDashboard adminDashboard = new AdminDashboard { Requests = dashboardRequests };

            return PartialView("_Requeststable", adminDashboard);


        }


        public IActionResult GetRequestorTypeWiseRequests(string type, string[] StatusArray, string region, string Name)
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

            int newrequestid = int.Parse(id);

            string name = _dashboard.GetPatientName(newrequestid);

            AdminCancleCase adminCancleCase = new AdminCancleCase
            {
                PatientName = name,
                requestId = newrequestid,
            };

            return PartialView("_CancleCaseModel", adminCancleCase);

        }

        public IActionResult PostCancleRequest(AdminCancleCase adminCancleCase, int requestId)
        {
            Request request = _dashboard.UpdateRequestToClose(adminCancleCase, requestId);

            return RedirectToAction("Dashboard", "Admindashboard");
        }

        public IActionResult GetPhysicianByRegion(int regionId)
        {

            List<Physician> physicians = _dashboard.FilterPhysicianByRegion(regionId);


            string response = JsonConvert.SerializeObject(physicians);

            return Json(response);

        }

        public IActionResult GetAssginCaseView(int requestId)
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


        public IActionResult PostAssignCase(AdminAssignCase adminAssignCase, int requestId)
        {
            Request request = _dashboard.AssignRequest(adminAssignCase, requestId);

            return RedirectToAction("Dashboard", "Admindashboard");
        }


        public IActionResult GetBlockCaseView(AdminBlockCase adminBlockCase, string requestId)
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



        public IActionResult PostBlockCase(AdminBlockCase adminBlockCase, int requestId)
        {

            Request request = _dashboard.BlockRequest(adminBlockCase, requestId);

            return RedirectToAction("Dashboard", "Admindashboard");
        }

        [Route("Admindashboard/Viewdocuments/{id}")]
        [HttpGet]
        public IActionResult ViewUploads(int id)
        {

            TempData["requestId"] = id;

            List<ViewDocument> docs = _dashboard.GetDocumentsByRequestId(id);


            Documents documents = new Documents
            {
                requestId = id,
                ViewDocuments = docs,
                FormFile = null
            };


            return View(documents);
        }

        [Route("Admindashboard/Uploaddocuments/{id}")]
        [HttpPost]
        public IActionResult UploadDocuments(Documents docs,int id)
        {

            //string path = HttpContext.Request.Path;

            //string[] paths = path.Split('/');

            //int requestId = int.Parse(paths[paths.Length - 1]);



            bool response = true;

            for(int i = 0; i < docs.FormFile.Count; i++)
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

            return RedirectToAction("ViewUploads",new { id = requestid });


        }


        [Route("Admindashboard/DeleteAllDoc")]
        [HttpPost]

        public IActionResult DeleteAllFiles(string[] deleteIds)
        {

            var array = JsonConvert.DeserializeObject<string[]>(deleteIds[0]);

            int[] newarray = new int[array.Length];

            for(int i = 0; i < array.Length; i++)
            {
                newarray[i] = int.Parse(array[i]);
            }


            _dashboard.DeleteAllFiles(newarray);

            int requestid = (int)TempData["requestId"];

            return RedirectToAction("ViewUploads", new { id = requestid });

        }

        [Route("Admindashboard/sendorder/{id}")]
        [HttpGet]


        public IActionResult SendOrder()
        {
            return View();
        }


    }
}
