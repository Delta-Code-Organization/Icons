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
            return View();
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
