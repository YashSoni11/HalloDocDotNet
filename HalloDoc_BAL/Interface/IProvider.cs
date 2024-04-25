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

        public bool CreateProviderAccount(CreateProviderAccount createProviderAccount,int adminId);

       public List<AccessAreas> GetAreaAccessByAccountType(int accountType, int roleId = 0);   

       public bool CreateRole(CreateRole createRole,int adminId);

        public bool DeleteRole(int roleId,int adminId);

        public bool CreateAdminAccount(CreateAdminAccountModel adminAccount,int adminId);

        public Role GetRoleById(int roleId);

        public bool EditRoleService(CreateRole createRole,int adminId, int id);

        public List<DayWisePhysicianShifts> GetAllPhysicianDayWiseShifts(int date,int month,int year,int regionId);

        public List<Physician> GetPhysicinForShiftsByRegionService(int regionId);


        public bool CreateShiftService(CreateShift createShift,int adminId,string Role);

        public List<RequestedShiftDetails> GetRequestedShiftDetails(int regionId);

        public bool ApproveShiftsService(List<RequestedShiftDetails> requestedShiftDetails, int adminId);

        public List<WeekWisePhysicianShifts> GetAllPhysicianWeekWiseShifts(int date, int month, int year, int regionId);

        public ViewShift GetShiftDetailsById(int shiftDetailsId);
        public List<MonthWisePhysicianShifts> GetAllPhysicianMonthWiseShifts(int date, int month, int year, int regionId);

        public List<ShiftInformation> GetDayWiseAllShiftInformation(int date, int month, int year, int regionId);

        public bool IsValidShift(CreateShift createShift);

        public List<UserAccess> GetAllAspNetUsers(int roleId);
        public bool DeleteRequestedShifts(List<RequestedShiftDetails> requestedShifts, int adminId);

        public bool DeleteShiftService(int shiftDetailId, int adminId);


        public bool EditShiftService(ViewShift viewShift, int adminId);

        public List<VendorList> GetVendorsData(string vendorName, int HealthProfessionId);

        public List<Healthprofessionaltype> GetAllHealthProfessionalTypes();

        public bool EditVendor(VendorDetails vendorDetails,int adminId,int id);

        public VendorDetails EditVendorDetailsView(int id);

        public bool DeleteVendorService(int id);

        public bool AddVendorService(VendorDetails vendorDetails);


        public List<SearchRecords> GetFillteredSearchRecordsData(int Status, string PatientName, int RequestType, DateTime FromDate, DateTime ToDate, string ProviderName, string Email, string Phone);

        public bool DeleteRecordService(int RequestId);

        public List<EmailLogs> GetFillteredEmailLogsData(string ReciverName, int RoleId, string EmailId, DateTime CreateDate, DateTime SentDate);


        public List<PatientHistory> GetPatientHistoryData(string FirstName, string LastName, string Email, string Phone);

        public bool EditOnBoardingData(ProviderDocuments providerDocuments, int physicianid);

        public List<PatientExplore> GetpatientExploreData(int userId);

        public List<BlockHistories> GetBlockHistoriesData(string name, DateTime createdAt, string Email, string Phone);


        public bool IsRequestBelongToProvider(int physicianId,int requestId);

        public string GetSearchRecordsExcelFIle(List<SearchRecords> sr);

    }
}
