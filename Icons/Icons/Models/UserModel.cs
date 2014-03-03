using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icons.Models
{
    public partial class User
    {
        #region Context
        IconsDBEntities db = new IconsDBEntities();
        #endregion

        public Returner Login()
        {
            if (db.Users.Any(p=>p.Username == this.Username && p.Password == this.Password))
            {
                var User = db.Users.Where(p => p.Username == this.Username && p.Password == this.Password).SingleOrDefault();
                return new Returner 
                { 
                    Data = User,
                    Message = Message.Success_Login
                };
            }
            else
            {
                return new Returner
                {
                    Message = Message.Username_or_password_is_wrong
                };
            }
        }
    }
}