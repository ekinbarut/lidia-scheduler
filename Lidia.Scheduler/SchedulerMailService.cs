using Lidia.Framework.Logging;
using Lidia.Framework.Models;
using Lidia.Scheduler.Common.Models.Responses;
using Lidia.Scheduler.Models;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Configuration;
using System.Net;

namespace Lidia.Scheduler
{
    public class SchedulerMailService
    {
        public void SendMail(Job job, APICallResponse processedResult)
        {

            // If you use mailService u need to customize

            LogService.Debug("Sending message to:" + "url");

            string Body = "Job Name : " + job.Name + "\r\n Response Type : " + Enum.GetName(typeof(ServiceResponseTypes), processedResult.Type) + "\r\n Response Message : " + processedResult.Message;

            RestClient client = new RestClient();

            client.BaseUrl = new Uri("x");
            client.Authenticator = new HttpBasicAuthenticator("api", ConfigurationManager.AppSettings["mailPassword"]);
            RestRequest request = new RestRequest();
            request.Credentials = new NetworkCredential("api", ConfigurationManager.AppSettings["mailPassword"]).GetCredential(client.BaseUrl, "Basic");
            request.AddParameter("from", "x@tgworkshop.com");
            request.AddParameter("to", "x@tgworkshop.com");
            request.AddParameter("subject", "Scheduler Info Message");
            request.AddParameter("text", Body);
            request.Method = Method.POST;
            request.Resource = "messages";
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            client.Post(request);

        }

        public void SendNewPasswordMail(string url,int userid,string email)
        {
            LogService.Debug("Sending new password to:" + email);
            

            string Body = "Your Mail Adress: " + email + "\r\n Reset Password Link: " + url;

            RestClient client = new RestClient();

            client.BaseUrl = new Uri("https://api.mailgun.net/v3/lidia.tgworkshop.com");
            client.Authenticator = new HttpBasicAuthenticator("api", ConfigurationManager.AppSettings["mailPassword"]);
            RestRequest request = new RestRequest();
            request.Credentials = new NetworkCredential("api", ConfigurationManager.AppSettings["mailPassword"]).GetCredential(client.BaseUrl, "Basic");
            request.AddParameter("from", "monitoring@tgworkshop.com");
            request.AddParameter("to", email);
            request.AddParameter("subject", "Scheduler Password");
            request.AddParameter("text", Body);
            request.Method = Method.POST;
            request.Resource = "messages";
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            client.Post(request);

        }
    }
}
