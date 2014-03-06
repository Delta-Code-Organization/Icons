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
        public bool UpdateTree(List<TreeNode> _Tree,TreeNode Parent = null)
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
                    UpdateTree(node.children,node);
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
    }
}