using HalloDoc_BAL.Interface;
using HalloDoc_DAL.AdminViewModels;
using HalloDoc_DAL.Context;
using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.Repositery
{
    public class ProviderDashboardServices:IProviderDashboard
    {

        private readonly HalloDocContext _context;

        public ProviderDashboardServices(HalloDocContext context)
        {
            _context = context;
        }


        public List<DashboardRequests> GetAllProviderRequests(int physicianId)
        {
            List<DashboardRequests> requests = _context.Requests.Where(q=>q.Physicianid == physicianId).Select(r => new DashboardRequests
            {
                Requestid = r.Requestid,
                Username = _context.Requestclients.Where(q => q.Requestid == r.Requestid).Select(r => r.Firstname + " " + r.Lastname).FirstOrDefault(),
        
                Address = _context.Users.Where(q => q.Userid == r.Userid).Select(r => r.Street + "," + r.City + "," + r.State + "," + r.Zipcode).FirstOrDefault(),
            
                Phone = _context.Requestclients.Where(q => q.Requestid == r.Requestid).Select(m => m.Phonenumber).FirstOrDefault(),
                status = r.Status,
                Requesttype = r.Requesttypeid,
                PhysicianName = _context.Physicians.Where(q => q.Physicianid == r.Physicianid).Select(q => q.Firstname + " " + q.Lastname).FirstOrDefault(),
                
                RequestorPhone = r.Requesttypeid != 1 ? r.Phonenumber : null,
                
            }).ToList();

            return requests;
        }

        public List<DashboardRequests> GetStatuswiseProviderRequestsService(string[] statusArray,int PhysicianId)
        {

            Dictionary<int, int> status = new Dictionary<int, int>();

            for (int i = 0; i < statusArray.Length; i++)
            {
                status.Add(int.Parse(statusArray[i]), 1);
            }


            var statuskey = status.Keys.ToList();

            List<DashboardRequests> dashboardRequests = _context.Requests.Where(q => statuskey.Contains(q.Status) && q.Physicianid == PhysicianId).Select(r => new DashboardRequests
            {
                Requestid = r.Requestid,
                Username = _context.Requestclients.Where(q => q.Requestid == r.Requestid).Select(r => r.Firstname + " " + r.Lastname).FirstOrDefault(),
                Requestor = r.Firstname + " " + r.Lastname,
                Address = _context.Users.Where(q => q.Userid == r.Userid).Select(r => r.Street + "," + r.City + "," + r.State + "," + r.Zipcode).FirstOrDefault(),
                Requestdate = r.Createddate,
                Phone = _context.Requestclients.Where(q => q.Requestid == r.Requestid).Select(m => m.Phonenumber).FirstOrDefault(),
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


            List<DashboardRequests> dashboardRequests = _context.Requests.Where(q => statuskey.Contains(q.Status) && q.Physicianid == PhysicianId).Select(r => new DashboardRequests
            {
                Requestid = r.Requestid,
                Username = _context.Requestclients.Where(q => q.Requestid == r.Requestid).Select(r => r.Firstname + " " + r.Lastname).FirstOrDefault(),
                Requestor = r.Firstname + " " + r.Lastname,
                Address = _context.Users.Where(q => q.Userid == r.Userid).Select(r => r.Street + "," + r.City + "," + r.State + "," + r.Zipcode).FirstOrDefault(),
                Requestdate = r.Createddate,
                Phone = _context.Requestclients.Where(q => q.Requestid == r.Requestid).Select(m => m.Phonenumber).FirstOrDefault(),
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



        public bool AcceptRequest(int requestId,int physicianId)
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

                _context.Requests.Update(request);
                _context.Requeststatuslogs.Add(requeststatuslog);

                _context.SaveChanges();


                return true;
            }catch(Exception ex)
            {


                return false;
                   
            }
        }


    }
}
