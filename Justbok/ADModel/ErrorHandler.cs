using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Justbok.ADModel
{
    public class ErrorHandler : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            try
            {
                string _from = ConfigurationManager.AppSettings["LogReportFrom"].ToString();
                string _to = ConfigurationManager.AppSettings["LogReportTo"].ToString();
                string _username = ConfigurationManager.AppSettings["UserName"].ToString();
                string _password = ConfigurationManager.AppSettings["Password"].ToString();
                Exception ex = filterContext.Exception;
                filterContext.ExceptionHandled = true;
                using (MailMessage mm = new MailMessage(_from, _to))
                {
                    mm.Subject = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt");
                    mm.Body = string.Format("{6}{6}Time Stamp: {0}{6}Exception Type: {1}{6}Message: {2}{6}Source: {3}{6}Target Site: {4}{6}Stack Trace: {5}", (object)DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"), "Exception", (object)ex.Message, (object)ex.Source, (object)ex.TargetSite, (object)ex.StackTrace, (object)Environment.NewLine);
                    mm.IsBodyHtml = false;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    NetworkCredential NetworkCred = new NetworkCredential(_username, _password);
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = 587;
                    smtp.Send(mm);
                }
            }
            catch (Exception)
            {
            }


        }
    }
}