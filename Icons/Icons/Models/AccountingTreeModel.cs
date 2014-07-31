using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data;

namespace Icons.Models
{
    [Serializable]
    public class TreeNode
    {
        int _id;

        public int id
        {
            get { return _id; }
            set { _id = value; }
        }
        string _name;

        public string name
        {
            get { return _name; }
            set { _name = value; }
        }
        List<TreeNode> _children = new List<TreeNode>();

        public List<TreeNode> children
        {
            get { return _children; }
            set { _children = value; }
        }

    }
    public partial class AccountingTree
    {
        #region Context
        MaksoudDBEntities db = new MaksoudDBEntities();
        #endregion
        public bool UpdateTree(List<TreeNode> _Tree, TreeNode Parent = null)
        {
            foreach (TreeNode node in _Tree)
            {
                AccountingTree CurrentNode = db.AccountingTrees.Where(p => p.Id == node.id).FirstOrDefault();
                if (CurrentNode == null)
                {
                    CurrentNode = new AccountingTree();
                    db.AccountingTrees.Add(CurrentNode);
                }
                CurrentNode.NodeName = node.name;
                if (Parent != null)
                    CurrentNode.Parent = Parent.id;
                else
                    CurrentNode.Parent = null;
                db.SaveChanges();
                if (node.children.Any())
                    UpdateTree(node.children, node);
            }
            return true;
        }
        public bool deleteNode(int id)
        {
            AccountingTree node = db.AccountingTrees.Where(p => p.Id == id).FirstOrDefault();
            if (node != null)
            {
                foreach (AccountingTree child in node.AccountingTree1)
                {
                    db.AccountingTrees.Remove(child);
                }
                db.SaveChanges();
                db.AccountingTrees.Remove(node);
                db.SaveChanges();
            }
            return true;
        }

        public Returner GetAllAccounts()
        {
            return new Returner
            {
                Data = (from A in db.AccountingTrees
                        select A).ToList()
            };
        }

        public Returner SaveWorkOrder(List<CustomerInvoiceLine> CIL, DateTime Date, int ProjID, int Acc, string Notes, double Total, int EditBy)
        {
            var CurrentProject = db.Projects.Where(p => p.Id == ProjID).SingleOrDefault();
            foreach (CustomerInvoiceLine item in CIL)
            {
                var CurrentStock = db.Stocks.Single(p => p.ProjectID == ProjID && p.ProductID == item.ProductId);
                CurrentStock.Quantity -= item.Qty;
                db.SaveChanges();
                StockTransaction ST = new StockTransaction();
                ST.Date = Date;
                ST.StockID = CurrentStock.Id;
                ST.ProductID = item.ProductId;
                ST.Quantity = -item.Qty;
                ST.Type = (int)StockTransactionsTypes.امر_عمل;
                db.StockTransactions.Add(ST);
                db.SaveChanges();
                var CurrentProduct = db.Products.Single(p => p.Id == item.ProductId);
                FinancialTransaction Ft = new FinancialTransaction();
                Ft.Debit = item.Total;
                Ft.Credit = 0;
                Ft.LastEditBy = EditBy;
                Ft.Confirmed = false;
                Ft.Notes = Notes;
                Ft.Statement = "امر شغل " + " بتاريخ " + Date.ToString("MM/dd/yyyy");
                Ft.TransactionDate = Date;
                Ft.FromAccount = CurrentProject.AccountID;
                Ft.ReferanceDocumentNumber = ST.Id;
                db.FinancialTransactions.Add(Ft);
                FinancialTransaction Ft1 = new FinancialTransaction();
                Ft1.Credit = item.Total;
                Ft1.LastEditBy = EditBy;
                Ft1.Confirmed = false;
                Ft1.Notes = Notes;
                Ft1.Statement = "امر شغل " + " بتاريخ " + Date.ToString("MM/dd/yyyy");
                Ft1.TransactionDate = Date;
                Ft1.FromAccount = CurrentProduct.AccountID;
                db.FinancialTransactions.Add(Ft1);
            }
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Work_Order_Saved_Successfully
            };
        }

