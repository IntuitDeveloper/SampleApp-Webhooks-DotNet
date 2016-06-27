////*********************************************************
// <copyright company="Intuit">
// Author:Nimisha
//
////*********************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Data.OleDb;
using Webhooks.Models.DTO;
using System.Web.Script.Serialization;
using System.Collections;
using Webhooks.Models.Service;
using System.Collections.Concurrent;
using System.Configuration;




namespace Webhooks.Models.Utility
{
    public class ProcessNotificationData
    {    
        private static string payloadLoaded = null;

        /// <summary>
        /// HASH the notification payload with HMAC_SHA256_ALGORITHM using <verifier> as the key
        /// Compare the value from above step to the intuit-signature header from the notification. The values should be identical.
        /// </summary>        
        //public static bool Validate(string payload, string verifier)
        public static bool Validate(string payload, object hmacHeaderSignature)
        {
            payloadLoaded = payload;
            
            //Get Webhooks verifier
            string verifier = ConfigurationManager.AppSettings["WebHooksVerifier"].ToString();

            if (hmacHeaderSignature == null)
            {
                return false;
            }
            try
            {
                var keyBytes = Encoding.UTF8.GetBytes(verifier);
                var dataBytes = Encoding.UTF8.GetBytes(payloadLoaded);

                //use the SHA256Managed Class to compute the hash
                var hmac = new HMACSHA256(keyBytes);
                var hmacBytes = hmac.ComputeHash(dataBytes);

                //Get payload signature value. 
                //var hmacHeaderSignature = HttpContext.Current.Request.Headers[SIGNATURE];//Header value
                var createPayloadSignature = Convert.ToBase64String(hmacBytes);//Payload value
                //spawn new thread

                //Compare webhooks response payload's signature with the signature passed in the header of the post webhooks request from Intuit. If they match, the call is verified.
                if ((string)hmacHeaderSignature == createPayloadSignature)
                {
                    //Add response payload to queue in a separate thread for processing if signature matches
                    Thread thread1 = new Thread(new ThreadStart(AddToQueue));
                    thread1.Start();
                    thread1.Join(60000);

                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;

            }

        }

      

        /// <summary>
        /// Add webhooks notifications to queue
        /// </summary>  
        private static void AddToQueue()
        {
            
            //Deserealiaze webhooks response payload
            WebhooksNotificationdto.WebhooksData webhooksData = JsonConvert.DeserializeObject<WebhooksNotificationdto.WebhooksData>(payloadLoaded);

            //Create a blocking queue/collection
            BlockingCollection<WebhooksNotificationdto.WebhooksData> dataItems = new BlockingCollection<WebhooksNotificationdto.WebhooksData>(1);


            Task.Run(() =>
            {
                  
                    // Add items to blocking collection(queue)
                    dataItems.Add(webhooksData);
                
                // Let consumer know we are done.
                dataItems.CompleteAdding();
            });

            Task.Run(() =>
            {
                while (!dataItems.IsCompleted)
                {
                    //Create WebhooksData reference
                    WebhooksNotificationdto.WebhooksData webhooksData1 = null;
             
                    try
                    {
                        //Take our Items from blocking collection(dequeue)
                        webhooksData1 = dataItems.Take();
                    }
                    catch (InvalidOperationException) { }

                    if (webhooksData1 != null)
                    {
                        //Start processing queue items
                        ProcessQueueItem(webhooksData1);
                    }
                }
                
            });
            

        }
           

        /// <summary>
        /// Process each item of queue
        /// </summary>  
        private static bool ProcessQueueItem(object queueItem1)
        {
            WebhooksNotificationdto.WebhooksData we = (WebhooksNotificationdto.WebhooksData)queueItem1;

            //Get realm from deserialized WebHooksData 
            foreach (WebhooksNotificationdto.EventNotifications eventNotifications in we.EventNotifications)
            {

                //Update last updated time for each realm in DB
                DBUtility.UpdateLastUpdatedDateDB(eventNotifications.RealmId);

            }
            return true;

        }


    }
}
