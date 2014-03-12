using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icons.Models
{
    public partial class EmployeeImprest
    {
        #region Context
        MaksoudDBEntities db = new MaksoudDBEntities();
        #endregion

        public Returner Add()
        {
            var EmployeeToAddImprests = db.Employees.Where(p => p.Id == this.EmpID).SingleOrDefault();
            EmployeeToAddImprests.EmployeeImprests.Add(this);
            db.SaveChanges();
            var Imprest = db.EmployeeImprests.Where(p => p.Id == this.Id).ToList();
            var ImprestInJSON = (from P in Imprest
                                 select new
                                 {
                                     P.Date,
                                     P.EmpID,
                                     P.Id,
                                     P.Imprest,
                                     Employee = new
                                     {
                                         P.Employee.Address,
                                         P.Employee.BasicSalary,
                                         P.Employee.BenifitAccID,
                                         P.Employee.DateOfBirth,
                                         P.Employee.EmpAccID,
                                         P.Employee.HiringDate,
                                         P.Employee.Id,
                                         P.Employee.ImprestAccID,
                                         P.Employee.Name,
                                         P.Employee.PenaltyAccID,
                                         P.Employee.Phone1,
                                         P.Employee.Phone2,
                                         P.Employee.SalaryType,
                                         P.Employee.SSN,
                                         P.Employee.Title
                                     }
                                 }).ToList().SingleOrDefault();
            return new Returner
            {
                Data = Imprest.SingleOrDefault(),
                DataInJSON = ImprestInJSON.ToJSON(),
                Message = Message.Imprest_Added_Successfully
            };
        }

        public Returner Remove()
        {
            var ImprestToRemove = db.EmployeeImprests.Where(p => p.Id == this.Id).SingleOrDefault();
            db.EmployeeImprests.Remove(ImprestToRemove);
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Imprest_Removed_Successfully
            };
        }

        public Returner GetAll()
        {
            var AllImprests = db.EmployeeImprests.Where(p => p.EmpID == this.EmpID).ToList();
            return new Returner
            {
                Data = AllImprests
            };
        }
    }
}