        public Returner WorkOrderReport(int ProjID, int ProdID, DateTime From, DateTime To)
        {
            var TheStock = db.Stocks.Where(p => p.ProjectID == ProjID && p.ProductID == ProdID).ToList();
            List<StockTransaction> LOST = new List<StockTransaction>();
            foreach (Stock item in TheStock)
            {
                foreach (StockTransaction ST in item.StockTransactions)
                {
                    if (ST.Date >= From && ST.Date <= To && ST.Type == (int)StockTransactionsTypes.امر_عمل)
                    {
                        LOST.Add(ST);
                    }
                }
            }
            var TheStockInJSON = (from S in LOST
                                  select new
                                  {
                                      S.Date,
                                      S.Id,
                                      S.Quantity
                                  });
            return new Returner
            {
                Data = LOST,
                DataInJSON = TheStockInJSON.ToJSON()
            };
        }

        public bool hasChild()
        {
            var AT = db.AccountingTrees.Where(p => p.Parent == this.Id).ToList();
            if (AT.Count != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private List<FinancialTransaction> GetFtChilds(AccountingTree Acc, int ToAcc)
        {
            List<FinancialTransaction> LOFTTT = new List<FinancialTransaction>();
            if (ToAcc == 0)
            {
                LOFTTT = Acc.FinancialTransactions.ToList();
                LOFTTT.AddRange(Acc.FinancialTransactions1);
            }
            else
            {
                LOFTTT = Acc.FinancialTransactions.Where(p => p.FromAccount == ToAcc).ToList();
                LOFTTT.AddRange(Acc.FinancialTransactions1.Where(p => p.FromAccount == ToAcc));
            }
            if (Acc.AccountingTree1.Count > 0)
            {
                foreach (AccountingTree Child in Acc.AccountingTree1)
                {
                    LOFTTT.AddRange(GetFtChilds(Child, ToAcc));
                }
            }
            return LOFTTT.OrderBy(p => p.Id).ToList();
        }
        public Returner FilterStatements(int FromAcc, int ToAcc)
        {
            List<FinancialTransaction> LOFT = new List<FinancialTransaction>();
            var Acc = db.AccountingTrees.Single(p => p.Id == FromAcc);
            LOFT = GetFtChilds(Acc, ToAcc);
            var ResInJson = (from F in LOFT
                             select new
                             {
                                 F.FromAccount,
                                 F.AssociatedAccount,
                                 F.Statement,
                                 F.TransactionDate,
                                 F.Notes,
                                 F.Debit,
                                 F.Credit,
                                 F.ReferanceDocumentNumber,
                                 AccountingTree = new
                                 {
                                     F.AccountingTree.NodeName
                                 },
                                 AccountingTree1 = new
                                 {
                                     F.AccountingTree1.NodeName
                                 }
                             });
            return new Returner
            {
                Data = LOFT,
                DataInJSON = ResInJson.ToJSON()
            };
        }

        public Returner AddFT(FinancialTransaction Ft)
        {
            Ft.Confirmed = false;
            db.FinancialTransactions.Add(Ft);
            db.SaveChanges();
            return new Returner
            {
                Data = Ft.Id,
                Message = Message.Financial_Transaction_Saved_Successfully
            };
        }

        public Returner GetCustomersAccounts()
        {
            var Customers = db.Customers.ToList();
            List<AccountingTree> LOAT = new List<AccountingTree>();
            foreach (Customer item in Customers)
            {
                var AT = db.AccountingTrees.Where(p => p.Id == item.AccountID).SingleOrDefault();
                LOAT.Add(AT);
            }
            return new Returner
            {
                Data = LOAT
            };
        }

        public Returner GetSuppliersAccounts()
        {
            var Suppliers = db.Suppliers.ToList();
            List<AccountingTree> LOAT = new List<AccountingTree>();
            foreach (Supplier item in Suppliers)
            {
                var AT = db.AccountingTrees.Where(p => p.Id == item.AccountingID).SingleOrDefault();
                LOAT.Add(AT);
            }
            return new Returner
            {
                Data = LOAT
            };
        }

        public Returner GetProjectsAccounts()
        {
            var Allproj = db.Projects.ToList();
            List<AccountingTree> LOAT = new List<AccountingTree>();
            var ProjectsUnderConstracting = db.AccountingTrees.Where(p => p.KeyAccountID == (int)KeyAccounts.Projects).SingleOrDefault();
            LOAT.Add(ProjectsUnderConstracting);
            foreach (Project item in Allproj)
            {
                LOAT.Add(item.AccountingTree);
            }
            return new Returner
            {
                Data = LOAT
            };
        }

        public Returner FilterReport(int Acc, DateTime From, DateTime To)
        {
            var ResForNum = db.FinancialTransactions.Where(p => p.FromAccount == Acc).ToList();
            var Res = db.FinancialTransactions.Where(p => p.FromAccount == Acc && (p.TransactionDate >= From && p.TransactionDate <= To)).ToList();
            var ResInJSON = (from F in Res
                             select new
                             {
                                 F.Credit,
                                 F.Debit,
                                 F.Statement,
                                 F.TransactionDate,
                                 DebitSum = ResForNum.Sum(p => p.Debit),
                                 CreditSum = ResForNum.Sum(p => p.Credit),
                             }).ToList();
            return new Returner
            {
                Data = Res,
                DataInJSON = ResInJSON.ToJSON()
            };
        }

        public Returner GetItemCard(int Cat, int Prod, int Proj, DateTime From, DateTime To)
        {
            if (Prod == 0 && Proj == 0)
            {
                if (Cat == 0)
                {
                    var Res = db.StockTransactions.Where(p => p.Date >= From && p.Date <= To).ToList();
                    var ResInJSON = (from F in Res
                                     select new
                                     {
                                         F.Quantity,
                                         F.Date,
                                         F.Id,
                                         F.Type,
                                         Product = new
                                         {
                                             F.Product.ProductName
                                         }
                                     }).ToList();
                    return new Returner
                    {
                        Data = Res,
                        DataInJSON = ResInJSON.ToJSON()
                    };
                }
                else
                {
                    var Res = db.StockTransactions.Where(p => p.Product.ProductCategory.Id == Cat && p.Date >= From && p.Date <= To).ToList();
                    var ResInJSON = (from F in Res
                                     select new
                                     {
                                         F.Quantity,
                                         F.Date,
                                         F.Id,
                                         F.Type,
                                         Product = new
                                         {
                                             F.Product.ProductName
                                         }
                                     }).ToList();
                    return new Returner
                    {
                        Data = Res,
                        DataInJSON = ResInJSON.ToJSON()
                    };
                }
            }
            else if (Prod == 0 && Proj != 0)
            {
                if (Cat == 0)
                {
                    var Res = db.StockTransactions.Where(p => p.Stock.ProjectID == Proj && p.Date >= From && p.Date <= To).ToList();
                    var ResInJSON = (from F in Res
                                     select new
                                     {
                                         F.Quantity,
                                         F.Date,
                                         F.Id,
                                         F.Type,
                                         Product = new
                                         {
                                             F.Product.ProductName
                                         }
                                     }).ToList();
                    return new Returner
                    {
                        Data = Res,
                        DataInJSON = ResInJSON.ToJSON()
                    };
                }
                else
                {
                    var Res = db.StockTransactions.Where(p => p.Product.Category == Cat && p.Stock.ProjectID == Proj && p.Date >= From && p.Date <= To).ToList();
                    var ResInJSON = (from F in Res
                                     select new
                                     {
                                         F.Quantity,
                                         F.Date,
                                         F.Id,
                                         F.Type,
                                         Product = new
                                         {
                                             F.Product.ProductName
                                         }
                                     }).ToList();
                    return new Returner
                    {
                        Data = Res,
                        DataInJSON = ResInJSON.ToJSON()
                    };
                }
            }
            else if (Prod != 0 && Proj == 0)
            {
                var Res = db.StockTransactions.Where(p => p.Stock.ProductID == Prod && p.Date >= From && p.Date <= To).ToList();
                var ResInJSON = (from F in Res
                                 select new
                                 {
                                     F.Quantity,
                                     F.Date,
                                     F.Id,
                                     F.Type,
                                     Product = new
                                     {
                                         F.Product.ProductName
                                     }
                                 }).ToList();
                return new Returner
                {
                    Data = Res,
                    DataInJSON = ResInJSON.ToJSON()
                };
            }
            else
            {
                var Res = db.StockTransactions.Where(p => p.Stock.ProductID == Prod && p.Stock.ProjectID == Proj && p.Date >= From && p.Date <= To).ToList();
                var ResInJSON = (from F in Res
                                 select new
                                 {
                                     F.Quantity,
                                     F.Date,
                                     F.Id,
                                     F.Type,
                                     Product = new
                                     {
                                         F.Product.ProductName
                                     }
                                 }).ToList();
                return new Returner
                {
                    Data = Res,
                    DataInJSON = ResInJSON.ToJSON()
                };
            }
        }

        public Returner CalInventory(int Cat, int Prod, int Proj)
        {
            if (Prod == 0 && Proj == 0)
            {
                if (Cat == 0)
                {
                    var Res = db.Stocks.ToList();
                    var ResInJSON = (from F in Res
                                     select new
                                     {
                                         F.Quantity,
                                         Project = new
                                         {
                                             F.Project.ProjectName
                                         },
                                         Product = new
                                         {
                                             F.Product.ProductName
                                         }
                                     }).ToList();
                    return new Returner
                    {
                        Data = Res,
                        DataInJSON = ResInJSON.ToJSON()
                    };
                }
                else
                {
                    var Res = db.Stocks.Where(p => p.Product.ProductCategory.Id == Cat).ToList();
                    var ResInJSON = (from F in Res
                                     select new
                                     {
                                         F.Quantity,
                                         Project = new
                                         {
                                             F.Project.ProjectName
                                         },
                                         Product = new
                                         {
                                             F.Product.ProductName
                                         }
                                     }).ToList();
                    return new Returner
                    {
                        Data = Res,
                        DataInJSON = ResInJSON.ToJSON()
                    };
                }
            }
            else if (Prod == 0 && Proj != 0)
            {
                if (Cat == 0)
                {
                    var Res = db.Stocks.Where(p => p.ProjectID == Proj).ToList();
                    var ResInJSON = (from F in Res
                                     select new
                                     {
                                         F.Quantity,
                                         Project = new
                                         {
                                             F.Project.ProjectName
                                         },
                                         Product = new
                                         {
                                             F.Product.ProductName
                                         }
                                     }).ToList();
                    return new Returner
                    {
                        Data = Res,
                        DataInJSON = ResInJSON.ToJSON()
                    };
                }
                else
                {
                    var Res = db.Stocks.Where(p => p.Product.Category == Cat && p.ProjectID == Proj).ToList();
                    var ResInJSON = (from F in Res
                                     select new
                                     {
                                         F.Quantity,
                                         Project = new
                                         {
                                             F.Project.ProjectName
                                         },
                                         Product = new
                                         {
                                             F.Product.ProductName
                                         }
                                     }).ToList();
                    return new Returner
                    {
                        Data = Res,
                        DataInJSON = ResInJSON.ToJSON()
                    };
                }
            }
            else if (Prod != 0 && Proj == 0)
            {
                var Res = db.Stocks.Where(p => p.ProductID == Prod).ToList();
                var ResInJSON = (from F in Res
                                 select new
                                 {
                                     F.Quantity,
                                     Project = new
                                     {
                                         F.Project.ProjectName
                                     },
                                     Product = new
                                     {
                                         F.Product.ProductName
                                     }
                                 }).ToList();
                return new Returner
                {
                    Data = Res,
                    DataInJSON = ResInJSON.ToJSON()
                };
            }
            else
            {
                var Res = db.Stocks.Where(p => p.ProductID == Prod && p.ProjectID == Proj).ToList();
                var ResInJSON = (from F in Res
                                 select new
                                 {
                                     F.Quantity,
                                     Project = new
                                     {
                                         F.Project.ProjectName
                                     },
                                     Product = new
                                     {
                                         F.Product.ProductName
                                     }
                                 }).ToList();
                return new Returner
                {
                    Data = Res,
                    DataInJSON = ResInJSON.ToJSON()
                };
            }
        }

        public Returner WorkOrderReport(int Proj, DateTime From, DateTime To)
        {
            var Rep = db.StockTransactions.Where(p => p.Stock.ProjectID == Proj && p.Date >= From && p.Date <= To && p.Type == (int)StockTransactionsTypes.امر_عمل).ToList();
            var RepInJSON = (from S in Rep
                             select new
                             {
                                 S.Date,
                                 S.Quantity,
                                 Product = new
                                 {
                                     S.Product.ProductName
                                 },
                                 Price = db.FinancialTransactions.Where(p => p.ReferanceDocumentNumber == S.Id).FirstOrDefault().Debit
                             }).ToList();
            return new Returner
            {
                Data = Rep,
                DataInJSON = RepInJSON.ToJSON()
            };
        }

        public Returner PayrollReport(int SalaryType, DateTime From, DateTime To)
        {
            var Rep = db.Payrolls.Where(p => p.Date >= From && p.Date <= To && p.Type == 3).ToList();
            DateTime Larger;
            List<CustomPayroll> LOCPR = new List<CustomPayroll>();
            foreach (Payroll PPP in Rep)
            {
                if (SalaryType == 2)
                {
                    Larger = PPP.Date.Value.AddDays(-15);
                }
                else if (SalaryType == 3)
                {
                    Larger = PPP.Date.Value.AddMonths(-1);
                }
                else
                {
                    Larger = PPP.Date.Value.AddDays(-7);
                }
                CustomPayroll CPR = new CustomPayroll();
                CPR.Id = Convert.ToInt32(PPP.EmpID);
                CPR.Date = PPP.Date;
                CPR.Pens = db.Payrolls.Where(p => p.Type == 2 && p.Date <= PPP.Date && p.Date >= Larger).ToList().Sum(p => p.Amount);
                CPR.Benis = db.Payrolls.Where(p => p.Type == 1 && p.Date <= PPP.Date && p.Date >= Larger).ToList().Sum(p => p.Amount);
                LOCPR.Add(CPR);
            }
            if (SalaryType == 2)
            {
                var RepInJSON = (from P in Rep
                                 select new
                                 {
                                     P.Amount,
                                     P.Date,
                                     P.Id,
                                     Pens = LOCPR.Where(p => p.Id == P.EmpID && p.Date == P.Date).SingleOrDefault().Pens,
                                     Benis = LOCPR.Where(p => p.Id == P.EmpID && p.Date == P.Date).SingleOrDefault().Benis,
                                     P.Type,
                                     Employee = new
                                     {
                                         P.Employee.Name,
                                         P.Employee.BasicSalary
                                     }
                                 }).ToList();
                return new Returner
                {
                    Data = Rep,
                    DataInJSON = RepInJSON.ToJSON()
                };
            }
            else if (SalaryType == 3)
            {
                var RepInJSON = (from P in Rep
                                 select new
                                 {
                                     P.Amount,
                                     P.Date,
                                     P.Id,
                                     Pens = LOCPR.Where(p => p.Id == P.EmpID && p.Date == P.Date).SingleOrDefault().Pens,
                                     Benis = LOCPR.Where(p => p.Id == P.EmpID && p.Date == P.Date).SingleOrDefault().Benis,
                                     P.Type,
                                     Employee = new
                                     {
                                         P.Employee.Name,
                                         P.Employee.BasicSalary
                                     }
                                 }).ToList();
                return new Returner
                {
                    Data = Rep,
                    DataInJSON = RepInJSON.ToJSON()
                };
            }
            else
            {
                var RepInJSON = (from P in Rep
                                 select new
                                 {
                                     P.Amount,
                                     P.Date,
                                     P.Id,
                                     Pens = LOCPR.Where(p => p.Id == P.EmpID && p.Date == P.Date).SingleOrDefault().Pens,
                                     Benis = LOCPR.Where(p => p.Id == P.EmpID && p.Date == P.Date).SingleOrDefault().Benis,
                                     P.Type,
                                     Employee = new
                                     {
                                         P.Employee.Name,
                                         P.Employee.BasicSalary
                                     }
                                 }).ToList();
                return new Returner
                {
                    Data = Rep,
                    DataInJSON = RepInJSON.ToJSON()
                };
            }
        }

        public Returner PayslipReport(int EmployeeID, DateTime From, DateTime To)
        {
            var Res = db.Payrolls.Where(p => p.EmpID == EmployeeID && p.Date >= From && p.Date <= To).ToList();
            var ResInJson = (from P in Res
                             select new
                             {
                                 P.Amount,
                                 P.Date,
                                 P.Type,
                                 Employee = new
                                 {
                                     P.Employee.BasicSalary,
                                     P.Employee.SalaryType
                                 }
                             }).ToList();
            return new Returner
            {
                Data = Res,
                DataInJSON = ResInJson.ToJSON()
            };
        }

        public Returner GetSalesByMonth(int I, List<FinancialTransaction> TotalSales)
        {
            DateTime Start = new DateTime(DateTime.Now.Year, I, 1);
            DateTime End;
            if (I == 12)
            {
                End = new DateTime(DateTime.Now.Year, I, 31);
            }
            else
            {
                End = new DateTime(DateTime.Now.Year, (I + 1), 1);
            }
            double? Val = TotalSales.Where(p => p.TransactionDate >= Start && p.TransactionDate <= End).ToList().Sum(p => p.Debit);
            return new Returner
            {
                Data = Val != null ? Val : 0
            };
        }

        public Returner GetAllInstallmentsByMonth(int I, List<Installment> AllIns)
        {
            DateTime Start = new DateTime(DateTime.Now.Year, I, 1);
            DateTime End;
            if (I == 12)
            {
                End = new DateTime(DateTime.Now.Year, I, 31);
            }
            else
            {
                End = new DateTime(DateTime.Now.Year, (I + 1), 1);
            }
            double? Val = AllIns.Where(p => p.DueDate >= Start && p.DueDate <= End).ToList().Sum(p => p.Amount);
            return new Returner
            {
                Data = Val != null ? Val : 0
            };
        }

        public Returner GetTotalSalesFtToSave()
        {
            var ParentID = db.AccountingTrees.SingleOrDefault(p => p.KeyAccountID == (int)KeyAccounts.Sales).Id;
            var TotalSales = db.FinancialTransactions.Where(p => p.AccountingTree.Id == ParentID).ToList();
            return new Returner
            {
                Data = TotalSales
            };
        }

        public Returner GetPaidInstallmentsFtToSave()
        {
            var allInstallments = db.Installments.Where(p => p.Customer.ID == 1055).ToList();
            List<FinancialTransaction> LOFT = new List<FinancialTransaction>();
            foreach (Installment item in allInstallments)
            {
                var Ft = db.FinancialTransactions.Where(p => p.FromAccount == item.Customer.AccountID && p.ReferanceDocumentNumber == item.Id).ToList();
                LOFT.AddRange(Ft);
            }
            return new Returner
            {
                Data = LOFT
            };
        }

        public Returner GetAllInstallmentsFtToSave()
        {
            var AllIns = db.Installments.ToList();
            return new Returner
            {
                Data = AllIns
            };
        }

        public Returner GetPaidInstallmentsByMonth(int I, List<FinancialTransaction> LOFT)
        {
            DateTime Start = new DateTime(DateTime.Now.Year, I, 1);
            DateTime End;
            if (I == 12)
            {
                End = new DateTime(DateTime.Now.Year, I, 31);
            }
            else
            {
                End = new DateTime(DateTime.Now.Year, (I + 1), 1);
            }
            var ToGetFrom = LOFT.Where(p => p.TransactionDate >= Start && p.TransactionDate <= End).ToList();
            if (ToGetFrom.Count > 0)
	        {
                double? Val = ToGetFrom.Sum(p => p.Credit);
                return new Returner
                {
                    Data = Val != null ? Val : 0
                };
            }
            else
            {
                return new Returner
                {
                    Data = 0
                };
            }
        }

        public Returner GetSalesFlotData(List<FinancialTransaction> LOFT)
        {
            double[,] FlotData = new double[,]
            { 
                {1,Convert.ToDouble(GetSalesByMonth(1,LOFT).Data)},
                {2,Convert.ToDouble(GetSalesByMonth(2,LOFT).Data)},
                {3,Convert.ToDouble(GetSalesByMonth(3,LOFT).Data)},
                {4,Convert.ToDouble(GetSalesByMonth(4,LOFT).Data)},
                {5,Convert.ToDouble(GetSalesByMonth(5,LOFT).Data)},
                {6,Convert.ToDouble(GetSalesByMonth(6,LOFT).Data)},
                {7,Convert.ToDouble(GetSalesByMonth(7,LOFT).Data)},
                {8,Convert.ToDouble(GetSalesByMonth(8,LOFT).Data)},
                {9,Convert.ToDouble(GetSalesByMonth(9,LOFT).Data)},
                {10,Convert.ToDouble(GetSalesByMonth(10,LOFT).Data)},
                {11,Convert.ToDouble(GetSalesByMonth(11,LOFT).Data)},
                {12,Convert.ToDouble(GetSalesByMonth(12,LOFT).Data)}
            };
            return new Returner
            {
                DataInJSON = FlotData.ToJSON()
            };
        }

        public Returner GetAllInstallmentsFlotData(List<Installment> AllIns)
        {
            double[,] FlotData = new double[,]
            { 
                {1,Convert.ToDouble(GetAllInstallmentsByMonth(1,AllIns).Data)},
                {2,Convert.ToDouble(GetAllInstallmentsByMonth(2,AllIns).Data)},
                {3,Convert.ToDouble(GetAllInstallmentsByMonth(3,AllIns).Data)},
                {4,Convert.ToDouble(GetAllInstallmentsByMonth(4,AllIns).Data)},
                {5,Convert.ToDouble(GetAllInstallmentsByMonth(5,AllIns).Data)},
                {6,Convert.ToDouble(GetAllInstallmentsByMonth(6,AllIns).Data)},
                {7,Convert.ToDouble(GetAllInstallmentsByMonth(7,AllIns).Data)},
                {8,Convert.ToDouble(GetAllInstallmentsByMonth(8,AllIns).Data)},
                {9,Convert.ToDouble(GetAllInstallmentsByMonth(9,AllIns).Data)},
                {10,Convert.ToDouble(GetAllInstallmentsByMonth(10,AllIns).Data)},
                {11,Convert.ToDouble(GetAllInstallmentsByMonth(11,AllIns).Data)},
                {12,Convert.ToDouble(GetAllInstallmentsByMonth(12,AllIns).Data)}
            };
            return new Returner
            {
                DataInJSON = FlotData.ToJSON()
            };
        }

        public Returner GetAPaidInstallmentsFlotData(List<FinancialTransaction> LOFT)
        {
            double[,] FlotData = new double[,]
            {
                {1,Convert.ToDouble(GetPaidInstallmentsByMonth(1,LOFT).Data)},
                {2,Convert.ToDouble(GetPaidInstallmentsByMonth(2,LOFT).Data)},
                {3,Convert.ToDouble(GetPaidInstallmentsByMonth(3,LOFT).Data)},
                {4,Convert.ToDouble(GetPaidInstallmentsByMonth(4,LOFT).Data)},
                {5,Convert.ToDouble(GetPaidInstallmentsByMonth(5,LOFT).Data)},
                {6,Convert.ToDouble(GetPaidInstallmentsByMonth(6,LOFT).Data)},
                {7,Convert.ToDouble(GetPaidInstallmentsByMonth(7,LOFT).Data)},
                {8,Convert.ToDouble(GetPaidInstallmentsByMonth(8,LOFT).Data)},
                {9,Convert.ToDouble(GetPaidInstallmentsByMonth(9,LOFT).Data)},
                {10,Convert.ToDouble(GetPaidInstallmentsByMonth(10,LOFT).Data)},
                {11,Convert.ToDouble(GetPaidInstallmentsByMonth(11,LOFT).Data)},
                {12,Convert.ToDouble(GetPaidInstallmentsByMonth(12,LOFT).Data)}
            };
            return new Returner
            {
                DataInJSON = FlotData.ToJSON()
            };
        }

        public class CustomPayroll
        {
            public int Id { get; set; }
            public DateTime? Date { get; set; }
            public double? Pens { get; set; }
            public double? Benis { get; set; }
        }
    }
}