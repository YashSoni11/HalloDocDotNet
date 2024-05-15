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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HalloDoc_BAL.Repositery
{
    public class InvoicingServices : IInvoice
    {


        private readonly HalloDocContext _context;


        public InvoicingServices(HalloDocContext context, IMapper mapper)
        {
            _context = context;
        }

        public bool UploadBill(IFormFile? file, int TimeSheetId)
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
        public AdminTimeSheet GetAdminTimeSheetDetailsList(int Physicianid, DateTime StartDate)
        {

            DateOnly date = new DateOnly(StartDate.Year, StartDate.Month, StartDate.Day);

            int timeSheetId = _context.Timesheets.Where(q => q.Physicianid == Physicianid && q.Startdate == date).Select(q => q.Timesheetid).FirstOrDefault();

            List<Timesheetdetail> timesheetdetails = _context.Timesheetdetails.OrderBy(q => q.Timesheetdate).Where(q => q.Timesheetid == timeSheetId).ToList();
            List<TimeSheetDetailReimbursement> timesheetdetailreimbursements = new List<TimeSheetDetailReimbursement>();



            if (timesheetdetails.Count == 0)
            {

                int currentMonth = StartDate.Month;

                int count = 0;



                 if (StartDate.Day == 1)
                {
                    count = 14;
                }
                else
                {


                    if (new DateTime(StartDate.Year, currentMonth,StartDate.Day).AddDays(16).Month == currentMonth)
                    {
                        count = 16;
                    }
                    else if (new DateTime(StartDate.Year, currentMonth, StartDate.Day).AddDays(15).Month == currentMonth)
                    {
                        count = 15;


                    }
                    else if (new DateTime(StartDate.Year, currentMonth, StartDate.Day).AddDays(14).Month == currentMonth)
                    {
                        count = 14;


                    }
                    else if (new DateTime(StartDate.Year, currentMonth, StartDate.Day).AddDays(13).Month == currentMonth)
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



            foreach (Timesheetdetail timesheetdetail in timesheetdetails)
            {
                TimeSheetDetailReimbursement? timesheetdetailreimbursement = _context.Timesheetdetailreimbursements.Where(q => q.Timesheetdetailid == timesheetdetail.Timesheetdetailid).Select(r => new TimeSheetDetailReimbursement
                {

                    Timesheetdetailid = r.Timesheetdetailid,
                    Timesheetdetailreimbursementid = r.Timesheetdetailreimbursementid,
                    Itemname = r.Itemname,
                    Amount = r.Amount,
                    BillName = r.Bill,
                    TimeSheetId = _context.Timesheetdetails.Where(q => q.Timesheetdetailid == r.Timesheetdetailid).Select(q => q.Timesheetid).FirstOrDefault(),


                }).FirstOrDefault();


                if (timesheetdetailreimbursement == null)
                {
                    timesheetdetailreimbursement = new TimeSheetDetailReimbursement();
                    timesheetdetailreimbursement.Timesheetdetailid = timesheetdetail.Timesheetdetailid;
                    timesheetdetailreimbursement.TimeSheetId = timesheetdetail.Timesheetid;

                }

                timesheetdetailreimbursements.Add(timesheetdetailreimbursement);
            }

            AdminTimeSheet timeSheet = new AdminTimeSheet();
            timeSheet.TimeSheetDetails = timesheetdetails;
            timeSheet.timesheetdetailreimbursements = timesheetdetailreimbursements;
            timeSheet.TimeSheetStartDate = StartDate;




            timeSheet.hoursPayrate = (int)_context.Payratebyproviders.Where(q => q.Payratecategoryid == 2).Select(q => q.Payrate).FirstOrDefault();
            timeSheet.WeekendPayrate = (int)_context.Payratebyproviders.Where(q => q.Payratecategoryid == 1).Select(q => q.Payrate).FirstOrDefault();
            timeSheet.HouseCallPayrate = (int)_context.Payratebyproviders.Where(q => q.Payratecategoryid == 7).Select(q => q.Payrate).FirstOrDefault();
            timeSheet.phoneConsultPayrate = (int)_context.Payratebyproviders.Where(q => q.Payratecategoryid == 4).Select(q => q.Payrate).FirstOrDefault();



            foreach(Timesheetdetail timesheetdetail1 in timeSheet.TimeSheetDetails)
            {


                timeSheet.TotalHoursPay += (int)timesheetdetail1.Totalhours * timeSheet.hoursPayrate;

                if(timesheetdetail1.Numberofhousecall != null)
                timeSheet.TotalHouseCallPay += (int)timesheetdetail1.Numberofhousecall * timeSheet.HouseCallPayrate;

                if(timesheetdetail1.Numberofphonecall != null)
                timeSheet.TotalPhoneCallPay += (int)timesheetdetail1.Numberofphonecall * timeSheet.phoneConsultPayrate;

                if (timesheetdetail1.Isweekend)
                {
                    timeSheet.TotalWeekendPay += timeSheet.WeekendPayrate;
                }
            }

            timeSheet.TotalAmount = timeSheet.TotalHoursPay+timeSheet.TotalPhoneCallPay+timeSheet.TotalHouseCallPay+timeSheet.TotalWeekendPay;

            return timeSheet;

        }

        public TimeSheet GetTimeSheetDetailsList(int Physicianid,DateTime StartDate)
        {

            DateOnly date = new DateOnly(StartDate.Year,StartDate.Month,StartDate.Day);

            int timeSheetId = _context.Timesheets.Where(q => q.Physicianid == Physicianid && q.Startdate == date).Select(q=>q.Timesheetid).FirstOrDefault();

            List<Timesheetdetail> timesheetdetails = _context.Timesheetdetails.OrderBy(q=>q.Timesheetdate).Where(q => q.Timesheetid == timeSheetId).ToList();
            List<TimeSheetDetailReimbursement> timesheetdetailreimbursements = new List<TimeSheetDetailReimbursement>();

            

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
                TimeSheetDetailReimbursement? timesheetdetailreimbursement = _context.Timesheetdetailreimbursements.Where(q => q.Timesheetdetailid == timesheetdetail.Timesheetdetailid).Select(r => new TimeSheetDetailReimbursement { 
                 
                    Timesheetdetailid = r.Timesheetdetailid,    
                    Timesheetdetailreimbursementid = r.Timesheetdetailreimbursementid,
                    Itemname = r.Itemname,
                    Amount = r.Amount,
                    BillName = r.Bill,
                    TimeSheetId = _context.Timesheetdetails.Where(q=>q.Timesheetdetailid == r.Timesheetdetailid).Select(q=>q.Timesheetid).FirstOrDefault(),


                }).FirstOrDefault();


                if (timesheetdetailreimbursement == null)
                {
                    timesheetdetailreimbursement= new TimeSheetDetailReimbursement();
                    timesheetdetailreimbursement.Timesheetdetailid = timesheetdetail.Timesheetdetailid;
                    timesheetdetailreimbursement.TimeSheetId = timesheetdetail.Timesheetid;

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

        public ShiftTimeSheetsModel GetShiftTimeSheetsDetails(int physicianId, DateTime StartDate)
        {

            DateOnly date = new DateOnly(StartDate.Year, StartDate.Month, StartDate.Day);

            int currentMonth = StartDate.Month;

            DateTime endDate;


            if (StartDate.Day == 1)
            {
                endDate = new DateTime(StartDate.Year, StartDate.Month, 14);

            }
            else
            {


                if (new DateTime(StartDate.Year, currentMonth, 15).AddDays(16).Month == currentMonth)
                {
                    endDate = new DateTime(StartDate.Year, StartDate.Month, 31);

                }
                else if (new DateTime(StartDate.Year, currentMonth, 15).AddDays(15).Month == currentMonth)
                {
                    endDate = new DateTime(StartDate.Year, StartDate.Month, 30);


                }
                else if (new DateTime(StartDate.Year, currentMonth, 15).AddDays(14).Month == currentMonth)
                {
                    endDate = new DateTime(StartDate.Year, StartDate.Month, 29);


                }
                else 
                {
                    endDate = new DateTime(StartDate.Year, StartDate.Month, 28);


                }
            }


                List<ShiftTimeSheet> ShiftTimeSheets = new List<ShiftTimeSheet>();    
            Timesheet? timeSheet = _context.Timesheets.FirstOrDefault(q => q.Startdate == date && q.Physicianid == physicianId);

             List<Timesheetdetail> timesheetdetails = _context.Timesheetdetails.OrderBy(q=>q.Timesheetdate).ToList();
           

            if (timeSheet != null)
            {


                for(int i = StartDate.Day; i <= endDate.Day; i++)
                {
                    ShiftTimeSheet shiftTimeSheets = new ShiftTimeSheet();

                    shiftTimeSheets.TimeSheetDetailId = timesheetdetails[i-1].Timesheetdetailid;
                    shiftTimeSheets.ShiftNo = _context.Shiftdetails.Where(q => q.Shiftdate.Day == i && q.Shiftdate.Month == StartDate.Month && q.Shiftdate.Year == StartDate.Year).Count();
                    shiftTimeSheets.ShiftDate = new DateTime(StartDate.Year, StartDate.Month, i);
                    shiftTimeSheets.NightShiftWeekend = 0;
                    shiftTimeSheets.PhoneConsultNightsWeekend = 0;
                    shiftTimeSheets.HouseCallNightsWeekend = 0;
                    shiftTimeSheets.HouseCall = 0;
                    shiftTimeSheets.BatchTesing = 0;
                    shiftTimeSheets.PhoneConsults = 0;


                    ShiftTimeSheets.Add(shiftTimeSheets);
                   
                }


               


            }


            ShiftTimeSheetsModel model = new ShiftTimeSheetsModel();
            model.ShiftTimeSheets = ShiftTimeSheets;
           

            return model;
        }


        public bool SaveTimeSheetReimbursmentDetails(TimeSheet timeSheetDetails, int UserId)
        {

            try
            {
                Timesheetdetailreimbursement timesheetdetailreimbursement = _context.Timesheetdetailreimbursements.FirstOrDefault(q => q.Timesheetdetailreimbursementid == timeSheetDetails.timesheetdetailreimbursements[0].Timesheetdetailreimbursementid);

                if (timesheetdetailreimbursement == null)
                {
                    timesheetdetailreimbursement = new Timesheetdetailreimbursement();
                    timesheetdetailreimbursement.Timesheetdetailid = (int)timeSheetDetails.timesheetdetailreimbursements[0].Timesheetdetailid;
                    timesheetdetailreimbursement.Itemname = timeSheetDetails.timesheetdetailreimbursements[0].Itemname;
                    timesheetdetailreimbursement.Amount = (int)timeSheetDetails.timesheetdetailreimbursements[0].Amount;
                    timesheetdetailreimbursement.Bill = timeSheetDetails.timesheetdetailreimbursements[0].Bill.FileName;

                    timesheetdetailreimbursement.Createdby = _context.Physicians.Where(q => q.Physicianid == UserId).Select(q => q.Aspnetuserid).FirstOrDefault();
                    timesheetdetailreimbursement.Createddate = DateTime.Now;

                    bool IsUploaded = UploadBill(timeSheetDetails.timesheetdetailreimbursements[0].Bill, timeSheetDetails.TimeSheetDetails[0].Timesheetid);

                    if (!IsUploaded)
                        return false;

                    _context.Timesheetdetailreimbursements.Add(timesheetdetailreimbursement);

                }
                else
                {
                    timesheetdetailreimbursement.Timesheetdetailid = (int)timeSheetDetails.timesheetdetailreimbursements[0].Timesheetdetailid;
                    timesheetdetailreimbursement.Itemname = timeSheetDetails.timesheetdetailreimbursements[0].Itemname;
                    timesheetdetailreimbursement.Amount = (int)timeSheetDetails.timesheetdetailreimbursements[0].Amount;
                    timesheetdetailreimbursement.Modifiedby = _context.Physicians.Where(q => q.Physicianid == UserId).Select(q => q.Aspnetuserid).FirstOrDefault();
                    timesheetdetailreimbursement.Modifieddate = DateTime.Now;

                    
                    if(timeSheetDetails.timesheetdetailreimbursements[0].Bill!= null)
                    {

                    timesheetdetailreimbursement.Bill = timeSheetDetails.timesheetdetailreimbursements[0].Bill != null? timeSheetDetails.timesheetdetailreimbursements[0].Bill.FileName:null;
                    bool IsUploaded = UploadBill(timeSheetDetails.timesheetdetailreimbursements[0].Bill, (int)timeSheetDetails.timesheetdetailreimbursements[0].TimeSheetId);
                    if (!IsUploaded)
                        return false;
                    }


                    _context.Timesheetdetailreimbursements.Update(timesheetdetailreimbursement);


                }

                _context.SaveChanges();

                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }



        public TimeSheetReibursmentModel GetTimeSheetReimbursmentDetails(int physicianId, DateTime StartDate)
        {


            DateOnly date = new DateOnly(StartDate.Year, StartDate.Month, StartDate.Day);


            Timesheet? timeSheet = _context.Timesheets.FirstOrDefault(q => q.Startdate == date && q.Physicianid == physicianId);



            TimeSheetReibursmentModel model = new TimeSheetReibursmentModel();

            if(timeSheet != null)


            {
            model.TimeSheetId = timeSheet.Timesheetid;

            List<Timesheetdetail> timesheetdetails = _context.Timesheetdetails.OrderBy(q=>q.Timesheetdate).Where(q => q.Timesheetid == timeSheet.Timesheetid).ToList();

            List<TimeSheetDetailReimbursement> timeSheetDetailReimbursements = new List<TimeSheetDetailReimbursement>();

            foreach(Timesheetdetail timesheetdetail in timesheetdetails)
            {

                TimeSheetDetailReimbursement? timesheetdetailreimbursement = _context.Timesheetdetailreimbursements.Where(q => q.Timesheetdetailid == timesheetdetail.Timesheetdetailid).Select(q => new TimeSheetDetailReimbursement
                {
                    Timesheetdetailid = q.Timesheetdetailid,
                    Timesheetdetailreimbursementid = q.Timesheetdetailreimbursementid,
                    Itemname = q.Itemname,
                    Date = timesheetdetail.Timesheetdate,
                    Amount = q.Amount,
                    BillName = q.Bill
                }).FirstOrDefault();

                timeSheetDetailReimbursements.Add(timesheetdetailreimbursement);


            }

            model.timeSheetDetailReimbursements = timeSheetDetailReimbursements;

            }

            return model;
        }


        public  PendingTimeSheetModel GetPendingTimeSheets(int PhysicianId, DateTime StartDate)
        {
            DateOnly date = new DateOnly(StartDate.Year, StartDate.Month, StartDate.Day);
            int currentMonth = StartDate.Month;

            DateTime endDate;


            if (StartDate.Day == 1)
            {
                endDate = new DateTime(StartDate.Year, StartDate.Month, 14);

            }
            else
            {


                if (new DateTime(StartDate.Year, currentMonth, 15).AddDays(16).Month == currentMonth)
                {
                    endDate = new DateTime(StartDate.Year, StartDate.Month, 31);

                }
                else if (new DateTime(StartDate.Year, currentMonth, 15).AddDays(15).Month == currentMonth)
                {
                    endDate = new DateTime(StartDate.Year, StartDate.Month, 30);


                }
                else if (new DateTime(StartDate.Year, currentMonth, 15).AddDays(14).Month == currentMonth)
                {
                    endDate = new DateTime(StartDate.Year, StartDate.Month, 29);


                }
                else
                {
                    endDate = new DateTime(StartDate.Year, StartDate.Month, 28);


                }
            }

            Timesheet?  timesheet = _context.Timesheets.Where(q=>q.Physicianid == PhysicianId && q.Startdate == date).FirstOrDefault();

                PendingTimeSheetModel pendingTimeSheetModel = new PendingTimeSheetModel();

            if(timesheet != null && timesheet.Isfinalize == true && timesheet.Isapproved == false)
            {


                pendingTimeSheetModel.StartDate = StartDate;
                pendingTimeSheetModel.EndTime = endDate;
                pendingTimeSheetModel.IsFinelized = true;
                pendingTimeSheetModel.IsApproved = false;
                pendingTimeSheetModel.TimeSheetId = timesheet.Timesheetid;
                pendingTimeSheetModel.PhysicianName = _context.Physicians.Where(q => q.Physicianid == timesheet.Physicianid).Select(q => q.Firstname + " " + q.Lastname).FirstOrDefault();

            }else if(timesheet != null && timesheet.Isfinalize == false)
            {
                pendingTimeSheetModel.IsFinelized = false;
                pendingTimeSheetModel.IsApproved = false;
                pendingTimeSheetModel.StartDate = StartDate;
                pendingTimeSheetModel.EndTime = endDate;
                pendingTimeSheetModel.PhysicianName = _context.Physicians.Where(q => q.Physicianid == timesheet.Physicianid).Select(q => q.Firstname + " " + q.Lastname).FirstOrDefault();

            }
            else
            {
                pendingTimeSheetModel.IsApproved= true;
                pendingTimeSheetModel.IsFinelized = true;
                pendingTimeSheetModel.StartDate = StartDate;
                pendingTimeSheetModel.EndTime = endDate;
            }


            return pendingTimeSheetModel;

        }


    }
}
