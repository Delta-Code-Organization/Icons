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

        internal List<TreeNode> Children
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

        public bool UpdateTree(List<TreeNode> _Tree)
        {
            
            foreach (TreeNode node in _Tree)
            {
                AccountingTree TreeNode = null;
                if (db.AccountingTrees.Where(p => p.Id == node.id).Count() == 1)
                {
                    TreeNode = db.AccountingTrees.Where(p => p.Id == node.id).First();
                    TreeNode.Id = node.id;
                    TreeNode.NodeName = node.name;
                }
                else
                {
                    TreeNode = new AccountingTree();
                    TreeNode.NodeName = node.name;
                    db.AccountingTrees.Add(TreeNode);
                    db.SaveChanges();
                }
                foreach(TreeNode child in node.Children)
                {
                    AccountingTree childNode = null ;
                    if (db.AccountingTrees.Where(p => p.Id == child.id).Count() == 1)
                    {
                        childNode = db.AccountingTrees.Where(p => p.Id == child.id).First();
                        childNode.Id = child.id;
                        childNode.Parent = node.id;
                        childNode.NodeName = child.name;
                    }
                    else
                    {
                        childNode = new AccountingTree();
                        childNode.NodeName = child.name;
                        db.AccountingTrees.Add(childNode);
                        db.SaveChanges();
                    }
                    TreeNode.AccountingTree1.Add(childNode);
                }
                db.SaveChanges();
                
            }
            return true;            
        }  
    }
}