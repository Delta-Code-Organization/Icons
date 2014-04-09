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
            var InvLines = db.CustomerInvoiceLines.Where(p => p.InvoiceId == ITD.Id).ToList();
            List<CustomerInvoiceLine> LOCIL = new List<CustomerInvoiceLine>();
            LOCIL = InvLines;
            foreach (CustomerInvoiceLine CIL in LOCIL)
            {
                Stock S = db.Stocks.Single(p => p.ProjectID == ITD.ProjectID && p.ProductID == CIL.ProductId);
                S.Quantity += CIL.Qty;
                StockTransaction ST = new StockTransaction();
                ST.Date = DateTime.Now;
                ST.ProductID = CIL.ProductId;
                ST.Quantity = CIL.Qty;
                ST.StockID = S.Id;
                ST.Type = (int)StockTransactionsTypes.حذف_فاتورة_بيع;
                db.StockTransactions.Add(ST);
                db.SaveChanges();
                db.CustomerInvoiceLines.Remove(CIL);
            }
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

        public Returner GetByID()
        {
            return new Returner
            {
                Data = db.CustomerInvoices.Where(p => p.Id == this.Id).SingleOrDefault()
            };
        }

        public Returner Edit(List<CustomerInvoiceLine> LOSIL)
        {
            var Inv = db.CustomerInvoices.Where(p => p.Id == this.Id).SingleOrDefault();
            Inv.CustomerID = this.CustomerID;
            Inv.Departed = this.Departed;
            Inv.InvoiceAccount = this.InvoiceAccount;
            Inv.InvoiceDate = this.InvoiceDate;
            Inv.InvoiceDiscount = this.InvoiceDiscount;
            Inv.InvoiceNet = this.InvoiceNet;
            Inv.InvoiceTotal = this.InvoiceTotal;
            Inv.LastEditBy = this.LastEditBy;
            Inv.ProjectID = this.ProjectID;
            db.SaveChanges();
            foreach (CustomerInvoiceLine item in LOSIL)
            {
                if (db.CustomerInvoiceLines.Any(p => p.Id == item.Id))
                {
                    continue;
                }
                else
                {
                    item.InvoiceId = this.Id;
                    db.CustomerInvoiceLines.Add(item);
                    StockTransaction ST = new StockTransaction();
                    ST.Date = DateTime.Now;
                    ST.ProductID = item.ProductId;
                    ST.Quantity = -item.Qty;
                    ST.StockID = db.Stocks.Single(p => p.ProductID == item.ProductId && p.ProjectID == Inv.ProjectID).Id;
                    ST.Type = (int)StockTransactionsTypes.تعديل_فاتورة_بيع;
                    db.StockTransactions.Add(ST);
                    var StockToEdit = db.Stocks.Single(p => p.ProductID == item.ProductId && p.ProjectID == Inv.ProjectID);
                    StockToEdit.Quantity += ST.Quantity;
                    db.SaveChanges();
                }
            }
            //new Stock().Sells(this.Id);
            return new Returner
            {
                Message = Message.Invoice_Added_Successfully
            };
        }
    }
}