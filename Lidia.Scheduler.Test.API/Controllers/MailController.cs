using Lidia.Framework.Models.Responses;
using Lidia.Framework.Models.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Http;

namespace Lidia.Scheduler.Test.API.Controllers
{
    [Route("api/Mail")]
    public class MailController : ApiController
    {
        [HttpGet]
        [Route("SendMail")]
        public ServiceResponse SendMail()
        {
            var logrecords = new List<ServiceLogRecord>();

            // Performance counters
            Stopwatch sw = new Stopwatch();
            sw.Start();

            MailMessage ePosta = new MailMessage();
            ePosta.From = new MailAddress("x@gmail.com");
            //
            ePosta.IsBodyHtml = true;

            ePosta.To.Add("x@gmail.com");
            //
            ePosta.Subject = "Scheduler Test";
            //
            ePosta.Body = "Test Mail";
            //
            SmtpClient smtp = new SmtpClient();
            //
            smtp.Credentials = new System.Net.NetworkCredential("x@gmail.com", "password");
            smtp.Port = 587;
            smtp.Host = "smtp.gmail.com";
            smtp.EnableSsl = true;
            object userState = ePosta;
            smtp.Send(ePosta);

            logrecords.Add(new ServiceLogRecord { Type = "Success", Body = "Mail gönderildi", TimeStamp = DateTime.Now });

            var response = new ServiceResponse()
            {
                Details = "Mail gönderildi",
                ServiceTook = sw.ElapsedMilliseconds,
                TotalTook = sw.ElapsedMilliseconds,
                Message = "Mail sent successfully!",
                Type = Framework.Models.ServiceResponseTypes.Success,
                LogRecords = logrecords
            };
            sw.Stop();
            return response;
        }
    }
}
