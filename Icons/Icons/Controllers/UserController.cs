using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Icons.Models;

namespace Icons.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/

        public ActionResult Login()
        {
            using (MaksoudDBEntities db = new MaksoudDBEntities())
            {
                var LOS = db.Screens.ToList();
                Session["AllScreens"] = LOS;
            }
            List<FinancialTransaction> LOFtIds = new List<FinancialTransaction>();
            List<Notification> LON = new Notification().GetNotifications(out LOFtIds);
            Session["UnConfirmedFt"] = LOFtIds;
            Session["Notification"] = LON;
            Session["NotificationCount"] = LON.Count;
            return View();
        }

        [HttpPost]
        public string Login(string Username, string Password)
        {
            User U = new User();
            U.Username = Username;
            U.Password = Password;
            Returner R = U.Login();
            if (R.Message == Message.Success_Login)
            {
                Session["User"] = R.Data;
                return "true";
            }
            else
            {
                return "false";
            }
        }

        [HttpPost]
        public void UpdateNotificationCount()
        {
            Session["NotificationCount"] = 0;
        }

        public void Logout()
        {
            Session.Clear();
        }

        public ActionResult CreateUser()
        {
            List<Screen> LOS = new List<Screen>();
            LOS = new Screen().GetScreens().Data as List<Screen>;
            List<string> GroupNames = new List<string>();
            foreach (Screen item in LOS)
            {
                GroupNames.Add(item.GroupName);
            }
            ViewBag.Screens = LOS;
            ViewBag.Groups = GroupNames;
            return View();
        }

        [HttpPost]
        public string AddAccount(string name, string pass, string per)
        {
            User A = new User();
            A.Password = pass;
            A.Status = 1;
            A.Username = name;
            string[] Access = per.Split('#');
            //List<Screen> LOUA = new List<Screen>();
            List<UserAccess> LOUA = new List<UserAccess>();
            foreach (string AP in Access)
            {
                if (LOUA.Any(p => p.ScreenID == Convert.ToInt32(AP.Replace("Opt1", "").Replace("Show", "").Replace("Edit", "").Replace("Delete", ""))))
                {
                    var UA = LOUA.Where(p => p.ScreenID == Convert.ToInt32(AP.Replace("Opt1", "").Replace("Show", "").Replace("Edit", "").Replace("Delete", ""))).SingleOrDefault();
                    if (AP.Contains("Opt1"))
                    {
                        UA.ScreenID = int.Parse(AP.Replace("Opt1", ""));
                        UA.Opt1 = true;
                        LOUA.Add(UA);
                    }
                    else if (AP.Contains("Show"))
                    {
                        UA.ScreenID = int.Parse(AP.Replace("Show", ""));
                        foreach (string item in Access.Where(p => p.Contains(UA.ScreenID.ToString())).ToList())
                        {
                            if (item.Contains("Opt1"))
                            {
                                UA.Opt1 = true;
                            }
                            if (item.Contains("Edit"))
                            {
                                UA.CanEdit = true;
                            }
                            if (item.Contains("Delete"))
                            {
                                UA.CanDelete = true;
                            }
                        }
                        LOUA.Add(UA);
                    }
                    else
                    {
                        if (AP.Contains("Edit"))
                        {
                            UA.ScreenID = int.Parse(AP.Replace("Edit", ""));
                            UA.CanEdit = true;
                        }
                        else if (AP.Contains("Delete"))
                        {
                            UA.ScreenID = int.Parse(AP.Replace("Delete", ""));
                            UA.CanDelete = true;
                        }
                        else
                        {
                            UA.ScreenID = int.Parse(AP);
                        }
                        LOUA.Add(UA);
                    }
                }
                else
                {
                    if (AP.Contains("Opt1"))
                    {
                        UserAccess UA = new UserAccess();
                        UA.ScreenID = int.Parse(AP.Replace("Opt1", ""));
                        UA.Opt1 = true;
                        LOUA.Add(UA);
                    }
                    else if (AP.Contains("Show"))
                    {
                        UserAccess UA = new UserAccess();
                        UA.ScreenID = int.Parse(AP.Replace("Show", ""));
                        foreach (string item in Access.Where(p => p.Contains(UA.ScreenID.ToString())).ToList())
                        {
                            if (item.Contains("Opt1"))
                            {
                                UA.Opt1 = true;
                            }
                            if (item.Contains("Edit"))
                            {
                                UA.CanEdit = true;
                            }
                            if (item.Contains("Delete"))
                            {
                                UA.CanDelete = true;
                            }
                        }
                        LOUA.Add(UA);
                    }
                    else
                    {
                        UserAccess UA = new UserAccess();
                        if (AP.Contains("Edit"))
                        {
                            UA.ScreenID = int.Parse(AP.Replace("Edit", ""));
                            UA.CanEdit = true;
                        }
                        else if (AP.Contains("Delete"))
                        {
                            UA.ScreenID = int.Parse(AP.Replace("Delete", ""));
                            UA.CanDelete = true;
                        }
                        else
                        {
                            UA.ScreenID = int.Parse(AP);
                        }
                        LOUA.Add(UA);
                    }
                }
            }
            Returner R = A.CreateAccount(LOUA);
            if (R.Message == Message.Username_Already_Exists)
            {
                return "false";
            }
            else
            {
                return "true";
            }
        }

        public ActionResult SearchUsers()
        {
            ViewBag.users = new User().GetAll().Data as List<User>;
            return View();
        }

        public string DeleteUser(int id)
        {
            User U = new User();
            U.ID = id;
            U.RemoveUser();
            return "true";
        }

        public ActionResult EditUser(int? id)
        {
            if (id == null)
            {
                if (Session["User"] != null)
                {
                    return RedirectToAction("SearchUsers", "User");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            List<Screen> LOS = new List<Screen>();
            LOS = new Screen().GetScreens().Data as List<Screen>;
            List<string> GroupNames = new List<string>();
            foreach (Screen item in LOS)
            {
                GroupNames.Add(item.GroupName);
            }
            ViewBag.Screens = LOS;
            ViewBag.Groups = GroupNames;
            User U = new User { ID = (int)id }.GetByID().Data as User;
            ViewBag.U = U;
            ViewBag.UserPer = U.UserAccesses;
            TempData["UID"] = (int)id;
            TempData.Keep();
            return View();
        }

        public string UpdateUser(string name, string pass, string per)
        {
            string[] Access = per.Split('#');
            //List<Screen> LOUA = new List<Screen>();
            List<UserAccess> LOUA = new List<UserAccess>();
            foreach (string AP in Access)
            {
                if (LOUA.Any(p => p.ScreenID == Convert.ToInt32(AP.Replace("Opt1", "").Replace("Show", "").Replace("Edit", "").Replace("Delete", ""))))
                {
                    var UA = LOUA.Where(p => p.ScreenID == Convert.ToInt32(AP.Replace("Opt1", "").Replace("Show", "").Replace("Edit", "").Replace("Delete", ""))).FirstOrDefault();
                    if (AP.Contains("Opt1"))
                    {
                        UA.ScreenID = int.Parse(AP.Replace("Opt1", ""));
                        UA.Opt1 = true;
                        LOUA.Add(UA);
                    }
                    else if (AP.Contains("Show"))
                    {
                        UA.ScreenID = int.Parse(AP.Replace("Show", ""));
                        foreach (string item in Access.Where(p => p.Contains(UA.ScreenID.ToString())).ToList())
                        {
                            if (item.Contains("Opt1"))
                            {
                                UA.Opt1 = true;
                            }
                            if (item.Contains("Edit"))
                            {
                                UA.CanEdit = true;
                            }
                            if (item.Contains("Delete"))
                            {
                                UA.CanDelete = true;
                            }
                        }
                        LOUA.Add(UA);
                    }
                    else
                    {
                        if (AP.Contains("Edit"))
                        {
                            UA.ScreenID = int.Parse(AP.Replace("Edit", ""));
                            UA.CanEdit = true;
                        }
                        else if (AP.Contains("Delete"))
                        {
                            UA.ScreenID = int.Parse(AP.Replace("Delete", ""));
                            UA.CanDelete = true;
                        }
                        else
                        {
                            UA.ScreenID = int.Parse(AP);
                        }
                        LOUA.Add(UA);
                    }
                }
                else
                {
                    if (AP.Contains("Opt1"))
                    {
                        UserAccess UA = new UserAccess();
                        UA.ScreenID = int.Parse(AP.Replace("Opt1", ""));
                        UA.Opt1 = true;
                        LOUA.Add(UA);
                    }
                    else if (AP.Contains("Show"))
                    {
                        UserAccess UA = new UserAccess();
                        UA.ScreenID = int.Parse(AP.Replace("Show", ""));
                        foreach (string item in Access.Where(p => p.Contains(UA.ScreenID.ToString())).ToList())
                        {
                            if (item.Contains("Opt1"))
                            {
                                UA.Opt1 = true;
                            }
                            if (item.Contains("Edit"))
                            {
                                UA.CanEdit = true;
                            }
                            if (item.Contains("Delete"))
                            {
                                UA.CanDelete = true;
                            }
                        }
                        LOUA.Add(UA);
                    }
                    else
                    {
                        UserAccess UA = new UserAccess();
                        if (AP.Contains("Edit"))
                        {
                            UA.ScreenID = int.Parse(AP.Replace("Edit", ""));
                            UA.CanEdit = true;
                        }
                        else if (AP.Contains("Delete"))
                        {
                            UA.ScreenID = int.Parse(AP.Replace("Delete", ""));
                            UA.CanDelete = true;
                        }
                        else
                        {
                            UA.ScreenID = int.Parse(AP);
                        }
                        LOUA.Add(UA);
                    }
                }
            }
            Returner R = new User
            {
                ID = (int)TempData["UID"],
                Username = name,
                Password = pass
            }.Update(LOUA);
            if (R.Message == Message.Username_Already_Exists)
            {
                TempData.Keep();
                return "false";
            }
            else
            {
                TempData.Keep();
                return "true";
            }
        }
    }
}
