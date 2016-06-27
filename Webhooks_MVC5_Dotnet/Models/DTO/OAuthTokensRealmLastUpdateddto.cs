////*********************************************************
// <copyright company="Intuit">
// Author:Nimisha
//
////*********************************************************
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;


namespace Webhooks.Models.DTO
{
    public class OAuthTokensRealmLastUpdateddto
    {
       

        //Get OauthTokens table fields
        public OAuthTokens OAuthTokens { get; set; }
        

        //Get DB connection string from config where Oauth tokens and Realm's Last Updated time is saved
        public string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["DBContext"].ToString(); }

        }   
        

        //Get app consumer key from config
        public string ConsumerKey
        {
            get
            {
                return ConfigurationManager.AppSettings["ConsumerKey"];
            }
        }

        //Get app consumer secret from config
        public string ConsumerSecret
        {
            get
            {
                return ConfigurationManager.AppSettings["ConsumerSecret"];
            }
        }   
           
      
    }



    /// <summary>
    /// Properties mapper class for OAuthTokens DB
    /// </summary>
    public class OAuthTokens
    {

        [Key]
        public int Id { get; set; }
        public string realmid { get; set; }
        public DateTime realmlastupdated { get; set; }
        public string access_token { get; set; }
        public string access_secret { get; set; }
        public string datasource { get; set; }
    }



    /// <summary>
    /// DBContext for OAuthTokens
    /// </summary>
    public class OAuthRealmLastUpdateddataContext : DbContext
    {

        public OAuthRealmLastUpdateddataContext(string connString)
            : base(connString)
        {
        }
        public DbSet<OAuthTokens> Tokens_RealmLastUpdatedDate { get; set; }
    }
    
}