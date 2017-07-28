**SampleApp-Webhooks-DotNet for Oauth1 app**
---------------------------------------------

This sample app is meant to provide working examples of how to integrate your app with the Intuit Small Business ecosystem. Specifically, this sample application demonstrates the following:

* Implementing webhooks endpoint to receive event notifications.
* Best practices to be followed while processing the event notifications.
* Sample code using [QuickBooks Online SDK]( https://developer.intuit.com/docs/0100_quickbooks_online/0400_tools/0005_accounting/0010.net_tools/0060_installing_the_.net_sdk) to call CDC API to sync data between the app and the QuickBooks Online company.

Please note that while these examples work, features not called out above are not intended to be taken and used in production business applications. In other words, this is not a seed project to be taken cart blanche and deployed to your production environment.

For example, certain concerns are not addressed at all in our samples (e.g. security, privacy, scalability). In our sample apps, we strive to strike a balance between clarity, maintainability, and performance where we can. However, clarity is ultimately the most important quality in a sample app.

Therefore there are certain instances where we might forgo a more complicated implementation (e.g. caching a frequently used value, robust error handling, more generic domain model structure) in favor of code that is easier to read. In that light, we welcome any feedback that makes our samples apps easier to learn from.

**Table of Contents**
----------------------------
*	[Requirements](#Requirements)
*	[First Use Instructions](#First Use Instructions)
*	[Running the code](#Running the code)
*	[Project Structure](#Project Structure)
*	[Reset the App](#Reset the app)


**Requirements**
-----------------------
In order to successfully run this sample app you need a few things:

1.	Visual Studio 2015 with MVC5 and SQL server express 2012, .Net Framework 4.6.1
2.	A [developer.intuit.com]( https://developer.intuit.com/) account
3.	An app on [developer.intuit.com]( https://developer.intuit.com/) and the associated app token, consumer key, and consumer secret.
4.	[QuickBooks .NET SDK]( https://developer.intuit.com/docs/0100_quickbooks_online/0400_tools/0005_accounting/0010.net_tools/0060_installing_the_.net_sdk) (already included in the sample app). You need to update/restore it using Nuget Package Manager
5.	Two [sandbox]( https://developer.intuit.com/v2/ui#/sandbox) companies, connect both companies with your developer.intuit.com->QBO app and generate the oauth tokens using existing sample apps or OAuth Playground. (The tokens and app keys would be required for saving the tokens in the webhooks sample app and testing the notification for the sandbox company.)
6.	[Fiddler]( http://www.telerik.com/fiddler)
7.	[Ngrok]( https://ngrok.com/)
8.	[Azure](https://azure.microsoft.com) subscription

**First Use Instructions**
------------------------------
1.	Clone/Download the GitHub repo to your computer
2.	Fill in the [web.config]( Webhooks_MVC5_Dotnet/Web.config) with values (app token, consumer key, consumer secret) by copying over from the keys section for your app.
3.	Also add webhooks subscribed entities in comma separated format(WebhooksEntities) and webhooks verifier token that was generated when you subscribed for webhoooks event.
4.	Enable logging in the config file by updating the local folder path in web.config(ServiceRequestLoggingLocation).
5.	Open the project from Visual Studio
6.	Populate the data in to tables from /Scripts folder after modifying the values for your sandbox companies’ realmid, some default datetime, access token, access token secret, daatasource as QBO. Run it only once.

**Running the code**
----------------------------
For webhooks, it is a requirement that your subscribe curl should be exposed over internet. It can be a rest endpoint or an azure app service. In the sample app we have, I will demonstrate both azure and localhost based testing for the webhooks. 
Once the sample app code is set on your computer, you can do the following steps to run the app:

**For Ngrok based local testing of the sample app-**

1.	Build the the project. Resolve any dll conflicts using Nuget Package Manager.
2.	Run the initial scripts for adding sandbox apptokens to the LocalDB->OAuthTokens table from Scripts->InsertScriptWebhooks.sql
3.	Download ‘ngrok.exe’ from https://ngrok.com/. This will help in mapping the localhost port of the sample app to the ngrok url exposed over internet.
4.	Run your sample app to see what port is displayed on the browser.
5.	Suppose it is running on https://localhost:49304/. After this, save this  port information in a text file and then you need stop the sample app.
6.	Run ngrok.exe and then type the following cmd-
ngrok http 49304 -host-header="localhost:49304"
(Here my sample app’s port number was 49304. In your case it might be different. So change the values below in port to the value you have saved from step 3.)
Generic cmd- ngrok http <<port>> -host-header=”localhost:<<port>>”
7.	Then you will get a mapping https url in the ngrok cmd prompt-
          Forwarding       https://92832de0.ngrok.io -> localhost:49304
        (Do not use the http url rather only https as only that supports webhooks)
8.	Copy the url https://92832de0.ngrok.io and paste it in browser and run it. You will get a bad gateway or some other error. Ignore it. Do not close the browser.
9.	Copy this same url https://92832de0.ngrok.io as the webhooks url for the app on developer.intuit.com and save.
10.	Now add a breakpoint point at the start of your sample app in HomeController. 
11.	Then run the sample app and it will open localhost:49304 on a second tab of the browser.
12.	Close the localhost:49304 tab but the Visual Studio instance will keep running from IDE and the iis process will attach to https://92832de0.ngrok.io now.
13.	 Do F10 for next 1 or 2 lines to make sure ngrok url is indeed attached to the iis process. 
14.	Then remove the breakpoint and do F5(run).
15.	This is it. You are now exposing the sample app over internet via ngrok url.
16.	The WebHooks server can now send notifications to your sample app.
17.	You can download fiddler from google and run it alongside your sample app. 
18.	Make some changes in the sandbox company you have.
19.	In about 5 mins, you can see fiddler logs the send a 200 response for the post on url  https://92832de0.ngrok.io and there is also a CDC request and response logged.
20.	You can then go to the LocalDb table OAuthTokens in the sample app and right click and do Show Data or do a New query->  Select * from OAuthTokens. You will see the realmlastupdated date-time has changed to reflect the webhooks notifications received time.
21.	You can keep this ngrok.exe and https://92832de0.ngrok.io url running along with fiddler to track notifications are received after every 5 mins if there are changes as frequent.

**For Azure based cloud testing of the sample app-**

**Note:You should have a valid Azure subscription to test this.** 

1.	Build the the project. Resolve any dll conflicts using Nuget Package Manager.
2.	Comment out the logging code in the DataServiceFactory.
	
//serviceContext.IppConfiguration.Logger.RequestLog.EnableRequestResponseLogging = true;
//serviceContext.IppConfiguration.Logger.RequestLog.ServiceRequestLoggingLocation = ConfigurationManager.AppSettings["ServiceRequestLoggingLocation"];

3.	Build the project in release mode. Right click on the project. Do publish.
4.	Go to Profile->Select a publish target->Microsoft Azure App service->Enter your Azure creds->Select subscription and create a New resource group if you do not have any.
5.	The Hosting tab will then display your selected values.
6.	Then go to Services tab.
7.	Add a SQL Sever and DB for WebHooks.
8.	Then select Create button.
9.	This will start the publish process of the sample app to Azure.
10.	Add a Firewall rule to the SQL server on Azure for adding the IP of the computer from where you will be accessing the server.
11.	Then from left side go to ->Server explorer->Azure->SQL databases->Right click your DB ->SQL Server Object Explorer.
12.	Open the Webhooks Db that you just now created on Azure from SQL Server Object Explorer.
13.	Right click on Tables->Add new table and then run this script
CREATE TABLE \[dbo].\[OAuthTokens] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [realmid]          NVARCHAR (MAX) NOT NULL,
    [realmlastupdated] DATETIME       NOT NULL,
    [access_token]     NVARCHAR (MAX) NULL,
    [access_secret]    NVARCHAR (MAX) NULL,
    [datasource]       NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_dbo.OAuthTokens] PRIMARY KEY CLUSTERED ([Id] ASC)
);
14.	Do Update from Top menu. This will create the OAuthTokens table in Webhooks DB you created on Azure.
15.	Then go to Azure Portal->SQL DB->WebHooks Databse->Settings->Get Ado.net connection string for this DB.
16.	Go to the sample apps web.config. Change the LocalDB connectionstring to now map to Azure Webhooks DB.
It should look something like this.
<add name="DBContext" connectionString="Server=tcp:webhooksserver.database.windows.net,1433;Data Source=webhooksserver.database.windows.net;Initial Catalog=WebhooksMVC5DotnetTest_db;Persist Security Info=False;User ID=fff;Password=fff;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30" providerName="System.Data.SqlClient"/>

17.	 Then save the sample app.
18.	 Run the initial scripts for adding sandbox apptokens to the Webhooks Azure DB from Scripts->InsertScriptWebhooks.sql
19.	Then again save the sample app. Build it in debug and release mode.
20.	Again right click your project in solution explorer and publish again.
21.	Then your sample app will be hosted on cloud and the browser will run the sample app. Copy the cloud/azure app url.
22.	Configure the cloud app url in the webhooks app on developer.intuit.com and you should start getting notifications on the sample app.
23.	Make some changes in sandbox company and then go to the Webhooks Azure OAuthTokens in the sample app in SQL Server Object Explorer ->right click and do Show Data or do a New query->  Select * from OAuthTokens
24.	You will see updated realmlastupdatedtime.
25.	Note: By default, Azure already enables HTTPS for your app with a wildcard certificate for the *.azurewebsites.net domain. So, verify that https in indeed used for wbehooks.

**Project Structure**
--------------------------------
**Standard MVC5 template is used for the sample app**

**Controller-**

* [HomeController.cs]( Webhooks_MVC5_Dotnet/Controllers/HomeController.cs)  (For receiving post of webhooks notifications from Intuit server)

**Models-**

**DTO-**

* [OAuthTokensRealmLastUpdateddto.cs]( Webhooks_MVC5_Dotnet/Models/DTO/OAuthTokensRealmLastUpdateddto.cs)  (Wrapper for OAuthTokens table data)
* [WebhooksNotificationdto.cs]( Webhooks_MVC5_Dotnet/Models/DTO/WebhooksNotificationdto.cs)  (Wrapper for Webhooks Notifications from POST on the sample app’s url)

**Service-**

* [DataServiceFactory.cs]( Webhooks_MVC5_Dotnet/Models/Service/DataServiceFactory.cs)  (For getting the ServiceContext object from App Keys and Tokens)
* [CDCSyncService.cs]( Webhooks_MVC5_Dotnet/Models/Service/CDCSyncService.cs)  (For making CDC calls to QBO API) 

**Utility-**
* [ProcessNotificationData.cs]( Webhooks_MVC5_Dotnet/Models/Utility/ProcessNotificationData.cs)  (For Signature Verification of Webhooks payload and a separate thread implementation for queue processing of the webhooks data and cdc api calls for the realms)
* [DBUtility.cs]( Webhooks_MVC5_Dotnet/Models/Utility/DBUtility.cs)  (For making DB calls for OAuthTokens)

**View-**
* [Index.cshtml](Webhooks_MVC5_Dotnet/Views/Home/Index.cshtml)  (Displays default page for the Webhooks app)

**Reset the App**
-----------------------
You can always right click the LocalDB table or the Azure table from from SQL Server object explorer and run a new query-
Delete from OAuthTokens -  to delete the existing records and start afresh.






