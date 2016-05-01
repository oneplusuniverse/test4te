using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using TestFORTE.Models;

namespace TestFORTE.Controllers
{
    public class HomeController : Controller
    {
        private Database1Entities db = new Database1Entities();
        private Entities dbu = new Entities();
        
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to IDEAS REPOSITORY!";
            //System.Web.Security.Roles.AddUserToRole("anotheradmin","Administrators");//RemoveUserFromRole("anotheradmin", "user");   
            return View();
        }

        [Authorize]
        public ActionResult Ideas()
        {
            var ideas = (from Ideas in db.Ideas select Ideas).ToList();
            for (int i = 0; i < ideas.Count; i++)
            {
                    if ((ideas[i].deleted == 1) || (ideas[i].Author != User.Identity.Name)) { ideas.RemoveAt(i); i--; }
            }
            return View(ideas);
        }



        [Authorize]
        public ActionResult Create()
        {
            Idea newIdea = new Idea();
            return View(newIdea);
        }
        [Authorize]
        [HttpPost]
        public ActionResult Create(Idea newIdea)
        {
            try
            {
               newIdea.Author = User.Identity.Name;
               newIdea.deleted = 0;
               newIdea.CreationDate = DateTime.Now;
               newIdea.LastEdit = DateTime.Now;
                if (ModelState.IsValid)
                {
                    db.Ideas.Add(newIdea);
                    db.SaveChanges();
                    return RedirectToAction("Ideas");
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError(String.Empty, ex);
            }
            return View(newIdea);
        }

        [Authorize]
        public ActionResult Edit(int id)
        {
            var ideaedit = (from Ideas in db.Ideas where Ideas.ID == id select Ideas).First();
            return View(ideaedit);
        }
        [Authorize]
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var ideaedit = (from Ideas in db.Ideas where Ideas.ID == id select Ideas).First();
            try
            {
                ideaedit.LastEdit = DateTime.Now;
               UpdateModel(ideaedit);
               db.SaveChanges();
               return RedirectToAction("Ideas");
            }
            catch 
            {
                return RedirectToAction("Ideas");
            }
        }
        [Authorize]
        public ActionResult ModeratorEdit(int id)
        {
            var ideaedit = (from Ideas in db.Ideas where Ideas.ID == id select Ideas).First();
            return View(ideaedit);
        }
        [Authorize(Roles = "moderators")]
        [HttpPost]
        public ActionResult ModeratorEdit(int id, FormCollection collection)
        {
            var ideaedit = (from Ideas in db.Ideas where Ideas.ID == id select Ideas).First();
            try
            {
                ideaedit.LastEdit = DateTime.Now;
                UpdateModel(ideaedit);
                db.SaveChanges();
                return RedirectToAction("ModeratorsPanel");
            }
            catch
            {
                return RedirectToAction("ModeratorsPanel");
            }
        }
        [Authorize(Roles = "Administrators")]
        public ActionResult AdminEdit(int id)
        {
            var ideaedit = (from Ideas in db.Ideas where Ideas.ID == id select Ideas).First();
            return View(ideaedit);
        }
        [Authorize]
        [HttpPost]
        public ActionResult AdminEdit(int id, FormCollection collection)
        {
            var ideaedit = (from Ideas in db.Ideas where Ideas.ID == id select Ideas).First();
            try
            {
                ideaedit.LastEdit = DateTime.Now;
                UpdateModel(ideaedit);
                db.SaveChanges();
                return RedirectToAction("AdminPanel");
            }
            catch
            {
                return RedirectToAction("AdminPanel");
            }
        }
        [Authorize]
        public ActionResult Delete(int id)
        {
            var ideaedit = (from Ideas in db.Ideas where Ideas.ID == id select Ideas).First();
            ideaedit.deleted = 1;
            UpdateModel(ideaedit);
            db.SaveChanges();
            //if (User.IsInRole("Administrators")) { return RedirectToAction("AdminPanel"); }
            //else if (User.IsInRole("moderators")) { return RedirectToAction("ModeratorsPanel"); }
            //else
            //{
                return RedirectToAction("Ideas");
            //}
        }

