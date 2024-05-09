using AutoMapper;
using HalloDoc_BAL.Interface;
using HalloDoc_DAL.Context;
using HalloDoc_DAL.InvoicingViewModels;
using HalloDoc_DAL.Models;
using Microsoft.AspNetCore.Http;
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

        public bool UploadBill(IFormFile file, int TimeSheetId)
        {

            string path = "";
            bool iscopied = false;

           

          


            try
            {
                if (file.Length > 0)
                {
                    string filename = file.FileName;
                    path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory())) + "\\wwwroot\\Bills\\" + TimeSheetId.ToString();

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    using (var filestream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }
                   
                    iscopied = true;
                }
                else
                {
                    iscopied = false;
                }

               
            }
            catch (Exception)
            {
                throw;
            }
            return iscopied;



        }

        public TimeSheet GetTimeSheetDetailsList(int Physicianid,DateTime StartDate)
        {

            DateOnly date = new DateOnly(StartDate.Year,StartDate.Month,StartDate.Day);

            int timeSheetId = _context.Timesheets.Where(q => q.Physicianid == Physicianid && q.Startdate == date).Select(q=>q.Timesheetid).FirstOrDefault();

            List<Timesheetdetail> timesheetdetails = _context.Timesheetdetails.OrderBy(q=>q.Timesheetdate).Where(q => q.Timesheetid == timeSheetId).ToList();
            List<Timesheetdetailreimbursement> timesheetdetailreimbursements = new List<Timesheetdetailreimbursement>();

            

            if(timesheetdetails.Count == 0)
            {

                int currentMonth = StartDate.Month;

                int count = 0;



                if (StartDate.Day == 1)
                {
                    count = 14;
                }
                else
                {


                    if (new DateTime(StartDate.Year, currentMonth, 15).AddDays(16).Month == currentMonth)
                    {
                        count = 16;
                    }
                    else if (new DateTime(StartDate.Year, currentMonth, 15).AddDays(15).Month == currentMonth)
                    {
                        count = 15;


                    }
                    else if (new DateTime(StartDate.Year, currentMonth, 15).AddDays(14).Month == currentMonth)
                    {
                        count = 14;


                    }
                    else if (new DateTime(StartDate.Year, currentMonth, 15).AddDays(13).Month == currentMonth)
                    {
                        count = 13;

                    }
                }




                

                for (int i = 1; i <= count; i++)
                {
                    Timesheetdetail detail = new Timesheetdetail();
                    detail.Totalhours = 0;
                    timesheetdetails.Add(detail);
                }

            }
           


            foreach(Timesheetdetail timesheetdetail in timesheetdetails)
            {
                Timesheetdetailreimbursement timesheetdetailreimbursement = _context.Timesheetdetailreimbursements.FirstOrDefault(q => q.Timesheetdetailid == timesheetdetail.Timesheetdetailid);

                if(timesheetdetailreimbursement == null)
                {
                    timesheetdetailreimbursement= new Timesheetdetailreimbursement();
                    timesheetdetailreimbursement.Timesheetdetailid = timesheetdetail.Timesheetdetailid;

                }

                timesheetdetailreimbursements.Add(timesheetdetailreimbursement);
            }

            TimeSheet timeSheet = new TimeSheet();
            timeSheet.TimeSheetDetails = timesheetdetails;
            timeSheet.timesheetdetailreimbursements = timesheetdetailreimbursements;
            timeSheet.TimeSheetStartDate = StartDate;

            return timeSheet;

        }

        public bool SaveTimeSheetDetails(TimeSheet timeSheetDetails,int UserId)
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

                        Timesheetdetailreimbursement timesheetdetailreimbursement = _context.Timesheetdetailreimbursements.FirstOrDefault(q => q.Timesheetdetailid == timeSheetDetails.TimeSheetDetails[i].Timesheetdetailid);

                        if(timesheetdetailreimbursement == null)
                        {
                            timesheetdetailreimbursement= new Timesheetdetailreimbursement();
                            timesheetdetailreimbursement.Timesheetdetailid = timeSheetDetails.timesheetdetailreimbursements[i].Timesheetdetailid;
                            timesheetdetailreimbursement.Itemname = timeSheetDetails.timesheetdetailreimbursements[i].Itemname;
                            timesheetdetailreimbursement.Amount = timeSheetDetails.timesheetdetailreimbursements[i].Amount;
                            timesheetdetailreimbursement.Bill = timeSheetDetails.timesheetdetailreimbursements[i].Bill;
     

                        }

                    }




                }
                else
                {
                    Timesheet timeSheet = new Timesheet();

                    timeSheet.Startdate = new DateOnly(timeSheetDetails.TimeSheetStartDate.Year, timeSheetDetails.TimeSheetStartDate.Month, timeSheetDetails.TimeSheetStartDate.Day);
                    timeSheet.Createddate = DateTime.Now;
                    timeSheet.Physicianid = UserId;
                    timeSheet.Createdby = _context.Physicians.Where(q => q.Physicianid == UserId).Select(q => q.Aspnetuserid).FirstOrDefault();
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
                        timesheetdetail.Timesheetdate = new DateOnly(timeSheet.Startdate.Year, timeSheet.Startdate.Month, timeSheet.Startdate.Day + i);
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
