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

        public Returner SaveWorkOrder(List<CustomerInvoiceLine> CIL,DateTime Date,int ProjID , int Acc,string Notes,double Total,int EditBy)
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
            }
            db.SaveChanges();
            FinancialTransaction Ft = new FinancialTransaction();
            Ft.Amount = Total;
            Ft.LastEditBy = EditBy;
            Ft.Notes = Notes;
            Ft.Statement = "";
            Ft.TransactionDate = Date;
            Ft.FromAccount = CurrentProject.AccountID;
            Ft.ToAccount = Acc;
            db.FinancialTransactions.Add(Ft);
            db.SaveChanges();
            var CurrentFt = db.FinancialTransactions.Where(p => p.Id == Ft.Id).SingleOrDefault();
            CurrentFt.Statement = "امر شغل رقم " + CurrentFt.Id + " بتاريخ " + Date.ToString("MM/dd/yyyy");
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Work_Order_Saved_Successfully
            };
        }
    }
}