using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icons.Models
{
    public partial class Contract
    {
        #region Context
        MaksoudDBEntities db = new MaksoudDBEntities();
        #endregion

        public Returner CreateContract(List<ContractOwner> LOCO, List<Installment> I, ProjectUnit PU, int OwnerID,int EditBy)
        {
            db.Contracts.Add(this);
            db.SaveChanges();
            var CurrentContract = db.Contracts.Where(p => p.Id == this.Id).SingleOrDefault();
            foreach (ContractOwner CO in LOCO)
            {
                CurrentContract.ContractOwners.Add(CO);
            }
            db.SaveChanges();
            CurrentContract = db.Contracts.Where(p => p.Id == this.Id).SingleOrDefault();
            foreach (Installment III in I)
            {
                CurrentContract.Installments.Add(III);
            }
            db.SaveChanges();
            CurrentContract = db.Contracts.Where(p => p.Id == this.Id).SingleOrDefault();
            var CustomerIIDD = CurrentContract.Installments.FirstOrDefault().ResponsibleID;
            var CurrentCustomer = db.Customers.Single(p => p.ID == CustomerIIDD);
            var CurrentProjectUnit = db.ProjectUnits.Single(p => p.Id == CurrentContract.UnitID);
            FinancialTransaction Ft = new FinancialTransaction();
            Ft.Amount = CurrentContract.Price;
            Ft.FromAccount = CurrentCustomer.AccountID;
            Ft.LastEditBy = EditBy;
            Ft.Notes = "";
            Ft.Statement = "عقد بيع " + Enum.GetName(typeof(UnitTypes), CurrentProjectUnit.UnitType) + " للعميل " + CurrentCustomer.Name;
            Ft.ToAccount = CurrentProjectUnit.AccountingID;
            Ft.TransactionDate = CurrentContract.Date;
            db.FinancialTransactions.Add(Ft);
            FinancialTransaction Ft1 = new FinancialTransaction();
            Ft1.Amount = CurrentContract.Paid;
            Ft1.FromAccount = CurrentProjectUnit.AccountingID;
            Ft1.LastEditBy = EditBy;
            Ft1.Notes = "";
            Ft1.Statement = "دفع مقدم بيع " + Enum.GetName(typeof(UnitTypes), CurrentProjectUnit.UnitType) + " للعميل " + CurrentCustomer.Name;
            Ft1.ToAccount = CurrentCustomer.AccountID;
            Ft1.TransactionDate = CurrentContract.Date;
            db.FinancialTransactions.Add(Ft1);
            foreach (Installment ite in CurrentContract.Installments)
            {
                FinancialTransaction Ft2 = new FinancialTransaction();
                Ft2.Amount = ite.Amount;
                Ft2.FromAccount = CurrentCustomer.AccountID;
                Ft2.LastEditBy = EditBy;
                Ft2.Notes = "";
                Ft2.Statement = "قسط بيع " + Enum.GetName(typeof(UnitTypes), CurrentProjectUnit.UnitType) + " للعميل " + CurrentCustomer.Name;
                Ft2.ToAccount = CurrentProjectUnit.AccountingID;
                Ft2.TransactionDate = CurrentContract.Date;
                db.FinancialTransactions.Add(Ft2);
            }
            db.SaveChanges();
            var ProjU = db.ProjectUnits.Where(p => p.Id == PU.Id).SingleOrDefault();
            ProjU.Owner = OwnerID;
            db.SaveChanges();
            return new Returner
            {
                Data = CurrentContract,
                Message = Message.Contract_Created_Successfully
            };
        }

        public Returner SearchInstallments(DateTime? From, DateTime? To, int? CusID)
        {
            List<Installment> LOI = new List<Installment>();
            if (CusID != null && From == null && To == null)
            {
                LOI = db.Installments.Where(p => p.ResponsibleID == CusID).ToList();
            }
            if (From != null && To != null && CusID == null)
            {
                LOI = db.Installments.Where(p => p.DueDate >= From && p.DueDate <= To).ToList();
            }
            if (From != null && To != null && CusID != null)
            {
                LOI = db.Installments.Where(p => p.DueDate >= From && p.DueDate <= To && p.ResponsibleID == CusID).ToList();
            }
            List<Customer> LOC = (from Cus in LOI
                                  select Cus.Customer).ToList();
            List<Customer> LONC = new List<Customer>();
            foreach (Customer item in LOC)
            {
                if (LONC.Any(p => p.ID == item.ID) == false)
                {
                    LONC.Add(item);
                }
            }
            var ResultInJSON = (from I in LONC
                                select new
                                {
                                    I.AccountID,
                                    I.Address,
                                    I.BirthDate,
                                    I.ID,
                                    I.Name,
                                    Installments = new List<object>((from III in I.Installments
                                                                     select new
                                                                     {
                                                                         III.Amount,
                                                                         III.ContractID,
                                                                         III.DueDate,
                                                                         III.Id,
                                                                         III.PaymentDate,
                                                                         III.ResponsibleID
                                                                     }).Cast<object>().ToList()).Cast<object>().ToList()
                                }).ToList();
            return new Returner
            {
                Data = LOI,
                DataInJSON = ResultInJSON.ToJSON()
            };
        }

        public Returner PayInstallment(int ID,int EditBy)
        {
            var Installment = db.Installments.Where(p => p.Id == ID).SingleOrDefault();
            Installment.PaymentDate = DateTime.Now;
            db.SaveChanges();
            FinancialTransaction Ft = new FinancialTransaction();
            Ft.Amount = Installment.Amount;
            Ft.FromAccount = Installment.Contract.ProjectUnit.AccountingID;
            Ft.LastEditBy = EditBy;
            Ft.Notes = "";
            Ft.Statement = "دفع قسط " + Enum.GetName(typeof(UnitTypes), Installment.Contract.ProjectUnit.UnitType) + " من العميل " + Installment.Customer.Name;
            Ft.ToAccount = Installment.Customer.AccountID;
            Ft.TransactionDate = DateTime.Now;
            db.FinancialTransactions.Add(Ft);
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Installment_Paid_Successfully
            };
        }
    }
}