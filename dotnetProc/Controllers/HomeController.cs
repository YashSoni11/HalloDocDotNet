using HalloDoc_BAL.Interface;
using HalloDoc_DAL.Context;
using HalloDoc_DAL.Models;
using HalloDoc_DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Newtonsoft.Json;
using MailKit.Net.Smtp;
using dotnetProc.Utils;

namespace dotnetProc.Controllers
{
    public class HomeController : Controller
    {
        

        private readonly IPatientReq _patientReq;
        private readonly IEmailService _emailService;
        private readonly IAccount _account;

        public HomeController(IPatientReq patientReq,IEmailService emailService,IAccount account)
        {
            _patientReq = patientReq;
            _emailService = emailService;
            _account = account;
        }

       

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CheckNumberAvailibility(string Phone)
        {

            bool IsPhoneBlocked = _patientReq.IsPhoneBlocked(Phone);

            return Json(new {IsPhone = IsPhoneBlocked,ModalMsg="Request With Provided Phonenumber Is Blocked!"});
        }



        public  IActionResult ckeckEmailAvailibility(string email)
        {

            if(email != null)
            {
       

                bool IsEmailBlocked = _patientReq.IsEmailBlocked(email);

                if (IsEmailBlocked)
                {
                   

                    return Json(new { IsEmailBLocked = true,ModalMsg= "Request With Provided Email Is Blocked !" });
                }

                var exist = _patientReq.CheckEmail(email);




                if(exist != null)
                {
            

                    return Json(new { Error = "Account Already Exist!" ,code = 401});
                }

            }


            return Json(new { code = 402 });

        }
       


      


        public IActionResult PatientRequest()
        {

            return View();
        }




        [HttpGet]
        [Route("Home/patientform")]
        public IActionResult FormByPatient()
        {

            return View();
        }






        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FormByPatient(PatientReq pr)
        {


             bool isemailexist = _patientReq.IsEmailExistance(pr.Email);
            bool isemailblocked = _patientReq.IsEmailBlocked(pr.Email);
            bool IsPhoneBlocked = _patientReq.IsPhoneBlocked(pr.Phonenumber);
            bool IsregionAvailable = _patientReq.IsRegionAvailable(pr.Location.State);

           
            
            if(isemailblocked == true || isemailblocked == true || IsPhoneBlocked || IsregionAvailable)
            {

                TempData["IsEmailExist"] = isemailexist == true?  "Account with this email already created.": TempData["IsEmailExist"] = "Account with this email is blocked.";



                if (IsPhoneBlocked)
                {
                  TempData["IsPhoneBlocked"] = "Account With This Number Is Blocked."; 
                }

                if (IsregionAvailable == false)
                {
                    TempData["IsRegionAvailable"] = "Region is not available.";
                }

                return View(pr);
            }
            else  if (ModelState.IsValid)
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


            Request patientRequest = _patientReq.AddRequest(patientInfo, user, "Patient",pr.Location.State);

            bool response =  _patientReq.AddRequestClient(pr, patientRequest.Requestid,pr.Location);

            bool response1 = _patientReq.UploadFile(pr.FileUpload, patientRequest.Requestid);


            return RedirectToAction("Login","Account");
            }
            else
            {
               return View(pr);
                
            }
        }




        [HttpGet]
        public IActionResult FormByFamily()
        {

            return View();
        }

        [HttpPost]
        public IActionResult FormByFamily(FamilyFriendModel familyFriendModel)
        {


            bool isemailexist = _patientReq.IsEmailExistance(familyFriendModel.PatientInfo.Email);
            bool isemailblocked = _patientReq.IsEmailBlocked(familyFriendModel.PatientInfo.Email);
            bool IsPhoneBlocked = _patientReq.IsPhoneBlocked(familyFriendModel.PatientInfo.Phonenumber);
            bool IsregionAvailable = _patientReq.IsRegionAvailable(familyFriendModel.patientLocation.State);



            if (isemailblocked == true || isemailblocked == true || IsPhoneBlocked || IsregionAvailable)
            {

                TempData["IsEmailExist"] = isemailexist == true ? "Account with this email already created." : TempData["IsEmailExist"] = "Account with this email is blocked.";



                if (IsPhoneBlocked)
                {
                    TempData["IsPhoneBlocked"] = "Account With This Number Is Blocked.";
                }

                if (IsregionAvailable == false)
                {
                    TempData["IsRegionAvailable"] = "Region is not available.";
                }

                return View(familyFriendModel);
            }

           else if (ModelState.IsValid)
            {

            int user = _patientReq.GetUserIdByEmail(familyFriendModel.PatientInfo.Email);

            Request patientRequest = _patientReq.AddRequest(familyFriendModel.FamilyFriendsIfo, user,"Family",familyFriendModel.patientLocation.State);

            bool response = _patientReq.AddRequestClient(familyFriendModel.PatientInfo, patientRequest.Requestid, familyFriendModel.patientLocation);


            bool response1 = _patientReq.UploadFile(familyFriendModel.PatientInfo.FileUpload,patientRequest.Requestid);

            return RedirectToAction("Login", "Account");
            }

            return View(familyFriendModel);
          
        }

