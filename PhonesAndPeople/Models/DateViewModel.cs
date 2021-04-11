using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhonesAndPeople.Models
{
    public class DateViewModel
    {
        public SelectList Years { get; set; }
        public SelectList Months { get; set; }
        public SelectList Days { get; set; }
    }
}