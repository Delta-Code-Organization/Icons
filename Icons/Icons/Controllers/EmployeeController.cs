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
    }
}
