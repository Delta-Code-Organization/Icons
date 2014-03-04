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
    }
}
