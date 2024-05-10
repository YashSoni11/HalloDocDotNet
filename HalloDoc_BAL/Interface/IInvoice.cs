using HalloDoc_DAL.InvoicingViewModels;
using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.Interface
{
    public interface IInvoice
    {

        public bool SaveTimeSheetDetails(TimeSheet timeSheetDetails,int UserId);

        public TimeSheet GetTimeSheetDetailsList(int Physicianid, DateTime StartDate);

        public bool SaveTimeSheetReimbursmentDetails(TimeSheet timeSheetDetails, int UserId);

        public List<ShiftTimeSheets> GetShiftTimeSheetsDetails(DateTime StartDate)



    }
}
