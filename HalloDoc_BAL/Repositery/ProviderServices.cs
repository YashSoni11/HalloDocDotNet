﻿using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using HalloDoc_BAL.Interface;
using HalloDoc_DAL.AdminViewModels;
using HalloDoc_DAL.Context;
using HalloDoc_DAL.Models;
using HalloDoc_DAL.ProviderViewModels;
using HalloDoc_DAL.ViewModels;
using ICSharpCode.SharpZipLib.GZip;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SixLabors.ImageSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
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


        public enum RequestorType
        {
            Patient = 1,
            Family,
            Conciearge,
            Business

        }

        public string UploadProviderSignatureFile(IFormFile file)
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

        public string UploadProviderFiles(IFormFile file,int PhysicianId,string FileName)
        {

      



            try
            {
                if (file.Length > 0)
                {




                    string directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Documents", PhysicianId.ToString());

                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                   
                        string path = Path.Combine(directory, FileName + Path.GetExtension(file.FileName));

                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                           file.CopyTo(fileStream);
                        }




                    return FileName;


                   
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


        public List<SelectedRegions> GetAllRegionsByPhysicianId(int PhysicinId)
        {
            try
            {
            List<SelectedRegions> selectedRegions = new List<SelectedRegions>();

            List<Region> regions = _context.Regions.ToList();


            List<Physicianregion> physicianregions = _context.Physicianregions.Where(q => q.Physicianid == PhysicinId).ToList();

            for (int i = 0; i < regions.Count; i++)
            {
                SelectedRegions selectedRegions1 = new SelectedRegions();
                selectedRegions1.regionId = regions[i].Regionid;
                selectedRegions1.regionName = regions[i].Name;

                if (physicianregions != null && physicianregions.Any(q => q.Regionid == regions[i].Regionid))
                {
                    selectedRegions1.IsSelected = true;
                }
                else
                {
                    selectedRegions1.IsSelected = false;
                }
                selectedRegions.Add(selectedRegions1);
            }

            return selectedRegions;

            }
            catch (Exception ex)
            {
                return new List<SelectedRegions>();
            }
        }
        public ProviderProfileView GetProviderData(int providerId)
        {
            try
            {


                Physician physician = _context.Physicians.FirstOrDefault(q => q.Physicianid == providerId);


                ProviderProfileView providerProfileView = new ProviderProfileView();

                ProviderAccountInfo providerAccountInfo = new ProviderAccountInfo()
                {
                    UserName = _context.Aspnetusers.Where(q => q.Id == physician.Aspnetuserid).Select(r => r.Username).FirstOrDefault(),
                    Status = physician.Status,
                    Role = _context.Roles.Where(q => q.Roleid == physician.Roleid).Select(r => r.Name).FirstOrDefault(),
                    roles = _context.Roles.Where(q=>q.Isdeleted == false).ToList(),


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
                    SelectedRegions = GetAllRegionsByPhysicianId(providerId),
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
                    AdminNotes = physician.Adminnotes,
                    IsSignatureUploaded = physician.Signature != null ? true : false
                };



                ProviderDocuments providerDocuments = new ProviderDocuments()
                {
                    IsContractAggreeMent = physician.Isagreementdoc[0],
                    IsBackGroundCheck = physician.Isbackgrounddoc[0],
                    IsHipaa = physician.Istrainingdoc[0],
                    IsNonDisClouser = physician.Isnondisclosuredoc[0],

                };




                providerProfileView.AccountInfo = providerAccountInfo;
                providerProfileView.ProviderMailingAndBillingInfo = providerMailingAndBillingInfo;
                providerProfileView.ProviderProfileInfo = providerProfileInfo;
                providerProfileView.ProviderInformation = providerInformation;
                providerProfileView.ProviderDocuments = providerDocuments;
                providerProfileView.PhysicianId = providerId;

                return providerProfileView;
            }
            catch (Exception ex)
            {
                return new ProviderProfileView();
            }

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


                for (int i = 0; i < pi.SelectedRegions.Count; i++)
                {
                    SelectedRegions selectedRegion = pi.SelectedRegions[i];

                    if (selectedRegion.IsSelected == true && (_context.Physicianregions.Any(q => q.Physicianid == provideId && q.Regionid == selectedRegion.regionId) == false))
                    {
                        Physicianregion physicianregion = new Physicianregion();
                        physicianregion.Regionid = (int)selectedRegion.regionId;
                        physicianregion.Physicianid = provideId;
                        _context.Physicianregions.Add(physicianregion);
                    }
                    else if (selectedRegion.IsSelected == false && (_context.Physicianregions.Any(q => q.Physicianid == provideId && q.Regionid == selectedRegion.regionId) == true))
                    {
                        Physicianregion physicianregion = _context.Physicianregions.FirstOrDefault(q => q.Physicianid == provideId && q.Regionid == selectedRegion.regionId);
                        _context.Physicianregions.Remove(physicianregion);
                    }
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
                    string? file = UploadProviderFiles(pp.Photo, provideId,"Photo");
                    if (file == null)
                    {
                        return false;
                    }
                    physician.Photo = file;
                }
                if (pp.Signature != null)
                {
                    string? file = UploadProviderSignatureFile(pp.Signature);
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


        public bool CreateProviderAccount(CreateProviderAccount createProviderAccount,int adminId)
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


                Aspnetuserrole aspnetuserrole = new Aspnetuserrole();
                aspnetuserrole.Userid = ProviderAspNetUser.Id;
                aspnetuserrole.Roleid = "Physician";

                _context.Aspnetuserroles.Add(aspnetuserrole);


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
                physician.Status = 1;
                physician.Zip = createProviderAccount.Zip;
                physician.Adminnotes = createProviderAccount.AdminNotes;
                physician.Altphone = createProviderAccount.AltPhone;
                physician.Businessname = createProviderAccount.BusinessName;
                physician.Businesswebsite = createProviderAccount.BusinessWebsite;
                physician.Mobile = createProviderAccount.Phone;
                physician.Syncemailaddress = createProviderAccount.Email;
                physician.Adminnotes = createProviderAccount.AdminNotes;
                physician.Isdeleted = false;
                physician.Createdby = _context.Admins.Where(q => q.Adminid == adminId).Select(q => q.Aspnetuserid).FirstOrDefault();






                _context.Physicians.Add(physician);
                _context.SaveChanges();

               int PhySicianId = _context.Physicians.OrderByDescending(q=>q.Physicianid).First().Physicianid;

                if (createProviderAccount.Photo != null)
                {
                    string fileName = UploadProviderFiles(createProviderAccount.Photo, PhySicianId, "Photo");
                    if (!fileName.IsNullOrEmpty())
                    {
                        Physician physician1 = _context.Physicians.FirstOrDefault(q => q.Physicianid == PhySicianId);
                        physician1.Photo = fileName;
                        _context.Physicians.Update(physician1);
                    }
                }

                for (int i = 0; i < createProviderAccount.Regions.Count; i++)
                {
                    SelectedRegions selectedRegion = createProviderAccount.Regions[i];

                    if (selectedRegion.IsSelected == true)
                    {
                        Physicianregion physicianregion = new Physicianregion();
                        physicianregion.Regionid = (int)selectedRegion.regionId;
                        physicianregion.Physicianid = PhySicianId;
                        _context.Physicianregions.Add(physicianregion);
                    }
                   
                }

                _context.SaveChanges();






                AddProviderDocuments(PhySicianId, createProviderAccount.ContractAgrrement, createProviderAccount.BackgroundCheck, createProviderAccount.HIPAA, createProviderAccount.NonDisclouser);


                return true;


            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public void AddProviderDocuments(int Physicianid, IFormFile ContractorAgreement, IFormFile BackgroundCheck, IFormFile HIPAA, IFormFile NonDisclosure)
        {

            try
            {

            var physicianData = _context.Physicians.FirstOrDefault(x => x.Physicianid == Physicianid);

            string directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Documents", Physicianid.ToString());

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

     


            if (ContractorAgreement != null)
            {
                string path = Path.Combine(directory, "Independent_Contractor" + Path.GetExtension(ContractorAgreement.FileName));

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    ContractorAgreement.CopyTo(fileStream);
                }

                physicianData.Isagreementdoc = new BitArray(1, true);
            }

            if (BackgroundCheck != null)
            {
                string path = Path.Combine(directory, "Background" + Path.GetExtension(BackgroundCheck.FileName));

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    BackgroundCheck.CopyTo(fileStream);
                }

                physicianData.Isbackgrounddoc = new BitArray(1, true);
            }

            if (HIPAA != null)
            {
                string path = Path.Combine(directory, "HIPAA" + Path.GetExtension(HIPAA.FileName));

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    HIPAA.CopyTo(fileStream);
                }

                physicianData.Istrainingdoc = new BitArray(1, true);
            }

            if (NonDisclosure != null)
            {
                string path = Path.Combine(directory, "Non_Disclosure" + Path.GetExtension(NonDisclosure.FileName));

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    NonDisclosure.CopyTo(fileStream);
                }

                physicianData.Isnondisclosuredoc = new BitArray(1, true);
            }

            _context.SaveChanges();
            }catch(Exception ex)
            {
                return;
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

                Aspnetuserrole aspnetuserrole = new Aspnetuserrole();
                aspnetuserrole.Userid = AdminAspNetUser.Id;
                aspnetuserrole.Roleid = "Admin";

                _context.Aspnetuserroles.Add(aspnetuserrole);

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

                int AdminId = _context.Admins.OrderByDescending(q => q.Adminid).First().Adminid;

                for (int i = 0; i < createAdminAccount.SelectedRegions?.Count; i++)
                {
                    SelectedRegions selectedRegion = createAdminAccount.SelectedRegions[i];

                    if (selectedRegion.IsSelected == true)
                    {
                        Adminregion adminregion = new Adminregion();
                        adminregion.Regionid = (int)selectedRegion?.regionId;
                        adminregion.Adminid = AdminId;
                        _context.Adminregions.Add(adminregion);
                    }

                }

                return true;


            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<AccessAreas> GetAreaAccessByAccountType(int accountType, int roleId = 0)
        {
            try
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

            }catch(Exception ex)
            {
                return new List<AccessAreas>();
            }
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
                    else if(createRole.AccessAreas[i].IsAreaSelected == false && (_context.Rolemenus.Any(q => q.Roleid == Roleid && q.Menuid == createRole.AccessAreas[i].AreaId) == true))
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

            try
            {

            bool IsAllRegion = false;

            if(regionId == 0)
            {
                IsAllRegion = true;
            }

            List<Physicianregion> physiciansregions = _context.Physicianregions.Where(q => IsAllRegion || q.Regionid == regionId).ToList();

            Dictionary<int, int> phyisicianId = new Dictionary<int, int>();


            List<Physician> physicians = new List<Physician>();

            foreach (Physicianregion physicianregion in physiciansregions)
            {
                Physician physician = _context.Physicians.FirstOrDefault(q => q.Physicianid == physicianregion.Physicianid &&q.Isdeleted == false);

                if (physician != null && !phyisicianId.ContainsKey(physician.Physicianid))
                {
                physicians.Add(physician);
                    phyisicianId[physician.Physicianid] = 1;

                }

            }

            return physicians;
            }catch(Exception ex)
            {
                return new List<Physician>();
            }

        }
        public List<DayWisePhysicianShifts> GetAllPhysicianDayWiseShifts(int date, int month, int year, int regionId)
        {
            try
            {


            List<Shiftdetail> shiftdetails = _context.Shiftdetails.Where(q=>q.Isdeleted == false).ToList();


            List<Physician> physicians = GetPhysicinForShiftsByRegionService(regionId);

            List<Shift> shifts = _context.Shifts.ToList();
            List<ShiftInformation> dayWiseShifts = new List<ShiftInformation>();


            List<DayWisePhysicianShifts> dayWisePhysicianShifts = new List<DayWisePhysicianShifts>();




            foreach (Physician physician in physicians)
            {
                List<Shift> PhysicianShifts = _context.Shifts.Where(q => q.Physicianid == physician.Physicianid).ToList();

                if (PhysicianShifts.Count == 0)
                {
                    DayWisePhysicianShifts dayWisePhysicianShift = new DayWisePhysicianShifts();

                    List<ShiftInformation> shiftList = new List<ShiftInformation>();

                    ShiftInformation dayWiseShift = new ShiftInformation();
                    dayWiseShift.physicianId = physician.Physicianid;
                    dayWiseShift.PhysicianName = physician.Firstname + " " + physician.Lastname;
                    dayWiseShift.status = -1;
                    dayWisePhysicianShift.physicianId = physician.Physicianid;
                    dayWisePhysicianShift.PhysicianName = physician.Firstname + " " + physician.Lastname;

                    shiftList.Add(dayWiseShift);

                    dayWisePhysicianShift.DayWiseShiftInformation = shiftList;

                    dayWisePhysicianShifts.Add(dayWisePhysicianShift);


                }
                else
                {
                    bool IsTodayShiftFound = false;

                    DayWisePhysicianShifts dayWisePhysicianShift = new DayWisePhysicianShifts();
                    dayWisePhysicianShift.physicianId = physician.Physicianid;
                    dayWisePhysicianShift.PhysicianName = physician.Firstname + " " + physician.Lastname;

                    List<ShiftInformation> daywiseShiftInformation = new List<ShiftInformation>();

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
                            dayWiseShift.startTime = shiftdetail.Starttime;
                            dayWiseShift.endTime = shiftdetail.Endtime;
                            dayWiseShift.status = shiftdetail.Status;
                            daywiseShiftInformation.Add(dayWiseShift);
                        }

                    }

                    if (IsTodayShiftFound == false)
                    {
                        ShiftInformation dayWiseShift = new ShiftInformation();
                        //dayWiseShift.physicianId = physician.Physicianid;
                        //dayWiseShift.PhysicianName = physician.Firstname + " " + physician.Lastname;
                        dayWiseShift.status = -1;
                        daywiseShiftInformation.Add(dayWiseShift);
                    }

                    dayWisePhysicianShift.DayWiseShiftInformation = daywiseShiftInformation.OrderBy(q => q.startTime).ToList();

                    dayWisePhysicianShifts.Add(dayWisePhysicianShift);

                }
            }

            return dayWisePhysicianShifts;
            }
            catch (Exception ex)
            {
                return new List<DayWisePhysicianShifts>();
            }
        }


        public List<ShiftInformation> GetDayWiseAllShiftInformation(int date, int month, int year, int regionId){

            try
            {

            List<ShiftInformation> shiftInformation = _context.Shiftdetails.Where(q => q.Shiftdate.Day == date && q.Shiftdate.Month == month && q.Shiftdate.Year == year && q.Isdeleted == false).Select(r => new ShiftInformation
            {
                ShiftDetailId = r.Shiftdetailid,
                PhysicianName = _context.Physicians.Where(q => q.Physicianid == _context.Shifts.Where(q => q.Shiftid == r.Shiftid).Select(r => r.Physicianid).FirstOrDefault()).Select(q => q.Firstname + " " + q.Lastname).FirstOrDefault(),
                startTime = r.Starttime,
                endTime = r.Endtime,
                ShiftDate = r.Shiftdate,
            }).ToList();

            return shiftInformation;
            }catch(Exception ex)
            {
                return new List<ShiftInformation>();
            }


         }


        public bool CreateShiftService(CreateShift createShift, int adminId,string Role)
        {


            try
            {
                Shift shift = new Shift();

                shift.Shiftid = _context.Shifts.OrderByDescending(q => q.Shiftid).Select(q => q.Shiftid).FirstOrDefault() + 1;
                shift.Physicianid = createShift.PhysicianId;
                shift.Startdate = new DateOnly(createShift.ShiftDate.Year, createShift.ShiftDate.Month, createShift.ShiftDate.Day);
                shift.Repeatupto = createShift.RepeatUpto;
                shift.Isrepeat = createShift.IsReapet;
                shift.Createddate = DateTime.Now;


                if(Role == "Admin")
                {
                    shift.Createdby = _context.Admins.Where(q => q.Adminid == adminId).Select(r => r.Aspnetuserid).FirstOrDefault();

                }else if(Role == "Physician")
                {
                    shift.Createdby = _context.Physicians.Where(q => q.Physicianid == adminId).Select(r => r.Aspnetuserid).FirstOrDefault();

                }

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

        public bool EditShiftService(ViewShift viewShift,int adminId)
        {
            try
            {
                Shiftdetail shiftdetail = _context.Shiftdetails.FirstOrDefault(q => q.Shiftdetailid == viewShift.ShiftDetailId);

                shiftdetail.Shiftdate = viewShift.ShiftDate;
                shiftdetail.Starttime = viewShift.startTime;
                shiftdetail.Endtime = viewShift.endTime;
                shiftdetail.Regionid = viewShift.regionId;
                shiftdetail.Modifiedby = _context.Admins.Where(q => q.Adminid == adminId).Select(r => r.Aspnetuserid).FirstOrDefault();
                shiftdetail.Modifieddate = DateTime.Now;

                Shift shift = _context.Shifts.FirstOrDefault(q => q.Shiftid == shiftdetail.Shiftid);

                shift.Physicianid = viewShift.physicianId;

                _context.Shifts.Update(shift);
                _context.Shiftdetails.Update(shiftdetail);

                _context.SaveChanges();

                return true;
              
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public bool IsValidShift(CreateShift createShift)
        {
            try
            {
                DateTime date = createShift.ShiftDate;

                DateTime startTime = createShift.StartTime;

                DateTime endTime = createShift.EndTime;

                List<Shift> shifts = _context.Shifts.Where(q => q.Physicianid == createShift.PhysicianId).ToList();

                List<Shiftdetail> shiftdetails = new List<Shiftdetail>();

                foreach(Shift shift in shifts)
                {

                    List<Shiftdetail> shiftdetails1 = _context.Shiftdetails.Where(q => q.Shiftid == shift.Shiftid && q.Isdeleted == false).ToList();
                    shiftdetails = shiftdetails.Concat(shiftdetails1).ToList();
                }


                bool isExists = false;

                foreach(Shiftdetail shiftdetail in shiftdetails)
                {
                    if (isExists)
                    {
                        break;
                    }
                    if(shiftdetail.Shiftdate == date)
                    {
                        if((startTime.Hour>shiftdetail.Starttime.Hour && startTime.Hour < shiftdetail.Endtime.Hour) || (endTime.Hour > shiftdetail.Starttime.Hour && endTime.Hour < shiftdetail.Endtime.Hour))
                        {
                            isExists = true;
                            break;
                        }else if(shiftdetail.Starttime.Hour == endTime.Hour)
                        {
                            if (endTime.Minute >= shiftdetail.Starttime.Minute)
                            {
                                isExists = true;
                                break;
                            }
                        }
                        else if(shiftdetail.Endtime.Hour == startTime.Hour)
                        {
                            if(startTime.Minute <= shiftdetail.Endtime.Minute)
                            {
                                isExists = true;
                                break;
                            }
                        }else if(shiftdetail.Starttime.Hour == startTime.Hour && shiftdetail.Endtime.Hour  == endTime.Hour)
                        {
                            isExists = true;
                            break;
                        }

                    }

                }

                return isExists;
            }catch(Exception e)
            {
                return false;
            }
        }


        public List<RequestedShiftDetails> GetRequestedShiftDetails(int regionId)
        {

            try
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
                requestedShiftDetails = _context.Shiftdetails.Where(q => q.Isdeleted == false && q.Regionid == regionId && q.Status == 0).Select(r => new RequestedShiftDetails
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

            }catch(Exception ex)
            {
                return new List<RequestedShiftDetails>();
            }
        }


        public bool ApproveShiftsService(List<RequestedShiftDetails> requestedShiftDetails, int adminId)
        {
            try
            {
                bool IsShifts = false;

                foreach (RequestedShiftDetails rd in requestedShiftDetails)
                {
                    if (rd.IsSelected == true)
                    {
                        Shiftdetail shiftdetail = _context.Shiftdetails.FirstOrDefault(q => q.Shiftdetailid == rd.ShiftDetailId);

                        shiftdetail.Status = 1;
                        shiftdetail.Modifieddate = DateTime.Now;
                        shiftdetail.Modifiedby = _context.Admins.Where(q => q.Adminid == adminId).Select(r => r.Aspnetuserid).FirstOrDefault();

                        _context.Shiftdetails.Update(shiftdetail);

                        IsShifts = true;
                    }
                }

                _context.SaveChanges();

                if(IsShifts == false)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public List<WeekWisePhysicianShifts> GetAllPhysicianWeekWiseShifts(int date, int month, int year, int regionId)
        {
            try
            {

            List<Shiftdetail> shiftdetails = _context.Shiftdetails.Where(q=>q.Isdeleted == false).ToList();


            List<Physician> physicians = GetPhysicinForShiftsByRegionService(regionId);

            List<Shift> shifts = _context.Shifts.ToList();
            List<WeekWisePhysicianShifts> weekWisePhysicianShiftsList = new List<WeekWisePhysicianShifts>();



            DateOnly startOfWeek = new DateOnly(year, month, date);

            foreach (Physician physician in physicians)
            {

                WeekWisePhysicianShifts weekWisePhysicianShifts = new WeekWisePhysicianShifts();
                List<ShiftInformation> WeekWiseShifts = new List<ShiftInformation>();
                weekWisePhysicianShifts.physicianId = physician.Physicianid;
                weekWisePhysicianShifts.PhysicianName = physician.Firstname + " " + physician.Lastname;
                    List<Shift> PhysicianShifts = _context.Shifts.Where(q => q.Physicianid == physician.Physicianid).ToList();

                for (int i = 0; i < 7; i++)
                {



                    if (PhysicianShifts.Count == 0)
                    {
                        ShiftInformation dayWiseShift = new ShiftInformation();
                        dayWiseShift.physicianId = physician.Physicianid;
                        dayWiseShift.PhysicianName = physician.Firstname + " " + physician.Lastname;
                        dayWiseShift.dayOfWeek = -1;
                        WeekWiseShifts.Add(dayWiseShift);
                        //break;
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
                                dayWiseShift.startTime = shiftdetail.Starttime;
                                dayWiseShift.endTime = shiftdetail.Endtime;
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
                            dayWiseShift.status = -1;
                            dayWiseShift.dayOfWeek = -1;
                            WeekWiseShifts.Add(dayWiseShift);
                        }

                    }
                    startOfWeek = startOfWeek.AddDays(1);
                }


                startOfWeek = new DateOnly(year, month, date);

                weekWisePhysicianShifts.WeekWiseShiftInformation = WeekWiseShifts;

                weekWisePhysicianShiftsList.Add(weekWisePhysicianShifts);



            }


            return weekWisePhysicianShiftsList;
            }
            catch (Exception ex)
            {
                return new List<WeekWisePhysicianShifts>();
                
            }

        }

        public ViewShift GetShiftDetailsById(int shiftDetailsId)
        {


            try
            {
                Shiftdetail shiftdetail = _context.Shiftdetails.FirstOrDefault(q => q.Shiftdetailid == shiftDetailsId && q.Isdeleted == false);

                if (shiftdetail == null)
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
                viewShift.RegionName = _context.Regions.Where(q => q.Regionid == viewShift.regionId ).Select(r => r.Name).FirstOrDefault();
                viewShift.Physicianname = _context.Physicians.Where(q => q.Physicianid == physisianId && q.Isdeleted == false).Select(r => r.Firstname + " " + r.Lastname).FirstOrDefault();


                return viewShift;
            }
            catch (Exception ex)
            {
                return new ViewShift();
            }
        }


        public List<MonthWisePhysicianShifts> GetAllPhysicianMonthWiseShifts(int date, int month, int year, int regionId)
        {



            try
            {

            List<Physician> physicians = GetPhysicinForShiftsByRegionService(regionId);

            //List<Shift> shifts = _context.Shifts.ToList();
            List<MonthWisePhysicianShifts> MonthWisePhysicianShiftsList = new List<MonthWisePhysicianShifts>();


            DateTime startOfMonth = new DateTime(year, month, date);

            int daysInMonth = DateTime.DaysInMonth(startOfMonth.Year, startOfMonth.Month);

            for (int i = 1; i <= daysInMonth; i++)
            {

                DateTime currentDate = new DateTime(startOfMonth.Year, startOfMonth.Month, i);

                List<Shiftdetail> shiftdetails = _context.Shiftdetails.Where(q => q.Shiftdate.Day == i && q.Shiftdate.Month == currentDate.Month && q.Shiftdate.Year == currentDate.Year && q.Isdeleted == false).ToList();
                MonthWisePhysicianShifts monthWisePhysicianShift = new MonthWisePhysicianShifts();

                if (shiftdetails.Count > 0)
                {
                    monthWisePhysicianShift.DayNumber = i;
                    List<ShiftInformation> shifts = new List<ShiftInformation>();
                    foreach (Shiftdetail shiftdetail in shiftdetails)
                    {
                        ShiftInformation shiftInformation = new ShiftInformation();
                        shiftInformation.ShiftDetailId = shiftdetail.Shiftdetailid;
                        shiftInformation.startTime = shiftdetail.Starttime;
                        shiftInformation.endTime = shiftdetail.Endtime;
                        shiftInformation.status = shiftdetail.Status;

                        int physicinId = _context.Shifts.Where(q => q.Shiftid == shiftdetail.Shiftid).Select(q => q.Physicianid).FirstOrDefault();
                        shiftInformation.PhysicianName = _context.Physicians.Where(q => q.Physicianid == physicinId).Select(q => q.Firstname + " " + q.Lastname).FirstOrDefault();  

                        shifts.Add(shiftInformation);
                    }
                    monthWisePhysicianShift.MonthWiseShiftInformation = shifts;
                }
                else
                {
                    monthWisePhysicianShift.DayNumber = -1;

                }

                MonthWisePhysicianShiftsList.Add(monthWisePhysicianShift);
            }

            return MonthWisePhysicianShiftsList;
            }
            catch (Exception ex)
            {
                return new List<MonthWisePhysicianShifts>();
            }

        }

        public List<UserAccess> GetAllAspNetUsers(int roleId)
        {

            try
            {
            List<UserAccess> users = new List<UserAccess>();

            bool isAllRole = false;

            if(roleId == 0)
            {
                isAllRole = true;
            }

            List<UserAccess> adminUsers = _context.Admins.Where(q=> (isAllRole || q.Roleid == roleId) &&q.Isdeleted == false  ).Select(r => new UserAccess
            {
                UserId = r.Adminid,
                AccountName = _context.Aspnetusers.Where(q=>q.Id == r.Aspnetuserid).Select(q=>q.Username).FirstOrDefault(),
                PhoneNumber = _context.Aspnetusers.Where(q => q.Id == r.Aspnetuserid).Select(q => q.Phonenumber).FirstOrDefault(),
                status =  r.Status == null?0: (int)r.Status,
                OpenRequest = _context.Requests.Where(q=>q.Createduserid == r.Adminid && q.Isdeleted == new BitArray(1, false)).Count(),
                AccountType = 0,
            }).ToList();

           users =  users.Concat(adminUsers).ToList();

            List<UserAccess> physicianUsers = _context.Physicians.Where(q =>( isAllRole||  q.Roleid == roleId) && q.Isdeleted == false ).Select(r => new UserAccess
            {
                UserId = r.Physicianid,
                AccountName = _context.Aspnetusers.Where(q => q.Id == r.Aspnetuserid).Select(q => q.Username).FirstOrDefault(),
                PhoneNumber = _context.Aspnetusers.Where(q => q.Id == r.Aspnetuserid).Select(q => q.Phonenumber).FirstOrDefault(),
                status = r.Status == null ? 0 : (int)r.Status,

                OpenRequest = _context.Requests.Where(q => q.Createduserid == r.Physicianid && q.Isdeleted == new BitArray(1, false)).Count(),
                AccountType = 1,
            }).ToList();


          users =   users.Concat(physicianUsers).ToList();

           // List<UserAccess> patientsUsers = _context.Users.Select(r => new UserAccess
           // {
           //     UserId = r.Userid,
           //     AccountName = _context.Aspnetusers.Where(q => q.Id == r.Aspnetuserid).Select(q => q.Username).FirstOrDefault(),
           //     PhoneNumber = _context.Aspnetusers.Where(q => q.Id == r.Aspnetuserid).Select(q => q.Phonenumber).FirstOrDefault(),
           //     status = r.Status == null ? 0 : (int)r.Status,
           //     OpenRequest = 0,
           //     AccountType = 2,
           // }).ToList();


           //users =  users.Concat(patientsUsers).ToList();

            return users;

            }catch(Exception ex)
            {
                return new List<UserAccess>();
            }

        }


        public bool DeleteRequestedShifts(List<RequestedShiftDetails> requestedShifts,int adminId)
        {
            try
            {
                bool IsAllShift = false;

                foreach(RequestedShiftDetails shift in requestedShifts) {

                    if(shift.IsSelected == true)
                    {

                    Shiftdetail shiftdetail = _context.Shiftdetails.FirstOrDefault(q => q.Shiftdetailid == shift.ShiftDetailId);

                    shiftdetail.Isdeleted = true;
                    shiftdetail.Modifieddate = DateTime.Now;
                    shiftdetail.Modifiedby = _context.Admins.Where(q => q.Adminid == adminId).Select(r => r.Aspnetuserid).FirstOrDefault();

                    _context.Shiftdetails.Update(shiftdetail);

                        IsAllShift = true;

                    }
                 
                }

                if(IsAllShift == false)
                {
                    return false;
                }

                _context.SaveChanges();
                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }


        public bool DeleteShiftService(int shiftDetailId,int adminId)
        {
            try
            {
            Shiftdetail shiftdetail = _context.Shiftdetails.FirstOrDefault(q => q.Shiftdetailid == shiftDetailId);

            shiftdetail.Isdeleted = true;
            shiftdetail.Modifieddate = DateTime.Now;
            shiftdetail.Modifiedby = _context.Admins.Where(q => q.Adminid == adminId).Select(r => r.Aspnetuserid).FirstOrDefault();

            _context.Shiftdetails.Update(shiftdetail);
                _context.SaveChanges();


                return true;
            }catch(Exception ex)
            {
                return false;
            }

        }
      public List<Healthprofessionaltype> GetAllHealthProfessionalTypes()
        {
            return _context.Healthprofessionaltypes.ToList();
        }

        public List<VendorList> GetVendorsData(string vendorName, int HealthProfessionId)
        {
            try
            {
                bool IsSearchByName = false;

                if (string.IsNullOrEmpty(vendorName))
                {
                    IsSearchByName = true;
                }

                bool IsSearchById = false;

                if(HealthProfessionId == 0)
                {
                    IsSearchById = true;
                }

                List<VendorList> vendorLists = _context.Healthprofessionals.Where(q=>(IsSearchByName || q.Vendorname.ToLower().Contains(vendorName.ToLower())  ) && (IsSearchById || q.Profession == HealthProfessionId ) && q.Isdeleted == false ).Select(r => new VendorList
                {
                    VendorId = r.Vendorid,
                    BusinessContact = r.Businesscontact,
                    BusinessName = r.Vendorname,
                    Email = r.Email,
                    FaxNumber = r.Faxnumber,
                    PhoneNumber = r.Phonenumber,
                    Profession = _context.Healthprofessionaltypes.Where(q => q.Healthprofessionalid == r.Profession).Select(r => r.Professionname).FirstOrDefault(),


                }).ToList();

                return vendorLists;
            }catch(Exception ex) {

                return new List<VendorList>();
            }
        }

        public VendorDetails EditVendorDetailsView(int id)
        {

            try
            {


                VendorDetails? vendorDetails = _context.Healthprofessionals.Where(q => q.Vendorid == id).Select(r => new VendorDetails { 


                    ProfessionId = (int)r.Profession,
                    Profession = _context.Healthprofessionaltypes.Where(q=>q.Healthprofessionalid == r.Profession).Select(r=>r.Professionname).FirstOrDefault(),
                    BusinessName = r.Vendorname,
                    BusinessContact = r.Businesscontact,
                    Email = r.Email,
                    FaxNumber = r.Faxnumber,
                    PhoneNumber = r.Phonenumber,
                    Street = r.Address,
                    regionId = (int)r.Regionid,
                    regionName = _context.Regions.Where(q=>q.Regionid == r.Regionid).Select(r=>r.Name).FirstOrDefault(),
                    City = r.City,
                    ZipCode = r.Zip,


                }).FirstOrDefault();

                vendorDetails.healthprofessionaltypes = _context.Healthprofessionaltypes.ToList();
                vendorDetails.regions = _context.Regions.ToList();

                return vendorDetails;
            }
            catch (Exception ex)
            {
                return new VendorDetails();
            }
        }

        public bool EditVendor(VendorDetails vendorDetails,int adminId, int id)
        {
            try
            {


                Healthprofessional healthprofessional = _context.Healthprofessionals.FirstOrDefault(q => q.Vendorid == id);

                

                healthprofessional.Profession = vendorDetails.ProfessionId;
                healthprofessional.Vendorname = vendorDetails.BusinessName;
                healthprofessional.Businesscontact = vendorDetails.BusinessContact;
                healthprofessional.Email = vendorDetails.Email;
                healthprofessional.Faxnumber = vendorDetails.FaxNumber;
                healthprofessional.Phonenumber = vendorDetails.PhoneNumber;
                healthprofessional.Address = vendorDetails.Street;
                healthprofessional.State = vendorDetails.regionName;
                healthprofessional.City = vendorDetails.City;
                healthprofessional.Zip = vendorDetails.ZipCode;
                healthprofessional.Regionid = vendorDetails.regionId;
                healthprofessional.Modifieddate = DateTime.Now;
               

                _context.Healthprofessionals.Update(healthprofessional);
                _context.SaveChanges();


                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }

        public bool DeleteVendorService(int id)
        {
            try
            {
                Healthprofessional healthprofessional = _context.Healthprofessionals.FirstOrDefault(q => q.Vendorid == id);

                healthprofessional.Isdeleted = true;
                healthprofessional.Modifieddate = DateTime.Now;

                _context.Healthprofessionals.Update(healthprofessional);
                _context.SaveChanges();


                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }


        public bool AddVendorService(VendorDetails vendorDetails)
        {
            try
            {

                Healthprofessional healthprofessional = new Healthprofessional();

                healthprofessional.Profession = vendorDetails.ProfessionId;
                healthprofessional.Vendorname = vendorDetails.BusinessName;
                healthprofessional.Businesscontact = vendorDetails.BusinessContact;
                healthprofessional.Email = vendorDetails.Email;
                healthprofessional.Faxnumber = vendorDetails.FaxNumber;
                healthprofessional.Phonenumber = vendorDetails.PhoneNumber;
                healthprofessional.Address = vendorDetails.Street;
                healthprofessional.State = _context.Regions.Where(q=>q.Regionid == vendorDetails.regionId).Select(r=>r.Name).FirstOrDefault();
                healthprofessional.City = vendorDetails.City;
                healthprofessional.Zip = vendorDetails.ZipCode;
                healthprofessional.Regionid = vendorDetails.regionId;
                healthprofessional.Createddate = DateTime.Now;
                healthprofessional.Isdeleted = false;


                _context.Healthprofessionals.Add(healthprofessional);
                _context.SaveChanges();

                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }

        public List<SearchRecords> GetFillteredSearchRecordsData(int Status, string PatientName, int RequestType, DateTime FromDate, DateTime ToDate, string ProviderName, string Email, string Phone)
        {
            try
            {

                List<Requestclient> requestclients = _context.Requestclients.ToList();
                List<SearchRecords> searchRecords = new List<SearchRecords>();

                bool IsAllStatus = false;
                bool IsAllPatientName = false;
                bool IsAllRequestTypes = false;
                bool IsAllFromDate = false;
                bool IsAllToDate = false;
                bool IsAllProviderName = false;
                bool IsAllEmail = false;
                bool IsAllPhone = false;

                if(Status == 0)
                {
                    IsAllStatus = true;
                }
                if (string.IsNullOrEmpty(PatientName))
                {
                    IsAllPatientName = true;
                }
                if(RequestType == 0)
                {
                    IsAllRequestTypes = true;
                }
                if(FromDate == DateTime.MinValue)
                {
                    IsAllFromDate= true;
                }
                if(ToDate == DateTime.MinValue)
                {
                    IsAllToDate = true;
                }
                if (string.IsNullOrEmpty(ProviderName))
                {
                    IsAllProviderName = true;
                }
                if (string.IsNullOrEmpty(Email))
                {
                    IsAllEmail = true;
                }
                if(string.IsNullOrEmpty(Phone))
                {
                    IsAllPhone = true;  
                }


                foreach(Requestclient rc in requestclients)
                {

                    SearchRecords searchRecord = new SearchRecords();

                    searchRecord.PatientName = rc.Firstname + " " + rc.Lastname;


                    searchRecord.RequestorType = _context.Requesttypes.Where(q => q.Requesttypeid == _context.Requests.Where(m => m.Requestid == rc.Requestid).Select(q => q.Requesttypeid).FirstOrDefault()).Select(q => q.Name).FirstOrDefault();

                    DateTime? serviceDate = _context.Encounterforms.Where(q => q.Requestid == rc.Requestid).Select(q => q.Createdat).FirstOrDefault();

                    searchRecord.RequestId = rc.Requestid;
                    searchRecord.DateOfService = serviceDate == null ? null : serviceDate;
                    searchRecord.CloseCaseDate = _context.Requeststatuslogs.Where(q => q.Requestid == rc.Requestid && (q.Status == 6 || q.Status == 7 || q.Status == 8)).Select(q => q.Createddate).FirstOrDefault();
                    searchRecord.Email = rc.Email;
                    searchRecord.PatientPhone = rc.Phonenumber;
                    searchRecord.Address = rc.Address;
                    searchRecord.Zip = rc.Zipcode;
                    searchRecord.RequestStatus = Enum.GetName(typeof(Status), _context.Requeststatuslogs.OrderByDescending(q => q.Createddate).Where(q => q.Requestid == rc.Requestid).Select(q => q.Status).FirstOrDefault());
                    searchRecord.Physician = _context.Physicians.Where(q => q.Physicianid == _context.Requests.Where(q => q.Requestid == rc.Requestid).Select(q => q.Physicianid).FirstOrDefault()).Select(r => r.Firstname + " " + r.Lastname).FirstOrDefault();
                    searchRecord.PatientNote = _context.Requeststatuslogs.OrderByDescending(q=>q.Createddate).Where(q=>q.Requestid == rc.Requestid && q.Status == 6).Select(q=>q.Notes).FirstOrDefault();
                    searchRecord.CanclledByProviderNote = "-";
                    searchRecord.Adminnote = _context.Requestnotes.Where(q=>q.Requestid == rc.Requestid).Select(q=>q.Adminnotes).FirstOrDefault();
                    searchRecord.PhysicianNote = _context.Requestnotes.Where(q => q.Requestid == rc.Requestid).Select(q => q.Physiciannotes).FirstOrDefault();

                    searchRecords.Add(searchRecord);

                }
                  


                searchRecords = searchRecords.Where(q=>(IsAllFromDate ||  q.DateOfService.GetValueOrDefault().Date == FromDate.Date) && (IsAllToDate || q.CloseCaseDate.GetValueOrDefault().Date == ToDate.Date)  &&   (IsAllStatus || q.RequestStatus == null || q.RequestStatus == Enum.GetName(typeof(Status),Status)) && (IsAllPatientName  || q.PatientName == PatientName) && (IsAllRequestTypes  || q.RequestorType == null || q.RequestorType == Enum.GetName(typeof(RequestorType),RequestType))   && (IsAllEmail || q.Email == Email) && (IsAllProviderName || q.Physician == null || q.Physician.ToLower().Contains(ProviderName.ToLower()) ) && (IsAllPhone || q.PatientPhone == Phone)   ).ToList();
             

                return searchRecords;
            }catch(Exception ex)
            {

                return new List<SearchRecords>();
            }
        }


        public bool DeleteRecordService(int RequestId)
        {


            try
            {

                Request request = _context.Requests.FirstOrDefault(q => q.Requestid == RequestId);

                request.Isdeleted = new BitArray(1,true);
                request.Modifieddate = DateTime.Now;

                _context.Requests.Update(request);

                _context.SaveChanges();

                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }

        public List<EmailLogs> GetFillteredEmailLogsData(string ReciverName, int RoleId, string EmailId, DateTime CreateDate, DateTime SentDate)
        {
            try
            {

                List<Requestclient> requestclients = _context.Requestclients.ToList();
                List<Emaillog> emaillogs = _context.Emaillogs.ToList();    


                List<EmailLogs> EmailLogs = new List<EmailLogs>();

                bool IsAllReciver = false;
                bool IsAllRole = false;
                  bool IsAllCreateDate = false;
                bool IsAllSentDate = false;
               bool IsAllEmail = false;
             

                if (string.IsNullOrEmpty(ReciverName))
                {
                    IsAllReciver = true;
                }
                if (RoleId == 0)
                {
                    IsAllRole = true;
                }
                if (CreateDate == DateTime.MinValue)
                {
                    IsAllCreateDate = true;
                }
                if (SentDate == DateTime.MinValue)
                {
                    IsAllSentDate = true;
                }
                if (string.IsNullOrEmpty(EmailId))
                {
                    IsAllEmail = true;
                }
           

                foreach(Emaillog emaillog in emaillogs)
                {
                    EmailLogs emailLogs = new EmailLogs();

                    emailLogs.Recipient = _context.Requests.Where(q => q.Requestid == emaillog.Requestid).Select(q => q.Firstname + "" + q.Lastname).FirstOrDefault();
                    emailLogs.Action = (int)emaillog.Action;
                    emailLogs.RoleName = _context.Roles.Where(q => q.Roleid == emaillog.Roleid).Select(q => q.Name).FirstOrDefault();
                    emailLogs.Email = emaillog.Emailid;
                    emailLogs.CreateDate = emaillog.Createdate;
                    emailLogs.SentDate = (DateTime)emaillog.Sentdate;
                    emailLogs.Sent = emaillog.Isemailsent[0];
                    emailLogs.ConfirmationNumber = emaillog.Confirmationnumber;


                    EmailLogs.Add(emailLogs);
                    
                }



                EmailLogs = EmailLogs.Where(q => (IsAllCreateDate || CreateDate.Date == q.CreateDate.Date) && (IsAllSentDate  || SentDate.Date == q.SentDate.Date) &&  (IsAllReciver || (q.Recipient != null && q.Recipient.ToLower().Contains(ReciverName.ToLower())) ) && (IsAllRole || q.RoleName == _context.Roles.Where(q => q.Roleid == RoleId).Select(q => q.Name).FirstOrDefault())  && (IsAllEmail || q.Email == EmailId)).ToList();


                return EmailLogs;
            }
            catch (Exception ex)
            {

                return new List<EmailLogs>();
            }
        }



        public bool EditOnBoardingData(ProviderDocuments providerProfileCm, int physicianid)
        {


            try
            {

            var physicianData = _context.Physicians.FirstOrDefault(x => x.Physicianid == physicianid);


                physicianData.Isagreementdoc = new BitArray(1, providerProfileCm.IsContractAggreeMent);
                physicianData.Isbackgrounddoc = new BitArray(1, providerProfileCm.IsBackGroundCheck);
                physicianData.Istrainingdoc = new BitArray(1, providerProfileCm.IsHipaa);
                physicianData.Isnondisclosuredoc =  new BitArray(1, providerProfileCm.IsNonDisClouser);


            string directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Documents", physicianData.Physicianid.ToString());

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (providerProfileCm.ContractAgrrement != null)
            {
                string path = Path.Combine(directory, "Independent_Contractor" + Path.GetExtension(providerProfileCm.ContractAgrrement.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    providerProfileCm.ContractAgrrement.CopyTo(fileStream);
                }

                physicianData.Isagreementdoc = new BitArray(1, true);
            }

            if (providerProfileCm.BackgroundCheck != null)
            {
                string path = Path.Combine(directory, "Background" + Path.GetExtension(providerProfileCm.BackgroundCheck.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    providerProfileCm.BackgroundCheck.CopyTo(fileStream);
                }

                physicianData.Isbackgrounddoc = new BitArray(1, true);
            }

            if (providerProfileCm.HIPAA != null)
            {
                string path = Path.Combine(directory, "HIPAA" + Path.GetExtension(providerProfileCm.HIPAA.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    providerProfileCm.HIPAA.CopyTo(fileStream);
                }

                physicianData.Istrainingdoc = new BitArray(1, true);
            }

            if (providerProfileCm.NonDisclouser != null)
            {
                string path = Path.Combine(directory, "Non_Disclosure" + Path.GetExtension(providerProfileCm.NonDisclouser.FileName));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    providerProfileCm.NonDisclouser.CopyTo(fileStream);
                }

                physicianData.Isnondisclosuredoc = new BitArray(1, true);
            }
            _context.SaveChanges();

                return true;
            }catch(Exception ex)
            {
                return false;
            }


            //if (providerProfileCm.LicenseDocument != null)
            //{
            //    string path = Path.Combine(directory, "Licence" + Path.GetExtension(providerProfileCm.LicenseDocument.FileName));

            //    if (File.Exists(path))
            //    {
            //        File.Delete(path);
            //    }

            //    using (var fileStream = new FileStream(path, FileMode.Create))
            //    {
            //        providerProfileCm.LicenseDocument.CopyTo(fileStream);
            //    }

            //    physicianData.Islicensedoc = new BitArray(1, true);
            //}


        }

        public List<PatientHistory> GetPatientHistoryData(string FirstName, string LastName, string Email, string Phone)
        {
            try
            {

                List<User> users = _context.Users.ToList();

                List<PatientHistory> patientHistories = new List<PatientHistory>();

                foreach(User user in users)
                {

                    PatientHistory patientHistory = new PatientHistory();

                    patientHistory.FirstName = user.Firstname;
                    patientHistory.LastName = user.Lastname;
                    patientHistory.Email = user.Email;
                    patientHistory.Phone = user.Mobile;
                    patientHistory.Address = user.Street + "," + user.City + "," + user.State + "," + user.Zipcode;
                    patientHistory.UserId = user.Userid;


                    patientHistories.Add(patientHistory);
                }


                bool IsAllFirstName = false;
                bool IsAllLastName = false;
                bool IsAllEmail = false;
                bool IsAllPhone = false;

                if (string.IsNullOrEmpty(FirstName))
                {
                    IsAllFirstName = true;
                }

                if (string.IsNullOrEmpty(LastName))
                {
                    IsAllLastName = true;
                }

                if (string.IsNullOrEmpty(Email))
                {
                    IsAllEmail = true;
                }

                if (string.IsNullOrEmpty(Phone))
                {
                    IsAllPhone = true;
                }



                patientHistories = patientHistories.Where(q => (IsAllFirstName || q.FirstName.ToLower().Contains(FirstName.ToLower())) && (IsAllLastName || q.LastName.ToLower().Contains(LastName.ToLower())) && (IsAllEmail || q.Email == Email) && (IsAllPhone || q.Phone == Phone)).ToList();


                return patientHistories;
            }
            catch(Exception ex)
            {
                return new List<PatientHistory>();
            }
        }



        public List<PatientExplore> GetpatientExploreData(int userId)
        {


            try
            {


                List<Request> requests = _context.Requests.Where(q =>q.Userid == userId && q.Isdeleted == new BitArray(1, false)).ToList();


                List<PatientExplore> patientExplores = new List<PatientExplore>();


                foreach (Request request in requests)
                {

                    PatientExplore patientExplore = new PatientExplore();

                    patientExplore.PatientName = request.Firstname + " " + request.Lastname;
                    patientExplore.CreatedDate = request.Createddate;
                    patientExplore.Confirmation = request.Confirmationnumber;
                    //patientExplore.IsFinalized = _context.Encounterforms.Where(q => q.Requestid == request.Requestid).Select(q => q.IsFinelized).FirstOrDefault();
                    patientExplore.IsFinalized = false;
                    patientExplore.RequestId = request.Requestid;

                    if (request.Physicianid != null)
                    {
                        patientExplore.Providername = _context.Physicians.Where(q => q.Physicianid == request.Physicianid).Select(q => q.Firstname + " " + q.Lastname).FirstOrDefault();

                    }
                    else
                    {
                        patientExplore.Providername = null;
                    }


                    if (request.Status >= 6)
                    {
                        patientExplore.ConcludeDate = (DateTime)_context.Requeststatuslogs.Where(q => q.Status == 6).Select(q => q.Createddate).FirstOrDefault();
                    }
                    else
                    {
                        patientExplore.ConcludeDate = null;
                    }

                    patientExplores.Add(patientExplore);


                }
                    return patientExplores;

            }catch(Exception ex)
            {
                return new List<PatientExplore>();
            }

        }



        public List<BlockHistories> GetBlockHistoriesData(string name,DateTime createdAt,string Email,string Phone)
        {


            try
            {

                List<Blockrequest> blockrequests = _context.Blockrequests.ToList();

                List<BlockHistories> patientHistories = new List<BlockHistories>();

                foreach (Blockrequest req in blockrequests)
                {

                    BlockHistories blockreq = new BlockHistories();

                    blockreq.PatientName = _context.Requests.Where(q => q.Requestid.ToString() == req.Requestid).Select(q => q.Firstname + " "+q.Lastname).FirstOrDefault();
                    blockreq.PhoneNumber = req.Phonenumber;
                    blockreq.Email = req.Email;
                    blockreq.CratedAr = (DateTime)req.Createddate;
                    blockreq.BlockNotes = req.Reason;
                    blockreq.IsActive = req.Isactive[0];
                    blockreq.RequestId = int.Parse(req.Requestid);

                    patientHistories.Add(blockreq);
                }


                bool IsAllName = false;
                bool IsAllDate = false;
                bool IsAllEmail = false;
                bool IsAllPhone = false;

                if (string.IsNullOrEmpty(name))
                {
                    IsAllName = true;
                }

                if (createdAt == DateTime.MinValue)
                {
                    IsAllDate = true;
                }

                if (string.IsNullOrEmpty(Email))
                {
                    IsAllEmail = true;
                }

                if (string.IsNullOrEmpty(Phone))
                {
                    IsAllPhone = true;
                }



                patientHistories = patientHistories.Where(q => (IsAllName || q.PatientName.ToLower().Contains(name.ToLower())) && (IsAllDate || q.CratedAr == createdAt) && (IsAllEmail || q.Email == Email) && (IsAllPhone || q.PhoneNumber == Phone)).ToList();


                return patientHistories;
            }
            catch (Exception ex)
            {
                return new List<BlockHistories>();
            }
        }


        public bool IsRequestBelongToProvider(int physicianId,int requestId)
        {

            return _context.Requests.Any(q => q.Requestid == requestId && q.Physicianid == physicianId);
        }

        public string GetSearchRecordsExcelFIle(List<SearchRecords> sr)
        {
            try
            {

                string[] header = { "Patientname", "Requestor", "Date of Service", "Close Case Date", "Email", "Phone", "Address", "Zip", "RequestStatus", "Physician" };

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
                    foreach (SearchRecords req in sr)
                    {

                        worksheet.Cell(row, j + 1).Value = req.PatientName;

                        j++;

                        worksheet.Cell(row, j + 1).Value = req.RequestorType;

                        j++;

                        worksheet.Cell(row, j + 1).Value = req.DateOfService;
                        j++;
                        worksheet.Cell(row, j + 1).Value = req.CloseCaseDate;
                        j++;
                        worksheet.Cell(row, j + 1).Value = req.Email;
                        j++;
                        worksheet.Cell(row, j + 1).Value = req.PatientPhone;
                        j++;
                        worksheet.Cell(row, j + 1).Value = req.Address;
                        j++;
                        worksheet.Cell(row, j + 1).Value = req.Zip;
                        j++;
                        worksheet.Cell(row, j + 1).Value = req.RequestStatus;
                        j++;
                        worksheet.Cell(row, j + 1).Value = req.Physician;
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

                        return "https://localhost:7008/Upload/" + fileName;

                    }
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }


        public MDsOnCallModel GetProviderOnCallData(int regionId)
        {


            var currentTime = new TimeOnly(DateTime.Now.Hour, DateTime.Now.Minute);
            BitArray deletedBit = new BitArray(new[] { false });


            List<Physician> physicians = GetPhysicinForShiftsByRegionService(regionId);

            Dictionary<int, int> AvailablePhysician = new Dictionary<int, int>();

            foreach(Physician p in physicians)
            {
                AvailablePhysician[p.Physicianid] = 1;
            }


            Dictionary<int, int> PhysicianId = new Dictionary<int, int>();

            List<Physician> OnCallPhysicians = new List<Physician>();

            List<Shiftdetail> shiftdetsils = _context.Shiftdetails.Where(sd => sd.Shiftdate.Date == DateTime.Today && currentTime >= sd.Starttime && currentTime <= sd.Endtime && sd.Isdeleted == false).ToList();


            foreach(Shiftdetail shiftdetail in shiftdetsils)
            {

                int physicianId = _context.Shifts.Where(q => q.Shiftid == shiftdetail.Shiftid).Select(q => q.Physicianid).FirstOrDefault();

                if (!AvailablePhysician.ContainsKey(physicianId))
                {
                    continue;
                }

                Physician physician = _context.Physicians.Where(q => q.Physicianid == physicianId).FirstOrDefault();

                OnCallPhysicians.Add(physician);
                 
                if(!PhysicianId.ContainsKey(physicianId))
                {
                    PhysicianId[physicianId] = 1;
                }
                else
                {
                    PhysicianId[physicianId]++;
                }
            }

           List<Physician> OffDutyPhysician = new List<Physician>();

            foreach(Physician physician in physicians)
            {
                if (!PhysicianId.ContainsKey(physician.Physicianid))
                {
                    OffDutyPhysician.Add(physician) ;
                }
            }



            MDsOnCallModel mdOnCall = new MDsOnCallModel
            {
                OnCall = OnCallPhysicians,
                OffDuty = OffDutyPhysician,
               
            };

            return mdOnCall;


        }



        //public LatLang GetLatLangFromZipCode(string ZipCode)
        //{
        //    var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={zipcode}&key={_apiKey}";

        //    var response = await _httpClient.GetAsync(url);

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        throw new HttpRequestException($"Failed to fetch data from the Geocoding API. Status code: {response.StatusCode}");
        //    }

        //    var contentStream = await response.Content.ReadAsStreamAsync();
        //    var json = await JsonSerializer.DeserializeAsync<GeocodingResponse>(contentStream);

        //    if (json is not null && json.Status == "OK" && json.Results.Length > 0)
        //    {
        //        var location = json.Results[0].Geometry.Location;
        //        return (location.Lat, location.Lng);
        //    }
        //    else
        //    {
        //        throw new Exception("Failed to fetch latitude and longitude.");
        //    }
        //}



    }
}
