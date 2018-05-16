using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace UniversityRating.Models
{
    public class AnketaTeacher
    {
        public Teacher teacher { get; set; }
        public string isState { get; set; }
        public string isVypusk { get; set; }

    }
}