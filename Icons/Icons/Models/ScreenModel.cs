using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icons.Models
{
    public partial class Screen
    {
        #region Context
        MaksoudDBEntities db = new MaksoudDBEntities();
        #endregion

        public Returner GetScreens()
        {
            var S = (from Sc in db.Screens
                     select Sc).ToList();
            return new Returner
            {
                Data = S
            };
        }
    }
}