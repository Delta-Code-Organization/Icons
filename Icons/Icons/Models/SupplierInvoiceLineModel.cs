using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icons.Models
{
    public partial class SupplierInvoiceLine
    {
        #region Context
        MaksoudDBEntities db = new MaksoudDBEntities();
        #endregion

        public Returner Add()
        {
            db.SupplierInvoiceLines.Add(this);
            db.SaveChanges();
            var LastInvoiceLine = db.SupplierInvoiceLines.Where(p => p.Id == this.Id).ToList();
            SupplierInvoiceLine SingleIL = LastInvoiceLine.SingleOrDefault();
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
                                                 IL.Product.ProductName
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
            var ILToRemove = db.SupplierInvoiceLines.Where(p => p.Id == this.Id).SingleOrDefault();
            var ILParent = db.SupplierInvoices.Single(p => p.Id == ILToRemove.InvoiceId);
            StockTransaction ST = new StockTransaction();
            ST.Date = DateTime.UtcNow.AddHours(3);
            ST.ProductID = ILToRemove.ProductId;
            ST.Quantity = -ILToRemove.Qty;
            ST.StockID = db.Stocks.Single(p => p.ProductID == ILToRemove.ProductId && p.ProjectID == ILParent.ProjectID).Id;
            ST.Type = (int)StockTransactionsTypes.تعديل_فاتورة_شراء;
            db.StockTransactions.Add(ST);
            var StockToEdit = db.Stocks.Single(p => p.ProductID == ILToRemove.ProductId && p.ProjectID == ILParent.ProjectID);
            StockToEdit.Quantity += ST.Quantity;
            ILParent.InvoiceTotal -= ILToRemove.Total;
            ILParent.InvoiceNet = ILParent.InvoiceTotal - ILParent.InvoiceDiscount;
            db.SupplierInvoiceLines.Remove(ILToRemove);
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Invoice_Line_Removed_Successfully
            };
        }

        public Returner GetByInvoiceID()
        {
            var Lines = db.SupplierInvoiceLines.Where(p => p.InvoiceId == this.InvoiceId).ToList();
            var LinesInJSON = (from L in Lines
                               select new
                               {
                                   L.Price,
                                   L.Qty,
                                   L.Total,
                                   Product = new
                                   {
                                       L.Product.ProductName
                                   }
                               }).ToList();
            return new Returner
            {
                Data = Lines,
                DataInJSON = LinesInJSON.ToJSON()
            };
        }
    }
}