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
    
    public partial class Project
    {
        public Project()
        {
            this.ProjectUnits = new HashSet<ProjectUnit>();
        }
    
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string ProjectAddress { get; set; }
        public Nullable<double> LandSpace { get; set; }
        public Nullable<double> FirstViewLength { get; set; }
        public Nullable<double> SecondViewLength { get; set; }
        public Nullable<double> ThirdViewLength { get; set; }
        public Nullable<double> ForthViewLength { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public Nullable<int> FloorsCount { get; set; }
        public Nullable<double> ExpectedCost { get; set; }
        public Nullable<double> OwnershipPercentage { get; set; }
        public Nullable<int> AccountID { get; set; }
    
        public virtual AccountingTree AccountingTree { get; set; }
        public virtual ICollection<ProjectUnit> ProjectUnits { get; set; }
    }
}
