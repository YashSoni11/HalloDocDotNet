using HalloDoc_BAL.Interface;
using HalloDoc_DAL.Context;
using HalloDoc_DAL.Models;
using HalloDoc_DAL.ViewModels;
using HalloDoc_DAL.AdminViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Web.Mvc;

namespace HalloDoc_BAL.Repositery
{
    public class Admindashboard : IAdmindashboard
    {


        private readonly HalloDocContext _context;


        public Admindashboard(HalloDocContext context)
        {
            _context = context;
        }



        public enum Status
        {
            Unassigned = 1,
            Unpaid,
            MDEnRoute,
            MDOnSite,
            Conclude,
            Cancelled,
            CancelledByPatient,
            Closed,
            Accepted,
            Clear,
        }


        public string GetPatientName(int id)
        {


            string name = _context.Requestclients.Where(q => q.Requestid == id).Select(r => r.Firstname + " " + r.Lastname).FirstOrDefault();

            return name;
        }

        public List<DashboardRequests> GetAllRequests()
        {

            List<DashboardRequests> requests = _context.Requests.Select(r => new DashboardRequests
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
            }).ToList();







            return requests;

        }

        public string GetAdminUsername(int id)
        {
            string name = _context.Admins.Where(q=>q.Adminid == id).Select(r=>r.Firstname).FirstOrDefault();

            return name;
        }


        public List<Region> GetAllRegions()
        {
            List<Region> regions = _context.Regions.ToList();

            return regions;
        }

        public List<Physician> GetAllPhysician()
        {
            List<Physician> physicians = _context.Physicians.ToList();

            return physicians;
        }

        public List<Physician> FilterPhysicianByRegion(int regionid)
        {
            List<Physician> physicians = _context.Physicians.Where(q => q.Regionid == regionid).ToList();

            return physicians;
        }

        public Request AssignRequest(AdminAssignCase assignCase, int requestId)
        {
            Request request = _context.Requests.FirstOrDefault(q => q.Requestid == requestId);

            if (request != null)
            {

                request.Physicianid = int.Parse(assignCase.SelectedPhycisianId);
                request.Modifieddate = DateTime.Now;
                request.Status = 9;


                Requeststatuslog requeststatuslog = new Requeststatuslog()
                {
                    Requestid = requestId,
                    Status = request.Status,
                    Physicianid = int.Parse(assignCase.SelectedPhycisianId),
                    Notes = assignCase.Description,
                    Createddate = DateTime.Now,
                };

                _context.Requests.Update(request);
                _context.Requeststatuslogs.Add(requeststatuslog);

                _context.SaveChanges();
            }


            return request;
        }


        public Request BlockRequest(AdminBlockCase blockRequest, int requestId)
        {
            Request request = _context.Requests.FirstOrDefault(q => q.Requestid == requestId);

            if (request != null)
            {
                request.Modifieddate = DateTime.Now;
                request.Status = 10;



                Requeststatuslog requeststatuslog = new Requeststatuslog()
                {
                    Requestid = requestId,
                    Status = request.Status,
                    Notes = blockRequest.ReasonForBlocking,
                    Createddate = DateTime.Now,
                };

                Blockrequest blockrequest = new Blockrequest()
                {
                    Phonenumber = request.Phonenumber,
                    Reason = blockRequest.ReasonForBlocking,
                    Requestid = requestId.ToString(),
                    Createddate = DateTime.Now,
                    Email = request.Email,

                };

                _context.Blockrequests.Add(blockrequest);
                _context.Requeststatuslogs.Add(requeststatuslog);
                _context.Requests.Update(request);
                _context.SaveChanges();

            }

            return request;
        }



        public List<DashboardRequests> GetRequestsByUsername(string name)
        {
            List<DashboardRequests> requests = _context.Requestclients.Where(q => q.Firstname.Contains(name) || q.Lastname.Contains(name)).Select(r => new DashboardRequests
            {
                Requestid = r.Requestid,
                Username = r.Firstname + " " + r.Lastname,
                Requestor = _context.Requests.Where(q => q.Requestid == r.Requestid).Select(r => r.Firstname + " " + r.Lastname).FirstOrDefault(),
                Address = r.Street + "," + r.City + "," + r.State + "," + r.Zipcode,
                Requestdate = _context.Requests.Where(q => r.Requestid == q.Requestid).Select(q => q.Createddate).FirstOrDefault(),
                Phone = r.Phonenumber,
                status = _context.Requests.Where(q => q.Requestid == r.Requestid).Select(r => r.Status).FirstOrDefault(),
                Requesttype = _context.Requests.Where(q => q.Requestid == r.Requestid).Select(r => r.Requesttypeid).FirstOrDefault(),
                Birthdate = new DateTime((r.Intyear ?? 0) == 0 ? 1 : (int)(r.Intyear ?? 0), DateTime.ParseExact(r.Strmonth, "MMMM", CultureInfo.InvariantCulture).Month, (r.Intdate ?? 0) == 0 ? 1 : (int)(r.Intdate ?? 0)),
                RequestorPhone = _context.Requests.Where(q => q.Requestid == r.Requestid).Select(m => m.Requesttypeid != 1 ? m.Phonenumber : null).FirstOrDefault(),
            }).ToList();

            return requests;
        }


