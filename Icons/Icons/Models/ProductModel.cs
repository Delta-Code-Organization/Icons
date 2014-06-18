using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icons.Models
{
    public partial class Product
    {
        #region Context
        MaksoudDBEntities db = new MaksoudDBEntities();
        #endregion

        public Returner Create()
        {
            if (db.Products.Any(p => p.ProductName == this.ProductName))
            {
                return new Returner
                {
                    Message = Message.Product_Name_Already_Exist
                };
            }
            var Cat = db.ProductCategories.Where(p => p.Id == this.Category).SingleOrDefault();
            if (Cat != null)
            {
                AccountingTree ProjectNode = new AccountingTree();
                ProjectNode.NodeName = this.ProductName;
                ProjectNode.Parent = Cat.AccTreeCode;
                db.AccountingTrees.Add(ProjectNode);
                db.SaveChanges();
            }
            var AccID = db.AccountingTrees.Where(p => p.NodeName == this.ProductName && p.Parent == Cat.AccTreeCode).SingleOrDefault();
            this.AccountID = AccID.Id;
            db.Products.Add(this);
            db.SaveChanges();
            new Stock
            {
                ProductID = this.Id
            }.NewProduct();
            return new Returner
            {
                Message = Message.Product_Created_Successfully
            };
        }

        public Returner GetAll()
        {
            return new Returner
            {
                Data = (from P in db.Products
                        select P).ToList()
            };
        }

        public Returner GetByID()
        {
            return new Returner
            {
                Data = (from P in db.Products
                        where P.Id == this.Id
                        select P).ToList().SingleOrDefault()
            };
        }

        public Returner Edit()
        {
            var ProdToEdit = db.Products.Where(p => p.Id == this.Id).SingleOrDefault();
            bool CatNotChanged = (ProdToEdit.Category == this.Category);
            int NewParent;
            var AccToEdit = db.AccountingTrees.Where(p => p.Id == ProdToEdit.AccountID).SingleOrDefault();
            if (CatNotChanged)
            {
                AccToEdit.NodeName = this.ProductName;
                db.SaveChanges();
                NewParent = (int)AccToEdit.Parent;
            }
            else
            {
                db.AccountingTrees.Remove(AccToEdit);
                db.SaveChanges();
                var Cat = db.ProductCategories.Where(p => p.Id == this.Category).SingleOrDefault();
                if (Cat != null)
                {
                    AccountingTree ProjectNode = new AccountingTree();
                    ProjectNode.NodeName = this.ProductName;
                    ProjectNode.Parent = Cat.AccTreeCode;
                    db.AccountingTrees.Add(ProjectNode);
                    db.SaveChanges();
                }
                NewParent = (int)Cat.AccTreeCode;
            }
            var AccID = db.AccountingTrees.Where(p => p.NodeName == this.ProductName && p.Parent == NewParent).SingleOrDefault();
            ProdToEdit.AccountID = AccID.Id;
            ProdToEdit.Category = this.Category;
            ProdToEdit.Description = this.Description;
            ProdToEdit.ProductName = this.ProductName;
            ProdToEdit.PtoSRate = this.PtoSRate;
            ProdToEdit.PurchaseUnit = this.PurchaseUnit;
            ProdToEdit.SalesUnit = this.SalesUnit;
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Product_Update_Successfully
            };
        }

        public Returner Remove()
        {
            var ProdToDelete = db.Products.Where(p => p.Id == this.Id).SingleOrDefault();
            if (db.Stocks.Any(p => p.ProductID == ProdToDelete.Id && p.Quantity > 0))
            {
                return new Returner
                {
                    Message = Message.Cannot_Delete_This_Product
                };
            }
            else
            {
                db.Database.ExecuteSqlCommand("delete from Stock where ProductID = {0}", ProdToDelete.Id);
            }
            db.SaveChanges();
            var AccToDelete = db.AccountingTrees.Where(p => p.Id == ProdToDelete.AccountID).SingleOrDefault();
            db.Products.Remove(ProdToDelete);
            db.SaveChanges();
            db.AccountingTrees.Remove(AccToDelete);
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Product_Deleted_Successfully
            };
        }

        public Returner GetLastProductPrice()
        {
            double ProdPrice = Convert.ToDouble(db.SupplierInvoiceLines.Where(p => p.ProductId == this.Id).OrderByDescending(p => p.Id).FirstOrDefault().Price);
            return new Returner
            {
                Data = ProdPrice
            };
        }

        public Returner GetByCat()
        {
            var Prods = db.Products.Where(p => p.Category == this.Category).ToList();
            var ProdsInJson = (from P in Prods
                               select new
                               {
                                   P.ProductName,
                                   P.Id
                               }).ToList();
            return new Returner
            {
                Data = Prods,
                DataInJSON = ProdsInJson.ToJSON()
            };
        }
    }
}