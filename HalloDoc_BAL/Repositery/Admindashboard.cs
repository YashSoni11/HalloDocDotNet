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
using System.Net.Mail;
using MailKit;
//using MailKit.Net.Smtp;
using System.Net;
using System.Web.Helpers;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using ClosedXML;
using ClosedXML.Excel;
//using System.Drawing;

namespace HalloDoc_BAL.Repositery
{
    public class Admindashboard : IAdmindashboard
    {


        private readonly HalloDocContext _context;
        private readonly IMapper _mapper;

        public Admindashboard(HalloDocContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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


        public List<Casetag> GetAllCaseTags()
        {
            List<Casetag> casetag = _context.Casetags.ToList();

            return casetag;
        }

        public string GetConfirmationNumber(int id)
        {
            return _context.Requests.Where(q => q.Requestid == id).Select(r => r.Confirmationnumber).FirstOrDefault();
        }

        public int GetRequestStatusByRequestId(int requestId)
        {
            return _context.Requests.Where(q=>q.Requestid == requestId).Select(q=>q.Status).FirstOrDefault();
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
                CallType = r.Calltype == null?0: (int)r.Calltype,
                Notes  = _context.Requeststatuslogs.Where(q=>q.Requestid == r.Requestid && (q.Status == 3 || q.Status == 4 || q.Status == 9)).Select(r => new TransferNotes
              {
                AdminId = r.Adminid,
                PhysicianId = r.Transtophysicianid,
                PhysicinName = _context.Physicians.Where(q => q.Physicianid == r.Transtophysicianid).Select(r => r.Businessname).FirstOrDefault(),
                TransferedDate = r.Createddate,
                Description = r.Notes
                
                
            }).ToList(),

            }).ToList();

            return requests;

        }

        public string GetAdminUsername(int id)
        {
            string aspId = _context.Admins.Where(q => q.Adminid == id).Select(r => r.Aspnetuserid).FirstOrDefault();

            string userName = _context.Aspnetusers.Where(q=>q.Id == aspId).Select(r=>r.Username).FirstOrDefault();

            return userName;
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

        public List<ProviderMenu> FilterProviderByRegions(int regionId)
        {

            List<ProviderMenu> physicians = new List<ProviderMenu>();
            if (regionId != 0)
            {
              physicians = _context.Physicians.Where(q => q.Regionid == regionId && q.Isdeleted == false).Select(r=> new ProviderMenu()
            {
                Name = r.Firstname + " " + r.Lastname,
                IsNoificationOn = (bool)_context.Physiciannotifications.Where(q => q.Physicianid == r.Physicianid).Select(r => r.Isnotificationstopped).FirstOrDefault(),
                Role = _context.Roles.Where(q => q.Roleid == r.Roleid).Select(r => r.Name).FirstOrDefault(),
                status = (int)r.Status,
                ProviderId = r.Physicianid,
            }).ToList();
            }
            else
            {
                physicians = _context.Physicians.Where(q=>q.Isdeleted == false).Select(r => new ProviderMenu()
                {

                    Name = r.Firstname + " " + r.Lastname,
                    IsNoificationOn = (bool)_context.Physiciannotifications.Where(q => q.Physicianid == r.Physicianid).Select(r => r.Isnotificationstopped).FirstOrDefault(),
                    Role = _context.Roles.Where(q => q.Roleid == r.Roleid).Select(r => r.Name).FirstOrDefault(),
                    status = (int)r.Status,
                    ProviderId = r.Physicianid,

                }).ToList();
            }

            return physicians;
        }

        public bool AssignRequest(AdminAssignCase assignCase, int requestId)
        {
            try
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
                    //Physicianid = int.Parse(assignCase.SelectedPhycisianId),
                    Notes = assignCase.Description,
                    Createddate = DateTime.Now,
                };

                _context.Requests.Update(request);
                //_context.Requeststatuslogs.Add(requeststatuslog);

                _context.SaveChanges();
            }


