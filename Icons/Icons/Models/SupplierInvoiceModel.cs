﻿using System;
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
            FinancialTransaction Ft = new FinancialTransaction();
            Ft.Confirmed = false;
            Ft.Debit = this.InvoiceNet;
            Ft.Credit = 0;
            Ft.FromAccount = 52;//purchases account
            Ft.Notes = "";
            Ft.ReferanceDocumentNumber = this.Id;
            Ft.Statement = "تسجيل فاتورة شراء للعميل " + this.Supplier.Name;
            Ft.TransactionDate = DateTime.Now;
            db.FinancialTransactions.Add(Ft);
            db.SaveChanges();
            FinancialTransaction Ft1 = new FinancialTransaction();
            Ft1.Confirmed = false;
            Ft1.Debit = this.InvoiceNet;
            Ft1.Credit = 0;
            Ft1.FromAccount = this.Supplier.AccountingID;//sales account
            Ft1.Notes = "";
            Ft1.ReferanceDocumentNumber = this.Id;
            Ft1.Statement = "تسجيل فاتورة شراء للعميل " + this.Supplier.Name;
            Ft1.TransactionDate = DateTime.Now;
            db.FinancialTransactions.Add(Ft1);
            db.SaveChanges();
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
            var FtToDelete = db.FinancialTransactions.Where(p => p.ReferanceDocumentNumber == ITD.Id).ToList();
            foreach (FinancialTransaction item in FtToDelete)
            {
                db.FinancialTransactions.Remove(item);
                db.SaveChanges();
            }
            foreach (SupplierInvoiceLine CIL in LOCIL)
            {
                Stock S = db.Stocks.Single(p => p.ProjectID == ITD.ProjectID && p.ProductID == CIL.ProductId);
                S.Quantity -= CIL.Qty;
                StockTransaction ST = new StockTransaction();
                ST.Date = DateTime.UtcNow.AddHours(3);
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
            Ft.Debit = ITD.InvoiceNet;
            Ft.Credit = 0;
            Ft.FromAccount = ITD.Supplier.AccountingID;
            Ft.LastEditBy = EditBy;
            Ft.Confirmed = false;
            Ft.Notes = "";
            Ft.Statement = "ترحيل فاتورة شراء";
            Ft.TransactionDate = DateTime.UtcNow.AddHours(3);
            db.FinancialTransactions.Add(Ft);
            FinancialTransaction Ft1 = new FinancialTransaction();
            Ft1.Credit = ITD.InvoiceNet;
            Ft1.Debit = 0;
            Ft1.LastEditBy = EditBy;
            Ft1.Confirmed = false;
            Ft1.Notes = "";
            Ft1.Statement = "ترحيل فاتورة شراء";
            Ft1.FromAccount = ITD.InvoiceAccount;
            Ft1.TransactionDate = DateTime.UtcNow.AddHours(3);
            db.FinancialTransactions.Add(Ft1);
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
            SupplierInvoice Sup = new SupplierInvoice();
            SupplierInvoice SupTd = new SupplierInvoice { Id = this.Id };
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
            SupTd.DeleteInvoice();
            Sup.Add(LOSIL);
            //db.SaveChanges();
            //foreach (SupplierInvoiceLine item in LOSIL)
            //{
            //    if (db.SupplierInvoiceLines.Any(p => p.Id == item.Id))
            //    {
            //        continue;
            //    }
            //    else
            //    {
            //        item.InvoiceId = this.Id;
            //        db.SupplierInvoiceLines.Add(item);
            //        StockTransaction ST = new StockTransaction();
            //        ST.Date = DateTime.UtcNow.AddHours(3);
            //        ST.ProductID = item.ProductId;
            //        ST.Quantity = item.Qty;
            //        ST.StockID = db.Stocks.Single(p => p.ProductID == item.ProductId && p.ProjectID == Sup.ProjectID).Id;
            //        ST.Type = (int)StockTransactionsTypes.تعديل_فاتورة_شراء;
            //        db.StockTransactions.Add(ST);
            //        var StockToEdit = db.Stocks.Single(p => p.ProductID == item.ProductId && p.ProjectID == Sup.ProjectID);
            //        StockToEdit.Quantity += ST.Quantity;
            //        db.SaveChanges();
            //    }
            //}
            ////new Stock().Purchases(this.Id);
            return new Returner
            {
                Message = Message.Invoice_Added_Successfully
            };
        }

        public Returner DeepSearch(string _Keyword)
        {
            List<SupplierInvoice> LOCI = new List<SupplierInvoice>();
            string IDS = "";
            string KeywordType = "string";
            int AfterParseInt;
            DateTime AfterParseDate;
            if (int.TryParse(_Keyword, out AfterParseInt))
            {
                KeywordType = "int";
            }
            if (DateTime.TryParse(_Keyword, out AfterParseDate))
            {
                KeywordType = "DateTime";
            }
            if (KeywordType == "string")
            {
                LOCI = db.SupplierInvoices.Where(p => p.Supplier.Name.Contains(_Keyword) || p.SupplierInvoiceLines.Any(n => n.Product.ProductName.Contains(_Keyword)) || p.AccountingTree.NodeName.Contains(_Keyword)).ToList();
            }
            if (KeywordType == "int")
            {
                LOCI = db.SupplierInvoices.Where(p => p.InvoiceDiscount == AfterParseInt || p.InvoiceNet == AfterParseInt || p.InvoiceTotal == AfterParseInt || p.SupplierInvoiceLines.Any(n => n.Price == AfterParseInt) || p.SupplierInvoiceLines.Any(n => n.Qty == AfterParseInt) || p.SupplierInvoiceLines.Any(n => n.Total == AfterParseInt)).ToList();
            }
            if (KeywordType == "DateTime")
            {
                LOCI = db.SupplierInvoices.Where(p => p.InvoiceDate == AfterParseDate).ToList();
            }
            foreach (SupplierInvoice item in LOCI)
            {
                IDS += item.Id + ",";
            }
            int LastCharIndex = IDS.Length - 1;
            IDS = IDS.Remove(LastCharIndex, 1);
            return new Returner
            {
                Data = IDS
            };
        }
    }
}