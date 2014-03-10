using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Icons.Models;

namespace Icons.Controllers
{
    public class SupplierController : Controller
    {
        //
        // GET: /Supplier/

        public ActionResult CreateSupplier()
        {
            return View();
        }

        public ActionResult Invoice()
        {
            ViewBag.InvoiceNum = Convert.ToInt32(new SupplierInvoice().InvoiceNumber().Data) + 1;
            List<Supplier> LOS = new Supplier().GetAll().Data as List<Supplier>;
            ViewBag.S = LOS;
            List<Product> LOP = new Product().GetAll().Data as List<Product>;
            ViewBag.P = LOP;
            return View();
        }

        [HttpPost]
        public JsonResult AddInvoiceLine(int Prod,int Qty,double Price,double Total)
        {
            SupplierInvoiceLine SIL = new SupplierInvoiceLine();
            SIL.ProductId = Prod;
            SIL.Qty = Qty;
            SIL.Price = Price;
            SIL.Total = Total;
            return SIL.Add().DataInJSON;
        }

        [HttpPost]
        public string RemoveInvoiceLine(int id)
        {
            SupplierInvoiceLine SIL = new SupplierInvoiceLine { Id = id };
            if (SIL.Remove().Message == Message.Invoice_Line_Removed_Successfully)
            {
                return "true";
            }
            else
            {
                return "false";
            }
        }

        [HttpPost]
        public string AddFullInvoice(int ISup, DateTime IDate, string IRef, double IDis, double ITotal, double INet, string LineIds)
        {
            SupplierInvoice SI = new SupplierInvoice();
            SI.Departed = false;
            SI.InvoiceDate = IDate;
            SI.InvoiceDiscount = IDis;
            SI.InvoiceNet = INet;
            SI.InvoiceTotal = ITotal;
            SI.LastEditBy = (Session["User"] as User).ID;
            SI.SupplierID = ISup;
            SI.SupplierReferenaceNo = IRef;
            string[] LOSIL = LineIds.Split(',');
            List<string> LOSILToSend = new List<string>();
            foreach (string item in LOSIL)
            {
                if (item != "0" && item != "" && item != null)
                {
                    LOSILToSend.Add(item);
                }
            }
            SI.Add(LOSILToSend);
            return "true";
        }

        [HttpPost]
        public string CreateSupplier(FormCollection FC)
        {
            Supplier S = new Supplier();
            S.Address = FC["address"].ToString();
            S.City = FC["city"].ToString();
            S.District = FC["district"].ToString();
            S.Mobile = FC["mobile"].ToString();
            S.Name = FC["name"].ToString();
            S.Notes = FC["notes"].ToString();
            S.Phone = FC["phone"].ToString();
            S.Create();
            return "true";
        }

        public ActionResult SuppliersSearch()
        {
            ViewBag.Supp = new Supplier().GetAll().Data as List<Supplier>;
            return View();
        }

        [HttpPost]
        public string Remove(int id)
        {
            Returner R = new Supplier
           {
               ID = id
           }.Remove();
            return "true";
        }

        public ActionResult EditSupplier(int? id)
        {
            if (id == null)
            {
                if (Session["User"] != null)
                {
                    return RedirectToAction("SearchUsers", "User");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            TempData["SID"] = (int)id;
            TempData.Keep();
            ViewBag.S = new Supplier { ID = (int)id }.GetByID().Data as Supplier;
            return View();
        }

        [HttpPost]
        public string EditSupplier(FormCollection FC)
        {
            Supplier S = new Supplier();
            S.ID = (int)TempData["SID"];
            S.Address = FC["address"].ToString();
            S.City = FC["city"].ToString();
            S.District = FC["district"].ToString();
            S.Mobile = FC["mobile"].ToString();
            S.Name = FC["name"].ToString();
            S.Notes = FC["notes"].ToString();
            S.Phone = FC["phone"].ToString();
            S.Edit();
            TempData.Keep();
            return "true";
        }
    }
}
