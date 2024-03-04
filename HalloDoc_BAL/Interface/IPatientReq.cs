﻿using HalloDoc_DAL.Models;
using HalloDoc_DAL.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.Interface
{
    public interface IPatientReq
    {

      public  void AddPatientReq(PatientReq pr);

      public Object CheckEmail(string email);

        public bool IsEmailExistance(string email);
        public Aspnetuser GetAspNetUserByEmail(string email);

        public int GetUserIdByEmail (string email);

        public Aspnetuser AddAspNetUser(PatientReq pr, string Password);

        public int AddUser(string AspNetUserId, PatientReq pr, AddressModel Location);

        public Request AddRequest(CmnInformation cm, int userId, string requestType,string state);

        public bool AddRequestClient(PatientReq patientReq,int requestId, AddressModel Location);

        public bool AddBusinessRequest(int businessId, int reqId);

        public Business AddBusiness(CmnInformation cm, AddressModel Location);


        public Concierge Addconciearge(AddressModel concieargeLocation, string name);

        public bool AddConciergeRequest(int conciergeId, int reqId);

        public User GetUserDataById(int userId);

        public bool UploadFile(IFormFile file,int request);


        public void AddConcieargeData(ConcieargeModel concieargeModel,int userid);

        public string GetHashedPassword(string password);


       public Requestclient GetRequestClientByEmail(string email);

        public Request UpdateRequestByRequestId(int requestId,int userId);

        public bool  IsRegionAvailable(string  region);

        public bool  IsEmailBlocked(string email);

        public bool IsPhoneBlocked(string phone);


        public object GetInfoValidation(string email, string phone, string region);

    }
}



