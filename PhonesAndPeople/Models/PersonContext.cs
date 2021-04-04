using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PhonesAndPeople.Models
{
    public class PersonContext : DbContext
    {
        public DbSet<Person> People { get; set; }
    }
    //public class PeopleDbInitializer : DropCreateDatabaseAlways<PersonContext>
    //{
    //    protected override void Seed(PersonContext db)
    //    {
    //        base.Seed(db);
    //    }
    //}
}