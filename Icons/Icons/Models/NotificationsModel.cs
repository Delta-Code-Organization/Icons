using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icons.Models
{
    public partial class Notification
    {
        #region Context
        MaksoudDBEntities db = new MaksoudDBEntities();
        #endregion

        public List<Notification> GetNotifications()
        {
            List<Notification> LON = new List<Notification>();
            int FtCount = db.FinancialTransactions.Where(p => p.Confirmed == false).ToList().Count;
            if (FtCount != 0)
            {
                Notification N = new Notification();
                N.Date = DateTime.UtcNow.AddHours(3);
                N.RedirectURL = "/Accounting/SearchFinancialTransactions";
                N.Status = 0;
                N.Text = "يوجد " + FtCount + " معاملات مالية لم تؤكد بعد ";
                LON.Add(N);
            }
            foreach (Installment item in db.Installments.ToList())
            {
                if (DateTime.UtcNow.AddHours(3) > item.DueDate)
                {
                    Notification Not = new Notification();
                    Not.Date = DateTime.UtcNow.AddHours(3);
                    Not.RedirectURL = "/Contract/SearchInstallments";
                    Not.Status = 0;
                    Not.Text = "قسط مستحق علي " + item.Customer.Name + " بقيمة " + item.Amount;
                    LON.Add(Not);
                }
            }
            foreach (Supplier item in db.Suppliers.ToList())
            {
                double? OnSup = db.FinancialTransactions.Where(p => p.FromAccount == item.AccountingID).Sum(p => p.Amount);
                double? ToSup = db.FinancialTransactions.Where(p => p.ToAccount == item.AccountingID).Sum(p => p.Amount);
                if (ToSup > OnSup)
                {
                    double? Balance = ToSup - OnSup;
                    Notification Not = new Notification();
                    Not.Date = DateTime.UtcNow.AddHours(3);
                    Not.RedirectURL = "/Supplier/SuppliersSearch";
                    Not.Status = 0;
                    Not.Text = "المورد " + item.Name + " لة " + Balance;
                    LON.Add(Not);
                }
            }
            return LON;
        }
    }
}