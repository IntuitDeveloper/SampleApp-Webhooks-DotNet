////*********************************************************
// <copyright company="Intuit">
// Author:Nimisha
//
////*********************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Webhooks.Models.DTO;
using Webhooks.Models.Service;
using System.IO;
using System.Text;

namespace Webhooks.Models.Utility
{
    public static class DBUtility
    {

        /// <summary>
        /// Add some intial data to DB
        /// </summary> 
        public static void AddInitialDatatoDB()
        {
            OAuthTokensRealmLastUpdateddto dbSettings = new OAuthTokensRealmLastUpdateddto();
            //Get connectionString for WebHooksDB
            string connectionString = dbSettings.ConnectionString;

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                DateTime currDate = DateTime.Now;
                //Add a sandbox realm and oauth keys for testing webhooks
                string s1 = "insert into OAuthTokens values('123145693359857','2016-05-06','qyprdFBgt5mkWX6nrsqtKPgKgzeB3rqTGwqANpV8YOnCWUK9','XaZCEi6T6J1s689RfEEsDDtQe5ASfk5LbaNqFILQ','QBO' )";
                using (SqlCommand myCommand = new SqlCommand(s1, sqlConnection))
                {

                    myCommand.CommandType = CommandType.Text;
                    myCommand.ExecuteNonQuery();

                }
                //Add a prod realmid and oauth keys for testing webhooks
                string s2 = "insert into OAuthTokens values('1269959970','2016-05-06','qyprdIyga7rdFS9Oe9IyXWZqqs6cYhfXUmR4iQD6XqX3iIrU','PYly9uD6DB8togTDfiqrOytMxafp2udvA5ds1qFV','QBO' )";
                using (SqlCommand myCommand = new SqlCommand(s2, sqlConnection))
                {

                    myCommand.CommandType = CommandType.Text;
                    myCommand.ExecuteNonQuery();

                }
                sqlConnection.Close();
            }
        }

        /// <summary>
        /// Call CDC for each realm for the configured entitites in app.
        /// Update the realmlastupdated col in OauthTokens DB
        /// </summary>  
        public static void UpdateLastUpdatedDateDB(string realmId)
        {
           

            OAuthTokensRealmLastUpdateddto dbSettings = new OAuthTokensRealmLastUpdateddto();
            //Get connectionString for WebHooksDB
            string connectionString = dbSettings.ConnectionString;

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                //Get OAuth tokens for making CDC call for the realm
                string query1 = "select * from OAuthTokens where realmid='"+ realmId +"'";
                using (SqlCommand myCommand1 = new SqlCommand(query1, sqlConnection))
                {
                    sqlConnection.Open();
                    using (SqlDataReader rd = myCommand1.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            OAuthTokensRealmLastUpdateddto oauthRealmLastUpdateddto = new OAuthTokensRealmLastUpdateddto();
                            oauthRealmLastUpdateddto.OAuthTokens  = new OAuthTokens();
                            oauthRealmLastUpdateddto.OAuthTokens.realmid = rd["realmid"].ToString();
                            //Check if this is required or not
                            //string testAccessToken = DBUtility.Decrypt(currentIndex.access_token, OAuthTokens_RealmLastUpdated.SecurityKey);
                            //string testAccessTokenSecret = DBUtility.Decrypt(currentIndex.access_secret, OAuthTokens_RealmLastUpdated.SecurityKey);
                            oauthRealmLastUpdateddto.OAuthTokens.access_token = rd["access_token"].ToString();
                            oauthRealmLastUpdateddto.OAuthTokens.access_secret = rd["access_secret"].ToString();
                            //OAuthTokensRealmLastUpdateddto.ConsumerKey =  ;
                            //OAuthTokensRealmLastUpdateddto.ConsumerSecret = ;

                            oauthRealmLastUpdateddto.OAuthTokens.datasource = rd["datasource"].ToString();
                            oauthRealmLastUpdateddto.OAuthTokens.realmlastupdated = (DateTime)rd["realmlastupdated"];

                            //Get CDC details for configured entities for the app for the time in the OAuthToken tables' realmlastupdated column
                            var cdcResponse = CDCSyncService.GetCDCDetails(oauthRealmLastUpdateddto);


                        }
                        rd.Close();
                    }
                   
                }

                //Update realmlastupdatedtime in OAuthTokens table
                DateTime cdcLastRunTime = DateTime.Now;
                string query2 = "UPDATE OAuthTokens SET realmlastupdated=@cdcLastRunTime WHERE realmid=@realmId";
                using (SqlCommand myCommand2 = new SqlCommand(query2, sqlConnection))
                {
                    
                    myCommand2.Parameters.AddWithValue("@cdcLastRunTime", cdcLastRunTime);
                    myCommand2.Parameters.AddWithValue("@realmId", realmId);
                    myCommand2.ExecuteNonQuery();
                   
                }
                sqlConnection.Close();
            }


        }
    }
}