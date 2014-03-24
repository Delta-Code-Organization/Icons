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

        public Returner CreateContract(List<ContractOwner> LOCO, List<Installment> I, ProjectUnit PU, int OwnerID)
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

        public Returner SearchInstallments(DateTime? From, DateTime? To, int? CusID)
        {
            List<Installment> LOI = new List<Installment>();
            if (CusID != null && From == null && To == null)
            {
                LOI = db.Installments.Where(p => p.ResponsibleID == CusID).ToList();
            }
            if (From != null && To != null && CusID == null)
            {
                LOI = db.Installments.Where(p => p.DueDate >= From && p.DueDate <= To).ToList();
            }
            if (From != null && To != null && CusID != null)
            {
                LOI = db.Installments.Where(p => p.DueDate >= From && p.DueDate <= To && p.ResponsibleID == CusID).ToList();
            }
            List<Customer> LOC = (from Cus in LOI
                                  select Cus.Customer).ToList();
            List<Customer> LONC = new List<Customer>();
            foreach (Customer item in LOC)
            {
                if (LONC.Any(p => p.ID == item.ID) == false)
                {
                    LONC.Add(item);
                }
            }
            var ResultInJSON = (from I in LONC
                                select new
                                {
                                    I.AccountID,
                                    I.Address,
                                    I.BirthDate,
                                    I.ID,
                                    I.Name,
                                    Installments = new List<object>((from III in I.Installments
                                                                     select new
                                                                     {
                                                                         III.Amount,
                                                                         III.ContractID,
                                                                         III.DueDate,
                                                                         III.Id,
                                                                         III.PaymentDate,
                                                                         III.ResponsibleID
                                                                     }).Cast<object>().ToList()).Cast<object>().ToList()
                                }).ToList();
            return new Returner
            {
                Data = LOI,
                DataInJSON = ResultInJSON.ToJSON()
            };
        }

        public Returner PayInstallment(int ID)
        {
            var Installment = db.Installments.Where(p => p.Id == ID).SingleOrDefault();
            Installment.PaymentDate = DateTime.Now;
            db.SaveChanges();
            return new Returner
            {
                Message = Message.Installment_Paid_Successfully
            };
        }
    }
}