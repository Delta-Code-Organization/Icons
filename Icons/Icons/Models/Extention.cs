using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Icons.Models
{
    public static class Extention
    {
        public static string ShowMessage(this Message message)
        {
            return message.ToString().Replace("_", " ");
        }

        public static JsonResult ToJSON(this object Obj)
        {
            JsonResult JR = new JsonResult
            {
                Data = Obj,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
            return JR;
        }
    }
}