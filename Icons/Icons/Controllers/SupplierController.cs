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
