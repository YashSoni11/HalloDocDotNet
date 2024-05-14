using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HalloDoc_DAL.InvoicingViewModels
{
    public class TimeSheet
    {

        public DateTime TimeSheetStartDate { get; set; }

        public List<Timesheetdetail>? TimeSheetDetails { get; set; }

        public  List<TimeSheetDetailReimbursement> timesheetdetailreimbursements { get; set; }
        



    }

    public class TimeSheetDetailReimbursement
    {
     
        public int? Timesheetdetailreimbursementid { get; set; }

        public int? TimeSheetId { get; set; }   

       
        public int? Timesheetdetailid { get; set; }

        public DateOnly Date { get; set; }

     
        public string? Itemname { get; set; } = null!;

     
        public int? Amount { get; set; }

       
        public IFormFile? Bill { get; set; }

        public string? BillName { get; set; }

      
        public bool? Isdeleted { get; set; }
    }


    public class ShiftTimeSheetsModel
    {
        public DateTime StartDate { get; set; }


        public List<ShiftTimeSheet> ShiftTimeSheets { get; set; }


    }


    public class TimeSheetReibursmentModel
    {
        public DateTime StartDate { get; set; }

        public int TimeSheetId { get; set; }
        public int currentPage { get; set; }

        public int TotalPages { get; set; }

        public List<TimeSheetDetailReimbursement> timeSheetDetailReimbursements { get; set; }

    }



    public class ShiftTimeSheet
    {
        public DateTime ShiftDate { get; set; }

        public int TimeSheetDetailId { get; set; }

        public int ShiftNo { get; set; }

        public int NightShiftWeekend { get; set; }

        public int HouseCall { get; set; }

        public int HouseCallNightsWeekend { get; set; }

        public int PhoneConsults { get; set; }

        public int PhoneConsultNightsWeekend { get; set; }


        public int BatchTesing { get; set; }
    }



    public class AdminTimeSheetModel
    {

        public List<Physician> physicians { get; set; }

         

    }



    public class PendingTimeSheetModel
    {

        public DateTime StartDate { get; set; }

        public DateTime EndTime { get; set; }

        public int TimeSheetId { get; set; }

        public bool IsFinelized { get; set; }

        public bool IsApproved { get; set; }

        public string PhysicianName { get;set; }
    }

}
