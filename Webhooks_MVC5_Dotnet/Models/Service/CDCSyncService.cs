////*********************************************************
// <copyright company="Intuit">
// Author:Nimisha
//
////*********************************************************
using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.QueryFilter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Webhooks.Models.DTO;
using Webhooks.Models.Service;
using System.Configuration;

namespace Webhooks.Models.Service
{
    public class CDCSyncService
    {
        DataServiceFactory dataserviceFactory = null;
        DataService dataService = null;
        //CDCSyncdto syncObjects = null;

        /// <summary>
        /// Fire up the service context in the constructor.
        /// </summary>
        /// <param name="oAuthorization"></param>
        public CDCSyncService(OAuthTokensRealmLastUpdateddto oAuthorization)
        {
            dataserviceFactory = new DataServiceFactory(oAuthorization);
            dataService = dataserviceFactory.getDataService();
            //syncObjects = new CDCSyncdto();

        }
        
        public ServiceContext ServiceContext { get { return dataserviceFactory.getServiceContext; } }
        
        
        
        /// <summary>
        /// Get CDC details
        /// </summary>
        /// <returns></returns>
        internal static IntuitCDCResponse GetCDCDetails(OAuthTokensRealmLastUpdateddto OAuthTokensRealmLastUpdateddto)
        {
            List<IEntity> entityList = new List<IEntity>();
            string getEntitiesForCDC = ConfigurationManager.AppSettings["WebhooksEntities"].ToString();

            //Get all configured entities for Webhooks to make cdc call
            //Break config values and then add to list of entities to fecth with CDC operation 
            string[] entities = getEntitiesForCDC.Trim().Split(',');
            foreach (string entity in entities)
            {

                switch (entity.ToLower())
                {
                    case "customer":
                        entityList.Add(new Customer());
                        break;
                    case "invoice":
                        entityList.Add(new Invoice());
                        break;
                    case "salesreceipt":
                        entityList.Add(new SalesReceipt());
                        break;
                    case "estimate":
                        entityList.Add(new Estimate());
                        break;
                    case "vendor":
                        entityList.Add(new Vendor());
                        break;
                    case "account":
                        entityList.Add(new Account());
                        break;
                    case "payment":
                        entityList.Add(new Payment());
                        break;
                    case "class":
                        entityList.Add(new Class());
                        break;
                    case "Item":
                        entityList.Add(new Item());
                        break;
                    case "billpayment":
                        entityList.Add(new BillPayment());
                        break;
                    case "employee":
                        entityList.Add(new Employee());
                        break;
                    case "purchase":
                        entityList.Add(new Purchase());
                        break;
                    default:
                        break;
                }
            }
                       
            //Create a CDCSyncService object
            CDCSyncService sync = new CDCSyncService(OAuthTokensRealmLastUpdateddto);//check for consumer key n secret
            
            //Get Dataservice object for calling CDC
            DataService qboService = new DataService(sync.dataserviceFactory.getServiceContext);
            
            //Make CDC call
            IntuitCDCResponse CDCResponse = qboService.CDC(entityList, OAuthTokensRealmLastUpdateddto.OAuthTokens.realmlastupdated);
           

            return CDCResponse;
        }

        

    }
}