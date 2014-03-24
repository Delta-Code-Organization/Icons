using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Icons.Models;

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

        [HttpPost]
        public string CreateEmployee(FormCollection FC)
        {
            Employee E = new Employee();
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
            FT.Amount = Convert.ToDouble(FC["penalty"]);
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
            FT.Amount = Convert.ToDouble(FC["benifit"]);
            FT.ToAccount = Convert.ToInt32(FC["FromAcc"]);
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
            ViewBag.AllP = new Employee { Id = (int)id }.GetImprests().Data as List<FinancialTransaction>;
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
            FT.Amount = Convert.ToDouble(FC["imprest"]);
            FT.ToAccount = Convert.ToInt32(FC["FromAcc"]);
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
            return View();
        }

        public void Pay(int id, double Total)
        {
            new Employee { Id = id }.Pay(Total, (Session["User"] as User).ID);
        }
    }
}