        [Authorize(Roles = "Administrators, moderators")]
        public ActionResult DeleteFromPanel(int id)
        {
            var ideaedit = (from Ideas in db.Ideas where Ideas.ID == id select Ideas).First();
            ideaedit.deleted = 1;
            UpdateModel(ideaedit);
            db.SaveChanges();
            if (User.IsInRole("Administrators")) { return RedirectToAction("AdminPanel"); }
            else if (User.IsInRole("moderators")) { return RedirectToAction("ModeratorsPanel"); }
            else
            {
            return RedirectToAction("Ideas");
            }
        }

        [Authorize]
        public ActionResult Restore(int id)
        {
            var ideaedit = (from Ideas in db.Ideas where Ideas.ID == id select Ideas).First();
            ideaedit.deleted = 0;
            UpdateModel(ideaedit);
            db.SaveChanges();
            if (User.IsInRole("Administrators"))
            {
                return RedirectToAction("AdminPanel");
            }else
            {
                return RedirectToAction("ModeratorsPanel");
            }
        }

        [Authorize]
        public ActionResult Remove(int id)
        {
            var ideaedit = (from Ideas in db.Ideas where Ideas.ID == id select Ideas).First();
            ideaedit.deleted = 2;
            UpdateModel(ideaedit);
            db.SaveChanges();
            return RedirectToAction("AdminPanel");
        }

        [Authorize]
        public ActionResult AbortRemoving(int id)
        {
            var ideaedit = (from Ideas in db.Ideas where Ideas.ID == id select Ideas).First();
            ideaedit.deleted = 1;
            UpdateModel(ideaedit);
            db.SaveChanges();
            return RedirectToAction("AdminPanel");
        }

        [Authorize]
        public ActionResult UserRemove(int id)
        {
            var ideaedit = (from Ideas in db.Ideas where Ideas.ID == id select Ideas).First();
            db.Ideas.Remove(ideaedit);
            UpdateModel(ideaedit);
            db.SaveChanges();
            return RedirectToAction("Ideas");
        }

        [Authorize]
        public ActionResult UserRestore(int id)
        {
            var ideaedit = (from Ideas in db.Ideas where Ideas.ID == id select Ideas).First();
            ideaedit.deleted = 0;
            UpdateModel(ideaedit);
            db.SaveChanges();
            return RedirectToAction("Ideas");
        }


        [Authorize(Roles = "Administrators")]
        public ActionResult AdminPanel()
        {
            var ideas = (from Ideas in db.Ideas select Ideas).ToList().OrderBy(x=>x.Author);
            //var users = from Users
            return View(ideas);
        }

        [Authorize(Roles = "moderators")]
        public ActionResult ModeratorsPanel()
        {
            var ideas = (from Ideas in db.Ideas select Ideas).ToList();
            return View(ideas);
        }

        [Authorize(Roles = "Administrators")]
        public ActionResult ViewUsers()
        {
            return View();
        }


        [Authorize(Roles = "Administrators")]
        public ActionResult MakeUser(String username, String userrole)
        {
            System.Web.Security.Roles.RemoveUserFromRole(username, userrole);            
            System.Web.Security.Roles.AddUserToRole(username, "user");

            return RedirectToAction("ViewUsers");
        }

        [Authorize(Roles = "Administrators")]
        public ActionResult MakeModerator(String username, String userrole)
        {
            System.Web.Security.Roles.RemoveUserFromRole(username, userrole);
            System.Web.Security.Roles.AddUserToRole(username, "moderators");

            return RedirectToAction("ViewUsers");
        }

        [Authorize(Roles = "Administrators")]
        public ActionResult MakeAdmin(String username, String userrole)
        {
            System.Web.Security.Roles.RemoveUserFromRole(username, userrole);
            System.Web.Security.Roles.AddUserToRole(username, "Administrators");

            return RedirectToAction("ViewUsers");
        }

    }
}
