using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UniversityRating.Models
{
    public class CriteriaListViewModel
    {
        public IEnumerable<Mark_Teachers> MarkTeachers { get; set; }
        public SelectList Categories { get; set; }
        public string SelectedCategoryId { get; set; }
    }
}