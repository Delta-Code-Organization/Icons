using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Icons.Models;

namespace Icons.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            ViewBag.CustomerNo = Convert.ToInt32(new Customer().GetCustomerNumbers().Data);
            ViewBag.TotalSales = Convert.ToDouble(new Project().TotalSales(0).Data);
            ViewBag.TotalCosts = Convert.ToDouble(new Project().TotalCosts(0).Data);
            ViewBag.PendingInstallments = Convert.ToDouble(new Project().PendingInstallment(0).Data);
            ViewBag.AllProj = new Project().GetAll().Data as List<Project>;
            ViewBag.SupplierDue = Convert.ToDouble(new Supplier().GetSupplierDue().Data);
            Session["PI"] = new AccountingTree().GetPaidInstallmentsFtToSave().Data;
            Session["AI"] = new AccountingTree().GetAllInstallmentsFtToSave().Data;
            Session["TS"] = new AccountingTree().GetTotalSalesFtToSave().Data;
            return View();
        }

        [HttpPost]
        public double GetCostsByProj(int ID) 
        {
            return Convert.ToDouble(new Project().TotalCosts(ID).Data);
        }

        [HttpPost]
        public double GetSalesByProj(int ID)
        {
            return Convert.ToDouble(new Project().TotalSales(ID).Data);
        }

        [HttpPost]
        public JsonResult GetFlotData()
        {
            return new AccountingTree().GetSalesFlotData((Session["TS"]) as List<FinancialTransaction>).DataInJSON;
        }

        [HttpPost]
        public JsonResult GetAllInsFlotData()
        {
            return new AccountingTree().GetAllInstallmentsFlotData((Session["AI"]) as List<Installment>).DataInJSON;
        }

        [HttpPost]
        public JsonResult GetPaidInsFlotData()
        {
            return new AccountingTree().GetAPaidInstallmentsFlotData((Session["PI"]) as List<FinancialTransaction>).DataInJSON;
        }

        public ActionResult ShowError()
        {
            String Error = Session["LastError"].ToString();
            ViewBag.Message = Error;
            return View("Error");
        }

        public ActionResult GeneralSearch(string Keyword)
        {
            General G = new General();
            List<Customer> LOC = new List<Customer>();
            List<Supplier> LOS = new List<Supplier>();
            List<SupplierInvoice> LOSI = new List<SupplierInvoice>();
            List<CustomerInvoice> LOCI = new List<CustomerInvoice>();
            List<FinancialTransaction> LOFT = new List<FinancialTransaction>();
            List<Product> LOP = new List<Product>();
            G.GeneralSearch(Keyword, out LOC, out LOS, out LOCI, out LOSI, out LOFT, out LOP);
            ViewBag.LOC = LOC;
            ViewBag.LOS = LOS;
            ViewBag.LOCI = LOCI;
            ViewBag.LOSI = LOSI;
            ViewBag.LOFT = LOFT;
            ViewBag.LOP = LOP;
            return View();
        }
    }
}
