using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icons.Models
{
    public partial class ProductCategory
    {
        #region Context
        MaksoudDBEntities db = new MaksoudDBEntities();
        #endregion

        public Returner Create()
        {
            var ParentAccount = db.AccountingTrees.Where(p => p.KeyAccountID == (int)KeyAccounts.Stock).FirstOrDefault();
            if (ParentAccount != null)
            {
                AccountingTree ProjectNode = new AccountingTree();
                ProjectNode.NodeName = this.CategoryName;
                ProjectNode.Parent = ParentAccount.Id;
                db.AccountingTrees.Add(ProjectNode);
                db.SaveChanges();
            }
            var AccID = db.AccountingTrees.Where(p => p.NodeName == this.CategoryName && p.Parent == ParentAccount.Id).SingleOrDefault();
            this.AccTreeCode = AccID.Id;
            db.ProductCategories.Add(this);
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Product_Category_Created_Successfully
            };
        }

        public Returner GetAll()
        {
            return new Returner
            {
                Data = (from PC in db.ProductCategories
                        select PC).ToList()
            };
        }

        public Returner GetByID()
        {
            return new Returner
            {
                Data = (from PC in db.ProductCategories
                        where PC.Id == this.Id
                        select PC).ToList().SingleOrDefault()
            };
        }

        public Returner Edit()
        {
            var CatToEdit = (from PC in db.ProductCategories
                             where PC.Id == this.Id
                             select PC).ToList().SingleOrDefault();
            CatToEdit.CategoryName = this.CategoryName;
            db.SaveChanges();
            var AccToEdit = db.AccountingTrees.Where(p => p.Id == CatToEdit.AccTreeCode).SingleOrDefault();
            AccToEdit.NodeName = this.CategoryName;
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Product_Category_Updated_Successfully
            };
        }

        public Returner Remove()
        {
            var CatToDelete = db.ProductCategories.Where(p => p.Id == this.Id).SingleOrDefault();
            db.ProductCategories.Remove(CatToDelete);
            db.SaveChanges();
            var AccToDelete = db.AccountingTrees.Where(p => p.Id == CatToDelete.AccTreeCode).SingleOrDefault();
            db.AccountingTrees.Remove(AccToDelete);
            db.SaveChanges();
            return new Returner 
            {
                Message = Models.Message.Product_Category_Deleted_Successfully
            };
        }
    }
}