using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icons.Models
{
    public partial class CustomerInvoice
    {
        #region Context
        MaksoudDBEntities db = new MaksoudDBEntities();
        #endregion

        public Returner Add(List<CustomerInvoiceLine> LOSIL)
        {
            db.CustomerInvoices.Add(this);
            db.SaveChanges();
            foreach (CustomerInvoiceLine item in LOSIL)
            {
                item.InvoiceId = this.Id;
                db.CustomerInvoiceLines.Add(item);
                db.SaveChanges();
            }
            new Stock().Sells(this.Id);
            return new Returner
            {
                Message = Message.Invoice_Added_Successfully
            };
        }

        public Returner InvoiceNumber()
        {
            int AllInvoicesCount = (from CI in db.CustomerInvoices
                                    select CI).ToList().Count;
            int MaxID;
            if (AllInvoicesCount != 0)
            {
                MaxID = db.CustomerInvoices.Max(p => p.Id);
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
                Data = (from CI in db.CustomerInvoices
                        select CI).ToList()
            };
        }

        public Returner DeleteInvoice()
        {
            var ITD = db.CustomerInvoices.Where(p => p.Id == this.Id).SingleOrDefault();
            db.CustomerInvoices.Remove(ITD);
            db.SaveChanges();

            return new Returner
            {
                Message = Message.Invoice_Deleted_Successfully
            };
        }

        public Returner Depart(int EditBy)
        {
            var ITD = db.CustomerInvoices.Where(p => p.Id == this.Id).SingleOrDefault();
            ITD.Departed = true;
            db.SaveChanges();
            FinancialTransaction Ft = new FinancialTransaction();
            Ft.Amount = ITD.InvoiceNet;
            Ft.FromAccount = ITD.InvoiceAccount;
            Ft.LastEditBy = EditBy;
            Ft.Notes = "";
            Ft.Statement = "ترحيل فاتورة بيع";
            Ft.ToAccount = ITD.Customer.AccountID;
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