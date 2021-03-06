﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icons.Models
{
    public partial class Customer
    {
        #region Context
        MaksoudDBEntities db = new MaksoudDBEntities();
        #endregion

        public Returner CreateCustomer()
        {
            var customer = db.Customers.Any(p => p.Name == this.Name);
            if (customer == true)
            {
                return new Returner
                {
                    Message = Message.Customer_Name_Already_Exist
                };
            }
            int AccID = AddCustomerNode();
            this.AccountID = AccID;
            db.Customers.Add(this);
            db.SaveChanges();
            var lastCustomer = db.Customers.OrderByDescending(p => p.ID).FirstOrDefault();
            return new Returner
            {
                Data = lastCustomer,
                Message = Message.Customer_Created_Successfully
            };

        }

        public Returner EditCustomer()
        {
            var update = db.Customers.Where(p => p.ID == this.ID).ToList().SingleOrDefault();
            update.Name = this.Name;
            update.Attach = this.Attach;
            update.FileName = this.FileName;
            update.Address = this.Address;
            update.Phone = this.Phone;
            update.BirthDate = this.BirthDate;
            update.Notes = this.Notes;
            UpdateCustomerNode((int)update.AccountID);
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Customer_Updated_Succeessfully
            };
        }

        public Returner DeleteCusotmer()
        {
            var delete = db.Customers.Where(p => p.ID == this.ID).ToList().SingleOrDefault();
            DeleteCustomerNode((int)delete.AccountID);
            db.Customers.Remove(delete);
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Customer_Deleted_Successfully
            };
        }

        public Returner GetAllCutomers()
        {
            var allCustomers = db.Customers.OrderByDescending(p => p.ID).ToList();
            return new Returner
            {
                Data = allCustomers,
                DataInJSON = allCustomers.ToJSON()
            };
        }

        public Returner GetCustomerData()
        {
            var getCustomerData = db.Customers.Where(p => p.ID == this.ID).SingleOrDefault();
            return new Returner
            {
                Data = getCustomerData,
                DataInJSON = getCustomerData.ToJSON()
            };
        }

        public Returner GetCustomerNumbers()
        {
            return new Returner
            {
                Data = db.Customers.ToList().Count
            };
        }

        #region PrivateMethods
        public int AddCustomerNode()
        {
            int ParentID = db.AccountingTrees.Where(p => p.KeyAccountID == (int)KeyAccounts.Customers).SingleOrDefault().Id;
            AccountingTree Node = new AccountingTree();
            Node.NodeName = this.Name;
            Node.Parent = ParentID;
            db.AccountingTrees.Add(Node);
            db.SaveChanges();
            return Node.Id;
        }

        public void UpdateCustomerNode(int AccID)
        {
            var AccToEdit = db.AccountingTrees.Where(p => p.Id == AccID).SingleOrDefault();
            AccToEdit.NodeName = this.Name;
            db.SaveChanges();
        }

        public void DeleteCustomerNode(int AccID)
        {
            var AccToDelete = db.AccountingTrees.Where(p => p.Id == AccID).SingleOrDefault();
            db.AccountingTrees.Remove(AccToDelete);
            db.SaveChanges();
        }
        #endregion
    }
}