using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icons.Models
{
    public partial class Supplier
    {
        #region Conetext
        MaksoudDBEntities db = new MaksoudDBEntities();
        #endregion

        public Returner Create()
        {
            db.Suppliers.Add(this);
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Supplier_created_successfully
            };
        }

        public Returner GetAll()
        {
            return new Returner
            {
                Data = (from S in db.Suppliers
                        select S).ToList()
            };
        }

        public Returner Remove()
        {
            var S = db.Suppliers.Where(p => p.ID == this.ID).SingleOrDefault();
            db.Suppliers.Remove(S);
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Supplier_deleted_successfully
            };
        }
    }
}