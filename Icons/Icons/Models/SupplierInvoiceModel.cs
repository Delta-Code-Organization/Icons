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
            var InvLines = db.SupplierInvoiceLines.Where(p => p.InvoiceId == ITD.Id).ToList();
            List<SupplierInvoiceLine> LOCIL = new List<SupplierInvoiceLine>();
            LOCIL = InvLines;
            foreach (SupplierInvoiceLine CIL in LOCIL)
            {
                Stock S = db.Stocks.Single(p => p.ProjectID == ITD.ProjectID && p.ProductID == CIL.ProductId);
                S.Quantity -= CIL.Qty;
                StockTransaction ST = new StockTransaction();
                ST.Date = DateTime.Now;
                ST.ProductID = CIL.ProductId;
                ST.Quantity = -CIL.Qty;
                ST.StockID = S.Id;
                ST.Type = (int)StockTransactionsTypes.حذف_فاتورة_شراء;
                db.StockTransactions.Add(ST);
                db.SaveChanges();
                db.SupplierInvoiceLines.Remove(CIL);
            }
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

        public Returner GetByID()
        {
            return new Returner
            {
                Data = db.SupplierInvoices.Single(p => p.Id == this.Id)
            };
        }

        public Returner Edit(List<SupplierInvoiceLine> LOSIL)
        {
            var Sup = db.SupplierInvoices.Single(p => p.Id == this.Id);
            Sup.Departed = this.Departed;
            Sup.InvoiceAccount = this.InvoiceAccount;
            Sup.InvoiceDate = this.InvoiceDate;
            Sup.InvoiceDiscount = this.InvoiceDiscount;
            Sup.InvoiceNet = this.InvoiceNet;
            Sup.InvoiceTotal = this.InvoiceTotal;
            Sup.LastEditBy = this.LastEditBy;
            Sup.ProjectID = this.ProjectID;
            Sup.SupplierID = this.SupplierID;
            Sup.SupplierReferenaceNo = this.SupplierReferenaceNo;
            db.SaveChanges();
            foreach (SupplierInvoiceLine item in LOSIL)
            {
                if (db.SupplierInvoiceLines.Any(p => p.Id == item.Id))
                {
                    continue;
                }
                else
                {
                    item.InvoiceId = this.Id;
                    db.SupplierInvoiceLines.Add(item);
                    StockTransaction ST = new StockTransaction();
                    ST.Date = DateTime.Now;
                    ST.ProductID = item.ProductId;
                    ST.Quantity = item.Qty;
                    ST.StockID = db.Stocks.Single(p => p.ProductID == item.ProductId && p.ProjectID == Sup.ProjectID).Id;
                    ST.Type = (int)StockTransactionsTypes.تعديل_فاتورة_شراء;
                    db.StockTransactions.Add(ST);
                    var StockToEdit = db.Stocks.Single(p => p.ProductID == item.ProductId && p.ProjectID == Sup.ProjectID);
                    StockToEdit.Quantity += ST.Quantity;
                    db.SaveChanges();
                }
            }
            //new Stock().Purchases(this.Id);
            return new Returner
            {
                Message = Message.Invoice_Added_Successfully
            };
        }
    }
}