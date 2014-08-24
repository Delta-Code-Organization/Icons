using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Icons.Models;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

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

        public ActionResult EditCustomer(int id)
        {
            Customer cust = new Customer();
            cust.ID = id;
            TempData["customerID"] = id;
            TempData.Keep();
            ViewBag.customerdata = cust.GetCustomerData().Data;
            //TempData["gender"] = Enum.GetName(typeof(Gender), ViewBag.customerdata.Gender);
            //TempData.Keep();
            return View();
        }

        public string EditCutomerData(string _Name, string _Address, string _Phone, string _Notes, string _BirthDate, string Attachment, string Ext, string FileName)
        {
            Customer cust = new Customer();
            string File = Attachment;
            if (File != " " || File != "")
            {
                if (Ext == "png" || Ext == "jpg" || Ext == "jpeg" || Ext == "gif")
                {
                    Image Img = LoadImage(File);
                    string Path;
                    using (Bitmap image = new Bitmap(Img))
                    {
                        //image object properties
                        var fileName = Guid.NewGuid() + ".png";
                        Path = @"/content/Attachs/" + fileName;
                        string ImagePath = Server.MapPath(Path);
                        image.Save(ImagePath, ImageFormat.Png);
                    }
                    cust.Attach = Path;
                }
                else
                {
                    var fileName = Guid.NewGuid() + "." + Ext;
                    string PathO = @"/content/Attachs/" + fileName;
                    string FilePath = Server.MapPath(PathO);
                    byte[] filebytes = Convert.FromBase64String(File);
                    FileStream fs = new FileStream(FilePath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None);
                    fs.Write(filebytes, 0, filebytes.Length);
                    fs.Close();
                    cust.Attach = PathO;
                }
                cust.FileName = FileName;
            }
            cust.ID = (int)TempData["customerID"];
            TempData.Keep();
            cust.Name = _Name;
            cust.Address = _Address;
            cust.Phone = _Phone;
            cust.LastEditBy = (Session["User"] as User).ID;
            DateTime.Parse(_BirthDate);
            DateTime Dt = Convert.ToDateTime(_BirthDate);
            cust.BirthDate = Dt;
            cust.Notes = _Notes;
            cust.EditCustomer();
            return "تم تعديل بيانات العميل بنجاح ! ";
        }

        private Image LoadImage(string Base64)
        {
            //get a temp image from bytes, instead of loading from disk
            //data:image/gif;base64,
            //this image is a single pixel (black)
            byte[] bytes = Convert.FromBase64String(Base64);
            Image image;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
            }

            return image;
        }

        public string CreateCustom(string _Name, string _Address, string _Phone, string _BirthDate, string _Notes, string Attachment, string Ext, string FileName)
        {
            Customer cust = new Customer();
            string File = Attachment;
            if (File != " " || File != "")
            {
                if (Ext == "png" || Ext == "jpg" || Ext == "jpeg" || Ext == "gif")
                {
                    Image Img = LoadImage(File);
                    string Path;
                    using (Bitmap image = new Bitmap(Img))
                    {
                        //image object properties
                        var fileName = Guid.NewGuid() + ".png";
                        Path = @"/content/Attachs/" + fileName;
                        string ImagePath = Server.MapPath(Path);
                        image.Save(ImagePath, ImageFormat.Png);
                    }
                    cust.Attach = Path;
                }
                else
                {
                    var fileName = Guid.NewGuid() + "." + Ext;
                    string PathO = @"/content/Attachs/" + fileName;
                    string FilePath = Server.MapPath(PathO);
                    byte[] filebytes = Convert.FromBase64String(File);
                    FileStream fs = new FileStream(FilePath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None);
                    fs.Write(filebytes, 0, filebytes.Length);
                    fs.Close();
                    cust.Attach = PathO;
                }
                cust.FileName = FileName;
            }
            cust.Name = _Name;
            cust.Address = _Address;
            cust.Phone = _Phone;
            cust.LastEditBy = (Session["User"] as User).ID;
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
            List<Project> LOProj = new Project().GetAll().Data as List<Project>;
            ViewBag.Proj = LOProj;
            ViewBag.AccTree = new AccountingTree().GetAllAccounts().Data as List<AccountingTree>;
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
        public string AddFullInvoice(int ISup, DateTime IDate, int ToAcc, int projId, double IDis, double ITotal, double INet, string LineIds)
        {
            CustomerInvoice SI = new CustomerInvoice();
            SI.Departed = false;
            SI.InvoiceDate = IDate;
            SI.InvoiceDiscount = IDis;
            SI.InvoiceNet = INet;
            SI.InvoiceAccount = ToAcc;
            SI.InvoiceTotal = ITotal;
            SI.LastEditBy = (Session["User"] as User).ID;
            SI.CustomerID = ISup;
            SI.ProjectID = projId;
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
            SI.Add(LOCILTS);
            return "true";
        }

        public ActionResult SearchInvoices()
        {
            ViewBag.I = new CustomerInvoice().GetAllInvoices().Data as List<CustomerInvoice>;
            return View();
        }

        [HttpPost]
        public void DeleteInvoice(int id)
        {
            new CustomerInvoice { Id = id }.DeleteInvoice();
        }

        [HttpPost]
        public void Depart(int id)
        {
            new CustomerInvoice { Id = id }.Depart((Session["User"] as User).ID);
        }

        [OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult EditInvoice(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("SearchInvoices", "Customer");
            }
            List<Customer> LOS = new Customer().GetAllCutomers().Data as List<Customer>;
            ViewBag.S = LOS;
            List<Product> LOP = new Product().GetAll().Data as List<Product>;
            ViewBag.P = LOP;
            List<Project> LOProj = new Project().GetAll().Data as List<Project>;
            ViewBag.Proj = LOProj;
            ViewBag.AccTree = new AccountingTree().GetAllAccounts().Data as List<AccountingTree>;
            ViewBag.I = new CustomerInvoice { Id = (int)id }.GetByID().Data as CustomerInvoice;
            TempData["IID"] = id;
            TempData.Keep();
            return View();
        }

        [HttpPost]
        public void DeleteInvoiceLine(int ID)
        {
            new CustomerInvoiceLine { Id = ID }.Remove();
        }

        [HttpPost]
        public string EditFullInvoice(int ISup, DateTime IDate, int ToAcc, int projId, double IDis, double ITotal, double INet, string LineIds)
        {
            CustomerInvoice SI = new CustomerInvoice();
            SI.Id = (int)TempData["IID"];
            SI.Departed = false;
            SI.InvoiceDate = IDate;
            SI.InvoiceDiscount = IDis;
            SI.InvoiceNet = INet;
            SI.InvoiceAccount = ToAcc;
            SI.InvoiceTotal = ITotal;
            SI.LastEditBy = (Session["User"] as User).ID;
            SI.CustomerID = ISup;
            SI.ProjectID = projId;
            string[] LOSIL = LineIds.Split(',');
            List<string> AIL = new List<string>(LOSIL);
            AIL.Remove("");
            List<CustomerInvoiceLine> LOCILTS = new List<CustomerInvoiceLine>();
            int Skip = 0;
            int ObjectsCount = AIL.Count / 5;
            for (int i = 1; i <= ObjectsCount; i++)
            {
                List<string> CurrentObject = new List<string>();
                CurrentObject = AIL.Skip(Skip).Take(5).ToList();
                CustomerInvoiceLine CILTS = new CustomerInvoiceLine();
                CILTS.Id = Convert.ToInt32(CurrentObject[0]);
                CILTS.ProductId = Convert.ToInt32(CurrentObject[1]);
                CILTS.Qty = Convert.ToDouble(CurrentObject[2]);
                CILTS.Price = Convert.ToDouble(CurrentObject[3]);
                CILTS.Total = Convert.ToDouble(CurrentObject[4]);
                LOCILTS.Add(CILTS);
                Skip += 5;
            }
            TempData.Keep();
            SI.Edit(LOCILTS);
            return "true";
        }

        public ActionResult DisplayInvoice(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("SearchInvoices", "Customer");
            }
            ViewBag.I = new CustomerInvoice { Id = (int)id }.GetByID().Data as CustomerInvoice;
            return View();
        }

        public JsonResult GetInvoiceLines(int _ID)
        {
            return new CustomerInvoiceLine { InvoiceId = _ID }.GetByInvoiceID().DataInJSON;
        }

        [HttpPost]
        public string DeepInvoiceSearch(string _Keyword)
        {
            return new CustomerInvoice().DeepSearch(_Keyword).Data.ToString();
        }
    }
}
