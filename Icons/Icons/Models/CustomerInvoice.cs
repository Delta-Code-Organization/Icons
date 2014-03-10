//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Icons.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CustomerInvoice
    {
        public CustomerInvoice()
        {
            this.CustomerInvoiceLines = new HashSet<CustomerInvoiceLine>();
        }
    
        public int Id { get; set; }
        public Nullable<int> CustomerID { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public Nullable<int> InvoiceAccount { get; set; }
        public Nullable<double> InvoiceTotal { get; set; }
        public Nullable<double> InvoiceDiscount { get; set; }
        public Nullable<double> InvoiceNet { get; set; }
        public Nullable<bool> Departed { get; set; }
        public Nullable<int> LastEditBy { get; set; }
    
        public virtual AccountingTree AccountingTree { get; set; }
        public virtual ICollection<CustomerInvoiceLine> CustomerInvoiceLines { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual User User { get; set; }
    }
}
