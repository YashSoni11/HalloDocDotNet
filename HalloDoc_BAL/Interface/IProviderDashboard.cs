using HalloDoc_DAL.AdminViewModels;
using HalloDoc_DAL.Models;
using HalloDoc_DAL.ProviderViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.Interface
{
    public interface IProviderDashboard
    {
        public List<DashboardRequests> GetAllProviderRequests(int physicianId);


        public List<DashboardRequests> GetStatuswiseProviderRequestsService(string[] statusArray, int PhysicianId);

        public List<DashboardRequests> GetProviderRequestsFromRequestorTypeService(int type, string[] statusArray,  string name,int PhysicianId);



        public bool AcceptRequest(int requestId, int physicianId);


        public bool SaveTypeOfCareService(int requestId,bool HouseCall);

        public bool HouseCallActionService(int id);

        public ConcludeCare GetConcludeCareDetails(int requestId);

        public bool ConcludeCareService(ConcludeCare concludeCare, int requestId, int UserId, string Role);

        public bool IsFormFinlized(int requestId);


        public bool FinalizeEncounterformService(int id);

        public List<Region> GetAllPhysicianRegions(int PhysicinId);

        public List<MonthWisePhysicianShifts> GetPhysicianMonthWiseShifts(int date, int month, int year, int PhysicianId);

        public bool TrasnferToAdminService(string message, int providerId, int requestId);

    }
}
