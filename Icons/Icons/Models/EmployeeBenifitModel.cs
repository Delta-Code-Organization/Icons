using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icons.Models
{
    public partial class EmployeeBenifit
    {
        #region Context
        MaksoudDBEntities db = new MaksoudDBEntities();
        #endregion

        public Returner Add()
        {
            var EmployeeToAddBenifit = db.Employees.Where(p => p.Id == this.EmpID).SingleOrDefault();
            EmployeeToAddBenifit.EmployeeBenifits.Add(this);
            db.SaveChanges();
            var Benifit = db.EmployeeBenifits.Where(p => p.Id == this.Id).ToList();
            var BenifitInJSON = (from P in Benifit
                                 select new
                                 {
                                     P.Date,
                                     P.EmpID,
                                     P.Id,
                                     P.Benifit,
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
                Data = Benifit.SingleOrDefault(),
                DataInJSON = BenifitInJSON.ToJSON(),
                Message = Message.Benifit_Added_Successfully
            };
        }

        public Returner Remove()
        {
            var BenifitToRemove = db.EmployeeBenifits.Where(p => p.Id == this.Id).SingleOrDefault();
            db.EmployeeBenifits.Remove(BenifitToRemove);
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Benifit_Removed_Successfully
            };
        }

        public Returner GetAll()
        {
            var AllBenifits = db.EmployeeBenifits.Where(p => p.EmpID == this.EmpID).ToList();
            return new Returner
            {
                Data = AllBenifits
            };
        }
    }
}