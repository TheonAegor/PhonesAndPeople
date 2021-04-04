using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PhonesAndPeople.Models;

namespace PhonesAndPeople.Controllers
{
    public class HomeController : Controller
    {
        PersonContext db = new PersonContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowPeople(int page = 1)
        {
            IndexViewModel ivm = new IndexViewModel();
            int pageSize = 10; //amount of objects on page
            IEnumerable<Person> personsPerPages;
            Person pers = new Person();

            personsPerPages = db.People.ToList()
                    .OrderBy(x => x.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize).ToList();
            PageInfo pageInfo = new PageInfo { PageNumber = page, PageSize = pageSize, TotalItems = db.People.Count() };
            ivm.PageInfo = pageInfo;
            ivm.People = personsPerPages;
            return View(ivm);
        }

        public async Task<RedirectToRouteResult> FillDbAsync(int page=1)
        {
            //IndexViewModel ivm = new IndexViewModel();
            //int pageSize = 10; //amount of objects on page
            //IEnumerable<Person> personsPerPages;
            Person pers = new Person();
            var client = new RestClient($"https://randomuser.me/api/?results=1000&nat=ru");
            var request = new RestRequest(Method.GET);
            IRestResponse response = await client.ExecuteAsync(request);
            int i = 0;
            if (response.IsSuccessful )
            {
                var content = JsonConvert.DeserializeObject<JToken>(response.Content);
                while (i < 1000)
                {
                    var str = content["results"][i];
                    pers.FirstName = str["name"]["title"].Value<string>();
                    pers.SecondName = str["name"]["first"].Value<string>();
                    pers.LastName = str["name"]["last"].Value<string>();
                    pers.Phone = str["phone"].Value<string>();
                    pers.Email = str["email"].Value<string>();
                    pers.Picture = str["picture"]["medium"].Value<string>();
                    pers.PictureBig = str["picture"]["large"].Value<string>();
                    db.People.Add(pers);
                    db.SaveChanges();
                    i++;
                }
            }
            //personsPerPages = db.People.ToList()
            //        .OrderBy(x => x.Id)
            //        .Skip((page - 1) * pageSize)
            //        .Take(pageSize).ToList();
            //PageInfo pageInfo = new PageInfo { PageNumber = page, PageSize = pageSize, TotalItems = db.People.Count() };
            //ivm.PageInfo = pageInfo;
            //ivm.People = personsPerPages;
            return RedirectToAction("ShowPeople");
        }

        [HttpPost]
        public ActionResult NameSearch(string fName, string sName, string lName)
        {
            var people = db.People.ToList();
            if (fName != "")
                people = people.Where(a => a.FirstName.Contains(fName)).ToList();
            if (sName != "")
                people = db.People.Where(a => a.SecondName.Contains(sName)).ToList();
            if (lName != "")
                people = people.Where(a => a.LastName.Contains(lName)).ToList();
            return PartialView(people);
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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}