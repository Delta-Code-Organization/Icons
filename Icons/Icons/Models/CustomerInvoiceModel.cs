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
            var Cussss = db.Customers.Single(p => p.ID == this.CustomerID);
            FinancialTransaction Ft = new FinancialTransaction();
            Ft.Confirmed = false;
            Ft.Debit = 0;
            Ft.Credit = this.InvoiceNet;
            Ft.FromAccount = 42;//sales account
            Ft.Notes = "";
            Ft.ReferanceDocumentNumber = this.Id;
            Ft.Statement = "تسجيل فاتورة بيع للعميل " + Cussss.Name;
            Ft.TransactionDate = DateTime.Now;
            db.FinancialTransactions.Add(Ft);
            db.SaveChanges();
            FinancialTransaction Ft1 = new FinancialTransaction();
            Ft1.Confirmed = false;
            Ft1.Debit = this.InvoiceNet;
            Ft1.Credit = 0;
            Ft1.FromAccount = Cussss.AccountID;//Customer account
            Ft1.Notes = "";
            Ft1.ReferanceDocumentNumber = this.Id;
            Ft1.Statement = "تسجيل فاتورة بيع للعميل " + Cussss.Name;
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
            var FtToDelete = db.FinancialTransactions.Where(p => p.ReferanceDocumentNumber == ITD.Id).ToList();
            foreach (FinancialTransaction item in FtToDelete)
            {
                db.FinancialTransactions.Remove(item);
                db.SaveChanges();
            }
            foreach (CustomerInvoiceLine CIL in LOCIL)
            {
                Stock S = db.Stocks.Single(p => p.ProjectID == ITD.ProjectID && p.ProductID == CIL.ProductId);
                S.Quantity += CIL.Qty;
                StockTransaction ST = new StockTransaction();
                ST.Date = DateTime.UtcNow.AddHours(3);
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
            Ft.Credit = ITD.InvoiceNet;
            Ft.Debit = 0;
            Ft.LastEditBy = EditBy;
            Ft.Notes = "";
            Ft.Confirmed = false;
            Ft.Statement = "ترحيل فاتورة بيع";
            Ft.FromAccount = ITD.Customer.AccountID;
            Ft.TransactionDate = DateTime.UtcNow.AddHours(3);
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
            CustomerInvoice CI1 = new CustomerInvoice
            {
                CustomerID = this.CustomerID,
                Departed = this.Departed,
                InvoiceAccount = this.InvoiceAccount,
                InvoiceDate = this.InvoiceDate,
                InvoiceDiscount = this.InvoiceDiscount,
                InvoiceNet = this.InvoiceNet,
                InvoiceTotal = this.InvoiceTotal,
                LastEditBy = this.LastEditBy,
                ProjectID = this.ProjectID,
            };
            CustomerInvoice CI = new CustomerInvoice { Id = this.Id };
            CI.DeleteInvoice();
            CI1.Add(LOSIL);
            //var Inv = db.CustomerInvoices.Where(p => p.Id == this.Id).SingleOrDefault();
            //Inv.CustomerID = this.CustomerID;
            //Inv.Departed = this.Departed;
            //Inv.InvoiceAccount = this.InvoiceAccount;
            //Inv.InvoiceDate = this.InvoiceDate;
            //Inv.InvoiceDiscount = this.InvoiceDiscount;
            //Inv.InvoiceNet = this.InvoiceNet;
            //Inv.InvoiceTotal = this.InvoiceTotal;
            //Inv.LastEditBy = this.LastEditBy;
            //Inv.ProjectID = this.ProjectID;
            //db.SaveChanges();
            //foreach (CustomerInvoiceLine item in LOSIL)
            //{
            //    if (db.CustomerInvoiceLines.Any(p => p.Id == item.Id))
            //    {
            //        continue;
            //    }
            //    else
            //    {
            //        item.InvoiceId = this.Id;
            //        db.CustomerInvoiceLines.Add(item);
            //        StockTransaction ST = new StockTransaction();
            //        ST.Date = DateTime.UtcNow.AddHours(3);
            //        ST.ProductID = item.ProductId;
            //        ST.Quantity = -item.Qty;
            //        ST.StockID = db.Stocks.Single(p => p.ProductID == item.ProductId && p.ProjectID == Inv.ProjectID).Id;
            //        ST.Type = (int)StockTransactionsTypes.تعديل_فاتورة_بيع;
            //        db.StockTransactions.Add(ST);
            //        var StockToEdit = db.Stocks.Single(p => p.ProductID == item.ProductId && p.ProjectID == Inv.ProjectID);
            //        StockToEdit.Quantity += ST.Quantity;
            //        db.SaveChanges();
            //    }
            //}
            ////new Stock().Sells(this.Id);
            return new Returner
            {
                Message = Message.Invoice_Added_Successfully
            };
        }

        public Returner DeepSearch(string _Keyword)
        {
            List<CustomerInvoice> LOCI = new List<CustomerInvoice>();
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
                LOCI = db.CustomerInvoices.Where(p => p.Customer.Name.Contains(_Keyword) || p.CustomerInvoiceLines.Any(n => n.Product.ProductName.Contains(_Keyword)) || p.AccountingTree.NodeName.Contains(_Keyword)).ToList();
            }
            if (KeywordType == "int")
            {
                LOCI = db.CustomerInvoices.Where(p => p.InvoiceDiscount == AfterParseInt || p.InvoiceNet == AfterParseInt || p.InvoiceTotal == AfterParseInt || p.CustomerInvoiceLines.Any(n => n.Price == AfterParseInt) || p.CustomerInvoiceLines.Any(n => n.Qty == AfterParseInt) || p.CustomerInvoiceLines.Any(n => n.Total == AfterParseInt)).ToList();
            }
            if (KeywordType == "DateTime")
            {
                LOCI = db.CustomerInvoices.Where(p => p.InvoiceDate == AfterParseDate).ToList();
            }
            foreach (CustomerInvoice item in LOCI)
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