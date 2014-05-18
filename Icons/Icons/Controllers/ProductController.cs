using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Icons.Models;

namespace Icons.Controllers
{
    public class ProductController : Controller
    {
        //
        // GET: /Product/

        public ActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public string CreateCategory(FormCollection FC)
        {
            ProductCategory PC = new ProductCategory();
            PC.CategoryName = FC["Cat"];
            PC.LastEditBy = (Session["User"] as User).ID;
            PC.Create();
            return "true";
        }

        public ActionResult SearchCategories()
        {
            ViewBag.PC = new ProductCategory().GetAll().Data as List<ProductCategory>;
            return View();
        }

        [HttpPost]
        public string RemoveCategory(int id)
        {
            ProductCategory PC = new ProductCategory();
            PC.Id = id;
            if (PC.Remove().Message == Message.Cannot_Delete_Category_That_Contains_Products)
            {
                return "false";
            }
            else
            {
                return "true";
            }
        }

        public ActionResult EditCategory(int? id)
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
            else
            {
                TempData["PCat"] = (int)id;
                TempData.Keep();
                ViewBag.PC = new ProductCategory { Id = (int)id }.GetByID().Data as ProductCategory;
                return View();
            }
        }

        [HttpPost]
        public string EditCategory(FormCollection FC)
        {
            ProductCategory PC = new ProductCategory();
            PC.Id = (int)TempData["PCat"];
            PC.CategoryName = FC["Cat"];
            PC.LastEditBy = (Session["User"] as User).ID;
            PC.Edit();
            TempData.Keep();
            return "true";
        }

        public ActionResult CreateProduct()
        {
            string[] PU = Enum.GetNames(typeof(PurchaseUnit));
            string[] SU = Enum.GetNames(typeof(SalesUnit));
            ViewBag.PU = PU;
            ViewBag.SU = SU;
            ViewBag.PC = new ProductCategory().GetAll().Data as List<ProductCategory>;
            return View();
        }

        [HttpPost]
        public string CreateProduct(FormCollection FC)
        {
            Product P = new Product();
            P.Category = Convert.ToInt32(FC["Cat"]);
            P.PtoSRate = Convert.ToInt32(FC["per"]);
            P.PurchaseUnit = Convert.ToInt32(FC["pu"]);
            P.SalesUnit = Convert.ToInt32(FC["su"]);
            P.LastEditBy = (Session["User"] as User).ID;
            P.Description = FC["notes"];
            P.ProductName = FC["name"];
            Returner R = P.Create();
            if (R.Message == Message.Product_Name_Already_Exist)
            {
                return "false";
            }
            else
            {
                return "true";
            }
        }

        public ActionResult SearchProducts()
        {
            ViewBag.P = new Product().GetAll().Data as List<Product>;
            return View();
        }

        [HttpPost]
        public string RemoveProduct(int id)
        {
            Product P = new Product();
            P.Id = id;
            if (P.Remove().Message == Message.Cannot_Delete_This_Product)
            {
                return "false";
            }
            else
            {
                return "true";
            }
        }

        public ActionResult EditProduct(int? id)
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
            else
            {
                string[] PU = Enum.GetNames(typeof(PurchaseUnit));
                string[] SU = Enum.GetNames(typeof(SalesUnit));
                ViewBag.PU = PU;
                ViewBag.SU = SU;
                ViewBag.PC = new ProductCategory().GetAll().Data as List<ProductCategory>;
                ViewBag.P = new Product { Id = (int)id }.GetByID().Data as Product;
                TempData["ProdToEditID"] = (int)id;
                TempData.Keep();
                return View();
            }
        }

        [HttpPost]
        public string EditProduct(FormCollection FC)
        {
            Product P = new Product();
            P.Id = (int)TempData["ProdToEditID"];
            P.Category = Convert.ToInt32(FC["Cat"]);
            P.PtoSRate = Convert.ToInt32(FC["per"]);
            P.PurchaseUnit = Convert.ToInt32(FC["pu"]);
            P.SalesUnit = Convert.ToInt32(FC["su"]);
            P.LastEditBy = (Session["User"] as User).ID;
            P.Description = FC["notes"];
            P.ProductName = FC["name"];
            P.Edit();
            TempData.Keep();
            return "true";
        }
    }
}
