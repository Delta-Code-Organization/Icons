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
        public void AddOwner(int CusID, double Percentage)
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
            C.LastEditBy = (Session["User"] as User).ID;
            List<Installment> LOI = new List<Installment>();
            double Amount = Convert.ToDouble(FC["cremaining"]);
            int InstallNum = Convert.ToInt32(FC["INum"]);
            double SingleInstallAmount = Convert.ToDouble(Amount / InstallNum);
            int MonthPeriod = Convert.ToInt32(FC["IMonthNum"]);
            DateTime LastMonthDate = Convert.ToDateTime(FC["IFirstDate"]);
            for (int i = 1; i <= InstallNum; i++)
            {
                Installment I = new Installment();
                I.Amount = SingleInstallAmount;
                I.LastEditBy = (Session["User"] as User).ID;
                if (i == 1)
                {
                    I.DueDate = LastMonthDate;
                }
                else
                {
                    I.DueDate = LastMonthDate.AddMonths(MonthPeriod);
                    LastMonthDate = (DateTime)I.DueDate;
                }
                I.PaymentDate = null;
                I.ResponsibleID = Convert.ToInt32(FC["iresponsibleid"]);
                LOI.Add(I);
            }
            List<ContractOwner> LOCO = new List<ContractOwner>();
            LOCO = TempData["LOCO"] as List<ContractOwner>;
            ProjectUnit PU = new ProjectUnit { Id = (int)C.UnitID };
            int UO = Convert.ToInt32(FC["iresponsibleid"]);
            C.CreateContract(LOCO, LOI, PU, UO, (Session["User"] as User).ID);
        }

        public ActionResult SearchInstallments()
        {
            ViewBag.S = new Customer().GetAllCutomers().Data as List<Customer>;
            return View();
        }

        [HttpPost]
        public JsonResult SearchInstallments(DateTime? fromdate, DateTime? todate, int? cusid)
        {
            int? CustomerID;
            if (cusid == 0)
            {
                CustomerID = null;
            }
            else
            {
                CustomerID = cusid;
            }
            return new Contract().SearchInstallments(fromdate, todate, CustomerID).DataInJSON;
        }

        [HttpPost]
        public void PayInstallment(int ID, DateTime PaymentDate)
        {
            new Contract().PayInstallment(ID, (Session["User"] as User).ID, PaymentDate);
        }

        

        [HttpPost]
        public void EditInstallment(FormCollection FC)
        {
            Installment I = new Installment();
            I.Id = Convert.ToInt32(FC["IDPop"]);
            I.DueDate = Convert.ToDateTime(FC["DueDatePop"]);
            I.Amount = Convert.ToDouble(FC["AmountPop"]);
            I.LastEditBy = (Session["User"] as User).ID;
            new Contract().EditInstallment(I);
        }
    }
}