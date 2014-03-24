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