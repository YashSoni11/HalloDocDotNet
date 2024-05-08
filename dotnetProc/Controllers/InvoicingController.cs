using HalloDoc_BAL.Interface;
using HalloDoc_DAL.InvoicingViewModels;
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

        public InvoicingController(IAdmindashboard dashboard, IAccount account, IAuthManager authManager, IEmailService emailService, IPatientReq patientReq, IProvider provider)
        {
            _dashboard = dashboard;
            _account = account;
            _emailService = emailService;
            _patientReq = patientReq;
            _provider = provider;
            _authManager = authManager;
        }



        [HttpGet]
        public IActionResult ProviderInvoicingView()
        {



            return View("ProviderInvoicing");
        }


        public IActionResult GetTimeSheetView(DateTime TimeSheetDate)
        {



            TimeSheet timeSheet = new TimeSheet();
            timeSheet.TimeSheetStartDate = TimeSheetDate;

            return View("TimeSheets", timeSheet);
             
        }


        public IActionResult SaveTimeSheetDetails(TimeSheet timeSheet)
        {

             bool response = 

        }

    }
}
