using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_DAL.InvoicingViewModels
{
    public class TimeSheet
    {

        public DateTime TimeSheetStartDate { get; set; }

        public List<Timesheetdetail>? TimeSheetDetails { get; set; }



    }
}
