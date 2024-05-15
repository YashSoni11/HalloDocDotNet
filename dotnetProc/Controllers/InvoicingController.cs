using HalloDoc_BAL.Interface;
using HalloDoc_DAL.InvoicingViewModels;
using HalloDoc_DAL.Models;
using HalloDoc_DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace dotnetProc.Controllers
{
    public class InvoicingController : Controller
    {


        private readonly IAdmindashboard _dashboard;
        private readonly IAccount _account;
        private readonly IEmailService _emailService;
        private readonly IPatientReq _patientReq;
        private readonly IProvider _provider;
        private readonly IAuthManager _authManager;
        private readonly IInvoice _invoice;

        public InvoicingController(IInvoice invoice, IAdmindashboard dashboard, IAccount account, IAuthManager authManager, IEmailService emailService, IPatientReq patientReq, IProvider provider)
        {
            _dashboard = dashboard;
            _account = account;
            _emailService = emailService;
            _patientReq = patientReq;
            _provider = provider;
            _authManager = authManager;
            _invoice = invoice;
        }



        [HttpGet]
        public IActionResult ProviderInvoicingView()
        {



            return View("ProviderInvoicing");
        }


        public IActionResult GetTimeSheetView(DateTime TimeSheetDate)
        {

            string token = HttpContext.Request.Cookies["jwt"];
            LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

            TimeSheet timeSheet = new TimeSheet();

            if (TempData.ContainsKey("TimeSheetTime"))
            {
                timeSheet.TimeSheetStartDate = (DateTime)TempData["TimeSheetTime"];
            }
            else
            {

            timeSheet.TimeSheetStartDate = TimeSheetDate;
            }

           

           timeSheet = _invoice.GetTimeSheetDetailsList(loggedInUser.UserId, timeSheet.TimeSheetStartDate);

            return View("TimeSheets", timeSheet);

        }


        public IActionResult SaveTimeSheetDetails(TimeSheet timeSheet)
        {


            string token = HttpContext.Request.Cookies["jwt"];
            LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

            bool response = _invoice.SaveTimeSheetDetails(timeSheet,loggedInUser.UserId);

            if (response)
            {
                TempData["ShowPositiveNotification"] = "Timesheetdetails saved successfully.";
            }
            else
            {
                TempData["ShowNegativeNotification"] = "Somthing went wrong!";
            }

            TempData["TimeSheetTime"] = timeSheet.TimeSheetStartDate;




            return loggedInUser.Role == "Admin" ? RedirectToAction("GetAdminTimeSheetView", "Admindashboard", new { TimeSheetDate = timeSheet.TimeSheetStartDate}): RedirectToAction("GetTimeSheetView");

        }


        public IActionResult SaveReimbursementDetails(TimeSheet timeSheet)
        {
            string token = HttpContext.Request.Cookies["jwt"];
            LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

            bool response = _invoice.SaveTimeSheetReimbursmentDetails(timeSheet, loggedInUser.UserId);

            if (response)
            {
                TempData["ShowPositiveNotification"] = "Reimbursmentdetails saved successfully.";
            }
            else
            {
                TempData["ShowNegativeNotification"] = "Somthing went wrong!";
            }

            TempData["TimeSheetTime"] = timeSheet.TimeSheetStartDate;

            return RedirectToAction("GetTimeSheetView");

        }


  

        public IActionResult GetShiftTimeSheetsDetails(DateTime StartDate)
        {
            string token = HttpContext.Request.Cookies["jwt"];
            LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);


            ShiftTimeSheetsModel model = _invoice.GetShiftTimeSheetsDetails(loggedInUser.UserId,StartDate);
            model.StartDate = StartDate;    
          

            return PartialView("_TimeSheetDetailsTable", model);


        }


        public IActionResult GetTimeSheetReibursmentDetails(int currentPage,DateTime StartDate)
        {
            string token = HttpContext.Request.Cookies["jwt"];
            LoggedInUser loggedInUser = _account.GetLoggedInUserFromJwt(token);

            TimeSheetReibursmentModel model = _invoice.GetTimeSheetReimbursmentDetails(loggedInUser.UserId, StartDate);

            if(model.timeSheetDetailReimbursements == null)
            {
                model.TotalPages = 0;

            }
            else
            {


            model.TotalPages = (int)Math.Ceiling((double)model.timeSheetDetailReimbursements.Count / 1);
            model.timeSheetDetailReimbursements = model.timeSheetDetailReimbursements.Skip(1 * (currentPage - 1)).Take(1).ToList();
            }
            model.currentPage = currentPage;
       

            return PartialView("_TimeSheetReibursmentTable", model);
        }



   


        public IActionResult GetAdminTimeSheetTableView(int physicianId,DateTime StartDate)
        {


             PendingTimeSheetModel pendingTimeSheetModels = _invoice.GetPendingTimeSheets(physicianId, StartDate);


            if(pendingTimeSheetModels != null && pendingTimeSheetModels.IsApproved == true && pendingTimeSheetModels.IsFinelized == true)
            {

                return Json(new { IsApproved = true });
            }
            else 
            {
                return PartialView("_PendingTimeSheetTable", pendingTimeSheetModels);
            }
           
        }

    }
}
