using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icons.Models
{
    public partial class ProjectUnit
    {
        #region Context
        MaksoudDBEntities db = new MaksoudDBEntities();
        #endregion

        public string DisplayText
        {
            get { return Project.ProjectName + "-" + Enum.GetName(typeof(UnitTypes), UnitType) + "-" + FloorNumber.ToString() + "-" + UnitSpace.ToString(); }
        }

        #region PrivateMethods
        private int CreateUnitNode()
        {
            int ProjectAccID = (int)(db.Projects.Where(p => p.Id == this.ProjectID).SingleOrDefault().AccountID);
            int ParentNode = db.AccountingTrees.Where(p => p.Id == ProjectAccID).SingleOrDefault().Id;
            AccountingTree UnitNode = new AccountingTree();
            UnitNode.NodeName = Enum.GetName(typeof(UnitTypes), this.UnitType);
            UnitNode.Parent = ParentNode;
            db.AccountingTrees.Add(UnitNode);
            db.SaveChanges();
            return UnitNode.Id;
        }

        private void RemoveUnitNode(int NodeID)
        {
            var NodeToRemove = db.AccountingTrees.Where(p => p.Id == NodeID).SingleOrDefault();
            db.AccountingTrees.Remove(NodeToRemove);
            db.SaveChanges();
        }

        private void EditUnitNode(int NodeID)
        {
            var NodeToEdit = db.AccountingTrees.Where(p => p.Id == NodeID).SingleOrDefault();
            NodeToEdit.NodeName = Enum.GetName(typeof(UnitTypes), this.UnitType);
            db.SaveChanges();
        }
        #endregion

        public Returner Create()
        {
            int AccID = CreateUnitNode();
            this.AccountingID = AccID;
            db.ProjectUnits.Add(this);
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Unit_Created_Successfully
            };
        }

        public Returner GetUnits()
        {
            return new Returner
            {
                Data = (from U in db.ProjectUnits
                        where U.ProjectID == this.ProjectID
                        select U).ToList()
            };
        }

        public Returner Delete()
        {
            var PU = db.ProjectUnits.Where(p => p.Id == this.Id).SingleOrDefault();
            RemoveUnitNode((int)PU.AccountingID);
            db.ProjectUnits.Remove(PU);
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Unit_Delete_Successfully
            };
        }

        public Returner Edit()
        {
            var PU = db.ProjectUnits.Where(p => p.Id == this.Id).SingleOrDefault();
            PU.ExpectedPrice = this.ExpectedPrice;
            PU.Finishing = this.Finishing;
            PU.FloorNumber = this.FloorNumber;
            PU.Notes = this.Notes;
            PU.UnitSpace = this.UnitSpace;
            PU.UnitType = this.UnitType;
            EditUnitNode((int)PU.AccountingID);
            db.SaveChanges();
            return new Returner { Message = Message.Unit_Updated_Successfully };
        }
    }
}