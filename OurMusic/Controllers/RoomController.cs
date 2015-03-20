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

namespace OurMusic.Controllers
{
    public class RoomController : Controller
    {
        private OurMusicEntities db = new OurMusicEntities();

        // GET: /Room/
        public async Task<ActionResult> Index()
        {
            var rooms = db.Rooms.Include(r => r.Person);
            return View(await rooms.ToListAsync());
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
                return RedirectToAction("Index");
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
            db.Rooms.Remove(room);
            await db.SaveChangesAsync();
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
