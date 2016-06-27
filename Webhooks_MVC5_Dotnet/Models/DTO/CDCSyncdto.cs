////*********************************************************
// <copyright company="Intuit">
// Author:Nimisha
//
////*********************************************************
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.QueryFilter;

namespace Webhooks.Models.DTO
{
    public class CDCSyncdto
    {
        
        
        public string CompanyId { get; set; }      
        public OAuthTokensRealmLastUpdateddto OauthToken { get; set; }

        //Get connectionstring from config
        public string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["DBContext"].ToString(); }

        }
        
       
    }
    
}