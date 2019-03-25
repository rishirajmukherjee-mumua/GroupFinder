using GroupFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GroupFinder.Controllers
{
    public class AppController : Controller
    {
        private GROUP_FINDEREntities db = new GROUP_FINDEREntities();
        // GET: App
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(ClassMate classMate)
        {
            var model =  from r in db.ClassMates where r.loginhash == classMate.loginhash 
                                         && r.email == classMate.email select r;
            

            var item = model.FirstOrDefault();
            if (item !=null)
            {
                Questions questions = new Questions();
                questions.classmate = item;
                questions.idealSaturdays = db.IdealSaturdays.ToList();
                questions.vacations = db.Vacations.ToList();
                questions.foods = db.Foods.ToList();
                return View("Questions", questions);
            }
            else 

            {
                ViewBag.NotValidUser = item;

            }
           

            return View("login");
        }

        public ActionResult Questions()
        {
            return View();
        }
    }
}