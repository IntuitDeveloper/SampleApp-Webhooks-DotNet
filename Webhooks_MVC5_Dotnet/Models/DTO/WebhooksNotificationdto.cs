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
using System.Security.Cryptography;
using Newtonsoft.Json;


namespace Webhooks.Models.DTO
{
    //Model for mapping Webhooks response to diff classes for deserialization
    public class WebhooksNotificationdto
    {
        public string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["DBContext"].ToString(); }

        }
        public class Entities
        {
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("id")]
            public string Id { get; set; }
            [JsonProperty("operation")]
            public string Operation { get; set; }
            [JsonProperty("lastUpdated")]
            public string LastUpdated { get; set; } //validate type
        }

        public class DataChangeEvents
        {
            [JsonProperty("entities")]
            public List<Entities> Entities { get; set; }

        }
        public class EventNotifications
        {
            [JsonProperty("realmId")]
            public string RealmId { get; set; }

            [JsonProperty("dataChangeEvent")]
            public DataChangeEvents DataEvents { get; set; }

        }
        public class WebhooksData
        {


            [JsonProperty("eventNotifications")]
            public List<EventNotifications> EventNotifications { get; set; }

        }

    }
    
}