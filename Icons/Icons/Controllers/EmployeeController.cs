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
    public class EmployeeController : Controller
    {
        //
        // GET: /Employee/

        public ActionResult CreateEmployee()
        {
            string[] SalaryTypes = Enum.GetNames(typeof(SalaryType));
            ViewBag.ST = SalaryTypes;
            return View();
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

        [HttpPost]
        public string CreateEmployee(FormCollection FC)
        {
            Employee E = new Employee();
            string File = FC["Attachment"];
            if (File != " " && File != "")
            {
                if (FC["Ext"] == "png" || FC["Ext"] == "jpg" || FC["Ext"] == "jpeg" || FC["Ext"] == "gif")
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
                    E.Attach = Path;
                }
                else
                {
                    var fileName = Guid.NewGuid() + "." + FC["Ext"];
                    string PathO = @"/content/Attachs/" + fileName;
                    string FilePath = Server.MapPath(PathO);
                    byte[] filebytes = Convert.FromBase64String(File);
                    FileStream fs = new FileStream(FilePath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None);
                    fs.Write(filebytes, 0, filebytes.Length);
                    fs.Close();
                    E.Attach = PathO;
                }
                E.FileName = FC["FileName"];
            }
            E.ExitDate = Convert.ToDateTime(FC["ExitDate"]);
            E.HoldingAssets = FC["HoldingAssets"];
            E.ExitDeliveredAssets = FC["ExitDeliveredAssets"];
            E.Notes = FC["Notes"];
            E.InsurancePercentage = Convert.ToDouble(FC["InsurancePercentage"]);
            E.InsuranceFrom = Convert.ToDateTime(FC["InsuranceFrom"]);
            E.InsuranceTo = Convert.ToDateTime(FC["InsuranceTo"]);
            E.Address = FC["address"];
            E.BasicSalary = Convert.ToInt32(FC["basicsalary"]);
            E.DateOfBirth = Convert.ToDateTime(FC["dateofbirth"]);
            E.HiringDate = Convert.ToDateTime(FC["hiringdate"]);
            E.Name = FC["name"];
            E.Phone1 = FC["phone1"];
            E.Phone2 = FC["phone2"];
            E.SalaryType = Convert.ToInt32(FC["salarytype"]);
            E.SSN = FC["ssn"];
            E.Title = FC["title"];
            E.LastEditBy = (Session["User"] as User).ID;
            E.Add();
            return "true";
        }

        public ActionResult SearchEmployees()
        {
            ViewBag.E = new Employee().GetAll().Data as List<Employee>;
            return View();
        }

        [HttpPost]
        public string RemoveEmployee(int id)
        {
            Employee E = new Employee();
            E.Id = id;
            E.Remove();
            return "true";
        }

        public ActionResult EditEmployee(int? id)
        {
            if (id == null)
            {
                if (Session["User"] != null)
                {
                    return RedirectToAction("SearchUsers", "User");
                }
                else
                {
                    return RedirectToAction("Login", "User");
                }
            }
            string[] SalaryTypes = Enum.GetNames(typeof(SalaryType));
            ViewBag.ST = SalaryTypes;
            ViewBag.E = new Employee { Id = (int)id }.GetByID().Data as Employee;
            TempData["EmpID"] = (int)id;
            TempData.Keep();
            return View();
        }

        [HttpPost]
        public string EditEmployee(FormCollection FC)
        {
            Employee E = new Employee();
            string File = FC["Attachment"];
            if (File != " " || File != "")
            {
                if (FC["Ext"] == "png" || FC["Ext"] == "jpg" || FC["Ext"] == "jpeg" || FC["Ext"] == "gif")
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
                    E.Attach = Path;
                }
                else
                {
                    var fileName = Guid.NewGuid() + "." + FC["Ext"];
                    string PathO = @"/content/Attachs/" + fileName;
                    string FilePath = Server.MapPath(PathO);
                    byte[] filebytes = Convert.FromBase64String(File);
                    FileStream fs = new FileStream(FilePath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None);
                    fs.Write(filebytes, 0, filebytes.Length);
                    fs.Close();
                    E.Attach = PathO;
                }
                E.FileName = FC["FileName"];
            }
            E.ExitDate = Convert.ToDateTime(FC["ExitDate"]);
            E.HoldingAssets = FC["HoldingAssets"];
            E.ExitDeliveredAssets = FC["ExitDeliveredAssets"];
            E.Notes = FC["Notes"];
            E.InsurancePercentage = Convert.ToDouble(FC["InsurancePercentage"]);
            E.InsuranceFrom = Convert.ToDateTime(FC["InsuranceFrom"]);
            E.InsuranceTo = Convert.ToDateTime(FC["InsuranceTo"]);
            E.Id = (int)TempData["EmpID"];
            E.Address = FC["address"];
            E.BasicSalary = Convert.ToInt32(FC["basicsalary"]);
            E.DateOfBirth = Convert.ToDateTime(FC["dateofbirth"]);
            E.HiringDate = Convert.ToDateTime(FC["hiringdate"]);
            E.Name = FC["name"];
            E.Phone1 = FC["phone1"];
            E.Phone2 = FC["phone2"];
            E.SalaryType = Convert.ToInt32(FC["salarytype"]);
            E.SSN = FC["ssn"];
            E.Title = FC["title"];
            E.LastEditBy = (Session["User"] as User).ID;
            E.Edit();
            TempData.Keep();
            return "true";
        }

        public ActionResult Penalties(int? id)
        {
            if (id == null)
            {
                if (Session["User"] != null)
                {
                    return RedirectToAction("SearchUsers", "User");
                }
                else
                {
                    return RedirectToAction("Login", "User");
                }
            }
            string[] SalaryTypes = Enum.GetNames(typeof(SalaryType));
            ViewBag.ST = SalaryTypes;
            ViewBag.E = new Employee { Id = (int)id }.GetByID().Data as Employee;
            ViewBag.AllP = new Employee { Id = (int)id }.GetPenalties().Data as List<FinancialTransaction>;
            ViewBag.AllAcc = new AccountingTree().GetAllAccounts().Data as List<AccountingTree>;
            TempData["EmpID"] = (int)id;
            TempData.Keep();
            return View();
        }

        [HttpPost]
        public void AddPenalty(FormCollection FC)
        {
            Employee E = new Employee { Id = (int)TempData["EmpID"] };
            FinancialTransaction FT = new FinancialTransaction();
            FT.Credit = Convert.ToDouble(FC["penalty"]);
            FT.Debit = 0;
            FT.FromAccount = Convert.ToInt32(FC["ToAcc"]);
            FT.LastEditBy = (Session["User"] as User).ID;
            FT.Notes = FC["Notes"];
            E.AddPenalty(FT);
            TempData.Keep();
        }

        public string RemovePenalty(int id)
        {
            Employee E = new Employee();
            E.RemovePenalty(id);
            return "true";
        }

        public ActionResult Benifits(int? id)
        {
            if (id == null)
            {
                if (Session["User"] != null)
                {
                    return RedirectToAction("SearchUsers", "User");
                }
                else
                {
                    return RedirectToAction("Login", "User");
                }
            }
            string[] SalaryTypes = Enum.GetNames(typeof(SalaryType));
            ViewBag.ST = SalaryTypes;
            ViewBag.E = new Employee { Id = (int)id }.GetByID().Data as Employee;
            ViewBag.AllP = new Employee { Id = (int)id }.GetBenifits().Data as List<FinancialTransaction>;
            ViewBag.AllAcc = new AccountingTree().GetAllAccounts().Data as List<AccountingTree>;
            TempData["EmpID"] = (int)id;
            TempData.Keep();
            return View();
        }

        [HttpPost]
        public void AddBenifit(FormCollection FC)
        {
            Employee E = new Employee { Id = (int)TempData["EmpID"] };
            FinancialTransaction FT = new FinancialTransaction();
            FT.Debit = Convert.ToDouble(FC["benifit"]);
            FT.Credit = 0;
            FT.FromAccount = Convert.ToInt32(FC["FromAcc"]);
            FT.LastEditBy = (Session["User"] as User).ID;
            FT.Notes = FC["Notes"];
            E.AddBenifit(FT);
            TempData.Keep();
        }

        public string RemoveBenifit(int id)
        {
            Employee E = new Employee();
            E.RemoveBenifit(id);
            return "true";
        }

        public ActionResult Imprests(int? id)
        {
            if (id == null)
            {
                if (Session["User"] != null)
                {
                    return RedirectToAction("SearchUsers", "User");
                }
                else
                {
                    return RedirectToAction("Login", "User");
                }
            }
            string[] SalaryTypes = Enum.GetNames(typeof(SalaryType));
            ViewBag.ST = SalaryTypes;
            ViewBag.E = new Employee { Id = (int)id }.GetByID().Data as Employee;
            List<FinancialTransaction> LOFT = new Employee { Id = (int)id }.GetImprests().Data as List<FinancialTransaction>;
            ViewBag.AllP = LOFT;
            ViewBag.AllAcc = new AccountingTree().GetAllAccounts().Data as List<AccountingTree>;
            TempData["EmpID"] = (int)id;
            TempData.Keep();
            return View();
        }

        [HttpPost]
        public void AddImprest(FormCollection FC)
        {
            Employee E = new Employee { Id = (int)TempData["EmpID"] };
            FinancialTransaction FT = new FinancialTransaction();
            FT.Debit = Convert.ToDouble(FC["imprest"]);
            FT.Credit = 0;
            FT.FromAccount = Convert.ToInt32(FC["FromAcc"]);
            FT.LastEditBy = (Session["User"] as User).ID;
            FT.Notes = FC["Notes"];
            E.AddImprest(FT);
            TempData.Keep();
        }

        public string RemoveImprest(int id)
        {
            Employee E = new Employee();
            E.RemoveImprest(id);
            return "true";
        }

        public ActionResult Payroll()
        {
            ViewBag.E = new Employee().GetAll().Data as List<Employee>;
            ViewBag.AllAcc = new AccountingTree().GetAllAccounts().Data as List<AccountingTree>;
            return View();
        }

        public string Pay(int id, double Total, DateTime PaymentDate, int ToAccID)
        {
            if (new Employee { Id = id }.Pay(Total, (Session["User"] as User).ID, PaymentDate, ToAccID).Message == Message.Cannot_pay_salary_at_Wrong_time)
            {
                return "false";
            }
            else
            {
                return "true";
            }
        }

        public ActionResult PayrollSearch()
        {
            ViewBag.AllAcc = new AccountingTree().GetAllAccounts().Data as List<AccountingTree>;
            string[] SalaryTypes = Enum.GetNames(typeof(SalaryType));
            ViewBag.S = SalaryTypes;
            return View();
        }

        [HttpPost]
        public JsonResult PayrollFilter(int Period)
        {
            return new Employee { SalaryType = Period }.PayrollSearch().DataInJSON;
        }
    }
}
