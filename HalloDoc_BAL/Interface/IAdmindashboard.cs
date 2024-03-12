﻿using HalloDoc_DAL.ViewModels;
using HalloDoc_DAL.AdminViewModels;
using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Utilities;

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

        public string GetAdminUsername(int id);

        public List<Healthprofessionaltype> GetOrderDetails();

        public List<Healthprofessional> GetHealthProfessionalsByProfessionId(int id);

        public Healthprofessional GetVendorByVendorId(int id);

        public Request TransferRequest(AdminAssignCase adminAssignCase, int requestId,int adminId);

        public bool SendDocumentsViaEmail(dynamic data);

        public bool ClearCaseByRequestid(string id,int adminId);

        public bool  CancleAgrrementByRequstId(CancleAgreement cancleAgreement,string requestId);

        public bool AgreeAgreementByRequestId(string requestId);

        public SendAgreement GetSendAgreementpopupInfo(string requestId);

        public byte[] GeneratePdf(Encounterform encounterform);

        public bool SaveEncounterForm(Encounterform encounterform);

        public Encounterform GetEncounterFormByRequestId(string id);

        public bool? IsEncounterFormFinlized(int id);
    }
}
