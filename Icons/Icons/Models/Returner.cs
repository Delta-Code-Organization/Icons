using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Icons.Models
{
    public class Returner
    {
        public object Data { get; set; }
        public JsonResult DataInJSON { get; set; }
        public Message Message { get; set; }
    }
}