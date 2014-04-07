using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icons.Models
{
    public partial class CustomerInvoiceLine
    {
        #region Context
        MaksoudDBEntities db = new MaksoudDBEntities();
        #endregion

        public Returner Add()
        {
            db.CustomerInvoiceLines.Add(this);
            db.SaveChanges();
            var LastInvoiceLine = db.CustomerInvoiceLines.Where(p => p.Id == this.Id).ToList();
            CustomerInvoiceLine SingleIL = LastInvoiceLine.SingleOrDefault();
            var prod = db.Products.Where(p => p.Id == SingleIL.ProductId).SingleOrDefault();
            LastInvoiceLine.FirstOrDefault().Product = prod;
            var LastInvoiceLineInJSON = (from IL in LastInvoiceLine
                                         select new
                                         {
                                             IL.Id,
                                             IL.Price,
                                             IL.Qty,
                                             IL.Total,
                                             Product = new
                                             {
                                                 IL.Product.ProductName,
                                                 IL.Product.Id
                                             }
                                         }).ToList().SingleOrDefault();
            return new Returner
            {
                Data = LastInvoiceLine.SingleOrDefault(),
                DataInJSON = LastInvoiceLineInJSON.ToJSON()
            };
        }

        public Returner Remove()
        {
            var ILToRemove = db.CustomerInvoiceLines.Where(p => p.Id == this.Id).SingleOrDefault();
            var ILParent = db.CustomerInvoices.Single(p => p.Id == ILToRemove.InvoiceId);
            StockTransaction ST = new StockTransaction();
            ST.Date = DateTime.Now;
            ST.ProductID = ILToRemove.ProductId;
            ST.Quantity = ILToRemove.Qty;
            ST.StockID = db.Stocks.Single(p => p.ProductID == ILToRemove.ProductId && p.ProjectID == ILParent.ProjectID).Id;
            ST.Type = (int)StockTransactionsTypes.تعديل_فاتورة_بيع;
            db.StockTransactions.Add(ST);
            var StockToEdit = db.Stocks.Single(p => p.ProductID == ILToRemove.ProductId && p.ProjectID == ILParent.ProjectID);
            StockToEdit.Quantity += ST.Quantity;
            ILParent.InvoiceTotal -= ILToRemove.Total;
            ILParent.InvoiceNet = ILParent.InvoiceTotal - ILParent.InvoiceDiscount;
            db.CustomerInvoiceLines.Remove(ILToRemove);
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Invoice_Line_Removed_Successfully
            };
        }

        public Returner GetByInvoiceID()
        {
            var Lines = db.CustomerInvoiceLines.Where(p => p.InvoiceId == this.InvoiceId).ToList();
            return new Returner
            {
                Data = Lines
            };
        }
    }
}