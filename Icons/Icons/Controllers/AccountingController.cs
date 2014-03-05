using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using Icons.Models;
namespace Icons.Controllers
{
   
    public class AccountingController : Controller
    {
        //
        // GET: /Accounting/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Tree()
        {
            return View("AccountingTree");
        }
        [HttpPost]
        public string SaveTree(string TreeJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            List<TreeNode> Trees = (List<TreeNode>)jss.Deserialize(TreeJson, typeof(List<TreeNode>));
            return "true";
        }

    }
}
