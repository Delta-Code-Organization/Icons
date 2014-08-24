using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icons.Models
{
    public class General
    {
        #region Context
        MaksoudDBEntities db = new MaksoudDBEntities();
        #endregion

        public void GeneralSearch(string Keyword, out List<Customer> LOC, out List<Supplier> LOS, out List<CustomerInvoice> LOCI, out List<SupplierInvoice> LOSI, out List<FinancialTransaction> LOFT, out List<Product> LOP)
        {
            LOC = db.Customers.Where(p => p.Address.Contains(Keyword) || p.Name.Contains(Keyword) || p.Notes.Contains(Keyword) || p.Phone.Contains(Keyword)).ToList();
            LOS = db.Suppliers.Where(p => p.Address.Contains(Keyword) || p.City.Contains(Keyword) || p.District.Contains(Keyword) || p.Mobile.Contains(Keyword) || p.Name.Contains(Keyword) || p.Notes.Contains(Keyword) || p.Phone.Contains(Keyword)).ToList();
            LOCI = db.CustomerInvoices.Where(p => p.Customer.Name.Contains(Keyword) || p.AccountingTree.NodeName.Contains(Keyword)).ToList();
            LOSI = db.SupplierInvoices.Where(p => p.Supplier.Name.Contains(Keyword) || p.SupplierReferenaceNo.Contains(Keyword) || p.AccountingTree.NodeName.Contains(Keyword)).ToList();
            int X;
            if (int.TryParse(Keyword,out X))
            {
                LOFT = db.FinancialTransactions.Where(p => p.AccountingTree.NodeName.Contains(Keyword) || p.AccountingTree1.NodeName.Contains(Keyword) || p.Notes.Contains(Keyword) || p.Statement.Contains(Keyword) || p.Id == X).ToList();
            }
            else
            {
                LOFT = db.FinancialTransactions.Where(p => p.AccountingTree.NodeName.Contains(Keyword) || p.AccountingTree1.NodeName.Contains(Keyword) || p.Notes.Contains(Keyword) || p.Statement.Contains(Keyword)).ToList();
            }
            LOP = db.Products.Where(p => p.Description.Contains(Keyword) || p.ProductCategory.CategoryName.Contains(Keyword) || p.ProductName.Contains(Keyword)).ToList();
        }
    }
}