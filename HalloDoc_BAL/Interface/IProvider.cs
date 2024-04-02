using HalloDoc_DAL.Models;
using HalloDoc_DAL.ProviderViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.Interface
{
    public interface IProvider
    {

           public ProviderProfileView GetProviderData(int providerId);

        public bool SaveProviderAccountInfo(ProviderAccountInfo providerAccountInfo, int provideId);

        public bool ResetProviderAccountPassword(string password, int id);


        public bool SaveProviderInformation(ProviderInformation providerInformation, int provideId);    

        public bool SaveProviderMailingAndBillingInfo(ProviderMailingAndBillingInfo providerMailingAndBillingInfo, int provideId);

        public bool SaveProviderProfileInfo(ProviderProfileInfo providerProfileInfo, int provideId);

        public bool DeleteProviderAccount(int providerId);


        public List<Role> GetAllRoles();

        public bool CreateProviderAccount(CreateProviderAccount createProviderAccount);

       public List<AccessAreas> GetAreaAccessByAccountType(int accountType, int roleId = 0);   

       public bool CreateRole(CreateRole createRole,int adminId);

        public bool DeleteRole(int roleId,int adminId);

        public bool CreateAdminAccount(CreateAdminAccountModel adminAccount,int adminId);

        public Role GetRoleById(int roleId);

        public bool EditRoleService(CreateRole createRole,int adminId, int id);

        public List<DayWisePhysicianShifts> GetAllPhysicianDayWiseShifts(int date,int month,int year,int regionId);

        public List<Physician> GetPhysicinForShiftsByRegionService(int regionId);


        public bool CreateShiftService(CreateShift createShift,int adminId);

        public List<RequestedShiftDetails> GetRequestedShiftDetails(int regionId);

        public bool ApproveShiftsService(List<RequestedShiftDetails> requestedShiftDetails, int adminId);

        public List<WeekWisePhysicianShifts> GetAllPhysicianWeekWiseShifts(int date, int month, int year, int regionId);

        public ViewShift GetShiftDetailsById(int shiftDetailsId);
        public List<MonthWisePhysicianShifts> GetAllPhysicianMonthWiseShifts(int date, int month, int year, int regionId);


    }
}
