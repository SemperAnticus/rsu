using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniversityRating.Models
{
   
    public class MarkObject
    {
        public bool IsUsing { get; set; }
        public string CriteriaName { get; set; }
        public int CriteriaId { get; set; }
        public int Count { get; set; }
        public bool HasDoc { get; set; }
        public List<HttpPostedFileBase> NewFiles { get; set; }
        public List<LoadedFiles> ExistingFiles { get; set; }
        public List<bool> IsRemoved { get; set; } 

    }

    public class LoadedFiles
    {
        public Status_Doc_Teacher file { get; set; }
        public bool IsRemoved { get; set; }
    }

    public class MarkObjectKafedra
    {
        public bool IsUsing { get; set; }
        public string CriteriaName { get; set; }
        public int CriteriaId { get; set; }
        public int Count { get; set; }
        public bool HasDoc { get; set; }
        public List<HttpPostedFileBase> NewFiles { get; set; }
        public List<LoadedFilesKafedra> ExistingFiles { get; set; }
        public List<bool> IsRemoved { get; set; }

    }

    public class LoadedFilesKafedra
    {
        public Status_Doc_Kafedra file { get; set; }
        public bool IsRemoved { get; set; }
    }

    public class MarkObjectFaculty
    {
        public bool IsUsing { get; set; }
        public string CriteriaName { get; set; }
        public int CriteriaId { get; set; }
        public int Count { get; set; }
        public bool HasDoc { get; set; }
        public List<HttpPostedFileBase> NewFiles { get; set; }
        public List<LoadedFilesFaculty> ExistingFiles { get; set; }
        public List<bool> IsRemoved { get; set; }

    }

    public class LoadedFilesFaculty
    {
        public Status_Doc_Facility file { get; set; }
        public bool IsRemoved { get; set; }
    }
}