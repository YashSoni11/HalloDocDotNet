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

     
        public string? Itemname { get; set; } = null!;

     
        public int? Amount { get; set; }

       
        public IFormFile? Bill { get; set; }

        public string? BillName { get; set; }

      
        public bool? Isdeleted { get; set; }
    }



    public class ShiftTimeSheets
    {
        public DateTime? ShiftDate { get; set; }

        public int ShiftNo { get; set; }

        public int NightShiftWeekend { get; set; }

        public int HouseCall { get; set; }

        public int HouseCallNightsWeekend { get; set; }

        public int PhoneConsults { get; set; }

        public int PhoneConsultNightsWeekend { get; set; }


        public int BatchTesing { get; set; }
    }

}