        public RequestTypeCounts GetAllRequestsCount(List<DashboardRequests> dashboardRequests)
        {
            Dictionary<string, int> reqCount = new Dictionary<string, int>();

            foreach (DashboardRequests req in dashboardRequests)
            {
                string reqstatus = @Enum.GetName(typeof(Status), req.status);

                if (reqCount.ContainsKey(reqstatus))
                {
                    reqCount[reqstatus]++;
                }
                else
                {
                    reqCount[reqstatus] = 1;
                }

            }

            RequestTypeCounts requestTypeCounts = new RequestTypeCounts
            {
                Newrequestcount = reqCount.ContainsKey("Unassigned") ? reqCount["Unassigned"] : 0,
                Pendingrequestcount = reqCount.ContainsKey("Accepted") ? reqCount["Accepted"] : 0,
                Activerequestcount = (reqCount.ContainsKey("MDEnRoute") ? reqCount["MDEnRoute"] : 0) + (reqCount.ContainsKey("MDOnSite") ? reqCount["MDOnSite"] : 0),
                Concluderequestcount = reqCount.ContainsKey("Conclude") ? reqCount["Conclude"] : 0,
                Tocloserequestcount = (reqCount.ContainsKey("Cancelled") ? reqCount["Cancelled"] : 0) + (reqCount.ContainsKey("CancelledByPatient") ? reqCount["CancelledByPatient"] : 0) + (reqCount.ContainsKey("Closed") ? reqCount["Closed"] : 0),
                Unpaidrequestcount = reqCount.ContainsKey("Unpaid") ? reqCount["Unpaid"] : 0,

            };

            return requestTypeCounts;
        }


        public List<DashboardRequests> GetStatuswiseRequests(string[] statusArray)
        {


            Dictionary<int, int> status = new Dictionary<int, int>();

            for (int i = 0; i < statusArray.Length; i++)
            {
                status.Add(int.Parse(statusArray[i]), 1);
            }


            var statuskey = status.Keys.ToList();

            List<DashboardRequests> dashboardRequests = _context.Requests.Where(q => statuskey.Contains(q.Status)).Select(r => new DashboardRequests
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


            }).ToList();

