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
            List<AccountingTree> Tree = db.AccountingTrees.Where(p => p.Parent == null || p.Parent == 0).ToList();
            List<AccountingTree> AllTrees = db.AccountingTrees.ToList();
            ViewBag.AllTrees = AllTrees;
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

        public ActionResult WorkOrder()
        {
            List<Product> LOP = new Product().GetAll().Data as List<Product>;
            ViewBag.P = LOP;
            List<Project> LOProj = new Project().GetAll().Data as List<Project>;
            ViewBag.Proj = LOProj;
            ViewBag.AccTree = new AccountingTree().GetAllAccounts().Data as List<AccountingTree>;
            return View();
        }

        [HttpPost]
        public string GetProdPrice(int ProdID)
        {
            return (new Product { Id = ProdID }.GetLastProductPrice().Data as double?).ToString();
        }

        [HttpPost]
        public void SaveWorkOrder(DateTime Date, int Acc, int ProjId, string Notes, double Total, string LineIds)
        {
            string[] LOSIL = LineIds.Split(',');
            List<string> AIL = new List<string>(LOSIL);
            AIL.Remove("");
            List<CustomerInvoiceLine> LOCILTS = new List<CustomerInvoiceLine>();
            int Skip = 0;
            int ObjectsCount = AIL.Count / 4;
            for (int i = 1; i <= ObjectsCount; i++)
            {
                List<string> CurrentObject = new List<string>();
                CurrentObject = AIL.Skip(Skip).Take(4).ToList();
                CustomerInvoiceLine CILTS = new CustomerInvoiceLine();
                CILTS.ProductId = Convert.ToInt32(CurrentObject[0]);
                CILTS.Qty = Convert.ToDouble(CurrentObject[1]);
                CILTS.Price = Convert.ToDouble(CurrentObject[2]);
                CILTS.Total = Convert.ToDouble(CurrentObject[3]);
                LOCILTS.Add(CILTS);
                Skip += 4;
            }
            new AccountingTree().SaveWorkOrder(LOCILTS, Date, ProjId, Acc, Notes, Total, (Session["User"] as User).ID);
        }

        public ActionResult WorkOrderReport()
        {
            ViewBag.Projects = new Project().GetAll().Data as List<Project>;
            ViewBag.Products = new Product().GetAll().Data as List<Product>;
            return View();
        }

        [HttpPost]
        public JsonResult FilterWorkOrderReport(int ProjId, int ProdId, DateTime From, DateTime To)
        {
            return new AccountingTree().WorkOrderReport(ProjId, ProdId, From, To).DataInJSON;
        }

        public ActionResult Statements()
        {
            ViewBag.Accs = new AccountingTree().GetAllAccounts().Data as List<AccountingTree>;
            return View();
        }

        [HttpPost]
        public JsonResult FilterStatements(int Acc1, int Acc2)
        {
            return new AccountingTree().FilterStatements(Acc1, Acc2).DataInJSON;
        }

        public ActionResult AddFinancialTransactions()
        {
            ViewBag.Accs = new AccountingTree().GetAllAccounts().Data as List<AccountingTree>;
            return View();
        }

        [HttpPost]
        public string AddFT(int Acc1, int Acc2, double Amount, string State, DateTime Date, string Notes)
        {
            return new AccountingTree().AddFT(new FinancialTransaction
            {
                FromAccount = Acc1,
                ToAccount = Acc2,
                Amount = Amount,
                Statement = State,
                TransactionDate = Date,
                Notes = Notes,
                LastEditBy = (Session["User"] as User).ID
            }).Data.ToString();
        }

        public ActionResult SearchFinancialTransactions()
        {
            ViewBag.LOFT = new FinancialTransaction().Search().Data as List<FinancialTransaction>;
            return View();
        }

        public ActionResult EditFinancialTransactions(int id)
        {
            TempData["FTID"] = id;
            TempData.Keep();
            ViewBag.FT = new FinancialTransaction { Id = id }.GetByID().Data as FinancialTransaction;
            ViewBag.Accs = new AccountingTree().GetAllAccounts().Data as List<AccountingTree>;
            return View();
        }

        [HttpPost]
        public void EditFT(int Acc1, int Acc2, double Amount, string State, DateTime Date, string Notes)
        {
            int id = (int)TempData["FTID"];
            TempData.Keep();
            FinancialTransaction Ft = new FinancialTransaction
            {
                Id = id,
                FromAccount = Acc1,
                ToAccount = Acc2,
                Amount = Amount,
                Statement = State,
                TransactionDate = Date,
                Notes = Notes,
                LastEditBy = (Session["User"] as User).ID
            };
            Ft.Edit();
        }

        [HttpPost]
        public void DeleteFT(int _ID)
        {
            new FinancialTransaction { Id = _ID }.Delete();
        }
    }
}
