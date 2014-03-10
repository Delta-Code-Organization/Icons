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

        public Returner Add(List<string> LOSIL)
        {
            db.CustomerInvoices.Add(this);
            db.SaveChanges();
            foreach (string item in LOSIL)
            {
                int CurrentID = Convert.ToInt32(item);
                var SSS = db.CustomerInvoiceLines.Where(p => p.Id == CurrentID).SingleOrDefault();
                SSS.InvoiceId = this.Id;
                db.SaveChanges();
            }
            return new Returner
            {
                Message = Message.Invoice_Added_Successfully
            };
        }

        public Returner InvoiceNumber()
        {
            int MaxID = db.CustomerInvoices.Max(p => p.Id);
            return new Returner
            {
                Data = MaxID
            };
        }
    }
}