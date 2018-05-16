using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniversityRating.Models
{
    public class LastMarksViewModel
    {
        public IEnumerable<Mark_Teachers> MarkTeachers { get; set; }
        public Teacher Teacher { get; set; }
        public int Staj { get; set; }
    }
}