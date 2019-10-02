using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    public class ToDosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        public ActionResult Index()
        {
        
            return View();
        }
        private IEnumerable<ToDo> GetMyToDos()
        {

            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
            IEnumerable<ToDo> TasksList = db.ToDos.ToList().Where(x => x.User == currentUser); 
            int countCompleted = 0;
            foreach(ToDo task in TasksList)
            {
                if (task.IsDone) {
                    countCompleted++;
                }
                
            }

            int todosCount = TasksList.Count();
            if (todosCount == 0) ViewBag.Percent = 0;
            else ViewBag.Percent = Math.Round(100f * ((float)countCompleted / todosCount));
            return TasksList;

        }
        public ActionResult BuildToDoTable()
        {
            return PartialView("_ToDoTablePartial", GetMyToDos());
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateToDo([Bind(Include = "Id,Description")] ToDo toDo)
        {
            if (ModelState.IsValid)
            {
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = db.Users.FirstOrDefault(
                   x => x.Id == currentUserId
                 );
                toDo.User = currentUser;
                toDo.IsDone = false;
                db.ToDos.Add(toDo);
                db.SaveChanges();
                
            }

            return PartialView("_ToDoTablePartial", GetMyToDos());
        }
        
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToDo toDo = db.ToDos.Find(id);

            if (toDo == null)
            {
                return HttpNotFound();
            }
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.FirstOrDefault(
               x => x.Id == currentUserId
             );
            if(toDo.User !=currentUser)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            return View(toDo);
        }

        [HttpPost]
        public ActionResult EditToDo(int? id,bool value)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToDo toDo = db.ToDos.Find(id);
            if (toDo == null)
            {
                return HttpNotFound();
            }
            else
            {
                toDo.IsDone = value;
                db.Entry(toDo).State = EntityState.Modified;
                db.SaveChanges();
            }
            return PartialView("_ToDoTablePartial", GetMyToDos());
        }


      
        [HttpPost]
        public ActionResult Delete(int id)
        {
            ToDo toDo = db.ToDos.Where(x => x.Id == id).FirstOrDefault();
            db.ToDos.Remove(toDo);
            db.SaveChanges();
            return PartialView("_ToDoTablePartial", GetMyToDos());
          
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