            return dashboardRequests;
        }
        public List<DashboardRequests> GetRequestsFromRequestorType(int type, string[] statusArray, string region, string name)
        {

            Dictionary<int, int> status = new Dictionary<int, int>();


            for (int i = 0; i < statusArray.Length; i++)
            {
                status.Add(int.Parse(statusArray[i]), 1);
            }


            var statuskey = status.Keys.ToList();


            List<DashboardRequests> dashboardRequests = _context.Requests.Where(q => statuskey.Contains(q.Status)).Select(r => new DashboardRequests
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

            }).ToList();

            if (type != 0)
            {
                dashboardRequests = dashboardRequests.Where(q => q.RequestTypeId == type).ToList();
            }


            if (region != null)
            {
                int newregionid = int.Parse(region);
                dashboardRequests = dashboardRequests.Where(q => q.RegionId == newregionid).ToList();

            }

            if (name != null)
            {
                dashboardRequests = dashboardRequests.Where(q => q.Username.Contains(name)).ToList();
            }


            return dashboardRequests;

        }

        public ClientRequest GetUserInfoFromRequestId(int requestId)
        {

            ClientRequest? requestclient = _context.Requestclients.Where(q => q.Requestid == requestId).Select(r => new ClientRequest
            {
                Firstname = r.Firstname,
                Lastname = r.Lastname,
                Email = r.Email,
                Phonenumber = r.Phonenumber,
                State = r.State,
                Street = r.Street,
                City = r.City,
                Zipcode = r.Zipcode,
                Birthdate = new DateTime((r.Intyear ?? 0) == 0 ? 1 : (int)(r.Intyear ?? 0), DateTime.ParseExact(r.Strmonth, "MMMM", CultureInfo.InvariantCulture).Month, (r.Intdate ?? 0) == 0 ? 1 : (int)(r.Intdate ?? 0)),
                Notes = r.Notes,
                RequestTypeId = _context.Requests.Where(q => q.Requestid == requestId).Select(r => r.Requesttypeid).FirstOrDefault(),
                ConfirmationNumber = _context.Requests.Where(q => q.Requestid == requestId).Select(r => r.Confirmationnumber).FirstOrDefault(),
            }).FirstOrDefault();

            return requestclient;
        }

        public RequestNotes GetNotesFromRequestId(int requestId)
        {
            RequestNotes? requestNotes = _context.Requestnotes.Where(q => q.Requestid == requestId).Select(r => new RequestNotes
            {
                TranferNotes = r.Physiciannotes,
                AdminNotes = r.Adminnotes,
                AddtionalNotes = r.Administrativenotes,
                PhysicianNotes = r.Physiciannotes

            }).FirstOrDefault();

            return requestNotes;
        }

        public ClientRequest UpdateClientRequest(ClientRequest clientRequest, int RequestId)
        {
            Requestclient requestclient = _context.Requestclients.Where(q => q.Requestid == RequestId).FirstOrDefault();


            if (requestclient != null)
            {

                requestclient.Email = clientRequest.Email;
                requestclient.Phonenumber = clientRequest.Phonenumber;

            }

            _context.Requestclients.Update(requestclient);

            _context.SaveChanges();

            return clientRequest;
        }


        public Request UpdateRequestToClose(AdminCancleCase adminCancleCase, int id)
        {
            Request request = _context.Requests.FirstOrDefault(q => q.Requestid == id);

            request.Casetag = adminCancleCase.Reason;
            request.Modifieddate = DateTime.Now;
            request.Status = 8;

            Requeststatuslog requeststatuslog = new Requeststatuslog
            {
                Requestid = adminCancleCase.requestId,
                Status = 8,
                Createddate = DateTime.Now,
                Notes = adminCancleCase.AdditionalNotes
            };

            _context.Requeststatuslogs.Add(requeststatuslog);
            _context.Requests.Update(request);

            _context.SaveChanges();

            return request;
        }

        public List<ViewDocument> GetDocumentsByRequestId(int requestId)
        {
           


            List<ViewDocument> documents = _context.Requestwisefiles.Where(u => u.Requestid == requestId && u.Isdeleted != true).Select(r => new ViewDocument
            {
                FileId = r.Requestwisefileid,
                filename = r.Filename,
                uploader = _context.Requests.FirstOrDefault(u => u.Requestid == requestId).Firstname,
                uploadDate = r.Createddate
            }).ToList();

            return documents;
        }


        public void DeleteFile(int id)
        {
            Requestwisefile requestwisefile = _context.Requestwisefiles.FirstOrDefault(q => q.Requestwisefileid == id);



            if (requestwisefile != null)
            {
                requestwisefile.Isdeleted = true;

                _context.Requestwisefiles.Update(requestwisefile);
                _context.SaveChanges();
            }

            return;
        }


        public void DeleteAllFiles(int[] IdArray)
        {
             
               for(int i = 0;i< IdArray.Length; i++)
            {
                DeleteFile(IdArray[i]);
            }

            return;
        }

        public List<Healthprofessionaltype> GetOrderDetails()
        {
       

            List<Healthprofessionaltype> healthprofessionaltypes  = _context.Healthprofessionaltypes.ToList();


            return healthprofessionaltypes;
        }
        public List<Healthprofessional> GetHealthProfessionalsByProfessionId(int id)
        {
            List<Healthprofessional> healthprofessionals = _context.Healthprofessionals.Where(q=>q.Profession == id).ToList();

            return healthprofessionals;
        }

        public Healthprofessional GetVendorByVendorId(int id)
        {
            Healthprofessional healthprofessional = _context.Healthprofessionals.FirstOrDefault(q=>q.Vendorid == id);

            return healthprofessional;
        }



        public bool PostOrderById(int id, Order order)
        {
         
          
            try
            {

               
               Orderdetail orderdetail = new Orderdetail();

                orderdetail.Requestid = id;
                orderdetail.Faxnumber = order.FaxNumber;
                orderdetail.Businesscontact = order.BusinessContact;
                orderdetail.Noofrefill = int.Parse(order.RefillNumber);
                orderdetail.Prescription = order.Prescription;
                orderdetail.Createddate = DateTime.Now;
                orderdetail.Email = order.Email;

                _context.Orderdetails.Add(orderdetail);
                _context.SaveChanges();

                return true;

            }catch (Exception ex) 
            {
                return false;
            }
        }


        public Request TransferRequest(AdminAssignCase adminAssignCase, int requestId,int adminId)
        {


            Request request = _context.Requests.FirstOrDefault(q => q.Requestid == requestId);

             if(request != null)
            {
                request.Physicianid = int.Parse(adminAssignCase.SelectedPhycisianId);
                request.Modifieddate = DateTime.Now;

            }


            Requeststatuslog requeststatuslog = new Requeststatuslog()
            {
                Requestid = requestId,
                Status = 9,
                Transtophysicianid = int.Parse(adminAssignCase.SelectedPhycisianId),
                Notes = adminAssignCase.Description,
                Createddate = DateTime.Now,
                Adminid = adminId
            };

            try
            {
                _context.Requeststatuslogs.Add(requeststatuslog);
                _context.Requests.Update(request);
                _context.SaveChanges();

                return request;
            }
            catch (Exception err)
            {
                return new Request();
            }

        }

    }


}
