using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniversityRating.Models
{
    public class ShowMarksObject
    {
        public string CategoryName { get; set; }
        public List<Mark_Teachers> CategoryMarks { get; set; }
    }

    public class ShowMarksObjectKafedra
    {
       public List<ManualMarks> ManualMarks { get; set; }
       public List<CalculatedMarks> CalculatedMarks { get; set; }
    }

    public class ManualMarks
    {
        public string CategoryName { get; set; }
        public List<Mark_Kafedra> CategoryMarks { get; set; }
    }

    public class CalculatedMarks
    {
        public string CategoryName { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public decimal Points { get; set; }
    }

    public class ShowMarksObjectFaculty
    {
        public List<ManualMarksFaculty> ManualMarks { get; set; }
        public List<CalculatedMarks> CalculatedMarks { get; set; }
    }

    public class ManualMarksFaculty
    {
        public string CategoryName { get; set; }
        public List<Mark_Facility> CategoryMarks { get; set; }
    }
}