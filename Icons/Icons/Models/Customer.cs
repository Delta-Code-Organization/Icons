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
    
    public partial class Customer
    {
        public Customer()
        {
            this.ContractOwners = new HashSet<ContractOwner>();
            this.CustomerInvoices = new HashSet<CustomerInvoice>();
            this.Installments = new HashSet<Installment>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public Nullable<System.DateTime> BirthDate { get; set; }
        public string Notes { get; set; }
        public Nullable<int> AccountID { get; set; }
    
        public virtual AccountingTree AccountingTree { get; set; }
        public virtual ICollection<ContractOwner> ContractOwners { get; set; }
        public virtual ICollection<CustomerInvoice> CustomerInvoices { get; set; }
        public virtual ICollection<Installment> Installments { get; set; }
    }
}