        [HttpGet]
        public IActionResult FormByConciearge()
        {

            return View();
        }

        [HttpPost]
        public IActionResult FormByConciearge(ConcieargeModel concieargeModel)
        {
            bool isemailexist = _patientReq.IsEmailExistance(concieargeModel.PatinentInfo.Email);
            bool isemailblocked = _patientReq.IsEmailBlocked(concieargeModel.PatinentInfo.Email);
            bool IsPhoneBlocked = _patientReq.IsPhoneBlocked(concieargeModel.PatinentInfo.Phonenumber);
            bool IsregionAvailable = _patientReq.IsRegionAvailable(concieargeModel.concieargeLocation.State);

            if (isemailblocked == true || isemailblocked == true || IsPhoneBlocked || IsregionAvailable)
            {

                TempData["IsEmailExist"] = isemailexist == true ? "Account with this email already created." : TempData["IsEmailExist"] = "Account with this email is blocked.";



                if (IsPhoneBlocked)
                {
                    TempData["IsPhoneBlocked"] = "Account With This Number Is Blocked.";
                }

                if (IsregionAvailable == false)
                {
                    TempData["IsRegionAvailable"] = "Region is not available.";
                }

                return View(concieargeModel);
            }

            else if(ModelState.IsValid)
            {
                int user = _patientReq.GetUserIdByEmail(concieargeModel.PatinentInfo.Email);

                Request patientRequest = _patientReq.AddRequest(concieargeModel.concieargeInformation, user, "Concierge",concieargeModel.PatinentInfo.Location.State);
            Concierge concierge = _patientReq.Addconciearge(concieargeModel.concieargeLocation, concieargeModel.concieargeInformation.FirstName);

            bool response = _patientReq.AddRequestClient(concieargeModel.PatinentInfo, patientRequest.Requestid, concieargeModel.concieargeLocation);


            bool response2 = _patientReq.AddConciergeRequest(concierge.Conciergeid, patientRequest.Requestid);
       

                string createid = Guid.NewGuid().ToString();



                string subject = "Create Account";

                string creatlink = "https://localhost:7008/CreateAccount/" + createid;

                string body = "Please click on <a asp-route-id='" + createid + "' href='" + creatlink + "'+>Create Account</a> to create your account";


                    //_emailService.SendEmail(concieargeModel.PatinentInfo.Email, subject, body);

            return RedirectToAction("Login", "Account");
            }

            return View(concieargeModel);
        }


        [HttpGet]
        public IActionResult FormByBusinessPartner()
        {

            return View();
        }




        [HttpPost]
        public IActionResult FormByBusinessPartner(BusinessReqModel businessReqModel)
        {
            bool isemailexist = _patientReq.IsEmailExistance(businessReqModel.PatientIfo.Email);
            bool isemailblocked = _patientReq.IsEmailBlocked(businessReqModel.PatientIfo.Email);
            bool IsPhoneBlocked = _patientReq.IsPhoneBlocked(businessReqModel.PatientIfo.Phonenumber);
            bool IsregionAvailable = _patientReq.IsRegionAvailable(businessReqModel.PatinentLocaiton.State);

            if (isemailblocked == true || isemailblocked == true || IsPhoneBlocked || IsregionAvailable)
            {

                TempData["IsEmailExist"] = isemailexist == true ? "Account with this email already created." : TempData["IsEmailExist"] = "Account with this email is blocked.";



                if (IsPhoneBlocked)
                {
                    TempData["IsPhoneBlocked"] = "Account With This Number Is Blocked.";
                }

                if (IsregionAvailable == false)
                {
                    TempData["IsRegionAvailable"] = "Region is not available.";
                }

                return View(businessReqModel);
            }
            else if (ModelState.IsValid)
            {

            int user = _patientReq.GetUserIdByEmail(businessReqModel.PatientIfo.Email);

            Request patientRequest = _patientReq.AddRequest(businessReqModel.BusinessInfo, user, "Business",businessReqModel.PatinentLocaiton.State);

            bool response = _patientReq.AddRequestClient(businessReqModel.PatientIfo, patientRequest.Requestid, businessReqModel.PatinentLocaiton);

            Business business = _patientReq.AddBusiness(businessReqModel.BusinessInfo, businessReqModel.PatinentLocaiton);

            bool response2 = _patientReq.AddBusinessRequest(business.Businessid, patientRequest.Requestid);

            return RedirectToAction("Login", "Account");
            }

            return View(businessReqModel);
        }



