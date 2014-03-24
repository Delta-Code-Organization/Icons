using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icons.Models
{
    public partial class Stock
    {
        #region Context
        MaksoudDBEntities db = new MaksoudDBEntities();
        #endregion

        public Returner NewProject()
        {
            Product P = new Product();
            List<Product> LOP = P.GetAll().Data as List<Product>;
            foreach (Product item in LOP)
            {
                Stock S = new Stock
                {
                    ProjectID = this.ProjectID,
                    ProductID = item.Id,
                    Quantity = 0
                };
                db.Stocks.Add(S);
            }
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Stock_Created_Successfully
            };
        }

        public Returner NewProduct()
        {
            Project P = new Project();
            List<Project> LOP = P.GetAll().Data as List<Project>;
            foreach (Project item in LOP)
            {
                Stock S = new Stock
                {
                    ProjectID = item.Id,
                    ProductID = this.ProductID,
                    Quantity = 0
                };
            }
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Stock_Created_Successfully
            };
        }

        public Returner Purchases(int InvoiceID)
        {
            int ProjID = (int)(db.SupplierInvoices.Where(p => p.Id == InvoiceID).SingleOrDefault().ProjectID);
            List<SupplierInvoiceLine> LOSIL = new List<SupplierInvoiceLine>();
            LOSIL = new SupplierInvoiceLine { InvoiceId = InvoiceID }.GetByInvoiceID().Data as List<SupplierInvoiceLine>;
            foreach (SupplierInvoiceLine item in LOSIL)
            {
                var StockToUpdate = db.Stocks.Where(p => p.ProjectID == ProjID && p.ProductID == item.ProductId).SingleOrDefault();
                StockToUpdate.Quantity += item.Qty;
                db.SaveChanges();
                StockTransaction ST = new StockTransaction
                {
                    Date = DateTime.Now,
                    ProductID = StockToUpdate.ProductID,
                    Quantity = item.Qty,
                    StockID = StockToUpdate.Id,
                    Type = (int)StockTransactionsTypes.Purchase
                };
                db.StockTransactions.Add(ST);
                db.SaveChanges();
            }
            return new Returner
            {
                Message = Message.Purchase_Operation_Finished_Successfully
            };
        }

        public Returner Sells(int InvoiceID)
        {
            int ProjID = (int)(db.CustomerInvoices.Where(p => p.Id == InvoiceID).SingleOrDefault().ProjectID);
            List<CustomerInvoiceLine> LOSIL = new List<CustomerInvoiceLine>();
            LOSIL = new CustomerInvoiceLine { InvoiceId = InvoiceID }.GetByInvoiceID().Data as List<CustomerInvoiceLine>;
            foreach (CustomerInvoiceLine item in LOSIL)
            {
                var StockToUpdate = db.Stocks.Where(p => p.ProjectID == ProjID && p.ProductID == item.ProductId).SingleOrDefault();
                StockToUpdate.Quantity -= item.Qty;
                db.SaveChanges();
                StockTransaction ST = new StockTransaction
                {
                    Date = DateTime.Now,
                    ProductID = StockToUpdate.ProductID,
                    Quantity = -item.Qty,
                    StockID = StockToUpdate.Id,
                    Type = (int)StockTransactionsTypes.Sell
                };
                db.StockTransactions.Add(ST);
                db.SaveChanges();
            }
            return new Returner
            {
                Message = Message.Sell_Operation_Finished_Successfully
            };
        }
    }
}