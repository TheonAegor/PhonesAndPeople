using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PhonesAndPeople.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity;

namespace PhonesAndPeople.Controllers
{
    public class AccountController : Controller
    {
        UserContext UserDb = new UserContext();
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // поиск пользователя в бд
                User user = null;
                user = UserDb.Users.FirstOrDefault(u => u.Email == model.Name && u.Password == model.Password);
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(model.Name, true); //устанавливаем аутентификационные куки
                    if (user.RoleId == 1)
                        return RedirectToAction("AdminView");
                    else
                    {
                        using (PersonContext db = new PersonContext())
                        {
                            var person = db.People.Find(user.Id - 1);
                            return RedirectToAction("SimpleView", person);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Пользователя с таким логином и паролем нет");
                }
            }

            return View(model);
        }

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = null;
                user = UserDb.Users.FirstOrDefault(u => u.Email == model.Name);
                if (user == null)
                {
                    // создаем нового пользователя
                    UserDb.Users.Add(new User { Email = model.Name, Password = model.Password, RoleId = 2});
                    UserDb.SaveChanges();

                    user = UserDb.Users.Where(u => u.Email == model.Name && u.Password == model.Password).FirstOrDefault();
                    // если пользователь удачно добавлен в бд
                    if (user != null)
                    {
                        FormsAuthentication.SetAuthCookie(model.Name, true);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Пользователь с таким логином уже существует");
                }
            }

            return View(model);
        }
        public async Task<string> FillDbAsync()
        {
            string result = null;
            PersonContext srcDb = new PersonContext();
            int i = 1;

            InitializeRoles();
            InitializeAdmin();
            if (srcDb.Database.Exists() )
            {
                while (i < srcDb.People.Count())
                {
                    var person = await srcDb.People.FindAsync(i);
                    UserDb.Users.Add(new User { Email = person.Email, Password = person.Password, RoleId = 2 });
                    UserDb.SaveChanges();
                    i++;
                }
                result = "Было добавлено " + i + " новых пользователе!";
            }
            return result;
        }

        [Authorize]
        public ActionResult AdminView()
        {
            return View(UserDb.Users.ToList());
        }
        public ActionResult SimpleView(Person person)
        {
            return View(person);
        }
        public void InitializeAdmin()
        {
            UserDb.Users.Add(new User {RoleId = 1, Email = "admin", Password = "admin" });
            UserDb.SaveChanges();
        }

        public void InitializeRoles()
        {
            UserDb.Roles.Add(new Role { Id = 1, Name = "admin" });
            UserDb.Roles.Add(new Role { Id = 2, Name = "user" });
            UserDb.SaveChanges();
        }
        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}
