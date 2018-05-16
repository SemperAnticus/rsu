using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniversityRating.Models;

namespace UniversityRating.Controllers
{
    public class HomeController : Controller
    {
        private UniversityRatingEntities8 db = new UniversityRatingEntities8();
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string login, string password)
        {
            ViewBag.user = db.Users;
            foreach (var u in ViewBag.user)
            {
                if (login == u.Login && password == u.Password)
                {
                    Session["User_Id"] = u.Id;
                    Session["auth"] = "yes";
                    Session["role"] = u.Id_Role;
                    if (Session["role"].Equals(2)) return Redirect("/TeachersRoom/MainMenu");
                    if (Session["role"].Equals(1)) return Redirect("/Admin/MainMenu");
                    if (Session["role"].Equals(3)) return Redirect("/ComissionRoom/MainMenu");
                }
            }

            Session["auth"] = "no";
            return Redirect("Login");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            ViewBag.roles = db.Roles;
            return View();
        }

        [HttpPost]
        public ActionResult Login(string login, string password)
        {
                ViewBag.user = db.Users;
                foreach (var u in ViewBag.user)
                {
                    if (login == u.Login && password == u.Password)
                    {
                         Session["User_Id"] = u.Id;
                        Session["auth"] = "yes";
                        Session["role"] = u.Id_Role;

                        if (Session["role"].Equals(2))
                        {
                            int idU = u.Id;
                            Teacher curT = db.Teachers.Single(t => t.Id_User == idU);
                        
                        if (db.Kafedras.Any(k => k.Id_TeacherZav==curT.Id))
                        {
                            Session["IsZav"] = true;
                        }

                        if (Session["Teacher_ID"] == null)
                        {
                            int idCurrentUser = (int)Session["User_ID"];
                            Session["Teacher_ID"] = db.Teachers.Single(t => t.Id_User == idCurrentUser).Id;
                        }
                        if (db.Facilities.Any(f => f.Id_TeacherDecan==curT.Id))
                        {
                            Session["IsDec"] = true;
                        }
                        return Redirect("/TeachersRoom/MainMenu");
                        }
                        if(Session["role"].Equals(1)) return Redirect("/Admin/MainMenu");
                        if (Session["role"].Equals(3)) return Redirect("/ComissionRoom/MainMenu");
                    }
                }
            
                Session["auth"] = "no";
                return Redirect("Index");
        }

        public ActionResult Logoff()
        {
            Session.RemoveAll();
            return Redirect("/Home");
        }

        public ActionResult WhatsMyLogin(char firstTLetter)
        {
            ViewBag.firstTLetter = firstTLetter;
            string fl = firstTLetter.ToString();
            List<Teacher> teachers = db.Teachers.Where(t=>t.Name.StartsWith(fl)).ToList();

            List<TeachersLogin> tl = new List<TeachersLogin>();
            foreach (var t in teachers)
            {
                tl.Add(new TeachersLogin()
                {
                    TName = t.Name,
                    TLogin = t.User.Login
                });
            }

            return View(tl);
        }
    }
}