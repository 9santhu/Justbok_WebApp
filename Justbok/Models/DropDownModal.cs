using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Justbok.Models
{
    public class DropDownModel
    {
        public DropDownModel()
        {
            DataList = new List<SelectListItem>();
        }

        public List<SelectListItem> DataList
        {
            get;
            set;
        }
    }
}