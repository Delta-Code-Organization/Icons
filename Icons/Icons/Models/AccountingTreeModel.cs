using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
    }
}