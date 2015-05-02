using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OurMusic.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using OurMusic.Hubs;

namespace OurMusic.Controllers
{
    public class RoomController : Controller
    {
        private OurMusicEntities db = new OurMusicEntities();
        public static UserManager<ApplicationUser> umanager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        private string LOGGEDIN_USER;
        private IHubContext _context = GlobalHost.ConnectionManager.GetHubContext<RoomHub>();
        // GET: /Room/
        public async Task<ActionResult> Index()
        {
            var rooms = db.Rooms.Include(r => r.Person);
            return View(await rooms.ToListAsync());
        }

        private Person getLoggedInPerson()
        {
            ApplicationUser user = umanager.FindById(User.Identity.GetUserId());
            var person = db.People.Where(x => x.userName == user.UserName).FirstOrDefault();
            return person;
        }

        // GET: /Room/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = await db.Rooms.FindAsync(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            ViewBag.Room = room;
            ViewBag.Person = getLoggedInPerson();
            return View(room);
        }

        // GET: /Room/Create
        public ActionResult Create()
        {
            ViewBag.administrator = new SelectList(db.People, "userID", "firstName");
            return View();
        }

        // POST: /Room/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="roomid,isPrivate,name,administrator")] Room room)
        {
            if (ModelState.IsValid)
            {
                room.roomid = Guid.NewGuid();
                db.Rooms.Add(room);
                await db.SaveChangesAsync();
                var roomtest = db.Rooms.Where(x => x.roomid == room.roomid).FirstOrDefault();
                roomtest.People.Add(getLoggedInPerson());
                roomtest.administrator = getLoggedInPerson().userID;
                await db.SaveChangesAsync();
                return RedirectToAction("Details", new { id = room.roomid });
            }

            ViewBag.administrator = new SelectList(db.People, "userID", "firstName", room.administrator);
            return RedirectToAction("Details", new { id = room.roomid });
        }

        // GET: /Room/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = await db.Rooms.FindAsync(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            ViewBag.administrator = new SelectList(db.People, "userID", "firstName", room.administrator);
            return View(room);
        }

        // POST: /Room/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="roomid,isPrivate,name,administrator")] Room room)
        {
            if (ModelState.IsValid)
            {
                db.Entry(room).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.administrator = new SelectList(db.People, "userID", "firstName", room.administrator);
            return View(room);
        }

        // GET: /Room/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = await db.Rooms.FindAsync(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            return View(room);
        }

        // POST: /Room/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            Room room = await db.Rooms.FindAsync(id);

            string roomName = room.name;
            var users = db.People.Where(x => x.activeRoom == room.roomid);
            foreach (var user in users)
                user.activeRoom = null;

            await db.SaveChangesAsync();

            db.Rooms.Remove(room);
            await db.SaveChangesAsync();
            RoomHub.alertRoomHasBeenDeleted(roomName);
            return RedirectToAction("Index");
        }

        // GET: /Room/Join/5
        public async Task<ActionResult> Join(Guid personid,Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = await db.Rooms.FindAsync(id);
            Person person = await db.People.FindAsync(personid);
            room.People.Add(person);
            await db.SaveChangesAsync();
            if (room == null)
            {
                return HttpNotFound();
            }
            return RedirectToAction("Details", new {id=id});
        }

        // GET: /Room/Leave/5
        public async Task<ActionResult> Leave(Guid personid, Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = await db.Rooms.FindAsync(id);
            Person person = await db.People.FindAsync(personid);
            room.People.Remove(person);
            await db.SaveChangesAsync();
            if (room == null)
            {
                return HttpNotFound();
            }
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
