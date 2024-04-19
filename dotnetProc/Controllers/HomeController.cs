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

        #region Phone,Email,Region,ExistActions
        public IActionResult CheckNumberAvailibility(string Phone)
        {

            bool IsPhoneBlocked = _patientReq.IsPhoneBlocked(Phone);

            return Json(new {IsPhone = IsPhoneBlocked,ModalMsg="Request With Provided Phonenumber Is Blocked!"});
        }


        public IActionResult CheckRegionAvailibility(int region)
        {
            bool isExists = _patientReq.IsRegionAvailable(region);

      

            return Json(new { Response = isExists,NegativeMsg= "Currently We Are Not Servicing In your Provided Region !",PositiveMsg="Location is available." });
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
        #endregion


        #region RequestTypeMenuActions
        public IActionResult PatientRequest()
        {

            return View();
        }
        #endregion


        #region FormByPatientActions
        [HttpGet]
        [Route("Home/patientform")]
        public IActionResult FormByPatient()
        {



            List<Region> regions = _account.GetAllRegions();

            PatientReq patientReq = new PatientReq();
            patientReq.regions = regions;

            return View(patientReq);
        }


        [HttpPost]
   
        public IActionResult PostFormByPatient(PatientReqModal pr)
        {


             bool isemailexist = _patientReq.IsEmailExistance(pr.patientReq.Email);
            bool isemailblocked = _patientReq.IsEmailBlocked(pr.patientReq.Email);
            bool IsPhoneBlocked = _patientReq.IsPhoneBlocked(pr.patientReq.Phonenumber);
            bool IsregionAvailable = _patientReq.IsRegionAvailable(pr.patientReq.Location.State);

           
            
            if(isemailblocked == true || isemailblocked == true || IsPhoneBlocked || IsregionAvailable == false)
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

                return RedirectToAction("FormByPatient");
            }
            else  if (ModelState.IsValid)
            {


            int user = _patientReq.GetUserIdByEmail(pr.patientReq.Email);

            if(user == 0)
            {
                Aspnetuser asp1 = _patientReq.AddAspNetUser(pr.patientReq, pr.Password);

                user = _patientReq.AddUser(asp1.Id, pr.patientReq, pr.patientReq.Location);
            }

            CmnInformation patientInfo = new CmnInformation
            {
                FirstName = pr.patientReq.FirstName,
                LastName = pr.patientReq.LastName,
                Email = pr.patientReq.Email,
                PhoneNumber = pr.patientReq.Phonenumber
            };


            Request patientRequest = _patientReq.AddRequest(patientInfo, user, "Patient",pr.patientReq.Location.State, "Patient");

            bool response =  _patientReq.AddRequestClient(pr.patientReq, patientRequest.Requestid,pr.patientReq.Location);

            bool response1 = _patientReq.UploadFile(pr.patientReq.FileUpload, patientRequest.Requestid);


            return RedirectToAction("Login","Account");
            }
            else
            {
               return RedirectToAction("FormByPatient");
                
            }
        }
        #endregion


        #region FormByFamilyActions
        [HttpGet]
        public IActionResult FormByFamily()
        {

            List<Region> regions = _account.GetAllRegions();

            PatientReq patientReq = new PatientReq();
            patientReq.regions = regions;

            FamilyFriendModel model = new FamilyFriendModel();
            model.PatientInfo = patientReq;

            return View(model);
        }

        [HttpPost]
        public IActionResult FormByFamily(FamilyFriendModel familyFriendModel)
        {


            bool isemailexist = _patientReq.IsEmailExistance(familyFriendModel.PatientInfo.Email);
            bool isemailblocked = _patientReq.IsEmailBlocked(familyFriendModel.PatientInfo.Email);
            bool IsPhoneBlocked = _patientReq.IsPhoneBlocked(familyFriendModel.PatientInfo.Phonenumber);
            bool IsregionAvailable = _patientReq.IsRegionAvailable(familyFriendModel.PatientInfo.Location.State);



            if (isemailblocked == true || isemailblocked == true || IsPhoneBlocked || IsregionAvailable == false)
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
                return RedirectToAction("FormByFamily");

            }

            else if (ModelState.IsValid)
            {

         

            Request patientRequest = _patientReq.AddRequest(familyFriendModel.FamilyFriendsIfo, 0,"Family",familyFriendModel.PatientInfo.Location.State, "Patient" );

            bool response = _patientReq.AddRequestClient(familyFriendModel.PatientInfo, patientRequest.Requestid, familyFriendModel.PatientInfo.Location);


            bool response1 = _patientReq.UploadFile(familyFriendModel.PatientInfo.FileUpload,patientRequest.Requestid);


               

                bool IsSent = _emailService.SendCreateAccountLink(familyFriendModel.PatientInfo.Email,patientRequest.Requestid);


                return RedirectToAction("Login", "Account");
            }

            return RedirectToAction("FormByFamily");


        }
        #endregion


        #region FormByConcieargeActions
        [HttpGet]
        public IActionResult FormByConciearge()
        {
            List<Region> regions = _account.GetAllRegions();

            PatientReq patientReq = new PatientReq();
            patientReq.regions = regions;

            ConcieargeModel model = new ConcieargeModel();
            model.PatinentInfo = patientReq;


            return View(model);
        }

        [HttpPost]
        public IActionResult FormByConciearge(ConcieargeModel concieargeModel)
        {
            bool isemailexist = _patientReq.IsEmailExistance(concieargeModel.PatinentInfo.Email);
            bool isemailblocked = _patientReq.IsEmailBlocked(concieargeModel.PatinentInfo.Email);
            bool IsPhoneBlocked = _patientReq.IsPhoneBlocked(concieargeModel.PatinentInfo.Phonenumber);
            bool IsregionAvailable = _patientReq.IsRegionAvailable(concieargeModel.concieargeLocation.State);

            if (isemailblocked == true || isemailblocked == true || IsPhoneBlocked || IsregionAvailable == false)
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

                return RedirectToAction("FormByConciearge");
            }

            else if( (!ModelState.IsValid && concieargeModel.PatinentInfo.Location == null) ||  ModelState.IsValid)
            {


                Request patientRequest = _patientReq.AddRequest(concieargeModel.concieargeInformation, 0, "Concierge",concieargeModel.concieargeLocation.State, "Patient");
            Concierge concierge = _patientReq.Addconciearge(concieargeModel.concieargeLocation, concieargeModel.concieargeInformation.FirstName);

            bool response = _patientReq.AddRequestClient(concieargeModel.PatinentInfo, patientRequest.Requestid, concieargeModel.concieargeLocation);


            bool response2 = _patientReq.AddConciergeRequest(concierge.Conciergeid, patientRequest.Requestid);


                bool IsSent = _emailService.SendCreateAccountLink(concieargeModel.PatinentInfo.Email,patientRequest.Requestid);


                return RedirectToAction("Login", "Account");
            }

            return RedirectToAction("FormByConciearge");

        }
        #endregion


        #region FormByBusinessActions
        [HttpGet]
        public IActionResult FormByBusinessPartner()
        {


            List<Region> regions = _account.GetAllRegions();

            PatientReq patientReq = new PatientReq();
            patientReq.regions = regions;

            BusinessReqModel model = new BusinessReqModel();
            model.PatientIfo = patientReq;


            return View(model);
        }


        [HttpPost]
        public IActionResult FormByBusinessPartner(BusinessReqModel businessReqModel)
        {
            bool isemailexist = _patientReq.IsEmailExistance(businessReqModel.PatientIfo.Email);
            bool isemailblocked = _patientReq.IsEmailBlocked(businessReqModel.PatientIfo.Email);
            bool IsPhoneBlocked = _patientReq.IsPhoneBlocked(businessReqModel.PatientIfo.Phonenumber);
            bool IsregionAvailable = _patientReq.IsRegionAvailable(businessReqModel.PatinentLocaiton.State);

            if (isemailblocked == true || isemailblocked == true || IsPhoneBlocked || IsregionAvailable == false)
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

                return RedirectToAction("FormByBusinessPartner");
            }
            else if (ModelState.IsValid)
            {

            

            Request patientRequest = _patientReq.AddRequest(businessReqModel.BusinessInfo, 0, "Business",businessReqModel.PatinentLocaiton.State, "Patient");

            bool response = _patientReq.AddRequestClient(businessReqModel.PatientIfo, patientRequest.Requestid, businessReqModel.PatinentLocaiton);

            Business business = _patientReq.AddBusiness(businessReqModel.BusinessInfo, businessReqModel.PatinentLocaiton);

            bool response2 = _patientReq.AddBusinessRequest(business.Businessid, patientRequest.Requestid);

                bool IsSent = _emailService.SendCreateAccountLink(businessReqModel.PatientIfo.Email, patientRequest.Requestid);


                return RedirectToAction("Login", "Account");
            }

            return RedirectToAction("FormByBusinessPartner");

        }
        #endregion


        #region FormForPatientActions
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
        public IActionResult PostPatientForm(PatientReq pr)
        {
            bool isemailexist = _patientReq.IsEmailExistance(pr.Email);
            bool isemailblocked = _patientReq.IsEmailBlocked(pr.Email);
            bool IsPhoneBlocked = _patientReq.IsPhoneBlocked(pr.Phonenumber);
            bool IsregionAvailable = _patientReq.IsRegionAvailable(pr.Location.State);



            if (isemailblocked == true || isemailblocked == true || IsPhoneBlocked || IsregionAvailable == false)
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

                return RedirectToAction("PatientForm");
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


            Request patientRequest = _patientReq.AddRequest(patientInfo, loggedInUser.UserId, "Patient", pr.Location.State, "Patient");

            bool response = _patientReq.AddRequestClient(pr, patientRequest.Requestid, pr.Location);

            bool response1 = _patientReq.UploadFile(pr.FileUpload, patientRequest.Requestid);

            return RedirectToAction("DashBoard", "Account");
            }

            return RedirectToAction("PatientForm");

        }
        #endregion


        #region FormForOthersActions
        [HttpGet]
        [Route("/Account/DashBoard/FormForOther")]
        public IActionResult FormForOthers()
        {


            List<Region> regions = _account.GetAllRegions();

            PatientReq patientReq = new PatientReq();
            patientReq.regions = regions;


            return View(patientReq);
        }

        [HttpPost]
        [Route("/Account/DashBoard/FormForOther")]
        public IActionResult PostFormForOthers(PatientReq pr)
        {
            bool isemailexist = _patientReq.IsEmailExistance(pr.Email);
            bool isemailblocked = _patientReq.IsEmailBlocked(pr.Email);
            bool IsPhoneBlocked = _patientReq.IsPhoneBlocked(pr.Phonenumber);
            bool IsregionAvailable = _patientReq.IsRegionAvailable(pr.Location.State);



            if (isemailblocked == true || isemailblocked == true || IsPhoneBlocked || IsregionAvailable == false)
            {

                TempData["ShowNegativeNotification"] = isemailexist == true ? "Account with this email already created." : TempData["ShowNegativeNotification"] = "Account with this email is blocked.";



                if (IsPhoneBlocked)
                {
                    TempData["ShowNegativeNotification"] = "Account With This Number Is Blocked.";
                }

                if (IsregionAvailable == false)
                {
                    TempData["ShowNegativeNotification"] = "Region is not available.";
                }

                return RedirectToAction("FormForOthers");
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


            Request patientRequest = _patientReq.AddRequest(patientInfo, loggedInUser.UserId, pr.Relation,pr.Location.State,"Patient");

            bool response = _patientReq.AddRequestClient(pr, patientRequest.Requestid, pr.Location);


            bool response1 = _patientReq.UploadFile(pr.FileUpload, patientRequest.Requestid);


                _emailService.SendCreateAccountLink(pr.Email, patientRequest.Requestid);

            return RedirectToAction("DashBoard","Account");

            }

            return RedirectToAction("Dashboard","Account");

        }
        #endregion

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