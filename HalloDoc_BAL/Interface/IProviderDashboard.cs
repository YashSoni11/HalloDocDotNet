using HalloDoc_DAL.AdminViewModels;
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

    }
}
