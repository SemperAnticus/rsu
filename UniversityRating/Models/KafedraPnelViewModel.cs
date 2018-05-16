using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniversityRating.Models
{
    public class KafedraPanelViewModel
    {
        public Kafedra kafedra { get; set; }
        public List<TeacherStatusViewModel> tsvmList { get; set; } 
    }

    public class FacultyPanelViewModel
    {
        public Facility faculty { get; set; }
        public List<KafedraStatusViewModel> ksvmList { get; set; } 
    }
}