            return true;

            }catch(Exception ex)
            {
                return false;
            }
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
                Notes = _context.Requeststatuslogs.Where(q => q.Requestid == r.Requestid && (q.Status == 3 || q.Status == 4 || q.Status == 9)).Select(r => new TransferNotes
                {
                    AdminId = r.Adminid,
                    PhysicianId = r.Transtophysicianid,
                    PhysicinName = _context.Physicians.Where(q => q.Physicianid == r.Transtophysicianid).Select(r => r.Businessname).FirstOrDefault(),
                    TransferedDate = r.Createddate,
                    Description = r.Notes
                }).ToList(),
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
                CallType = r.Calltype == null ? 0 : (int)r.Calltype,
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
                CallType = r.Calltype == null?0: (int)r.Calltype,
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


            if (region != null)
            {
                int newregionid = int.Parse(region);
                bool isAllRegion = newregionid == 0 ? true : false;
                dashboardRequests = dashboardRequests.Where(q => q.RegionId == newregionid || isAllRegion).ToList();

            }

            if (name != null)
            {
                dashboardRequests = dashboardRequests.Where(q => q.Username.ToLower().Contains(name.ToLower())).ToList();
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
                Status = _context.Requests.Where(q=>q.Requestid == requestId).Select(r=>r.Status).FirstOrDefault(),
            }).FirstOrDefault();

           

            return requestclient;
        }

        public RequestNotes GetNotesFromRequestId(int requestId)
        {
            RequestNotes? requestNotes = _context.Requestnotes.Where(q => q.Requestid == requestId).Select(r => new RequestNotes
            {
              
                AdminNotes = r.Adminnotes,
                AddtionalNotes = r.Administrativenotes,
                PhysicianNotes = r.Physiciannotes

            }).FirstOrDefault();

            if (requestNotes == null)
            {

                return requestNotes;
            }
            else
            {

            List<TransferNotes> transfernotes = new List<TransferNotes>();

             transfernotes = _context.Requeststatuslogs.Where(q => q.Requestid == requestId && (q.Status == 3 || q.Status == 4 || q.Status == 9)).Select(r=> new TransferNotes
            {
                AdminId = r.Adminid,
                PhysicianId = r.Transtophysicianid,
                PhysicinName = _context.Physicians.Where(q => q.Physicianid == r.Transtophysicianid).Select(r => r.Businessname).FirstOrDefault(),
                TransferedDate = r.Createddate,
                Description = r.Notes
        }).ToList();

            
            requestNotes.TranferNotes = transfernotes;

            return requestNotes;
            }
        }

        public bool SaveNotesChanges(string notes, int requestId,string Role,int UserId)
        {
            try
            {
            Requestnote requestNotes = _context.Requestnotes.FirstOrDefault(q => q.Requestid == requestId);

                if(requestNotes == null) {
                    Requestnote requestnote = new Requestnote();
                    requestnote.Requestid = requestId;
                    requestnote.Createddate = DateTime.Now;

                    if(Role == "Admin")
                    {
                       requestnote.Adminnotes = notes;
                        requestnote.Createdby = _context.Admins.Where(q => q.Adminid == UserId).Select(q => q.Aspnetuserid).FirstOrDefault();

                    }
                    else
                    {
                        requestnote.Physiciannotes = notes;
                        requestnote.Createdby = _context.Physicians.Where(q => q.Physicianid == UserId).Select(q => q.Aspnetuserid).FirstOrDefault();

                    }
                    _context.Requestnotes.Add(requestnote);
                }
                else
                {
                    if (Role == "Admin")
                    {
                        requestNotes.Adminnotes = notes;
                        requestNotes.Modifiedby = _context.Admins.Where(q => q.Adminid == UserId).Select(q => q.Aspnetuserid).FirstOrDefault();

                    }
                    else
                    {
                        requestNotes.Physiciannotes = notes;
                        requestNotes.Modifiedby = _context.Physicians.Where(q => q.Physicianid == UserId).Select(q => q.Aspnetuserid).FirstOrDefault();

                    }
                    requestNotes.Modifieddate = DateTime.Now;
                    _context.Requestnotes.Update(requestNotes);


                }


            _context.SaveChanges();
                return true;
            }catch(Exception ex)
            {
                return false;
            }
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


        public bool UpdateRequestToClose(AdminCancleCase adminCancleCase, int id)
        {
            try
            {

            Request request = _context.Requests.FirstOrDefault(q => q.Requestid == id);

            request.Casetag = adminCancleCase.Reason;
            request.Modifieddate = DateTime.Now;
            request.Status = 8;

            //Requeststatuslog requeststatuslog = new Requeststatuslog
            //{
            //    Requestid = (int)adminCancleCase.requestId,
            //    Status = 8,
            //    Createddate = DateTime.Now,
            //    Notes = adminCancleCase.AdditionalNotes
            //};

            //_context.Requeststatuslogs.Add(requeststatuslog);
            _context.Requests.Update(request);

            _context.SaveChanges();
                return true;
            }catch(Exception ex)
            {
                return false;
            }

           
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

            for (int i = 0; i < IdArray.Length; i++)
            {
                DeleteFile(IdArray[i]);
            }

            return;
        }

        public List<Healthprofessionaltype> GetOrderDetails()
        {


            List<Healthprofessionaltype> healthprofessionaltypes = _context.Healthprofessionaltypes.ToList();


            return healthprofessionaltypes;
        }
        public List<Healthprofessional> GetHealthProfessionalsByProfessionId(int id)
        {
            List<Healthprofessional> healthprofessionals = _context.Healthprofessionals.Where(q => q.Profession == id).ToList();

            return healthprofessionals;
        }

        public Healthprofessional GetVendorByVendorId(int id)
        {
            Healthprofessional healthprofessional = _context.Healthprofessionals.FirstOrDefault(q => q.Vendorid == id);

            return healthprofessional;
        }



        public bool PostOrderById(int id, Order order,string Role,int UserId)
        {


            try
            {


                Orderdetail orderdetail = new Orderdetail();


                if(Role == "Admin")
                {
                    orderdetail.Createdby = _context.Admins.Where(q => q.Adminid == UserId).Select(r => r.Aspnetuserid).FirstOrDefault();
                }else if(Role == "Physician")
                {
                    orderdetail.Createdby = _context.Physicians.Where(q => q.Physicianid == UserId).Select(r => r.Aspnetuserid).FirstOrDefault();

                }

                orderdetail.Requestid = id;
                orderdetail.Vendorid = int.Parse(order.Business);
                orderdetail.Faxnumber = order.FaxNumber;
                orderdetail.Businesscontact = order.BusinessContact;
                orderdetail.Noofrefill = int.Parse(order.RefillNumber);
                orderdetail.Prescription = order.Prescription;
                orderdetail.Createddate = DateTime.Now;
                orderdetail.Email = order.Email;

                _context.Orderdetails.Add(orderdetail);
                _context.SaveChanges();

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool TransferRequest(AdminAssignCase adminAssignCase, int requestId, int adminId)
        {
            try
            {

            Request request = _context.Requests.FirstOrDefault(q => q.Requestid == requestId);

            if (request != null)
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

            
                _context.Requeststatuslogs.Add(requeststatuslog);
                _context.Requests.Update(request);
                _context.SaveChanges();

                return true;
            }
            catch (Exception err)
            {
                return false;
            }

        }


        public bool SendDocumentsViaEmail(dynamic data)
        {
            var files = data["fileArray"];
            string uemail = data["UserMail"];

            var message = new MailMessage();
            message.From = new MailAddress("yash.soni@etatvasoft.com");
            message.Subject = "Documents";
            message.Body = "Please Find the attached documents.";
            message.IsBodyHtml = true;

            foreach (string file in files)
            {
                string newfile = file.Trim();
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", newfile);
                message.Attachments.Add(new Attachment(path));
            }

            message.To.Add(new MailAddress(uemail));


            try
            {
                using (var smtpClient = new SmtpClient("mail.etatvasoft.com"))
                {
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new NetworkCredential("yash.soni@etatvasoft.com", "kaX9Bjj8Sho");
                    smtpClient.EnableSsl = true;

                    smtpClient.Send(message);

                    return true;
                }

            }
            catch (Exception er)
            {
                return false;
            }
        }

        public string GetConfirmationNumberByRequestId(int requestId)
        {
            return _context.Requests.Where(q => q.Requestid == requestId).Select(r => r.Confirmationnumber).FirstOrDefault();
        }



        public bool ClearCaseByRequestid(string id, int adminId)
        {
            int newId = int.Parse(id);



            Request request = _context.Requests.FirstOrDefault(q => q.Requestid == newId);

            if (request != null)
            {
                request.Status = 10;
                request.Modifieddate = DateTime.Now;

                Requeststatuslog requeststatuslog = new Requeststatuslog()
                {
                    Requestid = newId,
                    Status = request.Status,
                    Createddate = DateTime.Now,
                    Adminid = adminId

                };

                try
                {
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
            else
            {
                return false;
            }
        }


        public bool AgreeAgreementByRequestId(string requestId)
        {
            int newId = int.Parse(requestId);



            Request request = _context.Requests.FirstOrDefault(q => q.Requestid == newId);

            if (request != null)
            {
                request.Status = 3;
                request.Modifieddate = DateTime.Now;

                //Requeststatuslog requeststatuslog = new Requeststatuslog()
                //{
                //    Requestid = newId,
                //    Status = request.Status,
                //    Createddate = DateTime.Now,

                //};

                try
                {
                    _context.Requests.Update(request);
                    //_context.Requeststatuslogs.Add(requeststatuslog);
                    _context.SaveChanges();

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }

        public bool CancleAgrrementByRequstId(CancleAgreement cancleAgreement, string requestId)
        {


            int newId = int.Parse(requestId);



            Request request = _context.Requests.FirstOrDefault(q => q.Requestid == newId);

            if (request != null)
            {
                request.Status = 8;
                request.Modifieddate = DateTime.Now;

                Requeststatuslog requeststatuslog = new Requeststatuslog()
                {
                    Requestid = newId,
                    Status = request.Status,
                    Createddate = DateTime.Now,
                    Notes = cancleAgreement.CanclationReason

                };

                try
                {
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
            else
            {
                return false;
            }
        }

        public SendAgreement GetSendAgreementpopupInfo(string requestId)
        {

            int newId = int.Parse(requestId);

            SendAgreement? data = _context.Requestclients.Where(q => q.Requestid == newId).Select(r => new SendAgreement
            {
                Phonenumber = r.Phonenumber,
                Email = r.Email,

            }).FirstOrDefault();

            int requestorType = _context.Requests.Where(q => q.Requestid == newId).Select(r => r.Requesttypeid).FirstOrDefault();

            data.Requestor = requestorType;

            data.requestId = requestId;

            return data;

        }


        public byte[] GeneratePdf(Encounterform encounterform)
        {
            using (var document = new PdfDocument())
            {
                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);
                var font = new XFont("Arial", 12);


                double x = 50;
                double y = 50;

                double oneby3x = 270;

                double leftx = 290;

                double lineHeight = 30;

                gfx.DrawString("Medical Report of" + " " + encounterform.Firstname + " " + encounterform.Lastname, font, XBrushes.Cyan, new XPoint(200, y));
                y += lineHeight;



                gfx.DrawString("First Name:", font, XBrushes.Black, new XPoint(x, y));
                gfx.DrawString(encounterform.Firstname != null ? encounterform.Firstname : "Not Available", font, XBrushes.Black, new XPoint(x + 70, y));


                gfx.DrawString("Last Name:", font, XBrushes.Black, new XPoint(leftx, y));
                gfx.DrawString(encounterform.Lastname != null ? encounterform.Lastname : "Not Available", font, XBrushes.Black, new XPoint(leftx + 70, y));

                y += lineHeight;

                gfx.DrawString("Location:", font, XBrushes.Black, new XPoint(x, y));
                gfx.DrawString(encounterform.Location != null ? encounterform.Location : "Not Available", font, XBrushes.Black, new XPoint(x + 70, y));
                y += lineHeight;

                gfx.DrawString("Date Of Birth:", font, XBrushes.Black, new XPoint(x, y));
                gfx.DrawString(encounterform.Dob.ToString() != null ? encounterform.Dob.ToString() : "Not Available", font, XBrushes.Black, new XPoint(x + 70, y));


                gfx.DrawString("Created At:", font, XBrushes.Black, new XPoint(leftx, y));
                gfx.DrawString(encounterform.Createdat.ToString() != null ? encounterform.Createdat.ToString() : "Not Available", font, XBrushes.Black, new XPoint(leftx + 70, y)); ;

                y += lineHeight;

                gfx.DrawString("PhoneNumber:", font, XBrushes.Black, new XPoint(x, y));
                gfx.DrawString(encounterform.Phonnumber != null ? encounterform.Phonnumber : "Not Available", font, XBrushes.Black, new XPoint(x + 80, y));


                gfx.DrawString("Email:", font, XBrushes.Black, new XPoint(leftx, y));
                gfx.DrawString(encounterform.Email != null ? encounterform.Email : "Not Available", font, XBrushes.Black, new XPoint(leftx + 70, y));

                y += lineHeight;

                gfx.DrawString("History of patinet illness Or Injury:", font, XBrushes.Black, new XPoint(x, y));
                gfx.DrawString(encounterform.History != null ? encounterform.History : "Not Available", font, XBrushes.Black, new XPoint(x + 220, y));
                y += lineHeight;


                gfx.DrawString("Medical History:", font, XBrushes.Black, new XPoint(x, y));
                gfx.DrawString(encounterform.MedicalHistory != null ? encounterform.MedicalHistory : "Not Available", font, XBrushes.Black, new XPoint(x + 90, y));
                y += lineHeight;

                gfx.DrawString("Medications:", font, XBrushes.Black, new XPoint(x, y));
                gfx.DrawString(encounterform.Medications != null ? encounterform.Medications : "Not Available", font, XBrushes.Black, new XPoint(x + 70, y));


                gfx.DrawString("Allergies:", font, XBrushes.Black, new XPoint(leftx, y));
                gfx.DrawString(encounterform.Allergies != null ? encounterform.Allergies : "Not Available", font, XBrushes.Black, new XPoint(leftx + 80, y));
                y += lineHeight;

                gfx.DrawString("Temp:", font, XBrushes.Black, new XPoint(x, y));
                gfx.DrawString(encounterform.Temperature != null ? encounterform.Temperature : "Not Available", font, XBrushes.Black, new XPoint(x + 80, y));


                gfx.DrawString("HR:", font, XBrushes.Black, new XPoint(oneby3x, y));
                gfx.DrawString(encounterform.Hr != null ? encounterform.Hr : "Not Available", font, XBrushes.Black, new XPoint(oneby3x + 35, y));


                gfx.DrawString("RR:", font, XBrushes.Black, new XPoint(oneby3x + 100, y));
                gfx.DrawString(encounterform.Rr != null ? encounterform.Rr : "Not Available", font, XBrushes.Black, new XPoint(oneby3x + 130, y));
                y += lineHeight;


                gfx.DrawString("Blood Pressure1:", font, XBrushes.Black, new XPoint(x, y));
                gfx.DrawString(encounterform.BloodPressure1 != null ? encounterform.BloodPressure1 : "Not Available", font, XBrushes.Black, new XPoint(x + 95, y));

                gfx.DrawString("Blood Pressure2:", font, XBrushes.Black, new XPoint(leftx, y));
                gfx.DrawString(encounterform.BloodPressure2 != null ? encounterform.BloodPressure2 : "Not Available", font, XBrushes.Black, new XPoint(leftx + 95, y));
                y += lineHeight;


                gfx.DrawString("O2:", font, XBrushes.Black, new XPoint(x, y));
                gfx.DrawString(encounterform.O2 != null ? encounterform.O2 : "Not Available", font, XBrushes.Black, new XPoint(x + 35, y));



                gfx.DrawString("Pain:", font, XBrushes.Black, new XPoint(leftx, y));
                gfx.DrawString(encounterform.Pain != null ? encounterform.Pain : "Not Available", font, XBrushes.Black, new XPoint(leftx + 35, y));
                y += lineHeight;


                gfx.DrawString("Heent:", font, XBrushes.Black, new XPoint(x, y));
                gfx.DrawString(encounterform.Heent != null ? encounterform.Heent : "Not Available", font, XBrushes.Black, new XPoint(x + 35, y));


                gfx.DrawString("CV:", font, XBrushes.Black, new XPoint(leftx, y));
                gfx.DrawString(encounterform.Cv != null ? encounterform.Cv : "Not Available", font, XBrushes.Black, new XPoint(leftx + 35, y));
                y += lineHeight;

                gfx.DrawString("Chest:", font, XBrushes.Black, new XPoint(x, y));
                gfx.DrawString(encounterform.Chest != null ? encounterform.Chest : "Not Available", font, XBrushes.Black, new XPoint(x + 35, y));


                gfx.DrawString("ABD:", font, XBrushes.Black, new XPoint(leftx, y));
                gfx.DrawString(encounterform.Abd != null ? encounterform.Abd : "Not Available", font, XBrushes.Black, new XPoint(leftx + 35, y));
                y += lineHeight;

                gfx.DrawString("Extr:", font, XBrushes.Black, new XPoint(x, y));
                gfx.DrawString(encounterform.Extr != null ? encounterform.Extr : "Not Available", font, XBrushes.Black, new XPoint(x + 35, y));


                gfx.DrawString("Skin:", font, XBrushes.Black, new XPoint(leftx, y));
                gfx.DrawString(encounterform.Skin != null ? encounterform.Skin : "Not Available", font, XBrushes.Black, new XPoint(leftx + 35, y));
                y += lineHeight;

                gfx.DrawString("Neuro:", font, XBrushes.Black, new XPoint(x, y));
                gfx.DrawString(encounterform.Neuro != null ? encounterform.Neuro : "Not Available", font, XBrushes.Black, new XPoint(x + 35, y));


                gfx.DrawString("Other:", font, XBrushes.Black, new XPoint(leftx, y));
                gfx.DrawString(encounterform.Other != null ? encounterform.Other : "Not Available", font, XBrushes.Black, new XPoint(leftx + 35, y));
                y += lineHeight;

                gfx.DrawString("Diognosis:", font, XBrushes.Black, new XPoint(x, y));
                gfx.DrawString(encounterform.Diognosis != null ? encounterform.Diognosis : "Not Available", font, XBrushes.Black, new XPoint(x + 80, y));


                gfx.DrawString("Treatment Plan:", font, XBrushes.Black, new XPoint(leftx, y));
                gfx.DrawString(encounterform.Treatmentplan != null ? encounterform.Treatmentplan : "Not Available", font, XBrushes.Black, new XPoint(leftx + 100, y));
                y += lineHeight;

                gfx.DrawString("Medications Dispensed:", font, XBrushes.Black, new XPoint(x, y));
                gfx.DrawString(encounterform.MedicationsDispesnsed != null ? encounterform.MedicationsDispesnsed : "Not Available", font, XBrushes.Black, new XPoint(x + 150, y));


                gfx.DrawString("Procedures:", font, XBrushes.Black, new XPoint(leftx, y));
                gfx.DrawString(encounterform.Procedures != null ? encounterform.Procedures : "Not Available", font, XBrushes.Black, new XPoint(leftx + 80, y));
                y += lineHeight;


                gfx.DrawString("Followup:", font, XBrushes.Black, new XPoint(x, y));
                gfx.DrawString(encounterform.Followup != null ? encounterform.Followup : "Not Available", font, XBrushes.Black, new XPoint(x + 80, y));
                y += lineHeight;


                using (var ms = new MemoryStream())
                {
                    document.Save(ms, false);
                    return ms.ToArray();
                }
            }
        }

        public bool SaveEncounterForm(Encounterform encounterform)
        {
            Encounterform encounterform1 = _context.Encounterforms.Where(q => q.Requestid == encounterform.Requestid).FirstOrDefault();

            if (encounterform1 != null)
            {
                try
                {

                    encounterform1.Lastname = encounterform.Lastname;
                    encounterform1.Firstname = encounterform.Firstname;
                    encounterform1.Location = encounterform.Location;
                    encounterform1.Dob = encounterform.Dob;
                    encounterform1.Createdat = encounterform.Createdat;
                    encounterform1.Email = encounterform.Email;
                    encounterform1.Phonnumber = encounterform.Phonnumber;
                    encounterform1.MedicalHistory = encounterform.MedicalHistory;
                    encounterform1.History = encounterform.History;
                    encounterform1.Medications = encounterform.Medications;
                    encounterform1.Allergies = encounterform.Allergies;
                    encounterform1.Temperature = encounterform.Temperature;
                    encounterform1.Hr = encounterform.Hr;
                    encounterform1.Rr = encounterform.Rr;
                    encounterform1.BloodPressure1 = encounterform.BloodPressure1;
                    encounterform1.BloodPressure2 = encounterform.BloodPressure2;
                    encounterform1.O2 = encounterform.O2;
                    encounterform1.Pain = encounterform.Pain;
                    encounterform1.Heent = encounterform.Heent;
                    encounterform1.Cv = encounterform.Cv;
                    encounterform1.Chest = encounterform.Chest;
                    encounterform1.Abd = encounterform.Abd;
                    encounterform1.Extr = encounterform.Extr;
                    encounterform1.Skin = encounterform.Skin;
                    encounterform1.Neuro = encounterform.Neuro;
                    encounterform1.Other = encounterform.Other;
                    encounterform1.Diognosis = encounterform.Diognosis;
                    encounterform1.Treatmentplan = encounterform.Treatmentplan;
                    encounterform1.MedicationsDispesnsed = encounterform.MedicationsDispesnsed;
                    encounterform1.Procedures = encounterform.Procedures;
                    encounterform1.Followup = encounterform.Followup;

                    _context.Encounterforms.Update(encounterform1);
                    _context.SaveChanges();
                    return true;
                }
                catch (Exception er)
                {
                    return false;
                }
            }
            else
            {
                try
                {
                    encounterform.IsFinelized = false;
                    _context.Encounterforms.Add(encounterform);
                    _context.SaveChanges();
                    return true;
                }
                catch (Exception er)
                {
                    return false;
                }
            }
        }

        public Encounterform GetEncounterFormByRequestId(string id)
        {
            int newid = int.Parse(id);

            return _context.Encounterforms.FirstOrDefault(q => q.Requestid == newid);
        }

        public bool? IsEncounterFormFinlized(int id)
        {
            return _context.Encounterforms.Where(q => q.Requestid == id).Select(r => r.IsFinelized).FirstOrDefault();


        }

        public CLoseCase GetDataForCloseCaseByRequestId(string id)
        {
            int newid = int.Parse(id);

            if (newid == 0)
            {
                return new CLoseCase();
            }

            CLoseCase? cLoseCase = _context.Requestclients.Where(q => q.Requestid == newid).Select(r => new CLoseCase
            {
                Firstname = r.Firstname,
                Lastname = r.Lastname,
                Phone = r.Phonenumber,
                Email = r.Email,
                Birthdate = new DateTime((r.Intyear ?? 0) == 0 ? 1 : (int)(r.Intyear ?? 0), DateTime.ParseExact(r.Strmonth, "MMMM", CultureInfo.InvariantCulture).Month, (r.Intdate ?? 0) == 0 ? 1 : (int)(r.Intdate ?? 0))
            }).FirstOrDefault();

            string confirmationnum = _context.Requests.Where(q => q.Requestid == newid).Select(r => r.Confirmationnumber).FirstOrDefault();

            if (confirmationnum.IsNullOrEmpty())
            {
                confirmationnum = "MD98098908";
            }

            List<ViewDocument> viewDocuments = _context.Requestwisefiles.Where(q => q.Requestid == newid).Select(r => new ViewDocument
            {
                FileId = r.Requestwisefileid,
                filename = r.Filename,
                uploadDate = r.Createddate,
            }).ToList();


            cLoseCase.ViewDocuments = viewDocuments;
            cLoseCase.ConfirmationNumber = confirmationnum;

            return cLoseCase;
        }

        public bool SaveDataForCloseState(CLoseCase cLoseCase)
        {
            int newid = int.Parse(cLoseCase.requestId);

            if (newid == 0)
            {
                return false;
            }
            Requestclient requestclient = _context.Requestclients.Where(q => q.Requestid == newid).FirstOrDefault();

            if (requestclient == null)
            {
                return false;
            }
            try
            {
                requestclient.Phonenumber = cLoseCase.Phone;
                requestclient.Email = cLoseCase.Email;

                _context.Requestclients.Update(requestclient);
                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool CloseCaseByRequestId(int requestid, int adminid)
        {



            Request request = _context.Requests.FirstOrDefault(q => q.Requestid == requestid);

            if (request != null)
            {
                request.Status = 2;
                request.Modifieddate = DateTime.Now;

                Requeststatuslog requeststatuslog = new Requeststatuslog()
                {
                    Requestid = requestid,
                    Status = request.Status,
                    Createddate = DateTime.Now,
                    Adminid = adminid

                };

                try
                {
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
            else
            {
                return false;
            }
        }



        public bool ExportExcelForCurrentPage(List<DashboardRequests> dashboardRequests)
        {
            return false;

        }

        public AdminProfile GetAdminProfileData(int adminid)
        {
            

            Admin admin = _context.Admins.FirstOrDefault(q => q.Adminid == adminid);

            if (admin != null)
            {

                AccountAndAdministratorInfo ai = new AccountAndAdministratorInfo()
                {
                    Firstname = admin.Firstname,
                    Lastname = admin.Lastname,
                    Confirmationemail = admin.Email,
                    Role = _context.Roles.Where(q => q.Roleid == admin.Roleid).Select(r => r.Name).FirstOrDefault(),
                    Phone = admin.Mobile,
                    Status = admin.Status == null?0: (int)(admin.Status),
                    AspnetAdminid = admin.Aspnetuserid

                };
                MailingAndBillingInfo mi = new MailingAndBillingInfo()
                {
                    State = _context.Regions.Where(q => q.Regionid == admin.Regionid).Select(r => r.Name).FirstOrDefault(),
                    AltPhone = admin.Altphone,
                    Address1 = admin.Address1,
                    Address2 = admin.Address2,
                    City = admin.City,
                    Zip = admin.Zip,

                };
                AdminProfile ap = new AdminProfile()
                {
                    accountInfo = ai,
                    mailingAndBillingInfo = mi
                };


                Aspnetuser aspnetuser = _context.Aspnetusers.Where(q=>q.Id == ap.accountInfo.AspnetAdminid).FirstOrDefault();


                   ap.accountInfo.Email = aspnetuser.Email;
                  ap.accountInfo.Username = aspnetuser.Username;
                List<Role> roles = _context.Roles.ToList();
                List<Region> regions = _context.Regions.ToList();

                List<SelectedRegions> selectedRegions = new List<SelectedRegions>();

                List<Adminregion> adminregion = _context.Adminregions.Where(q => q.Adminid == adminid).ToList();

                 for(int i = 0; i < regions.Count; i++)
                {
                        SelectedRegions selectedRegions1 = new SelectedRegions();
                        selectedRegions1.regionId = regions[i].Regionid;
                        selectedRegions1.regionName = regions[i].Name;

                    if(adminregion != null && adminregion.Any(q=>q.Regionid == regions[i].Regionid))
                    {
                        selectedRegions1.IsSelected = true;
                    }
                    else
                    {
                        selectedRegions1.IsSelected = false;
                    }
                    selectedRegions.Add(selectedRegions1);
                }

                ap.accountInfo.SelectedRegions = selectedRegions;
                ap.accountInfo.roles = roles;
                ap.mailingAndBillingInfo.regions = regions;

                return ap;
            }

            return new AdminProfile();
        }

        public bool ResetAdminPassword(int adminId,string password)
        {
            string aspnetId = _context.Admins.Where(q => q.Adminid == adminId).Select(r => r.Aspnetuserid).FirstOrDefault();

            Aspnetuser aspnetuser = _context.Aspnetusers.Where(q => q.Id == aspnetId).FirstOrDefault();

            try
            {
                aspnetuser.Passwordhash = password;

                _context.Aspnetusers.Update(aspnetuser);
                _context.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public bool SaveAdminAccountInfo(AdminProfile ap,int adminId)
        {
            try
            {

                 Admin admin = _context.Admins.FirstOrDefault(q => q.Adminid == adminId);
                Aspnetuser aspnetuser = _context.Aspnetusers.FirstOrDefault(q=>q.Id == admin.Aspnetuserid);

                aspnetuser.Username = ap.accountInfo.Username;
                aspnetuser.Email = ap.accountInfo.Email;
                admin.Firstname = ap.accountInfo.Firstname;
                admin.Lastname = ap.accountInfo.Lastname;
                admin.Mobile = ap.accountInfo.Phone;
                admin.Status = (short)ap.accountInfo.Status;
                admin.Email = ap.accountInfo.Confirmationemail;
                admin.Roleid = _context.Roles.Where(q => q.Name == ap.accountInfo.Role).Select(r => r.Roleid).FirstOrDefault();


                for(int i = 0; i < ap.accountInfo.SelectedRegions.Count; i++)
                {
                    SelectedRegions selectedRegion = ap.accountInfo.SelectedRegions[i];

                    if (selectedRegion.IsSelected == true && (_context.Adminregions.Any(q=>q.Adminid == adminId && q.Adminregionid == selectedRegion.regionId) == false))
                    {
                        Adminregion adminregion = new Adminregion();
                        adminregion.Regionid = (int)selectedRegion.regionId;
                        adminregion.Adminid = adminId;
                        _context.Adminregions.Add(adminregion);
                    }else if(selectedRegion.IsSelected == false && (_context.Adminregions.Any(q => q.Adminid == adminId && q.Adminregionid == selectedRegion.regionId) == true))
                    {
                        Adminregion adminregion = _context.Adminregions.FirstOrDefault(q => q.Adminid == adminId && q.Adminregionid == selectedRegion.regionId);
                        _context.Adminregions.Remove(adminregion);
                    }
                }


                _context.Admins.Update(admin);
                _context.SaveChanges();
                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }

        public bool SaveAdminMailingAndBillingInfo(AdminProfile ap, int adminId)
        {
            try
            {

                Admin admin = _context.Admins.FirstOrDefault(q => q.Adminid == adminId);

                admin.Address1 = ap.mailingAndBillingInfo.Address1;
                admin.Address2 = ap.mailingAndBillingInfo.Address2;
                admin.City = ap.mailingAndBillingInfo.City;
                admin.Regionid = _context.Regions.Where(q=>q.Name == ap.mailingAndBillingInfo.State).Select(r=>r.Regionid).FirstOrDefault();
                admin.Zip = ap.mailingAndBillingInfo.Zip;
                admin.Altphone = ap.mailingAndBillingInfo.AltPhone;

                _context.Admins.Update(admin);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public List<ProviderMenu> GetAllProviders()
        {
            List<ProviderMenu> providers = _context.Physicians.Select(r => new ProviderMenu()
            {
                Name = r.Firstname + " " + r.Lastname,
                IsNoificationOn = (bool)_context.Physiciannotifications.Where(q => q.Physicianid == r.Physicianid).Select(r => r.Isnotificationstopped).FirstOrDefault(),
                Role = _context.Roles.Where(q=>q.Roleid == r.Roleid).Select(r=>r.Name).FirstOrDefault(),
                status = (int)r.Status,
                ProviderId = r.Physicianid
            }).ToList();

            return providers;
        }


        public bool SaveProviderChanges(List<ProviderMenu> providers)
        {
            try
            {

                foreach(ProviderMenu provider in providers)
                {

                    Physiciannotification physiciannotification = _context.Physiciannotifications.Where(q => q.Physicianid == provider.ProviderId).FirstOrDefault();

                    physiciannotification.Isnotificationstopped = provider.IsNoificationOn;

                    _context.Physiciannotifications.Update(physiciannotification);
                }

                _context.SaveChanges();

                return true;
            } 
            catch(Exception ex)
            {
                return false;
            }
        }

        public List<Physicianlocation> GetAllPhysicianlocation()
        {
            List<Physicianlocation> physicianlocations = _context.Physicianlocations.ToList();

            return physicianlocations;
        }

        public string GetExcelFile(List<DashboardRequests> dashboardRequests)
        {

            try
            {

            string[] header = { "Name", "Date of Birth", "Requestor", "Phonenumber", "Address", "Notes" };

            using (var package = new XLWorkbook())
            {

                var worksheet = package.Worksheets.Add("Sheet1");
                int row = 1;
                for (int i = 0; i < header.Length; i++)
                {
                    worksheet.Cell(row, i + 1).Value = header[i];
                }

                row++;
                int j = 0;
                foreach (DashboardRequests req in dashboardRequests)
                {

                    worksheet.Cell(row, j + 1).Value = req.Username;

                    j++;

                    worksheet.Cell(row, j + 1).Value = req.Birthdate;

                    j++;

                    worksheet.Cell(row, j + 1).Value = req.Requestor;
                    j++;
                    worksheet.Cell(row, j + 1).Value = req.Phone + "/" + req.RequestorPhone;
                    j++;
                    worksheet.Cell(row, j + 1).Value = req.Address;
                    j++;
                    //worksheet.Cell(row, j + 1).Value = req.Notes;

                    j = 0;
                    row++;




                }

                byte[] fileBytes;
                using (var stream = new MemoryStream())
                {
                    package.SaveAs(stream);
                    stream.Seek(0, SeekOrigin.Begin);

                    fileBytes = stream.ToArray();
                    string fileName = Guid.NewGuid().ToString() + ".xlsx";
                    string filePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory())) + "\\wwwroot\\Upload";

                    string path = Path.Combine(filePath, fileName);

                   System.IO.File.WriteAllBytes(path, fileBytes);

                    string fileUrl = path;

                    return  "https://localhost:7008/Upload/" + fileName ;

                }
            }
            }catch(Exception ex)
            {
                return "";
            }
        }




    }


}
