using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniversityRating.Models
{
    public class TeacherStatusViewModel
    {
        public int IdTeacher { get; set; }
        public string FIO { get; set; }
        public int block1 { get; set; }
        public int block2 { get; set; }
        public int block3 { get; set; }
        public int block4 { get; set; }
        public int block5 { get; set; }
        public int blockTotal { get; set; }

    }

    public class KafedraStatusViewModel
    {
        public int IdKafedra { get; set; }
        public string Name { get; set; }
        public int block1 { get; set; }
        public int block2 { get; set; }
        public int block3 { get; set; }
        public int block4 { get; set; }
        public int block5 { get; set; }
        public int blockTotal { get; set; }

    }
}