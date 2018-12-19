using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Data.SqlClient;



namespace Justbok.ADModel
{
    public class CheckSessionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            base.OnActionExecuting(filterContext);

            if (filterContext.HttpContext.Session == null || filterContext.HttpContext.Session["LoggedGym"] == null)
            {
                RedirectToSessionExpire(filterContext);

            }
            else
            {
                if (IsUserSessionExpired(filterContext))
                {
                    RedirectToSessionExpire(filterContext);
                }
            }

        }

        bool IsUserSessionExpired(ActionExecutingContext filterContext)
        {
            try
            {
                if (filterContext.HttpContext.Session["LoggedGym"] == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                return true;

            }
        }

        void RedirectToSessionExpire(ActionExecutingContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new
            {
                controller = "Home",
                action = "SessionOut"
            }));

        }
    }
}