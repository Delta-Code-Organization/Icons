using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icons.Models
{
    public partial class Contract
    {
        #region Context
        MaksoudDBEntities db = new MaksoudDBEntities();
        #endregion

        public Returner CreateContract(List<ContractOwner> LOCO,List<Installment> I,ProjectUnit PU,int OwnerID)
        {
            db.Contracts.Add(this);
            db.SaveChanges();
            var CurrentContract = db.Contracts.Where(p => p.Id == this.Id).SingleOrDefault();
            foreach (ContractOwner CO in LOCO)
            {
                CurrentContract.ContractOwners.Add(CO);
            }
            db.SaveChanges();
            CurrentContract = db.Contracts.Where(p => p.Id == this.Id).SingleOrDefault();
            foreach (Installment III in I)
            {
                CurrentContract.Installments.Add(III);
            }
            db.SaveChanges();
            var ProjU = db.ProjectUnits.Where(p => p.Id == PU.Id).SingleOrDefault();
            ProjU.Owner = OwnerID;
            db.SaveChanges();
            return new Returner
            {
                Data = CurrentContract,
                Message = Message.Contract_Created_Successfully
            };
        }
    }
}