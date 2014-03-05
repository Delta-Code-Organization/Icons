﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Icons.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ShowError()
        {
            String Error = Session["LastError"].ToString();
            ViewBag.Message = Error;
            return View("Error");
        }
    }
}
