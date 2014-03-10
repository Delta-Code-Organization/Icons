using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Icons.Models;

namespace Icons.Controllers
{
    public class CustomerController : Controller
    {
        //
        // GET: /Customer/

        public ActionResult CreateCustomer()
        {
            return View();
        }

        public ActionResult SearchCustomers()
        {
            Customer cust = new Customer();
            ViewBag.allcustomers = cust.GetAllCutomers().Data;
            return View();
        }

        public ActionResult EditCustomer(int _ID)
        {
            Customer cust = new Customer();
            cust.ID = _ID;
            TempData["customerID"] = _ID;
            TempData.Keep();
            ViewBag.customerdata = cust.GetCustomerData().Data;
            //TempData["gender"] = Enum.GetName(typeof(Gender), ViewBag.customerdata.Gender);
            //TempData.Keep();
            return View();
        }

        public string EditCutomerData(string _Name, string _Address, string _Phone, string _Notes, string _BirthDate)
        {
            Customer cust = new Customer();
            cust.ID = (int)TempData["customerID"];
            TempData.Keep();
            cust.Name = _Name;
            cust.Address = _Address;
            cust.Phone = _Phone;
            DateTime.Parse(_BirthDate);
            DateTime Dt = Convert.ToDateTime(_BirthDate);
            cust.BirthDate = Dt;
            cust.Notes = _Notes;
            cust.EditCustomer();
            return "تم تعديل بيانات العميل بنجاح ! ";
        }

        public string CreateCustom(string _Name, string _Address, string _Phone, string _BirthDate, string _Notes)
        {
            Customer cust = new Customer();
            cust.Name = _Name;
            cust.Address = _Address;
            cust.Phone = _Phone;
            DateTime.Parse(_BirthDate);
            DateTime Dt = Convert.ToDateTime(_BirthDate);
            cust.BirthDate = Dt;
            cust.Notes = _Notes;
            cust.CreateCustomer();
            return "تم اضافة العميل بنجاح !";
        }

        public string RemoveCustomer(int _ID)
        {
            Customer cust = new Customer();
            cust.ID = _ID;
            cust.DeleteCusotmer();
            return "تم مسح العميل بنجاح !";
        }

        public ActionResult Invoice()
        {
            ViewBag.InvoiceNum = Convert.ToInt32(new CustomerInvoice().InvoiceNumber().Data) + 1;
            List<Customer> LOS = new Customer().GetAllCutomers().Data as List<Customer>;
            ViewBag.S = LOS;
            List<Product> LOP = new Product().GetAll().Data as List<Product>;
            ViewBag.P = LOP;
            return View();
        }

        [HttpPost]
        public JsonResult AddInvoiceLine(int Prod, int Qty, double Price, double Total)
        {
            CustomerInvoiceLine SIL = new CustomerInvoiceLine();
            SIL.ProductId = Prod;
            SIL.Qty = Qty;
            SIL.Price = Price;
            SIL.Total = Total;
            return SIL.Add().DataInJSON;
        }

        [HttpPost]
        public string RemoveInvoiceLine(int id)
        {
            CustomerInvoiceLine SIL = new CustomerInvoiceLine { Id = id };
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
        public string AddFullInvoice(int ISup, DateTime IDate, double IDis, double ITotal, double INet, string LineIds)
        {
            CustomerInvoice SI = new CustomerInvoice();
            SI.Departed = false;
            SI.InvoiceDate = IDate;
            SI.InvoiceDiscount = IDis;
            SI.InvoiceNet = INet;
            SI.InvoiceTotal = ITotal;
            SI.LastEditBy = (Session["User"] as User).ID;
            SI.CustomerID = ISup;
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
    }
}
