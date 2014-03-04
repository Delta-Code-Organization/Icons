using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Icons.Controllers
{
    public class ProjectController : Controller
    {
        //
        // GET: /Project/

        public ActionResult CreateProject()
        {
            return View();
        }

    }
}
