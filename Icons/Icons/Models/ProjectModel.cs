﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icons.Models
{
    public partial class Project
    {
        #region Context
        MaksoudDBEntities db = new MaksoudDBEntities();
        #endregion

        public Returner Add()
        {
            var ParentAccount = db.AccountingTrees.Where(p => p.KeyAccountID == (int)KeyAccounts.Projects).FirstOrDefault();
            if (ParentAccount != null)
            {
                AccountingTree ProjectNode = new AccountingTree();
                ProjectNode.NodeName = this.ProjectName;
                ProjectNode.Parent = ParentAccount.Id;
                db.AccountingTrees.Add(ProjectNode);
                db.SaveChanges();
            }
            var AccID = db.AccountingTrees.Where(p => p.NodeName == this.ProjectName && p.Parent == ParentAccount.Id).SingleOrDefault();
            this.AccountID = AccID.Id;
            db.Projects.Add(this);
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Project_Created_Successfully
            };
        }

        public Returner GetAll()
        {
            return new Returner
            {
                Data = (from P in db.Projects
                        select P).ToList()
            };
        }

        public Returner Remove()
        {
            var P = db.Projects.Where(p => p.Id == this.Id).SingleOrDefault();
            db.Projects.Remove(P);
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Project_Deleted_Successfully
            };
        }

        public Returner GetByID()
        {
            return new Returner
            {
                Data = db.Projects.Where(p => p.Id == this.Id).SingleOrDefault()
            };
        }

        public Returner Edit()
        {
            var P = db.Projects.Where(p => p.Id == this.Id).SingleOrDefault();
            P.ExpectedCost = this.ExpectedCost;
            P.FirstViewLength = this.FirstViewLength;
            P.FloorsCount = this.FloorsCount;
            P.ForthViewLength = this.ForthViewLength;
            P.LandSpace = this.LandSpace;
            P.OwnershipPercentage = this.OwnershipPercentage;
            P.ProjectAddress = this.ProjectAddress;
            P.ProjectName = this.ProjectName;
            P.SecondViewLength = this.SecondViewLength;
            P.ThirdViewLength = this.ThirdViewLength;
            P.CreationDate = this.CreationDate;
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Project_Updated_Successfully
            };
        }

        public Returner GetProjectUnits()
        {
            var ProjUnits = db.ProjectUnits.Where(p => p.ProjectID == this.Id).ToList();
            var ProjUnitsInJSON = (from U in ProjUnits
                                   select new
                                   {
                                       U.Id,
                                       U.ExpectedPrice,
                                       Finishing = Enum.GetName(typeof(Finishing), U.Finishing),
                                       U.FloorNumber,
                                       U.UnitSpace,
                                       UnitName = Enum.GetName(typeof(UnitTypes), U.UnitType)
                                   }).ToList();
            return new Returner
            {
                Data = ProjUnits,
                DataInJSON = ProjUnitsInJSON.ToJSON()
            };
        }
    }
}