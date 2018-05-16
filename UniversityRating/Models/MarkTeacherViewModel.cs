using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UniversityRating.Models
{
    public class MarkTeacherViewModel
    {
        public SelectList Blocks { get; set; }
        public string SelectedBlockId { get; set; }
        public SelectList Categories { get; set; }
        public string SelectedCategoryId { get; set; }
        public SelectList Criterias { get; set; }
        public string SelectedCriteriaId { get; set; }
        [Display(Name = "Количество")]
        public int Kolvo_ed { get; set; }


    }
}