using DocumentFormat.OpenXml.Office2010.Excel;
using HalloDoc_BAL.Interface;
using HalloDoc_DAL.AdminViewModels;
using HalloDoc_DAL.Context;
using HalloDoc_DAL.Models;
using HalloDoc_DAL.ProviderViewModels;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.Repositery
{
    public class ProviderDashboardServices : IProviderDashboard
    {

        private readonly HalloDocContext _context;

        public ProviderDashboardServices(HalloDocContext context)
        {
            _context = context;
        }


        public List<DashboardRequests> GetAllProviderRequests(int physicianId)
        {
            List<DashboardRequests> requests = _context.Requests.OrderByDescending(q => q.Createddate).Where(q => q.Physicianid == physicianId && q.Isdeleted == new BitArray(1, false)).Select(r => new DashboardRequests
            {
                Requestid = r.Requestid,
                Username = _context.Requestclients.Where(q => q.Requestid == r.Requestid).Select(r => r.Firstname + " " + r.Lastname).FirstOrDefault(),

                Address = _context.Users.Where(q => q.Userid == r.Userid).Select(r => r.Street + "," + r.City + "," + r.State + "," + r.Zipcode).FirstOrDefault(),
                CallType = r.Calltype == null ? 0 : (int)r.Calltype,

                Phone = _context.Requestclients.Where(q => q.Requestid == r.Requestid).Select(m => m.Phonenumber).FirstOrDefault(),
                status = r.Status,
                Requesttype = r.Requesttypeid,
                PhysicianName = _context.Physicians.Where(q => q.Physicianid == r.Physicianid).Select(q => q.Firstname + " " + q.Lastname).FirstOrDefault(),

                RequestorPhone = r.Requesttypeid != 1 ? r.Phonenumber : null,

            }).ToList();

            return requests;
        }

        public List<DashboardRequests> GetStatuswiseProviderRequestsService(string[] statusArray, int PhysicianId)
        {

            Dictionary<int, int> status = new Dictionary<int, int>();

            for (int i = 0; i < statusArray.Length; i++)
            {
                status.Add(int.Parse(statusArray[i]), 1);
            }


            var statuskey = status.Keys.ToList();

            List<DashboardRequests> dashboardRequests = _context.Requests.OrderByDescending(q => q.Createddate).Where(q => statuskey.Contains(q.Status) && q.Physicianid == PhysicianId && q.Isdeleted == new BitArray(1,false)).Select(r => new DashboardRequests
            {
                Requestid = r.Requestid,
                Username = _context.Requestclients.Where(q => q.Requestid == r.Requestid).Select(r => r.Firstname + " " + r.Lastname).FirstOrDefault(),
                Requestor = r.Firstname + " " + r.Lastname,
                Address = _context.Users.Where(q => q.Userid == r.Userid).Select(r => r.Street + "," + r.City + "," + r.State + "," + r.Zipcode).FirstOrDefault(),
                Requestdate = r.Createddate,
                Phone = _context.Requestclients.Where(q => q.Requestid == r.Requestid).Select(m => m.Phonenumber).FirstOrDefault(),
                CallType = r.Calltype == null ? 0 : (int)r.Calltype,
                status = r.Status,
                Requesttype = r.Requesttypeid,
                PhysicianName = _context.Physicians.Where(q => q.Physicianid == r.Physicianid).Select(q => q.Firstname + " " + q.Lastname).FirstOrDefault(),
                Birthdate = _context.Requestclients.Where(q => q.Requestid == r.Requestid).Select(r => new DateTime((r.Intyear ?? 0) == 0 ? 1 : (int)(r.Intyear ?? 0), DateTime.ParseExact(r.Strmonth, "MMMM", CultureInfo.InvariantCulture).Month, (r.Intdate ?? 0) == 0 ? 1 : (int)(r.Intdate ?? 0))).FirstOrDefault(),
                RequestorPhone = r.Requesttypeid != 1 ? r.Phonenumber : null,
                Notes = _context.Requeststatuslogs.Where(q => q.Requestid == r.Requestid && (q.Status == 3 || q.Status == 4 || q.Status == 9)).Select(r => new TransferNotes
                {
                    AdminId = r.Adminid,
                    PhysicianId = r.Transtophysicianid,
                    PhysicinName = _context.Physicians.Where(q => q.Physicianid == r.Transtophysicianid).Select(r => r.Businessname).FirstOrDefault(),
                    TransferedDate = r.Createddate,
                    Description = r.Notes
                }).ToList(),


            }).ToList();

            return dashboardRequests;
        }

        public List<DashboardRequests> GetProviderRequestsFromRequestorTypeService(int type, string[] statusArray, string name, int PhysicianId)
        {


            Dictionary<int, int> status = new Dictionary<int, int>();


            for (int i = 0; i < statusArray.Length; i++)
            {
                status.Add(int.Parse(statusArray[i]), 1);
            }


            var statuskey = status.Keys.ToList();


            List<DashboardRequests> dashboardRequests = _context.Requests.OrderByDescending(q => q.Createddate).Where(q => statuskey.Contains(q.Status) && q.Physicianid == PhysicianId).Select(r => new DashboardRequests
            {
                Requestid = r.Requestid,
                Username = _context.Requestclients.Where(q => q.Requestid == r.Requestid).Select(r => r.Firstname + " " + r.Lastname).FirstOrDefault(),
                Requestor = r.Firstname + " " + r.Lastname,
                Address = _context.Users.Where(q => q.Userid == r.Userid).Select(r => r.Street + "," + r.City + "," + r.State + "," + r.Zipcode).FirstOrDefault(),
                Requestdate = r.Createddate,
                Phone = _context.Requestclients.Where(q => q.Requestid == r.Requestid).Select(m => m.Phonenumber).FirstOrDefault(),
                CallType = r.Calltype == null ? 0 : (int)r.Calltype,
                status = r.Status,
                Requesttype = r.Requesttypeid,
                Requestortype = _context.Requesttypes.Where(q => q.Requesttypeid == r.Requesttypeid).Select(q => q.Name).FirstOrDefault(),
                PhysicianName = _context.Physicians.Where(q => q.Physicianid == r.Physicianid).Select(q => q.Firstname + " " + q.Lastname).FirstOrDefault(),
                Birthdate = _context.Requestclients.Where(q => q.Requestid == r.Requestid).Select(r => new DateTime((r.Intyear ?? 0) == 0 ? 1 : (int)(r.Intyear ?? 0), DateTime.ParseExact(r.Strmonth, "MMMM", CultureInfo.InvariantCulture).Month, (r.Intdate ?? 0) == 0 ? 1 : (int)(r.Intdate ?? 0))).FirstOrDefault(),
                RequestorPhone = r.Requesttypeid != 1 ? r.Phonenumber : null,
                RegionId = _context.Requestclients.Where(q => q.Requestid == r.Requestid).Select(q => q.Regionid).FirstOrDefault(),
                RequestTypeId = r.Requesttypeid,
                Notes = _context.Requeststatuslogs.Where(q => q.Requestid == r.Requestid && (q.Status == 3 || q.Status == 4 || q.Status == 9)).Select(r => new TransferNotes
                {
                    AdminId = r.Adminid,
                    PhysicianId = r.Transtophysicianid,
                    PhysicinName = _context.Physicians.Where(q => q.Physicianid == r.Transtophysicianid).Select(r => r.Businessname).FirstOrDefault(),
                    TransferedDate = r.Createddate,
                    Description = r.Notes
                }).ToList(),

            }).ToList();

            if (type != 0)
            {
                dashboardRequests = dashboardRequests.Where(q => q.RequestTypeId == type).ToList();
            }



            if (name != null)
            {
                dashboardRequests = dashboardRequests.Where(q => q.Username.ToLower().Contains(name.ToLower())).ToList();
            }


            return dashboardRequests;
        }



        public bool AcceptRequest(int requestId, int physicianId)
        {

            try
            {


                Request request = _context.Requests.FirstOrDefault(q => q.Requestid == requestId);


                request.Status = 9;
                request.Accepteddate = DateTime.Now;
                request.Modifieddate = DateTime.Now;


                Requeststatuslog requeststatuslog = new Requeststatuslog();


                requeststatuslog.Requestid = requestId;
                requeststatuslog.Status = request.Status;
                requeststatuslog.Createddate = DateTime.Now;
                requeststatuslog.Physicianid = physicianId;

                _context.Requests.Update(request);
                _context.Requeststatuslogs.Add(requeststatuslog);

                _context.SaveChanges();


                return true;
            }
            catch (Exception ex)
            {


                return false;

            }
        }

        public bool SaveTypeOfCareService(int requestId, bool HouseCall)
        {


            try
            {


                Request request = _context.Requests.FirstOrDefault(q => q.Requestid == requestId);


                if (HouseCall)
                {
                    request.Calltype = 1;
                    request.Status = 4;
                }
                else
                {

                    request.Calltype = 2;
                    request.Status = 5;
                }


                Requeststatuslog requeststatuslog = new Requeststatuslog();
                requeststatuslog.Status = request.Status;
                requeststatuslog.Requestid = request.Requestid;
                requeststatuslog.Createddate = DateTime.Now;

                _context.Requests.Update(request);
                _context.Requeststatuslogs.Add(requeststatuslog);

                _context.SaveChanges();
                return true;


            }
            catch (Exception ex)
            {
                return false;

            }

        }

        public bool HouseCallActionService(int id)
        {


            try
            {
                Request request = _context.Requests.FirstOrDefault(q => q.Requestid == id);
                request.Status = 5;
                request.Modifieddate = DateTime.Now;

                Requeststatuslog requeststatuslog = new Requeststatuslog();
                requeststatuslog.Status = 5;
                requeststatuslog.Requestid = request.Requestid;
                requeststatuslog.Createddate = DateTime.Now;


                _context.Requeststatuslogs.Add(requeststatuslog);
                _context.Requests.Update(request);

                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }


        public ConcludeCare GetConcludeCareDetails(int requestId)
        {

            string PatientName = _context.Requestclients.Where(q => q.Requestid == requestId).Select(q => q.Firstname + " " + q.Lastname).FirstOrDefault();


            bool? isFinlize = _context.Encounterforms.Where(q => q.Requestid == requestId).Select(q => q.IsFinelized).FirstOrDefault();

            ConcludeCare concludeCare = new ConcludeCare()
            {
                RequestId = requestId,
                PatientName = PatientName,
                IsFinilize = isFinlize == null?false:(bool)isFinlize,
            };

            return concludeCare;
        }

        public bool IsFormFinlized(int requestId)
        {


            bool? isFinlized = _context.Encounterforms.Where(q => q.Requestid == requestId).Select(q => q.IsFinelized).FirstOrDefault();

            if(isFinlized == null)
            {
                return false;
            }
            else
            {
                return (bool)isFinlized;
            }
        }


        public bool FinalizeEncounterformService(int id)
        {
            try
            {


                Encounterform? encounterform = _context.Encounterforms.FirstOrDefault(q => q.Requestid == id);

                if(encounterform == null)
                {
                    return false;
                }

                encounterform.IsFinelized = true;

                _context.Encounterforms.Update(encounterform);
                _context.SaveChanges();


                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }


        public bool ConcludeCareService(ConcludeCare concludeCare, int requestId, int UserId, string Role)
        {
            try
            {


                Request request = _context.Requests.FirstOrDefault(q => q.Requestid == requestId);

                request.Status = 8;
                request.Modifieddate = DateTime.Now;

                Requestnote requestNotes = _context.Requestnotes.Where(q => q.Requestid == requestId).FirstOrDefault();

                if (requestNotes == null)
                {
                    requestNotes = new Requestnote();

                    requestNotes.Physiciannotes = concludeCare.ProviderNotes;
                    requestNotes.Requestid = requestId;
                    requestNotes.Createdby = Role == "Admin" ? _context.Admins.Where(q => q.Adminid == UserId).Select(q => q.Aspnetuserid).FirstOrDefault() : _context.Physicians.Where(q => q.Physicianid == UserId).Select(q => q.Aspnetuserid).FirstOrDefault();
                    requestNotes.Createddate = DateTime.Now;
                    _context.Requestnotes.Add(requestNotes);
                }
                else
                {
                    requestNotes.Physiciannotes = concludeCare.ProviderNotes;
                    requestNotes.Requestid = requestId;
                    requestNotes.Modifiedby = Role == "Admin" ? _context.Admins.Where(q => q.Adminid == UserId).Select(q => q.Aspnetuserid).FirstOrDefault() : _context.Physicians.Where(q => q.Physicianid == UserId).Select(q => q.Aspnetuserid).FirstOrDefault();
                    requestNotes.Modifieddate = DateTime.Now;
                    _context.Requestnotes.Update(requestNotes);
                }


                Requeststatuslog requeststatuslog = new Requeststatuslog();

                requeststatuslog.Status = request.Status;
                requeststatuslog.Requestid = request.Requestid;
                requeststatuslog.Createddate = DateTime.Now;

                if(Role == "Admin")
                {
                    requeststatuslog.Adminid = UserId;
                }else if(Role == "Physician")
                {
                    requeststatuslog.Physicianid = UserId;
                }

                _context.Requeststatuslogs.Add(requeststatuslog);
                _context.Requests.Update(request);

                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {


                return false;
            }
        }




        public List<Region> GetAllPhysicianRegions(int PhysicinId)
        {

            List<Physicianregion> physicianregions = _context.Physicianregions.Where(q=>q.Physicianid == PhysicinId).ToList();  


            List<Region> regions = new List<Region>();  


            foreach(Physicianregion physicianregion in physicianregions)
            {

                Region region = _context.Regions.FirstOrDefault(q => physicianregion.Regionid == q.Regionid);

                    regions.Add(region);
            }

            return regions;
        }


        public List<MonthWisePhysicianShifts> GetPhysicianMonthWiseShifts(int date, int month, int year, int PhysicianId)
        {

            try
            {
            List<MonthWisePhysicianShifts> MonthWisePhysicianShiftsList = new List<MonthWisePhysicianShifts>();


            DateTime startOfMonth = new DateTime(year, month, date);

            int daysInMonth = DateTime.DaysInMonth(startOfMonth.Year, startOfMonth.Month);

            for (int i = 1; i <= daysInMonth; i++)
            {

                DateTime currentDate = new DateTime(startOfMonth.Year, startOfMonth.Month, i);

                  

                List<Shiftdetail> shiftdetails = _context.Shiftdetails.Where(q => q.Shiftdate.Day == i && q.Shiftdate.Month == currentDate.Month && q.Shiftdate.Year == currentDate.Year && q.Isdeleted == false).ToList();
                MonthWisePhysicianShifts monthWisePhysicianShift = new MonthWisePhysicianShifts();

                if (shiftdetails.Count > 0)
                {
                    monthWisePhysicianShift.DayNumber = i;
                    List<ShiftInformation> shifts = new List<ShiftInformation>();
                    foreach (Shiftdetail shiftdetail in shiftdetails)
                    {
                        if(_context.Shifts.Where(q=>q.Shiftid == shiftdetail.Shiftid).Select(q=>q.Physicianid == PhysicianId).FirstOrDefault())
                        {

                        ShiftInformation shiftInformation = new ShiftInformation();
                        shiftInformation.ShiftDetailId = shiftdetail.Shiftdetailid;
                        shiftInformation.startTime = shiftdetail.Starttime;
                        shiftInformation.endTime = shiftdetail.Endtime;
                        shiftInformation.status = shiftdetail.Status;

                        int physicinId = _context.Shifts.Where(q => q.Shiftid == shiftdetail.Shiftid).Select(q => q.Physicianid).FirstOrDefault();
                        shiftInformation.PhysicianName = _context.Physicians.Where(q => q.Physicianid == physicinId).Select(q => q.Firstname + " " + q.Lastname).FirstOrDefault();

                        shifts.Add(shiftInformation);
                        }

                    }
                    monthWisePhysicianShift.MonthWiseShiftInformation = shifts;
                }
                else
                {
                    monthWisePhysicianShift.DayNumber = -1;

                }

                MonthWisePhysicianShiftsList.Add(monthWisePhysicianShift);
            }

            return MonthWisePhysicianShiftsList;

            }catch(Exception ex)
            {
                return new List<MonthWisePhysicianShifts>();
            }
        }




        public bool TrasnferToAdminService(string message,int providerId,int requestId)
        {
            try
            {


                Request request = _context.Requests.FirstOrDefault(q => q.Requestid == requestId);

                request.Status = 1;
                request.Modifieddate = DateTime.Now;
                request.Physicianid = null;


                Requeststatuslog requeststatuslog = new Requeststatuslog();

                requeststatuslog.Status = 1;
                requeststatuslog.Notes = message;
                requeststatuslog.Requestid = requestId;
                requeststatuslog.Createddate = DateTime.Now;
                requeststatuslog.Transtoadmin = new BitArray(new[] { true });
                requeststatuslog.Physicianid = providerId;

                _context.Requeststatuslogs.Add(requeststatuslog);
                _context.Requests.Update(request);

                _context.SaveChanges();


                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }



    }
}
