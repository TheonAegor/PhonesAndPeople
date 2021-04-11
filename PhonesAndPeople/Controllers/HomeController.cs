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

        public async Task<ActionResult> Index()
        {
            using (var UserDb = new UserContext())
            {
                if (!UserDb.Database.Exists())
                {
                    var account = new AccountController();
                    await account.FillDbAsync();
                }
            }
            return RedirectToAction("ShowPeople");
        }
        [Authorize]
        public ActionResult Personal()
        {
            UserContext context = new UserContext();
            var user = new User();
            var person = new Person();
            
            user = context.Users.FirstOrDefault(u => u.Email == User.Identity.Name);
            if (user.Email != "admin")
            {
                person = db.People.FirstOrDefault(p => p.Email == user.Email);
                return RedirectToAction("SimpleView", "Account", person);
            }
            else
                return RedirectToAction("AdminView", "Account");
        }

        public ActionResult ShowPeople(SortState sortOrder = SortState.FNameAsc, string fName = "", string sName = "", string lName = "", int page = 1)
        {
            IndexViewModel ivm = new IndexViewModel();
            int pageSize = 10; //amount of objects on page
            IEnumerable<Person> personsPerPages;
            Person pers = new Person();
            List<Person> people = new List<Person>();

            ViewData["FNameSort"] = sortOrder == SortState.FNameAsc ? SortState.FNameDesc : SortState.FNameAsc;
            ViewData["SNameSort"] = sortOrder == SortState.SNameAsc ? SortState.SNameDesc : SortState.SNameAsc;
            ViewData["LNameSort"] = sortOrder == SortState.LNameAsc ? SortState.LNameDesc : SortState.LNameAsc;
            ViewData["Dob"] = sortOrder == SortState.DobAsc ? SortState.DobDesc : SortState.DobAsc;
            ViewData["LastSortAttr"] = sortOrder;
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
        public PartialViewResult ShowPpl(SortState sortOrder = SortState.FNameAsc, string fName = "", string sName = "", string lName = "", int page = 1)
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

            //IQueryable<Person> people = db.People;
            IQueryable<Person> ppl= people.AsQueryable(); ;
            
            ViewData["FNameSort"] = sortOrder == SortState.FNameAsc ? SortState.FNameDesc : SortState.FNameAsc;
            ViewData["SNameSort"] = sortOrder == SortState.SNameAsc ? SortState.SNameDesc : SortState.SNameAsc;
            ViewData["LNameSort"] = sortOrder == SortState.LNameAsc ? SortState.LNameDesc : SortState.LNameAsc;
            ViewData["Dob"] = sortOrder == SortState.DobAsc ? SortState.DobDesc : SortState.DobAsc;
            ViewData["LastSortAttr"] = sortOrder;

            if (sortOrder == SortState.FNameDesc) ppl = ppl.OrderByDescending(s => s.FirstName);
            if (sortOrder == SortState.FNameAsc) ppl = ppl.OrderBy(s => s.FirstName);
            if (sortOrder == SortState.SNameAsc) ppl = ppl.OrderBy(s => s.SecondName);
            if (sortOrder == SortState.SNameDesc) ppl = ppl.OrderByDescending(s => s.SecondName);
            if (sortOrder == SortState.LNameAsc) ppl = ppl.OrderBy(s => s.LastName);
            if (sortOrder == SortState.LNameDesc) ppl = ppl.OrderByDescending(s => s.SecondName);
            if (sortOrder == SortState.DobAsc) ppl = ppl.OrderBy(s => s.DoB);
            if (sortOrder == SortState.DobDesc) ppl = ppl.OrderByDescending(s => s.DoB);


            personsPerPages = ppl
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize).ToList();
            PageInfo pageInfo = new PageInfo { PageNumber = page, PageSize = pageSize, TotalItems = ppl.Count() };
            ivm.PageInfo = pageInfo;
            ivm.People = personsPerPages;

            return PartialView(ivm);
        }
        public async Task<RedirectToRouteResult> FillDbAsync(int page=1)
        {
            Person pers = new Person();
            var client = new RestClient($"https://randomuser.me/api/?results=1000&nat=ru");
            var request = new RestRequest(Method.GET);
            IRestResponse response = await client.ExecuteAsync(request);
            int i = 0;
            if (response.IsSuccessful )
            {
                var content = JsonConvert.DeserializeObject<JToken>(response.Content);
                while (i < content.Count())
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
                    pers.Password = str["login"]["password"].Value<string>();
                    db.People.Add(pers);
                    db.SaveChanges();
                    i++;
                }
            }
            return RedirectToAction("ShowPeople");
        }

        [Authorize(Roles = "admin")]
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
        [HttpGet]
        public ActionResult TestAjax()
        {
            return View();
        }

        [HttpPost]
        public PartialViewResult TestAjaxPost()
        {
            return PartialView();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}