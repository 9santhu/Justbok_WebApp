using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Justbok.Models
{
    public class SessionManger
    {
        public static int GymId
        {
            get
            {
                if (System.Web.HttpContext.Current.Session["LoggedGym"] != null)
                {
                    return Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
                }
                return 0;
            }
            set
            {
                System.Web.HttpContext.Current.Session["LoggedGym"] = value;
            }
        }
    }
}
