using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhonesAndPeople.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime DoB { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Picture { get; set; }
        public string PictureBig { get; set; }
    }

    public class PageInfo
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages
        {
            get { return (int)Math.Ceiling((decimal)TotalItems / PageSize);  }
        }
    }

    public class IndexViewModel
    {
        public IEnumerable<Person> People { get; set; }
        public PageInfo PageInfo { get; set; }
    }
    public enum SortState
    {
        FNameAsc,    // по имени по возрастанию
        FNameDesc,   // по имени по убыванию
        SNameAsc,    
        SNameDesc,
        LNameAsc,
        LNameDesc,
        AgeAsc, // по возрасту по возрастанию
        AgeDesc,    // по возрасту по убыванию
        CompanyAsc, // по компании по возрастанию
        CompanyDesc // по компании по убыванию
    }
}