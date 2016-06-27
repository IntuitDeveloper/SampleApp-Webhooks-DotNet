////*********************************************************
// <copyright company="Intuit">
// Author:Nimisha
//
////*********************************************************
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.IO;
using Webhooks.Models.Utility;
using System.Configuration;
using System.Net.Http;


namespace Webhooks.Controllers
{
    public class HomeController : Controller
    {

        
        //[ValidateAntiForgeryToken]
        public ActionResult Index()
        {   //Add intial Data to DB by running the sql cmds from Scripts->InsertScriptWebhooks.sql

            //Get webhooks response payload
            string jsonData = null;
            object hmacHeaderSignature=null;
            if (System.Web.HttpContext.Current.Request.InputStream.CanSeek)
            {
                //Move the cursor to beginning of stream if it has already been by json process
                System.Web.HttpContext.Current.Request.InputStream.Seek(0, SeekOrigin.Begin);
                jsonData = new StreamReader(this.Request.InputStream).ReadToEnd();
                //Get the value of webhooks header's signature
                hmacHeaderSignature = System.Web.HttpContext.Current.Request.Headers["intuit-signature"];
            }
             
            //Validate webhooks response by hading it with HMACSHA256 algo and comparing it with Intuit's header signature
            bool isRequestvalid = ProcessNotificationData.Validate(jsonData, hmacHeaderSignature);

            //If request is valid, send 200 Status to webhooks sever
            if (isRequestvalid == true)
            {
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
          
            //Defult pgae displayed will be the Index view page when application is running
            return View();
            
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //[HttpPost, ActionName("Index")]
        //public ActionResult IndexPost()
        //{
        //    ViewBag.Title = "Home Page1";

        //    return View();
        //}
              

    }
}