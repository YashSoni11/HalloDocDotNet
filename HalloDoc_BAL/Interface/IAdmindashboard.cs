using HalloDoc_DAL.ViewModels;
using HalloDoc_DAL.AdminViewModels;
using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.Interface
{
    public interface IAdmindashboard
    {


        public List<DashboardRequests> GetAllRequests();

        public List<DashboardRequests> GetRequestsByUsername(string name);

        public RequestTypeCounts GetAllRequestsCount(List<DashboardRequests> dashboardRequests);

        public List<DashboardRequests> GetStatuswiseRequests(string[] status);

        public List<DashboardRequests> GetRequestsFromRequestorType(int type, string[] statusArray, string region, string name);

        public ClientRequest GetUserInfoFromRequestId(int requestId);

        public RequestNotes GetNotesFromRequestId(int requestId);

        public ClientRequest UpdateClientRequest(ClientRequest clientRequest, int newrequestid);

        public string GetPatientName(int id);

        public Request UpdateRequestToClose(AdminCancleCase adminCancleCase, int id);

        public List<Region> GetAllRegions();

        public List<Physician> GetAllPhysician();

        public List<Physician> FilterPhysicianByRegion(int regionid);

        public Request AssignRequest(AdminAssignCase assignRequest, int requestId);

        public Request BlockRequest(AdminBlockCase blockRequest, int requestId);

        public List<ViewDocument> GetDocumentsByRequestId(int requestid);

        public void DeleteFile(int id);

        public void DeleteAllFiles(int[] IdArray);

        public bool PostOrderById(int id, Order order);
    }
}
