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




        public IActionResult FormByPatient()
        {

            return View();
        }






        [HttpPost]
        [ValidateAntiForgeryToken]



        public IActionResult FormByPatient(PatientReq pr)
        {


           _patientReq.AddPatientReq(pr);

            return RedirectToAction("Login");

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