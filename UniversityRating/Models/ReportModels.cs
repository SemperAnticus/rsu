using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc.Routing.Constraints;

namespace UniversityRating.Models
{
    public class ReportTeachers
    {
        public List<Teacher> ProfessorsList { get; set; } 
        public List<Teacher> DocentsList { get; set; }
        public List<Teacher> SeniorPiList { get; set; }
        public List<Teacher> PiList { get; set; } 
    }

    public class ReportKafedra
    {
        public List<KafedraReportElement> kafsByTotal { get; set; }
        public List<KafedraReportElement> kafsByB1 { get; set; }
        public List<KafedraReportElement> kafsByB2 { get; set; }
        public List<KafedraReportElement> kafsByB3 { get; set; }
        public List<KafedraReportElement> kafsByB4 { get; set; }
        public List<KafedraReportElement> kafsByB5 { get; set; }

    }

    public class KafedraReportElement
    {
        public string KafedraName { get; set; }
        public string FacultyName { get; set; }
        public double B1 { get; set; }
        public double B2 { get; set; }
        public double B3 { get; set; }
        public double B4 { get; set; }
        public double B5 { get; set; }
        public double BTotal { get; set; }

    }

    public class ReportFaculty
    {
        public List<FacultyReportElement> facsByTotal { get; set; }
        public List<FacultyReportElement> facsByB1 { get; set; }
        public List<FacultyReportElement> facsByB2 { get; set; }
        public List<FacultyReportElement> facsByB3 { get; set; }
        public List<FacultyReportElement> facsByB4 { get; set; }
    }

    public class FacultyReportElement
    {
        public string FacultyName { get; set; }
        public double b1 { get; set; }
        public double b2 { get; set; }
        public double b3 { get; set; }
        public double b4 { get; set; }
        public double bTotal { get; set; }
    }
}