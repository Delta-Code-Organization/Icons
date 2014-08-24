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

        public Returner CreateContract(List<ContractOwner> LOCO, List<Installment> I, ProjectUnit PU, int OwnerID, int EditBy, int RecievableAcc)
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
            Ft.Credit = CurrentContract.Price;
            Ft.Debit = 0;
            Ft.FromAccount = CurrentCustomer.AccountID;
            Ft.ReferanceDocumentNumber = CurrentContract.Id;
            Ft.LastEditBy = EditBy;
            Ft.Notes = "";
            Ft.Statement = "عقد بيع " + Enum.GetName(typeof(UnitTypes), CurrentProjectUnit.UnitType) + " للعميل " + CurrentCustomer.Name;
            Ft.Confirmed = false;
            Ft.TransactionDate = CurrentContract.Date;
            db.FinancialTransactions.Add(Ft);
            FinancialTransaction Ft6 = new FinancialTransaction();
            Ft6.Credit = 0;
            Ft6.Debit = CurrentContract.Price;
            Ft6.FromAccount = 42;//حساب المبيعات
            Ft6.ReferanceDocumentNumber = CurrentContract.Id;
            Ft6.LastEditBy = EditBy;
            Ft6.Notes = "";
            Ft6.Statement = "عقد بيع " + Enum.GetName(typeof(UnitTypes), CurrentProjectUnit.UnitType) + " للعميل " + CurrentCustomer.Name;
            Ft6.Confirmed = false;
            Ft6.TransactionDate = CurrentContract.Date;
            db.FinancialTransactions.Add(Ft6);
            FinancialTransaction Ft1 = new FinancialTransaction();
            Ft1.Debit = CurrentContract.Paid;
            Ft1.Credit = 0;
            Ft1.LastEditBy = EditBy;
            Ft1.Confirmed = false;
            Ft1.ReferanceDocumentNumber = CurrentContract.Id;
            Ft1.Notes = "";
            Ft1.Statement = "دفع مقدم بيع " + Enum.GetName(typeof(UnitTypes), CurrentProjectUnit.UnitType) + " للعميل " + CurrentCustomer.Name;
            Ft1.FromAccount = CurrentCustomer.AccountID;
            Ft1.TransactionDate = CurrentContract.Date;
            db.FinancialTransactions.Add(Ft1);
            FinancialTransaction Ft10 = new FinancialTransaction();
            Ft10.Debit = 0;
            Ft10.Credit = CurrentContract.Paid;
            Ft10.LastEditBy = EditBy;
            Ft10.Confirmed = false;
            Ft10.ReferanceDocumentNumber = CurrentContract.Id;
            Ft10.Notes = "";
            Ft10.Statement = "دفع مقدم بيع " + Enum.GetName(typeof(UnitTypes), CurrentProjectUnit.UnitType) + " للعميل " + CurrentCustomer.Name;
            Ft10.FromAccount = RecievableAcc;
            Ft10.TransactionDate = CurrentContract.Date;
            db.FinancialTransactions.Add(Ft10);
            //foreach (Installment ite in CurrentContract.Installments)
            //{
            //    FinancialTransaction Ft2 = new FinancialTransaction();
            //    Ft2.Amount = ite.Amount;
            //    Ft2.FromAccount = CurrentCustomer.AccountID;
            //    Ft2.LastEditBy = EditBy;
            //    Ft2.Notes = "";
            //    Ft2.Statement = "قسط بيع " + Enum.GetName(typeof(UnitTypes), CurrentProjectUnit.UnitType) + " للعميل " + CurrentCustomer.Name;
            //    Ft2.ToAccount = 42;
            //    Ft2.Confirmed = false;
            //    Ft2.TransactionDate = ite.DueDate;
            //    Ft2.ReferanceDocumentNumber = ite.Id;
            //    db.FinancialTransactions.Add(Ft2);
            //}
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
            //List<Installment> LOI = new List<Installment>();
            List<Contract> LOContracts = new List<Contract>();
            if (CusID != null && From == null && To == null)
            {
                LOContracts = db.Contracts.Where(p => p.Installments.FirstOrDefault().ResponsibleID == CusID).ToList();
                //LOI = db.Installments.Where(p => p.ResponsibleID == CusID).ToList();
            }
            if (From != null && To != null && CusID == null)
            {
                LOContracts = db.Contracts.Where(p => p.Installments.FirstOrDefault().DueDate >= From && p.Installments.FirstOrDefault().DueDate <= To).ToList();
                //LOI = db.Installments.Where(p => p.DueDate >= From && p.DueDate <= To).ToList();
            }
            if (From != null && To != null && CusID != null)
            {
                LOContracts = db.Contracts.Where(p => p.Installments.FirstOrDefault().DueDate >= From && p.Installments.FirstOrDefault().DueDate <= To && p.Installments.FirstOrDefault().ResponsibleID == CusID).ToList();
                //LOI = db.Installments.Where(p => p.DueDate >= From && p.DueDate <= To && p.ResponsibleID == CusID).ToList();
            }
            //List<Customer> LOC = (from Cus in LOI
            //                      select Cus.Customer).ToList();
            //List<Customer> LONC = new List<Customer>();
            //foreach (Customer item in LOC)
            //{
            //    if (LONC.Any(p => p.ID == item.ID) == false)
            //    {
            //        LONC.Add(item);
            //    }
            //}
            var ContractsInJSON = (from C in LOContracts
                                   select new
                                   {
                                       C.Date,
                                       C.Id,
                                       C.LastEditBy,
                                       C.Notes,
                                       C.Paid,
                                       C.Price,
                                       C.ProjectID,
                                       C.Remaining,
                                       C.UnitID,
                                       Installments = new List<object>((from III in C.Installments
                                                                        select new
                                                                        {
                                                                            III.Amount,
                                                                            III.ContractID,
                                                                            III.DueDate,
                                                                            III.Id,
                                                                            III.PaymentDate,
                                                                            III.ResponsibleID,
                                                                            Customer = new
                                                                            {
                                                                                III.Customer.Name
                                                                            }
                                                                        }).Cast<object>().ToList()).Cast<object>().ToList(),
                                       Project = new
                                       {
                                           C.Project.ProjectName
                                       },
                                       ProjectUnit = new
                                       {
                                           C.ProjectUnit.UnitType
                                       }
                                   }).ToList();
            //    var ResultInJSON = (from I in LONC
            //                        select new
            //                        {
            //                            I.AccountID,
            //                            I.Address,
            //                            I.BirthDate,
            //                            I.ID,
            //                            I.Name,
            //                            Installments = new List<object>((from III in I.Installments
            //                                                             select new
            //                                                             {
            //                                                                 III.Amount,
            //                                                                 III.ContractID,
            //                                                                 III.DueDate,
            //                                                                 III.Id,
            //                                                                 III.PaymentDate,
            //                                                                 III.ResponsibleID
            //                                                             }).Cast<object>().ToList()).Cast<object>().ToList()
            //                        }).ToList();
            return new Returner
            {
                Data = LOContracts,
                DataInJSON = ContractsInJSON.ToJSON()
            };
        }

        public Returner InstallmentsReport(DateTime From, DateTime To, int Cus, int Status, int ProjID)
        {
            List<Installment> LOI = new List<Installment>();
            if (Cus == 0)
            {
                if (ProjID == 0)
                {
                    if (Status == 0)
                    {
                        var I = db.Installments.Where(p => p.DueDate >= From && p.DueDate <= To).ToList();
                        LOI.AddRange(I);
                    }
                    else if (Status == 1)
                    {
                        var I = db.Installments.Where(p => p.DueDate >= From && p.DueDate <= To && p.PaymentDate != null).ToList();
                        LOI.AddRange(I);
                    }
                    else
                    {
                        var I = db.Installments.Where(p => p.DueDate >= From && p.DueDate <= To && p.PaymentDate == null).ToList();
                        LOI.AddRange(I);
                    }
                }
                else
                {
                    if (Status == 0)
                    {
                        var I = db.Installments.Where(p => p.DueDate >= From && p.DueDate <= To && p.Contract.ProjectID == ProjID).ToList();
                        LOI.AddRange(I);
                    }
                    else if (Status == 1)
                    {
                        var I = db.Installments.Where(p => p.DueDate >= From && p.DueDate <= To && p.PaymentDate != null && p.Contract.ProjectID == ProjID).ToList();
                        LOI.AddRange(I);
                    }
                    else
                    {
                        var I = db.Installments.Where(p => p.DueDate >= From && p.DueDate <= To && p.PaymentDate == null && p.Contract.ProjectID == ProjID).ToList();
                        LOI.AddRange(I);
                    }
                }
            }
            else
            {
                if (ProjID == 0)
                {
                    if (Status == 0)
                    {
                        var I = db.Installments.Where(p => p.DueDate >= From && p.DueDate <= To && p.ResponsibleID == Cus).ToList();
                        LOI.AddRange(I);
                    }
                    else if (Status == 1)
                    {
                        var I = db.Installments.Where(p => p.DueDate >= From && p.DueDate <= To && p.PaymentDate != null && p.ResponsibleID == Cus).ToList();
                        LOI.AddRange(I);
                    }
                    else
                    {
                        var I = db.Installments.Where(p => p.DueDate >= From && p.DueDate <= To && p.PaymentDate == null && p.ResponsibleID == Cus).ToList();
                        LOI.AddRange(I);
                    }
                }
                else
                {
                    if (Status == 0)
                    {
                        var I = db.Installments.Where(p => p.DueDate >= From && p.DueDate <= To && p.ResponsibleID == Cus && p.Contract.ProjectID == ProjID).ToList();
                        LOI.AddRange(I);
                    }
                    else if (Status == 1)
                    {
                        var I = db.Installments.Where(p => p.DueDate >= From && p.DueDate <= To && p.PaymentDate != null && p.ResponsibleID == Cus && p.Contract.ProjectID == ProjID).ToList();
                        LOI.AddRange(I);
                    }
                    else
                    {
                        var I = db.Installments.Where(p => p.DueDate >= From && p.DueDate <= To && p.PaymentDate == null && p.ResponsibleID == Cus && p.Contract.ProjectID == ProjID).ToList();
                        LOI.AddRange(I);
                    }
                }
            }
            var LOIInJSON = (from I in LOI
                             select new
                             {
                                 I.Amount,
                                 I.DueDate,
                                 I.Id,
                                 I.LastEditBy,
                                 I.PaymentDate,
                                 Project = new
                                 {
                                     I.Contract.Project.ProjectName
                                 },
                                 ProjectUnit = new
                                 {
                                     Name = Enum.GetName(typeof(UnitTypes), I.Contract.ProjectUnit.UnitType)
                                 },
                                 Customer = new
                                 {
                                     I.Customer.Name
                                 }
                             }).ToList();
            return new Returner
            {
                Data = LOI,
                DataInJSON = LOIInJSON.ToJSON()
            };
        }

        public Returner PayInstallment(int ID, int EditBy, DateTime PaymentDate, int RecievableAcc)
        {
            var Installment = db.Installments.Where(p => p.Id == ID).SingleOrDefault();
            Installment.PaymentDate = PaymentDate;
            db.SaveChanges();
            FinancialTransaction Ft = new FinancialTransaction();
            Ft.Credit = 0;
            Ft.Debit = Installment.Amount;
            Ft.FromAccount = Installment.Customer.AccountID;
            Ft.LastEditBy = EditBy;
            Ft.Notes = "";
            Ft.ReferanceDocumentNumber = Installment.Id;
            Ft.Confirmed = false;
            Ft.Statement = "دفع قسط " + Enum.GetName(typeof(UnitTypes), Installment.Contract.ProjectUnit.UnitType) + " من العميل " + Installment.Customer.Name;
            Ft.TransactionDate = PaymentDate;
            db.FinancialTransactions.Add(Ft);
            FinancialTransaction Ft1 = new FinancialTransaction();
            Ft1.Debit = 0;
            Ft1.Credit = Installment.Amount;
            Ft1.FromAccount = RecievableAcc;//Installment.Contract.ProjectUnit.AccountingID;
            Ft1.ReferanceDocumentNumber = Installment.Id;
            Ft1.LastEditBy = EditBy;
            Ft1.Notes = "";
            Ft1.Confirmed = false;
            Ft1.Statement = "دفع قسط " + Enum.GetName(typeof(UnitTypes), Installment.Contract.ProjectUnit.UnitType) + " من العميل " + Installment.Customer.Name;
            Ft1.TransactionDate = PaymentDate;
            db.FinancialTransactions.Add(Ft);
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Installment_Paid_Successfully
            };
        }

        public Returner EditInstallment(Installment I)
        {
            var ITE = db.Installments.Single(p => p.Id == I.Id);
            ITE.DueDate = I.DueDate;
            ITE.Amount = I.Amount;
            db.SaveChanges();
            if (ITE.PaymentDate != null)
            {
                var FTToUpdate = db.FinancialTransactions.Where(p => p.ReferanceDocumentNumber == ITE.Id).ToList();
                foreach (FinancialTransaction II in FTToUpdate)
                {
                    if (II.Debit == 0)
                    {
                        II.Credit = ITE.Amount;
                        II.TransactionDate = ITE.DueDate;
                        db.SaveChanges();
                    }
                    else
                    {
                        II.Debit = ITE.Amount;
                        II.TransactionDate = ITE.DueDate;
                        db.SaveChanges();
                    }
                }
            }
            return new Returner
            {
                Message = Message.Installment_Updated_Successfully
            };
        }

        public void AddInstallment(Installment I)
        {
            db.Installments.Add(I);
            db.SaveChanges();
        }

    }
}