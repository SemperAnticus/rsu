using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UniversityRating.Models
{
    public class CheckMarksViewModel
    {
        public IEnumerable<CheckMarksForView> Data { get; set; }

        public SelectList Objects { get; set; }
        public string SelectedObjectId { get; set; }
        public SelectList Blocks { get; set; }
        public string SelectedBlockId { get; set; }
        public SelectList Categories { get; set; }
        public string SelectedCategoryId { get; set; }
    }

    public class CheckMarksForView
    {
        public int IdMarkTeacher { get; set; }
        public Сriteria_Teachers Criteria { get; set; }
        public int? CriteriaCount { get; set; }
        public string Name { get; set; }
        public List<Status_Doc_Teacher> Documents { get; set; }
        public bool HasDocument { get; set; }
        public int? OldStatus { get; set; }
        public int? NewStatus { get; set; }
        public DateTime Date { get; set; }


    }

    public class CheckViewModel
    {
        public int Object { get; set; }
        public Mark_Teachers MT { get; set; }
        public string Description { get; set; }
    }
}