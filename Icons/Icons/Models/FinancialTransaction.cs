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
    
    public partial class FinancialTransaction
    {
        public int Id { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public Nullable<int> FromAccount { get; set; }
        public Nullable<int> ToAccount { get; set; }
        public Nullable<double> Amount { get; set; }
        public string Statement { get; set; }
        public string Notes { get; set; }
        public Nullable<int> ReferanceDocumentNumber { get; set; }
        public Nullable<int> LastEditBy { get; set; }
    
        public virtual AccountingTree AccountingTree { get; set; }
        public virtual AccountingTree AccountingTree1 { get; set; }
        public virtual User User { get; set; }
    }
}
