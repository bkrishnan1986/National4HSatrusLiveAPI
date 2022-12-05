using Microsoft.Xrm.Sdk.Client;
using System;
using System.Configuration;

namespace National4HSatrusLive.Services
{
    public class OrganizationService
    {
        public static OrganizationServiceProxy GetCrmService()
        {
            var crmUrl = new Uri(ConfigurationManager.ConnectionStrings["CrmWebServer"].ConnectionString);
            var authCredentials = new AuthenticationCredentials();
            authCredentials.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["CrmUserName"];
            authCredentials.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["CrmPassword"];

            var creds = new AuthenticationCredentials();
            var service = new OrganizationServiceProxy(uri: crmUrl, homeRealmUri: null, clientCredentials: authCredentials.ClientCredentials, deviceCredentials: null);

            return service;
        }
    }
}