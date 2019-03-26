using GroupFinder.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SpotifyAPI.Web; //Base Namespace
using SpotifyAPI.Web.Auth; //All Authentication-related classes
using SpotifyAPI.Web.Enums; //Enums
using SpotifyAPI.Web.Models;
using System.Web.Script.Serialization;
using OpenWeatherMap;

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
        public async Task<ActionResult> Login(ClassMate classMate)
        {
            var model = from r in db.ClassMates
                        where r.loginhash == classMate.loginhash
                && r.email == classMate.email
                        select r;


            var item = model.FirstOrDefault();
            if (item != null)
            {
                Questions questions = new Questions();
                questions.classmate = item;
                questions.idealSaturdays = db.IdealSaturdays.ToList();
                questions.vacations = db.Vacations.ToList();
                questions.classmateid = item.ClassMateId;
                questions.foods = db.Foods.ToList();
                string weather = await getWeather();
                ViewData["weather"] = weather;
                return View("Questions", questions);

            }
            else

            {
                ViewBag.NotValidUser = item;

            }


            return View("login");
        }


        public  async Task<string> getWeather()
        {
            var client = new OpenWeatherMapClient("68b8644b93b1b90aec47e87b59f8612d");
            var currentWeather = await client.CurrentWeather.GetByName("Tampa");
            //Console.WriteLine(currentWeather.Weather.Value);
            return currentWeather.Temperature.Value.ToString() + " " +  currentWeather.Temperature.Unit + " outside you will see : " 
                +  currentWeather.Weather.Value;
        }

        public ActionResult Questions()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> QuestionsSubmit(Questions questions)
        {
            ClassMateFood classMateFood = new ClassMateFood();
            classMateFood.ClassMateId = questions.classmateid;
            classMateFood.FoodId = questions.foodid;
            ClassMateVacation classMateVacation= new ClassMateVacation();
            classMateVacation.ClassMateId = questions.classmateid;
            classMateVacation.VacationId = questions.vactionid;
            IdealSaturdayClassMate idealSaturdayClassMate = new IdealSaturdayClassMate();
            idealSaturdayClassMate.ClassMateId = questions.classmateid;
            idealSaturdayClassMate.IdealSaturdayId = questions.idealsaturdayid;
            SearchItem listsongs = await GetSpotifyData(questions.songorartist);
            db.IdealSaturdayClassMates.Add(idealSaturdayClassMate);
            db.ClassMateVacations.Add(classMateVacation);
            db.ClassMateFoods.Add(classMateFood);
            db.SaveChanges();
            return View("ThankYou");
        }

      



        public async Task<SearchItem> GetSpotifyData(string searchTerm)
        {
            CredentialsAuth auth = new CredentialsAuth("078c7392711e4b978d6a3bd21984c93c", "8b7f7c3b96454fcd8b42193a179ca19b");
            Token token = await auth.GetToken();
            SpotifyWebAPI api = new SpotifyWebAPI() { TokenType = token.TokenType, AccessToken = token.AccessToken };
            SearchItem item =  api.SearchItems(searchTerm, SearchType.All, 10, 0, "US");
            return item;
        }
    }
}