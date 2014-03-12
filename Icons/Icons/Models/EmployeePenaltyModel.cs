using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icons.Models
{
    public partial class EmployeePenalty
    {
        #region Context
        MaksoudDBEntities db = new MaksoudDBEntities();
        #endregion

        public Returner Add()
        {
            var EmployeeToAddPenalty = db.Employees.Where(p => p.Id == this.EmpID).SingleOrDefault();
            EmployeeToAddPenalty.EmployeePenalties.Add(this);
            db.SaveChanges();
            var Penalty = db.EmployeePenalties.Where(p => p.Id == this.Id).ToList();
            var PenaltyInJSON = (from P in Penalty
                                 select new
                                 {
                                     P.Date,
                                     P.EmpID,
                                     P.Id,
                                     P.Penalty,
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
                Data = Penalty.SingleOrDefault(),
                DataInJSON = PenaltyInJSON.ToJSON(),
                Message = Message.Penalty_Added_Successfully
            };
        }

        public Returner Remove()
        {
            var PenaltyToRemove = db.EmployeePenalties.Where(p => p.Id == this.Id).SingleOrDefault();
            db.EmployeePenalties.Remove(PenaltyToRemove);
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Penalty_Removed_Successfully
            };
        }

        public Returner GetAll()
        {
            var AllPenalties = db.EmployeePenalties.Where(p => p.EmpID == this.EmpID).ToList();
            return new Returner
            {
                Data = AllPenalties
            };
        }
    }
}