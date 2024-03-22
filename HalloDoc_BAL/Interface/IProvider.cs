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
    }
}
