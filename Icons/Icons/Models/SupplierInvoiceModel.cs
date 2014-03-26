using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icons.Models
{
    public partial class SupplierInvoice
    {
        #region Context
        MaksoudDBEntities db = new MaksoudDBEntities();
        #endregion    

        public Returner Add(List<SupplierInvoiceLine> LOSIL)
        {
            db.SupplierInvoices.Add(this);
            db.SaveChanges();
            foreach (SupplierInvoiceLine item in LOSIL)
            {
                item.InvoiceId = this.Id;
                db.SupplierInvoiceLines.Add(item);
                db.SaveChanges();
            }
            new Stock().Purchases(this.Id);
            return new Returner
            {
                Message = Message.Invoice_Added_Successfully
            };
        }

        public Returner InvoiceNumber()
        {
            int InvoicesCount = (from SI in db.SupplierInvoices
                                 select SI).ToList().Count;
            int MaxID;
            if (InvoicesCount != 0)
            {
                MaxID = db.SupplierInvoices.Max(p => p.Id);
            }
            else
            {
                MaxID = 0;
            }
            return new Returner
            {
                Data = MaxID
            };
        }

        public Returner GetAllInvoices()
        {
            return new Returner
            {
                Data = (from I in db.SupplierInvoices
                            select I).ToList()
            };
        }

        public Returner DeleteInvoice()
        {
            var ITD = db.SupplierInvoices.Single(p => p.Id == this.Id);
            db.SupplierInvoices.Remove(ITD);
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Invoice_Deleted_Successfully
            };
        }

        public Returner Depart(int EditBy)
        {
            var ITD = db.SupplierInvoices.Where(p => p.Id == this.Id).SingleOrDefault();
            ITD.Departed = true;
            db.SaveChanges();
            FinancialTransaction Ft = new FinancialTransaction();
            Ft.Amount = ITD.InvoiceNet;
            Ft.FromAccount = ITD.Supplier.AccountingID;
            Ft.LastEditBy = EditBy;
            Ft.Notes = "";
            Ft.Statement = "ترحيل فاتورة شراء";
            Ft.ToAccount = ITD.InvoiceAccount;
            Ft.TransactionDate = DateTime.Now;
            db.FinancialTransactions.Add(Ft);
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Invoice_Departed_Successfully
            };
        }
    }
}