using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icons.Models
{
    public partial class Project
    {
        #region Context
        MaksoudDBEntities db = new MaksoudDBEntities();
        #endregion

        public Returner Add()
        {
            var ParentAccount = db.AccountingTrees.Where(p => p.KeyAccountID == (int)KeyAccounts.Projects).FirstOrDefault();
            if (ParentAccount != null)
            {
                AccountingTree ProjectNode = new AccountingTree();
                ProjectNode.NodeName = this.ProjectName;
                ProjectNode.Parent = ParentAccount.Id;
                db.AccountingTrees.Add(ProjectNode);
                db.SaveChanges();
            }
            var AccID = db.AccountingTrees.Where(p => p.NodeName == this.ProjectName && p.Parent == ParentAccount.Id).SingleOrDefault();
            this.AccountID = AccID.Id;
            db.Projects.Add(this);
            db.SaveChanges();
            new Stock
            {
                ProjectID = this.Id
            }.NewProject();
            return new Returner
            {
                Message = Message.Project_Created_Successfully
            };
        }

        public Returner GetAll()
        {
            return new Returner
            {
                Data = (from P in db.Projects
                        select P).ToList()
            };
        }

        public Returner Remove()
        {
            var P = db.Projects.Where(p => p.Id == this.Id).SingleOrDefault();
            if (db.Stocks.Any(p => p.ProjectID == P.Id && p.Quantity > 0))
            {
                return new Returner
                {
                    Message = Message.This_Project_Have_Stock_Cannot_Be_Deleted
                };
            }
            else
            {
                db.Database.ExecuteSqlCommand("delete from Stock where ProjectID = {0}", P.Id);
            }
            db.SaveChanges();
            var PAcc = db.AccountingTrees.Where(p => p.Id == (int)P.AccountID).SingleOrDefault();
            db.AccountingTrees.Remove(PAcc);
            db.SaveChanges();
            db.Projects.Remove(P);
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Project_Deleted_Successfully
            };
        }

        public Returner GetByID()
        {
            return new Returner
            {
                Data = db.Projects.Where(p => p.Id == this.Id).SingleOrDefault()
            };
        }

        public Returner Edit()
        {
            var P = db.Projects.Where(p => p.Id == this.Id).SingleOrDefault();
            var PAcc = db.AccountingTrees.Where(p => p.Id == P.AccountID).SingleOrDefault();
            PAcc.NodeName = this.ProjectName;
            db.SaveChanges();
            P.ExpectedCost = this.ExpectedCost;
            P.FirstViewLength = this.FirstViewLength;
            P.FloorsCount = this.FloorsCount;
            P.ForthViewLength = this.ForthViewLength;
            P.LandSpace = this.LandSpace;
            P.OwnershipPercentage = this.OwnershipPercentage;
            P.ProjectAddress = this.ProjectAddress;
            P.ProjectName = this.ProjectName;
            P.SecondViewLength = this.SecondViewLength;
            P.ThirdViewLength = this.ThirdViewLength;
            P.CreationDate = this.CreationDate;
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Project_Updated_Successfully
            };
        }

        public Returner GetProjectUnits()
        {
            var ProjUnits = db.ProjectUnits.Where(p => p.ProjectID == this.Id).ToList();
            var ProjUnitsInJSON = (from U in ProjUnits
                                   select new
                                   {
                                       U.Id,
                                       U.ExpectedPrice,
                                       Finishing = Enum.GetName(typeof(Finishing), U.Finishing),
                                       U.FloorNumber,
                                       U.UnitSpace,
                                       U.DisplayText,
                                       UnitName = Enum.GetName(typeof(UnitTypes), U.UnitType)
                                   }).ToList();
            return new Returner
            {
                Data = ProjUnits,
                DataInJSON = ProjUnitsInJSON.ToJSON()
            };
        }

        public Returner TotalSales(int ProjID)
        {
            if (ProjID == 0)
            {
                var ParentID = db.AccountingTrees.SingleOrDefault(p => p.KeyAccountID == (int)KeyAccounts.Sales).Id;
                var TotalSales = db.FinancialTransactions.Where(p => p.FromAccount == ParentID).ToList().Sum(p => p.Debit);
                return new Returner
                {
                    Data = TotalSales
                };
            }
            else
            {
                var ProjContratcs = db.Projects.Where(p => p.Id == ProjID).SingleOrDefault().Contracts.ToList();
                List<FinancialTransaction> LOFT = new List<FinancialTransaction>();
                foreach (Contract item in ProjContratcs)
                {
                    var Ft = db.FinancialTransactions.Where(p => p.FromAccount == 42 && p.ReferanceDocumentNumber == item.Id).ToList().FirstOrDefault();
                    if (Ft != null)
                    {
                        LOFT.Add(Ft);
                    }
                }
                var TotalSales = LOFT.Sum(p => p.Debit);
                return new Returner
                {
                    Data = TotalSales
                };
            }
        }

        public Returner TotalCosts(int ProjID)
        {

            if (ProjID == 0)
            {
                //39 is the id of projects under constarction
                var TotalSales = db.FinancialTransactions.Where(p => p.FromAccount == 39).ToList().Sum(p => p.Debit);
                return new Returner
                {
                    Data = TotalSales
                };
            }
            else
            {
                var ProjContratcs = db.Projects.Where(p => p.Id == ProjID).SingleOrDefault().Contracts.ToList();
                List<FinancialTransaction> LOFT = new List<FinancialTransaction>();
                foreach (Contract item in ProjContratcs)
                {
                    var Ft = db.FinancialTransactions.Where(p => p.FromAccount == 39 && p.ReferanceDocumentNumber == item.Id).ToList().FirstOrDefault();
                    if (Ft != null)
                    {
                        LOFT.Add(Ft);
                    }
                }
                var TotalSales = LOFT.Sum(p => p.Debit);
                return new Returner
                {
                    Data = TotalSales
                };
            }
        }

        public Returner PendingInstallment(int ProjID)
        {
            if (ProjID == 0)
            {
                var PenInstallments = db.Installments.Where(p => p.PaymentDate == null).ToList().Sum(p => p.Amount);
                return new Returner
                {
                    Data = PenInstallments
                };
            }
            else
            {
                var PenInstallments = db.Installments.Where(p => p.PaymentDate == null && p.Contract.ProjectID == ProjID).ToList().Sum(p => p.Amount);
                return new Returner
                {
                    Data = PenInstallments
                };
            }
        }
    }
}