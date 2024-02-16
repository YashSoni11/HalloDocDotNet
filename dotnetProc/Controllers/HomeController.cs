using HalloDoc_BAL.Interface;
using HalloDoc_DAL.Context;
using HalloDoc_DAL.Models;
using HalloDoc_DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;



namespace dotnetProc.Controllers
{
    public class HomeController : Controller
    {
        

        private readonly IPatientReq _patientReq;


        public HomeController(IPatientReq patientReq)
        {
            _patientReq = patientReq;
        }

       

        public IActionResult Index()
        {
            return View();
        }

     
        public  IActionResult ckeckEmailAvailibility(string email)
        {

            if(email != null)
            {
                //Aspnetuser exist =  _context.Aspnetusers.FirstOrDefault(u => u.Email == email);

                var exist = _patientReq.CheckEmail(email);

                if(exist != null)
                {
                    
                    return Json(new { Error = "Account Already Exist!" ,code = 401});
                }

            }


            return Json(new { code = 402 });

        }
       


        public IActionResult ForgotPass()
        {
            return View();
        }


        public IActionResult PatientRequest()
        {

            return View();
        }




        [HttpGet]
        public IActionResult FormByPatient()
        {

            return View();
        }






        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FormByPatient(PatientReq pr)
        {


            int user = _patientReq.GetUserIdByEmail(pr.Email);

            if(user == 0)
            {
                Aspnetuser asp1 = _patientReq.AddAspNetUser(pr, pr.Password);

                user = _patientReq.AddUser(asp1.Id, pr, pr.Location);
            }

            CmnInformation patientInfo = new CmnInformation
            {
                FirstName = pr.FirstName,
                LastName = pr.LastName,
                Email = pr.Email,
                PhoneNumber = pr.Phonenumber
            };


            Request patientRequest = _patientReq.AddRequest(patientInfo, user, "Patient");

            bool response =  _patientReq.AddRequestClient(pr, patientRequest.Requestid,pr.Location);

            bool response1 = _patientReq.UploadFile(pr.FileUpload, patientRequest.Requestid);


            return RedirectToAction("Login","Account");

        }




        [HttpGet]
        public IActionResult FormByFamily()
        {

            return View();
        }

        [HttpPost]
        public IActionResult FormByFamily(FamilyFriendModel familyFriendModel)
        {

            int user = _patientReq.GetUserIdByEmail(familyFriendModel.PatientInfo.Email);

            Request patientRequest = _patientReq.AddRequest(familyFriendModel.FamilyFriendsIfo, user,"Family");

            bool response = _patientReq.AddRequestClient(familyFriendModel.PatientInfo, patientRequest.Requestid, familyFriendModel.patientLocation);


            bool response1 = _patientReq.UploadFile(familyFriendModel.PatientInfo.FileUpload,patientRequest.Requestid);

            return RedirectToAction("Login", "Account");

          
        }

        [HttpGet]
        public IActionResult FormByConciearge()
        {

            return View();
        }

        [HttpPost]
        public IActionResult FormByConciearge(ConcieargeModel concieargeModel)
        {

            int user = _patientReq.GetUserIdByEmail(concieargeModel.PatinentInfo.Email);

            Request patientRequest = _patientReq.AddRequest(concieargeModel.concieargeInformation, user, "Concierge");

            bool response = _patientReq.AddRequestClient(concieargeModel.PatinentInfo, patientRequest.Requestid, concieargeModel.concieargeLocation);

            Concierge concierge = _patientReq.Addconciearge(concieargeModel.concieargeLocation, concieargeModel.concieargeInformation.FirstName);

            bool response2 = _patientReq.AddConciergeRequest(concierge.Conciergeid, patientRequest.Requestid);



            return RedirectToAction("Login", "Account");

        }


        [HttpGet]
        public IActionResult FormByBusinessPartner()
        {

            return View();
        }




        [HttpPost]
        public IActionResult FormByBusinessPartner(BusinessReqModel businessReqModel)
        {

            int user = _patientReq.GetUserIdByEmail(businessReqModel.PatientIfo.Email);

            Request patientRequest = _patientReq.AddRequest(businessReqModel.BusinessInfo, user, "Business");

            bool response = _patientReq.AddRequestClient(businessReqModel.PatientIfo, patientRequest.Requestid, businessReqModel.PatinentLocaiton);

            Business business = _patientReq.AddBusiness(businessReqModel.BusinessInfo, businessReqModel.PatinentLocaiton);

            bool response2 = _patientReq.AddBusinessRequest(business.Businessid, patientRequest.Requestid);

            return RedirectToAction("Login", "Account");

        }



        [HttpGet]
        [Route("/Account/DashBoard/PatientForm")]
        public IActionResult PatientForm()
        {

            int userId = (int)HttpContext.Session.GetInt32("LoginId");

            User user = _patientReq.GetUserDataById(userId);

            PatientReq newReq = new PatientReq();

            newReq.FirstName = user.Firstname;
            newReq.LastName = user.Lastname;
            newReq.Email = user.Email;
            newReq.Phonenumber = user.Mobile;



            return View(newReq);
        }



        [HttpPost]
        [Route("/Account/DashBoard/PatientForm")]
        public IActionResult PatientForm(PatientReq pr)
        {

            int userId = (int)HttpContext.Session.GetInt32("LoginId");

            User user = _patientReq.GetUserDataById(userId);

            CmnInformation patientInfo = new CmnInformation
            {
                FirstName = pr.FirstName,
                LastName = pr.LastName,
                Email = pr.Email,
                PhoneNumber = pr.Phonenumber
            };


            Request patientRequest = _patientReq.AddRequest(patientInfo, userId, "Patient");

            bool response = _patientReq.AddRequestClient(pr, patientRequest.Requestid, pr.Location);

            bool response1 = _patientReq.UploadFile(pr.FileUpload, patientRequest.Requestid);




            return RedirectToAction("DashBoard", "Account");
        }







        [HttpGet]
        [Route("/Account/DashBoard/FormForOther")]
        public IActionResult FormForOthers()
        {
            return View();
        }

        [HttpPost]
        [Route("/Account/DashBoard/FormForOther")]
        public IActionResult FormForOthers(PatientReq pr)
        {

            int userId = (int)HttpContext.Session.GetInt32("LoginId");

            User user = _patientReq.GetUserDataById(userId);

            CmnInformation patientInfo = new CmnInformation
            {
                FirstName = user.Firstname,
                LastName = user.Lastname,
                Email = user.Email,
                PhoneNumber = user.Mobile
            };


            Request patientRequest = _patientReq.AddRequest(patientInfo, userId, pr.Relation);

            bool response = _patientReq.AddRequestClient(pr, patientRequest.Requestid, pr.Location);

            bool response1 = _patientReq.UploadFile(pr.FileUpload, patientRequest.Requestid);





            return RedirectToAction("DashBoard","Account");
        }

        public IActionResult Privacy()
        {
            return View();
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}