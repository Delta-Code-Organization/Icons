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

        #region PrivateMethods
        private int AddSupplierAccountingTree()
        {
            int ParentSupplierID = db.AccountingTrees.Where(p => p.KeyAccountID == (int)KeyAccounts.Suppliers).SingleOrDefault().Id;
            AccountingTree SupplierNode = new AccountingTree();
            SupplierNode.NodeName = this.Name;
            SupplierNode.Parent = ParentSupplierID;
            db.AccountingTrees.Add(SupplierNode);
            db.SaveChanges();
            return SupplierNode.Id;
        }

        private void RemoveSupplierNode(int SupplierNodeID)
        {
            var AccToRemove = db.AccountingTrees.Where(p => p.Id == SupplierNodeID).SingleOrDefault();
            db.AccountingTrees.Remove(AccToRemove);
            db.SaveChanges();
        }

        private void UpdateSuplierNode(int SupplierNodeID)
        {
            var AccToEdit = db.AccountingTrees.Where(p => p.Id == SupplierNodeID).SingleOrDefault();
            AccToEdit.NodeName = this.Name;
            db.SaveChanges();
        }
        #endregion

        public Returner Create()
        {
            int AccID = AddSupplierAccountingTree();
            this.AccountingID = AccID;
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

        public Returner GetByID()
        {
            return new Returner
            {
                Data = (from S in db.Suppliers
                        where S.ID == this.ID
                        select S).ToList().SingleOrDefault()
            };
        }

        public Returner Remove()
        {
            var S = db.Suppliers.Where(p => p.ID == this.ID).SingleOrDefault();
            RemoveSupplierNode((int)S.AccountingID);
            db.Suppliers.Remove(S);
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Supplier_deleted_successfully
            };
        }

        public Returner Edit()
        {
            var S = db.Suppliers.Where(p => p.ID == this.ID).SingleOrDefault();
            S.Address = this.Address;
            S.City = this.City;
            S.District = this.District;
            S.Mobile = this.Mobile;
            S.Name = this.Name;
            S.Notes = this.Notes;
            S.Phone = this.Phone;
            UpdateSuplierNode((int)S.AccountingID);
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Supplier_updated_successfully
            };
        }
    }
}