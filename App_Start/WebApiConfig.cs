using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using National4HSatrusLive.Controllers;
using National4HSatrusLive.Models;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.ServiceModel.Description;
using System.Web.Http;

namespace National4HSatrusLive
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var jsonFormatter = config.Formatters.JsonFormatter;
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            jsonFormatter.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
        }

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
