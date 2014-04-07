using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icons.Models
{
    public partial class Employee
    {
        #region Context
        MaksoudDBEntities db = new MaksoudDBEntities();
        #endregion

        #region PrivateMethods
        private List<int> AddEmployerAccountingTrees(string EmployerName)
        {
            //Get Parents IDs
            int ParentOfEmpAccID = db.AccountingTrees.Where(p => p.KeyAccountID == (int)KeyAccounts.Employee).SingleOrDefault().Id;
            int ParentOfImprestAccID = db.AccountingTrees.Where(p => p.KeyAccountID == (int)KeyAccounts.Imprestes).SingleOrDefault().Id;
            int ParentOfBenifitAccID = db.AccountingTrees.Where(P => P.KeyAccountID == (int)KeyAccounts.Benifits).SingleOrDefault().Id;
            int ParentOfPenaltyAccID = db.AccountingTrees.Where(p => p.KeyAccountID == (int)KeyAccounts.Others).SingleOrDefault().Id;
            //Define Accounting Trees Objects To Save To DB
            AccountingTree EmpNode = new AccountingTree();
            AccountingTree ImprestNode = new AccountingTree();
            AccountingTree BenifitNode = new AccountingTree();
            AccountingTree PenaltyNode = new AccountingTree();
            //Fill Object Properties 
            EmpNode.NodeName = EmployerName;
            EmpNode.Parent = ParentOfEmpAccID;
            ImprestNode.NodeName = EmployerName;
            ImprestNode.Parent = ParentOfImprestAccID;
            BenifitNode.NodeName = EmployerName;
            BenifitNode.Parent = ParentOfBenifitAccID;
            PenaltyNode.NodeName = EmployerName;
            PenaltyNode.Parent = ParentOfPenaltyAccID;
            //Add Objects To Context
            db.AccountingTrees.Add(EmpNode);
            db.AccountingTrees.Add(ImprestNode);
            db.AccountingTrees.Add(BenifitNode);
            db.AccountingTrees.Add(PenaltyNode);
            //Save Changes To DB
            db.SaveChanges();
            //Fill List Of Int With Saved Nodes IDs
            List<int> LON = new List<int>();
            LON.Add(EmpNode.Id);
            LON.Add(ImprestNode.Id);
            LON.Add(BenifitNode.Id);
            LON.Add(PenaltyNode.Id);
            //Return List Of Nodes IDs
            return LON;
        }

        private void RemoveEmployerAccountingTrees(List<int> LON)
        {
            foreach (int NodeID in LON)
            {
                var NodeToRemove = db.AccountingTrees.Where(p => p.Id == NodeID).SingleOrDefault();
                db.AccountingTrees.Remove(NodeToRemove);
            }
            db.SaveChanges();
        }

        private void EditEmployerAccountingTrees(string EmployerName, List<int> LON)
        {
            int NEmpAccID = LON[0];
            int NImprestAccID = LON[1];
            int NBenifitAccID = LON[2];
            int NPenaltyAccID = LON[3];
            //Get Accounting Trees Objects To Update
            AccountingTree EmpNode = db.AccountingTrees.Where(p => p.Id == NEmpAccID).SingleOrDefault();
            AccountingTree ImprestNode = db.AccountingTrees.Where(p => p.Id == NImprestAccID).SingleOrDefault();
            AccountingTree BenifitNode = db.AccountingTrees.Where(p => p.Id == NBenifitAccID).SingleOrDefault();
            AccountingTree PenaltyNode = db.AccountingTrees.Where(p => p.Id == NPenaltyAccID).SingleOrDefault();
            //Update Object Properties 
            EmpNode.NodeName = EmployerName;
            ImprestNode.NodeName = EmployerName;
            BenifitNode.NodeName = EmployerName;
            PenaltyNode.NodeName = EmployerName;
            //Save Changes To DB
            db.SaveChanges();
        }
        #endregion

        #region PublicMethod
        public Returner Add()
        {
            List<int> LON = new List<int>();
            LON = AddEmployerAccountingTrees(this.Name);
            this.EmpAccID = LON[0];
            this.ImprestAccID = LON[1];
            this.BenifitAccID = LON[2];
            this.PenaltyAccID = LON[3];
            db.Employees.Add(this);
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Employee_Created_Successfully
            };
        }

        public Returner GetAll()
        {
            return new Returner
            {
                Data = (from E in db.Employees
                        select E).ToList()
            };
        }

        public Returner GetByID()
        {
            return new Returner
            {
                Data = (from E in db.Employees
                        where E.Id == this.Id
                        select E).ToList().SingleOrDefault()
            };
        }

        public Returner Remove()
        {
            var Emp = db.Employees.Where(p => p.Id == this.Id).SingleOrDefault();
            List<int> LON = new List<int>();
            LON.Add((int)Emp.EmpAccID);
            LON.Add((int)Emp.ImprestAccID);
            LON.Add((int)Emp.BenifitAccID);
            LON.Add((int)Emp.PenaltyAccID);
            RemoveEmployerAccountingTrees(LON);
            db.Employees.Remove(Emp);
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Employee_Deleted_Successfully
            };
        }

        public Returner Edit()
        {
            var Emp = db.Employees.Where(p => p.Id == this.Id).SingleOrDefault();
            Emp.Address = this.Address;
            Emp.BasicSalary = this.BasicSalary;
            Emp.DateOfBirth = this.DateOfBirth;
            Emp.HiringDate = this.HiringDate;
            Emp.Name = this.Name;
            Emp.Phone1 = this.Phone1;
            Emp.Phone2 = this.Phone2;
            Emp.SalaryType = this.SalaryType;
            Emp.SSN = this.SSN;
            Emp.Title = this.Title;
            List<int> LON = new List<int>();
            LON.Add((int)Emp.EmpAccID);
            LON.Add((int)Emp.ImprestAccID);
            LON.Add((int)Emp.BenifitAccID);
            LON.Add((int)Emp.PenaltyAccID);
            EditEmployerAccountingTrees(Emp.Name, LON);
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Employee_Updated_Successfully
            };
        }

        public Returner AddBenifit(FinancialTransaction FT)
        {
            var Employee = db.Employees.Single(p => p.Id == this.Id);
            FT.Statement = "اضافة مكافأة لعميل";
            FT.FromAccount = Employee.BenifitAccID;
            FT.TransactionDate = DateTime.Now;
            db.FinancialTransactions.Add(FT);
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Benifit_Added_Successfully
            };
        }

        public Returner AddPenalty(FinancialTransaction FT)
        {
            var Employee = db.Employees.Single(p => p.Id == this.Id);
            FT.Statement = "اضافة جزاء لعميل";
            FT.ToAccount = Employee.PenaltyAccID;
            FT.TransactionDate = DateTime.Now;
            db.FinancialTransactions.Add(FT);
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Penalty_Added_Successfully
            };
        }

        public Returner AddImprest(FinancialTransaction FT)
        {
            var Employee = db.Employees.Single(p => p.Id == this.Id);
            FT.Statement = "اضافة عهدة لعميل";
            FT.FromAccount = Employee.ImprestAccID;
            FT.TransactionDate = DateTime.Now;
            db.FinancialTransactions.Add(FT);
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Imprest_Added_Successfully
            };
        }

        public Returner GetPenalties()
        {
            var E = db.Employees.Single(p => p.Id == this.Id);
            return new Returner
            {
                Data = db.FinancialTransactions.Where(p => p.FromAccount == E.PenaltyAccID).ToList()
            };
        }

        public Returner GetBenifits()
        {
            var E = db.Employees.Single(p => p.Id == this.Id);
            return new Returner
            {
                Data = db.FinancialTransactions.Where(p => p.ToAccount == E.BenifitAccID).ToList()
            };
        }

        public Returner GetImprests()
        {
            var E = db.Employees.Single(p => p.Id == this.Id);
            return new Returner
            {
                Data = db.FinancialTransactions.Where(p => p.FromAccount == E.ImprestAccID).ToList()
            };
        }

        public Returner RemovePenalty(int FTID)
        {
            var FTToRemove = db.FinancialTransactions.Where(p => p.Id == FTID).SingleOrDefault();
            db.FinancialTransactions.Remove(FTToRemove);
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Penalty_Removed_Successfully
            };
        }

        public Returner RemoveBenifit(int FTID)
        {
            var FTToRemove = db.FinancialTransactions.Where(p => p.Id == FTID).SingleOrDefault();
            db.FinancialTransactions.Remove(FTToRemove);
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Benifit_Removed_Successfully
            };
        }

        public Returner RemoveImprest(int FTID)
        {
            var FTToRemove = db.FinancialTransactions.Where(p => p.Id == FTID).SingleOrDefault();
            db.FinancialTransactions.Remove(FTToRemove);
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Imprest_Removed_Successfully
            };
        }

        public Returner Pay(double Total, int EditBy, DateTime PaymentDate, int ToAcc)
        {
            var E = db.Employees.Single(p => p.Id == this.Id);
            var lastTransaction = db.FinancialTransactions.Where(p => p.FromAccount == E.EmpAccID).OrderByDescending(p => p.TransactionDate).FirstOrDefault();
            if (lastTransaction != null)
            {
                switch (E.SalaryType)
                {
                    case 1:
                        if (PaymentDate < ((DateTime)lastTransaction.TransactionDate).AddHours(24))
                        {
                            return new Returner
                            {
                                Message = Message.Cannot_pay_salary_at_Wrong_time
                            };
                        }
                        break;
                    case 2:
                        if (PaymentDate < ((DateTime)lastTransaction.TransactionDate).AddDays(15))
                        {
                            return new Returner
                            {
                                Message = Message.Cannot_pay_salary_at_Wrong_time
                            };
                        }
                        break;
                    case 3:
                        if (PaymentDate < ((DateTime)lastTransaction.TransactionDate).AddDays(30))
                        {
                            return new Returner
                            {
                                Message = Message.Cannot_pay_salary_at_Wrong_time
                            };
                        }
                        break;
                }
            }
            else
            {
                FinancialTransaction Ft = new FinancialTransaction();
                Ft.Amount = Total;
                Ft.FromAccount = E.EmpAccID;
                Ft.LastEditBy = EditBy;
                Ft.Notes = "";
                Ft.Statement = "دفع راتب للموظف " + E.Name;
                Ft.ToAccount = ToAcc;
                Ft.TransactionDate = PaymentDate;
                db.FinancialTransactions.Add(Ft);
                db.SaveChanges();
            }
            return new Returner
            {
                Message = Message.Salary_Paid_Successfully
            };
        }
        #endregion
    }
}