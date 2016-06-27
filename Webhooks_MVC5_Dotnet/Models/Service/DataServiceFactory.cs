////*********************************************************
// <copyright company="Intuit">
// Author:Nimisha
//
////*********************************************************
using Intuit.Ipp.Core;
using Intuit.Ipp.DataService;
using Intuit.Ipp.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Webhooks.Models.DTO;

namespace Webhooks.Models.Service
{
    public class DataServiceFactory
    {
        private OAuthRequestValidator oAuthRequestValidator = null;
        private DataService dataService = null;
        IntuitServicesType intuitServicesType = new IntuitServicesType();
        private ServiceContext serviceContext = null;
        public ServiceContext getServiceContext { get; set; }
        /// <summary>
        /// Allocate memory for service context objects
        /// </summary>
        public DataServiceFactory(OAuthTokensRealmLastUpdateddto oAuthorization)
        {
            try
            {
                oAuthRequestValidator = new OAuthRequestValidator(
                oAuthorization.OAuthTokens.access_token, 
                oAuthorization.OAuthTokens.access_secret, 
                oAuthorization.ConsumerKey, 
                oAuthorization.ConsumerSecret);
            intuitServicesType = oAuthorization.OAuthTokens.datasource == "QBO" ? IntuitServicesType.QBO : IntuitServicesType.None;
            serviceContext = new ServiceContext(oAuthorization.OAuthTokens.realmid.ToString(), intuitServicesType, oAuthRequestValidator);
            serviceContext.IppConfiguration.BaseUrl.Qbo = ConfigurationManager.AppSettings["ServiceContext.BaseUrl.Qbo"];
            //serviceContext.IppConfiguration.Logger.RequestLog.EnableRequestResponseLogging = true;
            //serviceContext.IppConfiguration.Logger.RequestLog.ServiceRequestLoggingLocation = ConfigurationManager.AppSettings["ServiceRequestLoggingLocation"];
            getServiceContext = serviceContext;
            dataService = new DataService(serviceContext);
            }
            catch (Intuit.Ipp.Exception.FaultException ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Return the current data service
        /// </summary>
        public DataService getDataService()
        {
            return dataService;
        }
    }
}