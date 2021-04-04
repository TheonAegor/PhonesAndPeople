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
using System.Data.Entity;

namespace PhonesAndPeople.Controllers
{
    public class HomeController : Controller
    {
        PersonContext db = new PersonContext();
        List<Person> ppl = new List<Person>();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowPeople(string fName = "", string sName = "", string lName = "", int page = 1)
        {
            IndexViewModel ivm = new IndexViewModel();
            int pageSize = 10; //amount of objects on page
            IEnumerable<Person> personsPerPages;
            Person pers = new Person();
            List<Person> people = new List<Person>();

            people = db.People.ToList();
            if (fName != "")
                people = people.Where(a => a.FirstName.Contains(fName)).ToList();
            if (sName != "")
                people = people.Where(a => a.SecondName.Contains(sName)).ToList();
            if (lName != "")
                people = people.Where(a => a.LastName.Contains(lName)).ToList();
            personsPerPages = db.People.ToList()
                    .OrderBy(x => x.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize).ToList();
            PageInfo pageInfo = new PageInfo { PageNumber = page, PageSize = pageSize, TotalItems = db.People.Count() };
            ivm.PageInfo = pageInfo;
            ivm.People = personsPerPages;
            return View(ivm);
        }
        [HttpPost]
        public ActionResult ShowPpl(string fName = "", string sName = "", string lName = "", int page = 1)
        {
            IndexViewModel ivm = new IndexViewModel();
            int pageSize = 10; //amount of objects on page
            IEnumerable<Person> personsPerPages;
            Person pers = new Person();
            List<Person> people = new List<Person>();

            people = db.People.ToList();
            if (fName != "")
                people = people.Where(a => a.FirstName.Contains(fName)).ToList();
            if (sName != "")
                people = people.Where(a => a.SecondName.Contains(sName)).ToList();
            if (lName != "")
                people = people.Where(a => a.LastName.Contains(lName)).ToList();

            personsPerPages = people
                    .OrderBy(x => x.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize).ToList();
            PageInfo pageInfo = new PageInfo { PageNumber = page, PageSize = pageSize, TotalItems = people.Count() };
            ivm.PageInfo = pageInfo;
            ivm.People = personsPerPages;

            return PartialView(ivm);
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
                    pers.DoB = str["dob"]["date"].Value<DateTime>();
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

        public ActionResult ShowSearchResult(int page = 1)
        {
            IndexViewModel ivm = new IndexViewModel();
            int pageSize = 10; //amount of objects on page
            IEnumerable<Person> personsPerPages;
            Person pers = new Person();
            List<Person> people = new List<Person>();

            personsPerPages = people
                    .OrderBy(x => x.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize).ToList();
            PageInfo pageInfo = new PageInfo { PageNumber = page, PageSize = pageSize, TotalItems = db.People.Count() };
            ivm.PageInfo = pageInfo;
            ivm.People = personsPerPages;

            return PartialView(ivm);
        }

        [HttpPost]
        public ActionResult NameSearch(string fName ="", string sName = "", string lName = "", int page = 1)
        {
            IndexViewModel ivm = new IndexViewModel();
            int pageSize = 10; //amount of objects on page
            IEnumerable<Person> personsPerPages;
            Person pers = new Person();

            List<Person> people = new List<Person>();
            people = db.People.ToList();
            if (fName != "")
                people = people.Where(a => a.FirstName.Contains(fName)).ToList();
            if (sName != "")
                people = people.Where(a => a.SecondName.Contains(sName)).ToList();
            if (lName != "")
                people = people.Where(a => a.LastName.Contains(lName)).ToList();

            personsPerPages = ppl
                    .OrderBy(x => x.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize).ToList();
            PageInfo pageInfo = new PageInfo { PageNumber = page, PageSize = pageSize, TotalItems = db.People.Count() };
            ivm.PageInfo = pageInfo;
            ivm.People = personsPerPages;

            return PartialView();
        }

        public async Task<ActionResult> Sort(SortState sortOrder = SortState.FNameAsc)
        {
            IQueryable<Person> people = db.People;
            ViewData["FNameSort"] = sortOrder == SortState.FNameAsc ? SortState.FNameDesc : SortState.FNameAsc;
            ViewData["SNameSort"] = sortOrder == SortState.SNameAsc ? SortState.SNameDesc : SortState.SNameAsc;
            ViewData["LNameSort"] = sortOrder == SortState.LNameAsc ? SortState.LNameDesc : SortState.LNameAsc;

            if (sortOrder == SortState.FNameDesc) people = people.OrderByDescending(s => s.FirstName);
            if (sortOrder == SortState.FNameAsc) people = people.OrderBy(s => s.FirstName);
            if (sortOrder == SortState.SNameAsc) people = people.OrderBy(s => s.SecondName);
            if (sortOrder == SortState.SNameDesc) people = people.OrderByDescending(s => s.SecondName);
            if (sortOrder == SortState.LNameAsc) people = people.OrderBy(s => s.LastName); 
            if (sortOrder == SortState.LNameDesc) people = people.OrderByDescending(s => s.SecondName);
            return View(await people.AsNoTracking().ToListAsync());
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