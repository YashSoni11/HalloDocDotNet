using DocumentFormat.OpenXml.Bibliography;
using HalloDoc_BAL.Interface;
using HalloDoc_DAL.AdminViewModels;
using HalloDoc_DAL.Context;
using HalloDoc_DAL.Models;
using HalloDoc_DAL.ProviderViewModels;
using HalloDoc_DAL.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HalloDoc_BAL.Repositery
{
    public class ProviderServices : IProvider
    {

        private readonly HalloDocContext _context;

        public ProviderServices(HalloDocContext context)
        {
            _context = context;
        }

        string[] days = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
        public string UploadProviderFiles(IFormFile file)
        {

            string path = "";
            bool iscopied = false;



            try
            {
                if (file.Length > 0)
                {
                    string filename = file.FileName;
                    path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory())) + "\\wwwroot\\Upload";
                    using (var filestream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }

                    return filename;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }




        }
        public ProviderProfileView GetProviderData(int providerId)
        {
            Physician physician = _context.Physicians.FirstOrDefault(q => q.Physicianid == providerId);


            ProviderProfileView providerProfileView = new ProviderProfileView();

            ProviderAccountInfo providerAccountInfo = new ProviderAccountInfo()
            {
                UserName = _context.Aspnetusers.Where(q => q.Id == physician.Aspnetuserid).Select(r => r.Username).FirstOrDefault(),
                Status = physician.Status,
                Role = _context.Roles.Where(q => q.Roleid == physician.Roleid).Select(r => r.Name).FirstOrDefault(),
                roles = _context.Roles.ToList(),

            };

            ProviderInformation providerInformation = new ProviderInformation()
            {
                FirstName = physician.Firstname,
                LastName = physician.Lastname,
                Email = physician.Email,
                PhoneNumber = physician.Mobile,
                MedicalLicenseNumber = physician.Medicallicense,
                NPINumber = physician.Npinumber,
                SyncEmail = physician.Syncemailaddress,
            };

            ProviderMailingAndBillingInfo providerMailingAndBillingInfo = new ProviderMailingAndBillingInfo()
            {
                Address1 = physician.Address1,
                Address2 = physician.Address2,
                City = physician.City,
                StateId = physician.Regionid,
                StateName = _context.Regions.Where(q => q.Regionid == physician.Regionid).Select(r => r.Name).FirstOrDefault(),
                Phone = physician.Altphone,
                Zip = physician.Zip,
                regions = _context.Regions.ToList(),
            };

            ProviderProfileInfo providerProfileInfo = new ProviderProfileInfo()
            {
                BusinessName = physician.Businessname,
                BusinessWebsite = physician.Businesswebsite,
                SignatureImage = GetImageBytesFromFile(physician.Signature),

            };

            providerProfileView.AccountInfo = providerAccountInfo;
            providerProfileView.ProviderMailingAndBillingInfo = providerMailingAndBillingInfo;
            providerProfileView.ProviderProfileInfo = providerProfileInfo;
            providerProfileView.ProviderInformation = providerInformation;


            return providerProfileView;

        }

        public bool SaveProviderAccountInfo(ProviderAccountInfo providerAccountInfo, int provideId)
        {
            try
            {


                Physician physician = _context.Physicians.FirstOrDefault(q => q.Physicianid == provideId);
                Aspnetuser aspnetuser = _context.Aspnetusers.FirstOrDefault(q => q.Id == physician.Aspnetuserid);

                aspnetuser.Username = providerAccountInfo.UserName;
                physician.Status = (short)providerAccountInfo.Status;
                physician.Roleid = _context.Roles.Where(q => q.Name == providerAccountInfo.Role).Select(q => q.Roleid).FirstOrDefault();

                _context.Aspnetusers.Update(aspnetuser);
                _context.Physicians.Update(physician);

                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }


        public bool ResetProviderAccountPassword(string password, int id)
        {
            try
            {
                string aspnetId = _context.Physicians.Where(q => q.Physicianid == id).Select(r => r.Aspnetuserid).FirstOrDefault();

                Aspnetuser aspnetuser = _context.Aspnetusers.FirstOrDefault(q => q.Id == aspnetId);

                aspnetuser.Passwordhash = password;

                _context.Aspnetusers.Update(aspnetuser);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SaveProviderInformation(ProviderInformation pi, int provideId)
        {
            try
            {


                Physician physician = _context.Physicians.FirstOrDefault(q => q.Physicianid == provideId);

                physician.Firstname = pi.FirstName;
                physician.Lastname = pi.LastName;
                physician.Email = pi.Email;
                physician.Mobile = pi.PhoneNumber;
                physician.Medicallicense = pi.MedicalLicenseNumber;
                physician.Npinumber = pi.NPINumber;
                physician.Syncemailaddress = pi.SyncEmail;


                _context.Physicians.Update(physician);

                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public bool SaveProviderMailingAndBillingInfo(ProviderMailingAndBillingInfo pm, int provideId)
        {
            try
            {


                Physician physician = _context.Physicians.FirstOrDefault(q => q.Physicianid == provideId);

                physician.Address1 = pm.Address1;
                physician.Address2 = pm.Address2;
                physician.City = pm.City;
                physician.Zip = pm.Zip;
                physician.Altphone = pm.Phone;


                _context.Physicians.Update(physician);

                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public bool SaveProviderProfileInfo(ProviderProfileInfo pp, int provideId)
        {
            try
            {


                Physician physician = _context.Physicians.FirstOrDefault(q => q.Physicianid == provideId);

                physician.Businessname = pp.BusinessName;
                physician.Businesswebsite = pp.BusinessWebsite;
                physician.Adminnotes = pp.AdminNotes;


                if (pp.Photo != null)
                {
                    string? file = UploadProviderFiles(pp.Photo);
                    if (file == null)
                    {
                        return false;
                    }
                    physician.Photo = file;
                }
                if (pp.Signature != null)
                {
                    string? file = UploadProviderFiles(pp.Signature);
                    if (file == null)
                    {
                        return false;
                    }
                    physician.Signature = file;
                }

                _context.Physicians.Update(physician);

                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public bool DeleteProviderAccount(int providerId)
        {
            try
            {
                Physician physician = _context.Physicians.FirstOrDefault(q => q.Physicianid == providerId);

                physician.Isdeleted = true;


                _context.Physicians.Update(physician);

                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }


        }


        public byte[] GetImageBytesFromFile(string filename)
        {
            try
            {

                string imagePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory())) + "\\wwwroot\\Upload\\" + filename;


                if (!System.IO.File.Exists(imagePath))
                {
                    return null;
                }


                byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);


                string contentType = GetContentType(filename);


                return imageBytes;
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        private string GetContentType(string fileName)
        {
            // You can enhance this logic to handle other image formats
            if (fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            {
                return "image/jpeg";
            }
            else if (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            {
                return "image/png";
            }
            // Add more content types as needed (e.g., .gif, .bmp, etc.)

            return "application/octet-stream"; // Default content type
        }


        public List<Role> GetAllRoles()
        {
            return _context.Roles.Where(q => q.Isdeleted == false).ToList();
        }

        public Aspnetuser AddAspNetUser(Aspnetuser pr)
        {

            try
            {

                Aspnetuser aspNetUser1 = new Aspnetuser
                {
                    Id = Guid.NewGuid().ToString(),
                    Username = pr.Username,
                    Email = pr.Email,
                    Passwordhash = pr.Passwordhash,
                    Phonenumber = pr.Phonenumber,
                    Createddate = DateTime.Now
                };


                _context.Aspnetusers.Add(aspNetUser1);

                _context.SaveChanges();

                return aspNetUser1;
            }
            catch (Exception ex)
            {
                return new Aspnetuser();
            }
        }


        public bool CreateProviderAccount(CreateProviderAccount createProviderAccount)
        {

            try
            {
                Aspnetuser aspnetuser = new Aspnetuser()
                {
                    Username = createProviderAccount.UserName,
                    Email = createProviderAccount.Email,
                    Passwordhash = createProviderAccount.Password,
                    Phonenumber = createProviderAccount.Phone
                };


                Aspnetuser ProviderAspNetUser = AddAspNetUser(aspnetuser);

                if (ProviderAspNetUser == null)
                {
                    return false;
                }

                Physician physician = new Physician();

                physician.Aspnetuserid = ProviderAspNetUser.Id;
                physician.Firstname = createProviderAccount.FirstName;
                physician.Lastname = createProviderAccount.LastName;
                physician.Email = createProviderAccount.Email;
                physician.Roleid = createProviderAccount.Role;
                physician.Medicallicense = createProviderAccount.MedicalLicenseNumber;
                physician.Npinumber = createProviderAccount.NPINumber;
                physician.Address1 = createProviderAccount.Address1;
                physician.Address2 = createProviderAccount.Address2;
                physician.City = createProviderAccount.City;
                physician.Regionid = createProviderAccount.StateId;
                physician.Status = (short)createProviderAccount.Status;
                physician.Zip = createProviderAccount.Zip;
                physician.Adminnotes = createProviderAccount.AdminNotes;
                physician.Altphone = createProviderAccount.AltPhone;
                physician.Businessname = createProviderAccount.BusinessName;
                physician.Businesswebsite = createProviderAccount.BusinessWebsite;
                physician.Isdeleted = false;


                if (createProviderAccount.Photo != null)
                {
                    string fileName = UploadProviderFiles(createProviderAccount.Photo);
                    if (!fileName.IsNullOrEmpty())
                    {
                        physician.Photo = fileName;
                    }
                }


                _context.Physicians.Add(physician);

                _context.SaveChanges();

                return false;


            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool CreateAdminAccount(CreateAdminAccountModel createAdminAccount, int adminId)
        {
            try
            {
                Aspnetuser aspnetuser = new Aspnetuser()
                {
                    Username = createAdminAccount.UserName,
                    Email = createAdminAccount.Email,
                    Passwordhash = createAdminAccount.Password,
                    Phonenumber = createAdminAccount.Phone
                };


                Aspnetuser AdminAspNetUser = AddAspNetUser(aspnetuser);
                string aspnetId = _context.Admins.Where(q => q.Adminid == adminId).Select(q => q.Aspnetuserid).FirstOrDefault();

                if (AdminAspNetUser == null)
                {
                    return false;
                }

                Admin admin = new Admin();


                admin.Aspnetuserid = AdminAspNetUser.Id;
                admin.Firstname = createAdminAccount.FirstName;
                admin.Lastname = createAdminAccount.LastName;
                admin.Email = createAdminAccount.Email;
                admin.Roleid = createAdminAccount.Role;
                admin.Address1 = createAdminAccount.Address1;
                admin.Address2 = createAdminAccount.Address2;
                admin.City = createAdminAccount.City;
                admin.Regionid = createAdminAccount.StateId;
                admin.Zip = createAdminAccount.Zip;
                admin.Altphone = createAdminAccount.AltPhone;
                admin.Createdby = aspnetId;
                admin.Isdeleted = false;




                _context.Admins.Add(admin);

                _context.SaveChanges();

                return true;


            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<AccessAreas> GetAreaAccessByAccountType(int accountType, int roleId = 0)
        {
            List<AccessAreas> menus = _context.Menus.Where(q => q.Accounttype == accountType).Select(r => new AccessAreas
            {
                AreaId = r.Menuid,
                AreaName = r.Name
            }).ToList();

            if (roleId != 0)
            {
                for (int i = 0; i < menus.Count; i++)
                {
                    if (_context.Rolemenus.Any(q => q.Roleid == roleId && menus[i].AreaId == q.Menuid))
                    {
                        menus[i].IsAreaSelected = true;
                    }
                }
            }

            return menus;
        }

        public bool CreateRole(CreateRole createRole, int adminId)
        {
            try
            {
                string aspnetId = _context.Admins.Where(q => q.Adminid == adminId).Select(r => r.Aspnetuserid).FirstOrDefault();
                Role role = new Role();
                int lasRoleId = _context.Roles.OrderByDescending(q => q.Roleid).Select(q => q.Roleid).FirstOrDefault();

                role.Roleid = lasRoleId + 1;
                role.Name = createRole.RoleName;
                role.Accounttype = (short)createRole.AccountType;
                role.Createddate = DateTime.Now;
                role.Createdby = aspnetId;
                role.Isdeleted = false;

                _context.Roles.Add(role);



                for (int i = 0; i < createRole.AccessAreas.Count; i++)
                {
                    if (createRole.AccessAreas[i].IsAreaSelected)
                    {
                        Rolemenu rolemenu = new Rolemenu();

                        rolemenu.Menuid = createRole.AccessAreas[i].AreaId;
                        rolemenu.Roleid = role.Roleid;


                        _context.Rolemenus.Add(rolemenu);
                    }

                }

                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        public bool DeleteRole(int roleId, int adminId)
        {
            try
            {


                Role role = _context.Roles.FirstOrDefault(r => r.Roleid == roleId);
                string aspnetId = _context.Admins.Where(q => q.Adminid == adminId).Select(r => r.Aspnetuserid).FirstOrDefault();

                role.Isdeleted = true;
                role.Modifieddate = DateTime.Now;
                role.Modifiedby = aspnetId;

                _context.Roles.Update(role);

                _context.SaveChanges();


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public Role GetRoleById(int roleId)
        {
            return _context.Roles.FirstOrDefault(q => q.Roleid == roleId && q.Isdeleted == false);
        }

        public bool EditRoleService(CreateRole createRole, int adminId, int Roleid)
        {
            try
            {

                Role role = _context.Roles.Where(q => q.Roleid == Roleid && q.Isdeleted == false).FirstOrDefault();

                role.Name = createRole.RoleName;
                role.Accounttype = (short)createRole.AccountType;
                role.Modifieddate = DateTime.Now;
                role.Modifiedby = _context.Admins.Where(q => q.Adminid == adminId).Select(r => r.Aspnetuserid).FirstOrDefault();

                for (int i = 0; i < createRole.AccessAreas.Count; i++)
                {
                    if (createRole.AccessAreas[i].IsAreaSelected == true && (_context.Rolemenus.Any(q => q.Roleid == Roleid && q.Menuid == createRole.AccessAreas[i].AreaId) == false))
                    {
                        Rolemenu rolemenu = new Rolemenu();
                        rolemenu.Roleid = Roleid;
                        rolemenu.Menuid = createRole.AccessAreas[i].AreaId;
                        _context.Rolemenus.Add(rolemenu);
                    }
                    else
                    {
                        Rolemenu rolemenu = _context.Rolemenus.FirstOrDefault(q => q.Roleid == Roleid && q.Menuid == createRole.AccessAreas[i].AreaId == true);
                        if (rolemenu != null)
                        {
                            _context.Rolemenus.Remove(rolemenu);
                        }
                    }
                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<Physician> GetPhysicinForShiftsByRegionService(int regionId)
        {
            List<Physician> physicians = new List<Physician>();
            if (regionId != 0)
            {
                physicians = _context.Physicians.Where(q => q.Regionid == regionId && q.Isdeleted == false).ToList();
            }
            else
            {
                physicians = _context.Physicians.Where(q => q.Isdeleted == false).ToList();
            }

            return physicians;
        }
        public List<ShiftInformation> GetAllPhysicianDayWiseShifts(int date, int month, int year, int regionId)
        {

            List<Shiftdetail> shiftdetails = _context.Shiftdetails.ToList();


            List<Physician> physicians = GetPhysicinForShiftsByRegionService(regionId);

            List<Shift> shifts = _context.Shifts.ToList();
            List<ShiftInformation> dayWiseShifts = new List<ShiftInformation>();

            foreach (Physician physician in physicians)
            {
                List<Shift> PhysicianShifts = _context.Shifts.Where(q => q.Physicianid == physician.Physicianid).ToList();

                if (PhysicianShifts.Count == 0)
                {
                    ShiftInformation dayWiseShift = new ShiftInformation();
                    dayWiseShift.physicianId = physician.Physicianid;
                    dayWiseShift.PhysicianName = physician.Firstname + " " + physician.Lastname;
                    dayWiseShift.startTime = -1;
                    dayWiseShifts.Add(dayWiseShift);
                }
                else
                {
                    bool IsTodayShiftFound = false;

                    foreach (Shift shift in PhysicianShifts)
                    {
                        DateOnly currentDate = new DateOnly(year, month, date);
                        Shiftdetail shiftdetail = _context.Shiftdetails.FirstOrDefault(q => (q.Shiftid == shift.Shiftid) && (q.Shiftdate.Year == currentDate.Year) && (q.Shiftdate.Month == currentDate.Month) && (q.Shiftdate.Day == currentDate.Day) && (q.Isdeleted == false));



                        if (shiftdetail == null)
                        {

                            continue;
                        }
                        else
                        {
                            IsTodayShiftFound = true;
                            ShiftInformation dayWiseShift = new ShiftInformation();

                            dayWiseShift.physicianId = physician.Physicianid;
                            dayWiseShift.PhysicianName = physician.Firstname + " " + physician.Lastname;
                            dayWiseShift.startTime = shiftdetail.Starttime.Hour;
                            dayWiseShift.endTime = shiftdetail.Endtime.Hour;
                            dayWiseShift.status = shiftdetail.Status;
                            dayWiseShifts.Add(dayWiseShift);
                        }

                    }

                    if (IsTodayShiftFound == false)
                    {
                        ShiftInformation dayWiseShift = new ShiftInformation();
                        dayWiseShift.physicianId = physician.Physicianid;
                        dayWiseShift.PhysicianName = physician.Firstname + " " + physician.Lastname;
                        dayWiseShift.startTime = -1;
                        dayWiseShifts.Add(dayWiseShift);
                    }

                }
            }

            return dayWiseShifts;
        }


        public bool CreateShiftService(CreateShift createShift, int adminId)
        {


            try
            {
                Shift shift = new Shift();

                shift.Shiftid = _context.Shifts.OrderByDescending(q => q.Shiftid).Select(q => q.Shiftid).FirstOrDefault() + 1;
                shift.Physicianid = createShift.PhysicianId;
                shift.Startdate = new DateOnly(createShift.ShiftDate.Year, createShift.ShiftDate.Month, createShift.ShiftDate.Day);
                shift.Repeatupto = createShift.RepeatUpto;
                shift.Createdby = _context.Admins.Where(q => q.Adminid == adminId).Select(r => r.Aspnetuserid).FirstOrDefault();
                shift.Isrepeat = createShift.IsReapet;
                shift.Createddate = DateTime.Now;

                _context.Shifts.Add(shift);


                Shiftdetail firtShiftdetail = new Shiftdetail();

                firtShiftdetail.Shiftid = shift.Shiftid;
                firtShiftdetail.Shiftdate = createShift.ShiftDate;
                firtShiftdetail.Starttime = new TimeOnly(createShift.StartTime.Hour, createShift.StartTime.Minute, createShift.StartTime.Second);
                firtShiftdetail.Endtime = new TimeOnly(createShift.EndTime.Hour, createShift.EndTime.Minute, createShift.EndTime.Second);
                firtShiftdetail.Isdeleted = false;
                firtShiftdetail.Status = 0;
                firtShiftdetail.Regionid = 1;

                _context.Shiftdetails.Add(firtShiftdetail);


                if (createShift.IsReapet == true)
                {



                    foreach (SelectedDays selectedDays in createShift.SelectedDays)
                    {


                        if (selectedDays.IsSelected == true)
                        {


                            Shiftdetail shiftdetail = new Shiftdetail();
                            int daysUntillRequestedDay = (selectedDays.DayId - (int)createShift.ShiftDate.DayOfWeek + 7) % 7;

                            DateTime futureDate = createShift.ShiftDate.AddDays(daysUntillRequestedDay);

                            if (futureDate <= createShift.ShiftDate)
                            {
                                futureDate = futureDate.AddDays(7);
                            }



                            shiftdetail.Shiftid = shift.Shiftid;
                            shiftdetail.Shiftdate = futureDate;
                            shiftdetail.Starttime = new TimeOnly(createShift.StartTime.Hour, createShift.StartTime.Minute, createShift.StartTime.Second);
                            shiftdetail.Endtime = new TimeOnly(createShift.EndTime.Hour, createShift.EndTime.Minute, createShift.EndTime.Second);
                            shiftdetail.Isdeleted = false;
                            shiftdetail.Status = 0;
                            shiftdetail.Regionid = 1;
                            _context.Shiftdetails.Add(shiftdetail);

                            DateTime currentDate = futureDate;

                            for (int i = 1; i < createShift.RepeatUpto; i++)
                            {
                                Shiftdetail repeatShifts = new Shiftdetail();

                                repeatShifts.Shiftid = shift.Shiftid;
                                repeatShifts.Shiftdate = currentDate.AddDays(7);
                                repeatShifts.Starttime = new TimeOnly(createShift.StartTime.Hour, createShift.StartTime.Minute, createShift.StartTime.Second);
                                repeatShifts.Endtime = new TimeOnly(createShift.EndTime.Hour, createShift.EndTime.Minute, createShift.EndTime.Second);
                                repeatShifts.Isdeleted = false;
                                repeatShifts.Status = 0;
                                repeatShifts.Regionid = 1;

                                currentDate = currentDate.AddDays(7);

                                _context.Shiftdetails.Add(repeatShifts);
                            }
                        }

                    }


                }



                _context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        public List<RequestedShiftDetails> GetRequestedShiftDetails(int regionId)
        {

            List<RequestedShiftDetails> requestedShiftDetails = new List<RequestedShiftDetails>();
            if (regionId == 0)
            {
                requestedShiftDetails = _context.Shiftdetails.Where(q => q.Isdeleted == false).Select(r => new RequestedShiftDetails
                {
                    PhysicianName = _context.Physicians.Where(q => q.Physicianid == _context.Shifts.Where(q => q.Shiftid == r.Shiftid).Select(r => r.Physicianid).FirstOrDefault()).Select(r => r.Firstname + " " + r.Lastname).FirstOrDefault(),
                    Day = r.Shiftdate,
                    StartTime = r.Starttime,
                    EndTime = r.Endtime,
                    Region = _context.Regions.Where(q => q.Regionid == r.Regionid).Select(r => r.Name).FirstOrDefault(),
                    ShiftDetailId = r.Shiftdetailid,

                }).ToList();
            }
            else
            {
                requestedShiftDetails = _context.Shiftdetails.Where(q => q.Isdeleted == false && q.Regionid == regionId).Select(r => new RequestedShiftDetails
                {
                    PhysicianName = _context.Physicians.Where(q => q.Physicianid == _context.Shifts.Where(q => q.Shiftid == r.Shiftid).Select(r => r.Physicianid).FirstOrDefault()).Select(r => r.Firstname + " " + r.Lastname).FirstOrDefault(),
                    Day = r.Shiftdate,
                    StartTime = r.Starttime,
                    EndTime = r.Endtime,
                    Region = _context.Regions.Where(q => q.Regionid == r.Regionid).Select(r => r.Name).FirstOrDefault(),
                    ShiftDetailId = r.Shiftdetailid,

                }).ToList();

            }


            return requestedShiftDetails;
        }


        public bool ApproveShiftsService(List<RequestedShiftDetails> requestedShiftDetails, int adminId)
        {
            try
            {
                foreach (RequestedShiftDetails rd in requestedShiftDetails)
                {
                    if (rd.IsSelected == true)
                    {
                        Shiftdetail shiftdetail = _context.Shiftdetails.FirstOrDefault(q => q.Shiftdetailid == rd.ShiftDetailId);

                        shiftdetail.Status = 1;
                        shiftdetail.Modifieddate = DateTime.Now;
                        shiftdetail.Modifiedby = _context.Admins.Where(q => q.Adminid == adminId).Select(r => r.Aspnetuserid).FirstOrDefault();

                        _context.Shiftdetails.Update(shiftdetail);
                    }
                }

                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public List<WeekWisePhysicianShifts> GetAllPhysicianWeekWiseShifts(int date, int month, int year, int regionId)
        {

            List<Shiftdetail> shiftdetails = _context.Shiftdetails.ToList();


            List<Physician> physicians = GetPhysicinForShiftsByRegionService(regionId);

            List<Shift> shifts = _context.Shifts.ToList();
            List<WeekWisePhysicianShifts> weekWisePhysicianShiftsList = new List<WeekWisePhysicianShifts>();



            DateOnly startOfWeek = new DateOnly(year, month, date);

            foreach (Physician physician in physicians)
            {

                WeekWisePhysicianShifts weekWisePhysicianShifts = new WeekWisePhysicianShifts();
                List<ShiftInformation> WeekWiseShifts = new List<ShiftInformation>();
                weekWisePhysicianShifts.physicianId = physician.Physicianid;
                weekWisePhysicianShifts.PhysicianName = physician.Firstname+" "+physician.Lastname;

                for (int i = 0; i < 7; i++)
                {

                    List<Shift> PhysicianShifts = _context.Shifts.Where(q => q.Physicianid == physician.Physicianid).ToList();


                    if (PhysicianShifts.Count == 0)
                    {
                        ShiftInformation dayWiseShift = new ShiftInformation();
                        dayWiseShift.physicianId = physician.Physicianid;
                        dayWiseShift.PhysicianName = physician.Firstname + " " + physician.Lastname;
                        dayWiseShift.dayOfWeek = -1;
                        WeekWiseShifts.Add(dayWiseShift);
                        break;
                    }
                    else
                    {
                        bool IsTodayShiftFound = false;

                        foreach (Shift shift in PhysicianShifts)
                        {
                            DateOnly currentDate = new DateOnly(startOfWeek.Year, startOfWeek.Month, startOfWeek.Day);
                            Shiftdetail shiftdetail = _context.Shiftdetails.FirstOrDefault(q => (q.Shiftid == shift.Shiftid) && (q.Shiftdate.Year == currentDate.Year) && (q.Shiftdate.Month == currentDate.Month) && (q.Shiftdate.Day == currentDate.Day) && (q.Isdeleted == false));



                            if (shiftdetail == null)
                            {

                                continue;
                            }
                            else
                            {
                                IsTodayShiftFound = true;
                                ShiftInformation dayWiseShift = new ShiftInformation();

                                dayWiseShift.physicianId = physician.Physicianid;
                                dayWiseShift.PhysicianName = physician.Firstname + " " + physician.Lastname;
                                dayWiseShift.startTime = shiftdetail.Starttime.Hour;
                                dayWiseShift.endTime = shiftdetail.Endtime.Hour;
                                dayWiseShift.status = shiftdetail.Status;
                                dayWiseShift.ShiftDetailId = shiftdetail.Shiftdetailid;
                                dayWiseShift.ShiftId = shiftdetail.Shiftid;
                                dayWiseShift.dayOfWeek = (int)new DateTime(startOfWeek.Year, startOfWeek.Month, startOfWeek.Day).DayOfWeek;
                                WeekWiseShifts.Add(dayWiseShift);
                            }

                        }

                        if (IsTodayShiftFound == false)
                        {
                            ShiftInformation dayWiseShift = new ShiftInformation();
                            dayWiseShift.physicianId = physician.Physicianid;
                            dayWiseShift.PhysicianName = physician.Firstname + " " + physician.Lastname;
                            dayWiseShift.startTime = -1;
                            dayWiseShift.dayOfWeek = -1;
                            WeekWiseShifts.Add(dayWiseShift);
                        }

                    }
                startOfWeek = startOfWeek.AddDays(1);
                }

                weekWisePhysicianShifts.WeekWiseShiftInformation = WeekWiseShifts;

                weekWisePhysicianShiftsList.Add(weekWisePhysicianShifts);



            }


            return weekWisePhysicianShiftsList;
        }

        public ViewShift GetShiftDetailsById(int shiftDetailsId)
        {


            try
            {
                Shiftdetail shiftdetail = _context.Shiftdetails.FirstOrDefault(q => q.Shiftdetailid == shiftDetailsId);

                 if(shiftdetail == null)
                {
                    return new ViewShift();
                }

                ViewShift viewShift = new ViewShift();

                int physisianId = _context.Shifts.Where(q => q.Shiftid == shiftdetail.Shiftid).Select(r => r.Physicianid).FirstOrDefault();

                viewShift.ShiftDetailId = shiftdetail.Shiftdetailid;
                viewShift.physicianId = physisianId;
                viewShift.ShiftDate = shiftdetail.Shiftdate;
                viewShift.startTime = shiftdetail.Starttime;
                viewShift.endTime = shiftdetail.Endtime;
                viewShift.status = shiftdetail.Status;
                viewShift.ShiftId = shiftdetail.Shiftid;
                viewShift.regionId = (int)shiftdetail.Regionid;
                viewShift.RegionName = _context.Regions.Where(q => q.Regionid == viewShift.regionId).Select(r => r.Name).FirstOrDefault();
                viewShift.Physicianname = _context.Physicians.Where(q => q.Physicianid == physisianId).Select(r => r.Firstname+" "+r.Lastname).FirstOrDefault();
               

                return viewShift;
            }catch(Exception ex)
            {
                return new ViewShift();
            }
        }

    }
}