        [HttpGet]
        [Route("/Account/DashBoard/PatientForm")]
        public IActionResult PatientForm()
        {
            string token = Request.Cookies["jwt"];


            LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

            int userId = loggedInUser.UserId;

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
            bool isemailexist = _patientReq.IsEmailExistance(pr.Email);
            bool isemailblocked = _patientReq.IsEmailBlocked(pr.Email);
            bool IsPhoneBlocked = _patientReq.IsPhoneBlocked(pr.Phonenumber);
            bool IsregionAvailable = _patientReq.IsRegionAvailable(pr.Location.State);



            if (isemailblocked == true || isemailblocked == true || IsPhoneBlocked || IsregionAvailable)
            {

                TempData["IsEmailExist"] = isemailexist == true ? "Account with this email already created." : TempData["IsEmailExist"] = "Account with this email is blocked.";



                if (IsPhoneBlocked)
                {
                    TempData["IsPhoneBlocked"] = "Account With This Number Is Blocked.";
                }

                if (IsregionAvailable == false)
                {
                    TempData["IsRegionAvailable"] = "Region is not available.";
                }

                return View(pr);
            }
            else if (ModelState.IsValid)
            {

                string token = Request.Cookies["jwt"];

                LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

         

                User user = _patientReq.GetUserDataById(loggedInUser.UserId);

            CmnInformation patientInfo = new CmnInformation
            {
                FirstName = pr.FirstName,
                LastName = pr.LastName,
                Email = pr.Email,
                PhoneNumber = pr.Phonenumber
            };


            Request patientRequest = _patientReq.AddRequest(patientInfo, loggedInUser.UserId, "Patient", pr.Location.State);

            bool response = _patientReq.AddRequestClient(pr, patientRequest.Requestid, pr.Location);

            bool response1 = _patientReq.UploadFile(pr.FileUpload, patientRequest.Requestid);

            return RedirectToAction("DashBoard", "Account");
            }

            return View(pr);
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
            bool isemailexist = _patientReq.IsEmailExistance(pr.Email);
            bool isemailblocked = _patientReq.IsEmailBlocked(pr.Email);
            bool IsPhoneBlocked = _patientReq.IsPhoneBlocked(pr.Phonenumber);
            bool IsregionAvailable = _patientReq.IsRegionAvailable(pr.Location.State);



            if (isemailblocked == true || isemailblocked == true || IsPhoneBlocked || IsregionAvailable)
            {

                TempData["IsEmailExist"] = isemailexist == true ? "Account with this email already created." : TempData["IsEmailExist"] = "Account with this email is blocked.";



                if (IsPhoneBlocked)
                {
                    TempData["IsPhoneBlocked"] = "Account With This Number Is Blocked.";
                }

                if (IsregionAvailable == false)
                {
                    TempData["IsRegionAvailable"] = "Region is not available.";
                }

                return View(pr);
            }
            else if (ModelState.IsValid)
            {
                string token = Request.Cookies["jwt"];

                LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

                User user = _patientReq.GetUserDataById(loggedInUser.UserId);

            CmnInformation patientInfo = new CmnInformation
            {
                FirstName = user.Firstname,
                LastName = user.Lastname,
                Email = user.Email,
                PhoneNumber = user.Mobile
            };


            Request patientRequest = _patientReq.AddRequest(patientInfo, loggedInUser.UserId, pr.Relation,pr.Location.State);

            bool response = _patientReq.AddRequestClient(pr, patientRequest.Requestid, pr.Location);

            bool response1 = _patientReq.UploadFile(pr.FileUpload, patientRequest.Requestid);

            return RedirectToAction("DashBoard","Account");

            }

            return View(pr);

        }

        public IActionResult CheckRegionAvailibility(string region)
        {
            bool isExists = _patientReq.IsRegionAvailable(region);

      

            return Json(new { Response = isExists,ModalMsg= "Currently We Are Not Servicing In your Provided Region !" });
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