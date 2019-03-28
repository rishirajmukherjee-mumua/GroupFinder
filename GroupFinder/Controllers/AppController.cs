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
                if(hasClassMateAnsweredQuestions(item.ClassMateId))
                {
                    return View("Matches");
                }
                Questions questions = new Questions();
                questions.classmate = item;
                questions.idealSaturdays = db.IdealSaturdays.ToList();
                questions.vacations = db.Vacations.ToList();
                questions.classmateid = item.ClassMateId;
                questions.foods = db.Foods.ToList();
                string weather = await getWeather();
                double conversion = Convert.ToDouble(weather);
                double farenheit = conversion - 273.15 * 33.8;
              ViewData["weather"] = farenheit;
                ViewData["classMateName"] = item.fullname;
                return View("Questions", questions);

            }
            else

            {
                ViewBag.NotValidUser = "Please enter the correct email and password";

            }


            return View("login");
        }


        public Boolean hasClassMateAnsweredQuestions(int classmateId)
        {
            var model = from r in db.ClassMateFoods
                        where r.ClassMateId == classmateId
               
                        select r;
            var item = model.FirstOrDefault();
            if(item != null)
            {
                return true;
            }
            return false;

        }


        public async Task<string> getWeather()
        {
            var client = new OpenWeatherMapClient("68b8644b93b1b90aec47e87b59f8612d");
            var currentWeather = await client.CurrentWeather.GetByName("Tampa");
            //Console.WriteLine(currentWeather.Weather.Value);
            return currentWeather.Temperature.Value.ToString();
        }

        public ActionResult Questions()
        {
            return View();
        }


        public ActionResult Matches()
        {
            return View();
        }

        public  SongGenre AddSongGenre(string songGenre, int classmateId)
        {
            var model = from r in db.SongGenres
                        where r.genre == songGenre && r.ClassMateId == classmateId
                        select r;
            SongGenre item = model.FirstOrDefault();
            if(item == null)
            {
                SongGenre songGenreDb = new SongGenre();
                songGenreDb.genre = songGenre;
                songGenreDb.ClassMateId = classmateId;
                db.SongGenres.Add(songGenreDb);
                db.SaveChanges();
                item = songGenreDb;
            }
            return item;
        }


        [HttpPost]
        public async Task<ActionResult> QuestionsSubmit(Questions questions)
        {
            ClassMateFood classMateFood = new ClassMateFood();
            classMateFood.ClassMateId = questions.classmateid;
            classMateFood.FoodId = questions.foodid;
            ClassMateVacation classMateVacation = new ClassMateVacation();
            classMateVacation.ClassMateId = questions.classmateid;
            classMateVacation.VacationId = questions.vactionid;
            IdealSaturdayClassMate idealSaturdayClassMate = new IdealSaturdayClassMate();
            idealSaturdayClassMate.ClassMateId = questions.classmateid;
            idealSaturdayClassMate.IdealSaturdayId = questions.idealsaturdayid;
            List<String> songGenres = await GetSpotifyGenreData(questions.songorartist);
            foreach(string genre in songGenres)
            {
                SongGenre songGenre = AddSongGenre(genre, questions.classmateid);
            }
            db.IdealSaturdayClassMates.Add(idealSaturdayClassMate);
            db.ClassMateVacations.Add(classMateVacation);
            db.ClassMateFoods.Add(classMateFood);
            db.SaveChanges();
            return View("ThankYou");
        }


        public async Task<List<String>> GetSpotifyGenreData(string searchTerm)
        {
            CredentialsAuth auth = new CredentialsAuth("078c7392711e4b978d6a3bd21984c93c", "8b7f7c3b96454fcd8b42193a179ca19b");
            Token token = await auth.GetToken();
            SpotifyWebAPI api = new SpotifyWebAPI() { TokenType = token.TokenType, AccessToken = token.AccessToken };
            SearchItem item = api.SearchItems(searchTerm, SearchType.All, 10, 0, "US");
            List<FullArtist> artists = item.Artists.Items.ToList();
            List<String> genreList = new List<String>();
            foreach (FullArtist artist in artists)
            {
                foreach (string genre in artist.Genres)
                {
                    genreList.Add(genre);
                }
            }
            return genreList;
        }



        public async Task<JsonResult> GetSpotifyData(string searchTerm)
        {
            CredentialsAuth auth = new CredentialsAuth("078c7392711e4b978d6a3bd21984c93c", "8b7f7c3b96454fcd8b42193a179ca19b");
            Token token = await auth.GetToken();
            SpotifyWebAPI api = new SpotifyWebAPI() { TokenType = token.TokenType, AccessToken = token.AccessToken };
            SearchItem item = api.SearchItems(searchTerm, SearchType.All, 10, 0, "US");
            List<FullArtist> artists = item.Artists.Items.ToList();
            List<AjaxArtist> ajaxArtists = new List<AjaxArtist>();
            foreach (FullArtist artist in artists)
            {
                AjaxArtist artistajax = new AjaxArtist();
                artistajax.Id = artist.Id;
                artistajax.Name = artist.Name;
                ajaxArtists.Add(artistajax);
            }
            return Json(ajaxArtists, JsonRequestBehavior.AllowGet);
        }

    }
}