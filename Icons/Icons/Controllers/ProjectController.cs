using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Icons.Models;

namespace Icons.Controllers
{
    public class ProjectController : Controller
    {
        //
        // GET: /Project/

        public ActionResult CreateProject()
        {
            return View();
        }

        [HttpPost]
        public string CreateProject(FormCollection FC)
        {
            Project P = new Project();
            P.CreationDate = Convert.ToDateTime(FC["date"]);
            P.ExpectedCost = Convert.ToDouble(FC["cost"]);
            P.FirstViewLength = Convert.ToDouble(FC["first"]);
            P.FloorsCount = Convert.ToInt32(FC["floors"]);
            P.ForthViewLength = Convert.ToDouble(FC["forth"]);
            P.LandSpace = Convert.ToInt32(FC["space"]);
            P.OwnershipPercentage = Convert.ToDouble(FC["per"]);
            P.ProjectAddress = FC["address"];
            P.ProjectName = FC["name"];
            P.SecondViewLength = Convert.ToDouble(FC["second"]);
            P.ThirdViewLength = Convert.ToDouble(FC["third"]);
            P.Add();
            return "true";
        }

        public ActionResult SearchProjects()
        {
            ViewBag.P = new Project().GetAll().Data as List<Project>;
            return View();
        }

        [HttpPost]
        public string Remove(int ID)
        {
            if (new Project { Id = ID }.Remove().Message == Message.This_Project_Have_Stock_Cannot_Be_Deleted)
            {
                return "false";
            }
            else
            {
                return "true";
            }
        }

        public ActionResult EditProject(int? id)
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
            ViewBag.P = new Project { Id = (int)id }.GetByID().Data as Project;
            TempData["ProjID"] = (int)id;
            TempData.Keep();
            return View();
        }

        [HttpPost]
        public string EditProject(FormCollection FC)
        {
            Project P = new Project();
            P.Id = (int)TempData["ProjID"];
            P.CreationDate = Convert.ToDateTime(FC["date"]);
            P.ExpectedCost = Convert.ToDouble(FC["cost"]);
            P.FirstViewLength = Convert.ToDouble(FC["first"]);
            P.FloorsCount = Convert.ToInt32(FC["floors"]);
            P.ForthViewLength = Convert.ToDouble(FC["forth"]);
            P.LandSpace = Convert.ToInt32(FC["space"]);
            P.OwnershipPercentage = Convert.ToDouble(FC["per"]);
            P.ProjectAddress = FC["address"];
            P.ProjectName = FC["name"];
            P.SecondViewLength = Convert.ToDouble(FC["second"]);
            P.ThirdViewLength = Convert.ToDouble(FC["third"]);
            P.Edit();
            TempData.Keep();
            return "true";
        }

        public ActionResult CreateProjectUnits(int id)
        {
            string[] UnitTypes = Enum.GetNames(typeof(UnitTypes));
            ViewBag.T = UnitTypes;
            string[] Finishing = Enum.GetNames(typeof(Finishing));
            ViewBag.F = Finishing;
            ViewBag.P = new Project { Id = id }.GetByID().Data as Project;
            ViewBag.PU = new ProjectUnit { ProjectID = id }.GetUnits().Data as List<ProjectUnit>;
            TempData["PID"] = id;
            TempData.Keep();
            return View();
        }

        [HttpPost]
        public string CreateUnit(FormCollection FC)
        {
            ProjectUnit PU = new ProjectUnit();
            PU.ExpectedPrice = Convert.ToDouble(FC["price"]);
            PU.Finishing = Convert.ToInt32(FC["finish"]);
            PU.FloorNumber = Convert.ToInt32(FC["floors"]);
            PU.Notes = FC["notes"];
            PU.ProjectID = (int)TempData["PID"];
            PU.UnitSpace = Convert.ToInt32(FC["space"]);
            PU.UnitType = Convert.ToInt32(FC["type"]);
            PU.Create();
            TempData.Keep();
            return "true";
        }

        [HttpPost]
        public string DeleteUnit(int id)
        {
            new ProjectUnit { Id = id }.Delete();
            TempData.Keep();
            return "true";
        }

        public string EditUnit(FormCollection FC)
        {
            ProjectUnit PU = new ProjectUnit();
            PU.Id = Convert.ToInt32(FC["PUnitID"]);
            PU.ExpectedPrice = Convert.ToDouble(FC["Pprice"]);
            PU.Finishing = Convert.ToInt32(FC["Pfinish"]);
            PU.FloorNumber = Convert.ToInt32(FC["Pfloors"]);
            PU.Notes = FC["Pnotes"];
            PU.ProjectID = (int)TempData["PID"];
            PU.UnitSpace = Convert.ToInt32(FC["Pspace"]);
            PU.UnitType = Convert.ToInt32(FC["Ptype"]);
            TempData.Keep();
            PU.Edit();
            return "true";
        }
    }
}
