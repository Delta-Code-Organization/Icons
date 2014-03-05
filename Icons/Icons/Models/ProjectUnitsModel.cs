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

        public Returner Create()
        {
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
            db.SaveChanges();
            return new Returner { Message = Message.Unit_Updated_Successfully };
        }
    }
}