using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Icons.Models;

namespace Icons.Controllers
{
    public class ContractController : Controller
    {
        //
        // GET: /Contract/

        public ActionResult CreateContract()
        {
            ViewBag.P = new Project().GetAll().Data as List<Project>;
            ViewBag.C = new Customer().GetAllCutomers().Data as List<Customer>;
            return View();
        }

        [HttpPost]
        public JsonResult GetUnits(int id)
        {
            return new Project { Id = id }.GetProjectUnits().DataInJSON;
        }

        [HttpPost]
        public void AddOwner(int CusID,double Percentage)
        {
            ContractOwner Co = new ContractOwner();
            Co.CustomerID = CusID;
            Co.Percentage = Percentage;
            List<ContractOwner> LOCO = new List<ContractOwner>();
            if (TempData["LOCO"] != null)
            {
                LOCO = TempData["LOCO"] as List<ContractOwner>;
                LOCO.Add(Co);
                TempData["LOCO"] = LOCO;
                TempData.Keep();
            }
            else
            {
                LOCO.Add(Co);
                TempData["LOCO"] = LOCO;
                TempData.Keep();
            }
        }

        public void DeleteOwner(int CusID)
        {
            List<ContractOwner> LOCO = TempData["LOCO"] as List<ContractOwner>;
            if (LOCO.Count != 0)
            {
                var CO = LOCO.Where(p => p.CustomerID == CusID).SingleOrDefault();
                if (CO != null)
                {
                    LOCO.Remove(CO);
                    TempData["LOCO"] = LOCO;
                    TempData.Keep();
                }
            }
        }

        [HttpPost]
        public void CreateContract(FormCollection FC)
        {
            Contract C = new Contract();
            C.Date = DateTime.Now;
            C.Paid = Convert.ToDouble(FC["cpaid"]);
            C.Price = Convert.ToDouble(FC["cprice"]);
            C.ProjectID = Convert.ToInt32(FC["cprojectid"]);
            C.Remaining = Convert.ToDouble(FC["cremaining"]);
            C.UnitID = Convert.ToInt32(FC["cunitid"]);
            C.Notes = FC["cnotes"];
            Installment I = new Installment();
            I.Amount = Convert.ToDouble(FC["iamount"]);
            I.DueDate = Convert.ToDateTime(FC["iduedate"]);
            I.PaymentDate = null;
            I.ResponsibleID = Convert.ToInt32(FC["iresponsibleid"]);
            List<ContractOwner> LOCO = new List<ContractOwner>();
            LOCO = TempData["LOCO"] as List<ContractOwner>;
            ProjectUnit PU = new ProjectUnit { Id = (int)C.UnitID };
            int UO = Convert.ToInt32(FC["iresponsibleid"]);
            C.CreateContract(LOCO, I, PU, UO);
        }
    }
}