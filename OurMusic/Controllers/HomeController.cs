using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OurMusic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OurMusic.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private OurMusicEntities db = new OurMusicEntities();
        public static UserManager<ApplicationUser> umanager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        private string LOGGEDIN_USER;
        public ActionResult Index()
        {
            Person p = getLoggedInPerson();
            ViewBag.LoggedInPerson = p;
            var rooms = db.Rooms.ToList();
            return View(rooms);
        }

        private Person getLoggedInPerson()
        {
            ApplicationUser user = umanager.FindById(User.Identity.GetUserId());
            var person = db.People.Where(x => x.userName == user.UserName).FirstOrDefault();
            return person;
        }
        public ActionResult Chat()
        {
            return View();
        }
    
    }
}