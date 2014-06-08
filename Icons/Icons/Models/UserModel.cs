using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icons.Models
{
    public partial class User
    {
        #region Context
        MaksoudDBEntities db = new MaksoudDBEntities();
        #endregion

        public Returner Login()
        {
            if (db.Users.Any(p => p.Username == this.Username && p.Password == this.Password && p.Status == 1))
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

        public Returner CreateAccount(List<UserAccess> LOUA)
        {
            if (db.Users.Any(p => p.Username == this.Username))
            {
                return new Returner
                {
                    Message = Message.Username_Already_Exists
                };
            }
            else
            {
                db.Users.Add(this);
                db.SaveChanges();
                var U = db.Users.Where(P => P.ID == this.ID).SingleOrDefault();
                foreach (UserAccess UA in LOUA)
                {
                    UA.UserID = U.ID;
                    db.UserAccesses.Add(UA);
                    db.SaveChanges();
                }
                //List<UserAccess> LOSTAP = new List<UserAccess>();
                //foreach (UserAccess item in LOUA)
                //{
                //    LOSTAP.Add(db.UserAccesses.Where(p => p.ID == item.ID).SingleOrDefault());
                //}
                //foreach (Screen item in LOSTAP)
                //{
                //    U.Screens.Add(item);
                //}
                //db.SaveChanges();
                return new Returner
                {
                    Message = Message.Account_created_successfully
                };
            }
        }

        public Returner GetAll()
        {
            return new Returner
            {
                Data = (from U in db.Users
                        where U.Status == 1
                        select U).ToList()
            };
        }

        public Returner GetByID()
        {
            return new Returner
            {
                Data = (from U in db.Users
                        where U.ID == this.ID
                        select U).SingleOrDefault()
            };
        }

        public Returner RemoveUser()
        {
            var U = db.Users.Where(p => p.ID == this.ID).SingleOrDefault();
            U.Status = 0;
            db.SaveChanges();
            return new Returner
            {
                Message = Message.User_Deleted_Successfully
            };
        }

        public Returner Update(List<UserAccess> LOS)
        {
            var U = db.Users.Where(p => p.ID == this.ID).SingleOrDefault();
            if (db.Users.Any(p => p.Username == this.Username && p.ID != U.ID && p.Status == 1))
            {
                return new Returner
                {
                    Message = Message.Username_Already_Exists
                };
            }
            else
            {
                U.Password = this.Password;
                List<UserAccess> STLO = U.UserAccesses.ToList();
                foreach (UserAccess item in STLO)
                {
                    U.UserAccesses.Remove(item);
                }
                U.Username = this.Username;
                db.SaveChanges();
                foreach (UserAccess UA in LOS)
                {
                    UA.UserID = U.ID;
                    db.UserAccesses.Add(UA);
                    db.SaveChanges();
                }
                return new Returner
                {
                    Message = Message.User_updated_successfully
                };
            }
        }
    }
}