using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icons.Models
{
    public partial class FinancialTransaction
    {
        #region Context
        MaksoudDBEntities db = new MaksoudDBEntities();
        #endregion

        public Returner Search()
        {
            return new Returner
            {
                Data = db.FinancialTransactions.ToList()
            };
        }

        public Returner GetByID()
        {
            return new Returner
            {
                Data = db.FinancialTransactions.Single(p => p.Id == this.Id)
            };
        }

        public void Edit()
        {
            var FT = db.FinancialTransactions.Single(p => p.Id == this.Id);
            FT.Debit = this.Debit;
            FT.Credit = this.Credit;
            FT.FromAccount = this.FromAccount;
            FT.LastEditBy = this.LastEditBy;
            FT.Notes = this.Notes;
            FT.Statement = this.Statement;
            FT.TransactionDate = this.TransactionDate;
            db.SaveChanges();
        }

        public void Delete()
        {
            var FT = db.FinancialTransactions.Single(p => p.Id == this.Id);
            db.FinancialTransactions.Remove(FT);
            db.SaveChanges();
        }

        public void Confirm()
        {
            var FT = db.FinancialTransactions.Single(p => p.Id == this.Id);
            FT.Confirmed = true;
            db.SaveChanges();
        }
    }
}