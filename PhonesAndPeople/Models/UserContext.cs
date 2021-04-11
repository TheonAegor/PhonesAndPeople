using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PhonesAndPeople.Models
{
    public class UserContext : DbContext
    {
        public UserContext() : base("DefaultConnection")
        { }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
    }

    //public class UserDbInitializer : DropCreateDatabaseAlways<UserContext>
    //{
    //    protected override void Seed(UserContext db)
    //    {
    //        db.Roles.Add(new Role { Id = 1, Name = "admin" });
    //        db.Roles.Add(new Role { Id = 2, Name = "user" });
    //        db.Users.Add(new User { RoleId = 1, Email = "admin", Password = "admin" });
    //        base.Seed(db);
    //    }
    //}
}