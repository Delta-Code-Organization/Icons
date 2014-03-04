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
            List<Screen> LOUA = new List<Screen>();
            foreach (string AP in Access)
            {
                Screen UA = new Screen();
                UA.ID = int.Parse(AP);
                LOUA.Add(UA);
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
            ViewBag.UserPer = U.Screens;
            TempData["UID"] = (int)id;
            TempData.Keep();
            return View();
        }

        public string UpdateUser(string name, string pass, string per)
        {
            string[] Access = per.Split('#');
            List<Screen> LOUA = new List<Screen>();
            foreach (string AP in Access)
            {
                Screen UA = new Screen();
                UA.ID = int.Parse(AP);
                LOUA.Add(UA);
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
