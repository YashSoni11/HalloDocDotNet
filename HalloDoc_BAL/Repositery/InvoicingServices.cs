using AutoMapper;
using HalloDoc_BAL.Interface;
using HalloDoc_DAL.Context;
using HalloDoc_DAL.InvoicingViewModels;
using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.Repositery
{
    public class InvoicingServices : IInvoice
    {


        private readonly HalloDocContext _context;


        public InvoicingServices(HalloDocContext context, IMapper mapper)
        {
            _context = context;
        }


        public bool SaveTimeSheetDetails(TimeSheet timeSheetDetails)
        {


            try
            {

                bool IsTimeSheetExists = _context.Timesheets.Any(q => q.Timesheetid == timeSheetDetails.TimeSheetDetails[0].Timesheetid);


                if (IsTimeSheetExists)
                {
                    List<Timesheetdetail> timesheetdetails = _context.Timesheetdetails.OrderBy(q => q.Timesheetdetailid).Where(q => q.Timesheetid == timeSheetDetails.TimeSheetDetails[0].Timesheetid).ToList();

                    for (int i = 0; i < timeSheetDetails.TimeSheetDetails?.Count(); i++)
                    {
                        timesheetdetails[i].Totalhours = timeSheetDetails.TimeSheetDetails[i].Totalhours;
                        timesheetdetails[i].Numberofhousecall = timeSheetDetails.TimeSheetDetails[i].Numberofhousecall;
                        timesheetdetails[i].Numberofphonecall = timeSheetDetails.TimeSheetDetails[i].Numberofphonecall;
                        timesheetdetails[i].Isweekend = timeSheetDetails.TimeSheetDetails[i].Isweekend;

                    }

                }
                else
                {
                    Timesheet timeSheet = new Timesheet();

                    timeSheet.Startdate = new DateOnly(timeSheetDetails.TimeSheetStartDate.Year, timeSheetDetails.TimeSheetStartDate.Month, timeSheetDetails.TimeSheetStartDate.Day);
                    timeSheet.Createddate = DateTime.Now;
                    timeSheet.Isapproved = false;
                    timeSheet.Isfinalize = false;

                    int endDay = 0;
                  


                    int currentMonth = timeSheet.Startdate.Month;




                    if (timeSheet.Startdate.Day == 1)
                    {

                        timeSheet.Enddate = new DateOnly(timeSheetDetails.TimeSheetStartDate.Year, timeSheetDetails.TimeSheetStartDate.Month, 14);

                    }
                    else
                    {


                        if (new DateTime(timeSheet.Startdate.Year, currentMonth, 15).AddDays(16).Month == currentMonth)
                        {
                            timeSheet.Enddate = new DateOnly(timeSheetDetails.TimeSheetStartDate.Year, timeSheetDetails.TimeSheetStartDate.Month, 31);

                        }
                        else if (new DateTime(timeSheet.Startdate.Year, currentMonth, 15).AddDays(15).Month == currentMonth)
                        {
                            timeSheet.Enddate = new DateOnly(timeSheetDetails.TimeSheetStartDate.Year, timeSheetDetails.TimeSheetStartDate.Month, 30);


                        }
                        else if (new DateTime(timeSheet.Startdate.Year, currentMonth, 15).AddDays(14).Month == currentMonth)
                        {
                            timeSheet.Enddate = new DateOnly(timeSheetDetails.TimeSheetStartDate.Year, timeSheetDetails.TimeSheetStartDate.Month, 29);


                        }
                        else if (new DateTime(timeSheet.Startdate.Year, currentMonth, 15).AddDays(13).Month == currentMonth)
                        {
                            timeSheet.Enddate = new DateOnly(timeSheetDetails.TimeSheetStartDate.Year, timeSheetDetails.TimeSheetStartDate.Month, 28);


                        }
                    }


                    _context.Timesheets.Add(timeSheet);
                    _context.SaveChanges();

                    int TimeSheetId = _context.Timesheets.OrderByDescending(q => q.Timesheetid).Select(q => q.Timesheetid).FirstOrDefault();

                    for(int i = 0;i<timeSheetDetails.TimeSheetDetails?.Count;i++)
                    {
                        Timesheetdetail timesheetdetail = new Timesheetdetail();

                        timesheetdetail.Timesheetid = TimeSheetId;
                        timesheetdetail.Totalhours = timeSheetDetails.TimeSheetDetails[i].Totalhours;
                        timesheetdetail.Numberofhousecall = timeSheetDetails.TimeSheetDetails[i].Numberofhousecall;
                        timesheetdetail.Numberofphonecall = timeSheetDetails.TimeSheetDetails[i].Numberofphonecall;
                        timesheetdetail.Isweekend = timeSheetDetails.TimeSheetDetails[i].Isweekend;

                        _context.Timesheetdetails.Add(timesheetdetail);
                    }


                }

                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
