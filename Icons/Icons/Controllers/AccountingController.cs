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
        Icons.Models.MaksoudDBEntities db = new MaksoudDBEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Tree()
        {
            List<AccountingTree> Tree = db.AccountingTrees.Where(p=>p.Parent == null || p.Parent == 0).ToList();
            ViewBag.Nodes = Tree;
            return View("AccountingTree");
        }
        public ActionResult Settings()
        {
            List<AccountingTree> Tree = db.AccountingTrees.ToList();
            ViewBag.Nodes = Tree;
            return View("AccountsSetting");
        }
        [HttpPost]
        public string SaveTree(string TreeJson)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            List<TreeNode> Trees = (List<TreeNode>)jss.Deserialize(TreeJson, typeof(List<TreeNode>));
            new AccountingTree().UpdateTree(Trees);
            return "true";
        }
        [HttpPost]
        public string DeleteNode(string id)
        {
            new AccountingTree().deleteNode(int.Parse(id));
            return "true";
        }
        [HttpPost]
        public string SaveSettings(string Suppliers, string Customers, string Employee,
            string Banks, string Projects, string Imprest,
            string Sales, string Safes, string Stock)
        {
            int _Suppliers = int.Parse(Suppliers);
            int _Customers = int.Parse(Customers);
            int _Employee = int.Parse(Employee);
            int _Banks = int.Parse(Banks);
            int _Projects = int.Parse(Projects);
            int _Imprest = int.Parse(Imprest);
            int _Sales = int.Parse(Sales);
            int _Safes = int.Parse(Safes);
            int _Stock = int.Parse(Stock);
            db.AccountingTrees.Where(p => p.Id == _Suppliers).First().KeyAccountID = (int)KeyAccounts.Suppliers;
            db.AccountingTrees.Where(p => p.Id == _Customers).First().KeyAccountID = (int)KeyAccounts.Customers;
            db.AccountingTrees.Where(p => p.Id == _Employee).First().KeyAccountID = (int)KeyAccounts.Employee;
            db.AccountingTrees.Where(p => p.Id == _Banks).First().KeyAccountID = (int)KeyAccounts.Banks;
            db.AccountingTrees.Where(p => p.Id == _Projects).First().KeyAccountID = (int)KeyAccounts.Projects;
            db.AccountingTrees.Where(p => p.Id == _Imprest).First().KeyAccountID = (int)KeyAccounts.Imprestes;
            db.AccountingTrees.Where(p => p.Id == _Sales).First().KeyAccountID = (int)KeyAccounts.Sales;
            db.AccountingTrees.Where(p => p.Id == _Safes).First().KeyAccountID = (int)KeyAccounts.Safes;
            db.AccountingTrees.Where(p => p.Id == _Stock).First().KeyAccountID = (int)KeyAccounts.Stock;
            db.SaveChanges();
            return "true";
        }

    }
}
