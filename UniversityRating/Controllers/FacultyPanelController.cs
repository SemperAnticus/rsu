using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniversityRating.Models;

namespace UniversityRating.Controllers
{
    public class FacultyPanelController : Controller
    {
        private UniversityRatingEntities8 db = new UniversityRatingEntities8();
        // GET: KafedraPanel
        public ActionResult Index()
        {
            int idCurrentTeacher = (int)Session["Teacher_Id"];
            Facility fac = db.Facilities.Single(f => f.Id_TeacherDecan == idCurrentTeacher);

            List<Kafedra> facKafedras = db.Kafedras.Where(t => t.Id_Facility == fac.Id).ToList();
            List<KafedraStatusViewModel> ksvmL = new List<KafedraStatusViewModel>();

            foreach (var k in facKafedras)
            {
                int b1 = 0, b2 = 0, b3 = 0, b4 = 0, b5 = 0, bTotal = 0;
                if (db.Mark_Kafedra.Any(m => m.Id_Kafedra == k.Id && m.Criteria_Kafedra.Category_Kafedra.Block_Kafedra.Id == 1))
                {
                    b1 = db.Mark_Kafedra.Where(
                            m => m.Id_Kafedra == k.Id && m.Criteria_Kafedra.Category_Kafedra.Block_Kafedra.Id == 1)
                            .Sum(m => m.Kolvo_Mark);
                }
                if (db.Mark_Kafedra.Any(m => m.Id_Kafedra == k.Id && m.Criteria_Kafedra.Category_Kafedra.Block_Kafedra.Id == 2))
                {
                    b2 = db.Mark_Kafedra.Where(
                            m => m.Id_Kafedra == k.Id && m.Criteria_Kafedra.Category_Kafedra.Block_Kafedra.Id == 2)
                            .Sum(m => m.Kolvo_Mark);
                }
                if (db.Mark_Kafedra.Any(m => m.Id_Kafedra == k.Id && m.Criteria_Kafedra.Category_Kafedra.Block_Kafedra.Id == 3))
                {
                    b3 = db.Mark_Kafedra.Where(
                            m => m.Id_Kafedra == k.Id && m.Criteria_Kafedra.Category_Kafedra.Block_Kafedra.Id == 3)
                            .Sum(m => m.Kolvo_Mark);
                }
                if (db.Mark_Kafedra.Any(m => m.Id_Kafedra == k.Id && m.Criteria_Kafedra.Category_Kafedra.Block_Kafedra.Id == 4))
                {
                    b4 = db.Mark_Kafedra.Where(
                            m => m.Id_Kafedra == k.Id && m.Criteria_Kafedra.Category_Kafedra.Block_Kafedra.Id == 4)
                            .Sum(m => m.Kolvo_Mark);
                }
                if (db.Mark_Kafedra.Any(m => m.Id_Kafedra == k.Id && m.Criteria_Kafedra.Category_Kafedra.Block_Kafedra.Id == 5))
                {
                    b5 = db.Mark_Kafedra.Where(
                            m => m.Id_Kafedra == k.Id && m.Criteria_Kafedra.Category_Kafedra.Block_Kafedra.Id == 5)
                            .Sum(m => m.Kolvo_Mark);
                }
                if (db.Mark_Kafedra.Any(m => m.Id_Kafedra == k.Id))
                {
                    bTotal = db.Mark_Kafedra.Where(
                            m => m.Id_Kafedra == k.Id).Sum(m => m.Kolvo_Mark);
                }
                ksvmL.Add(new KafedraStatusViewModel()
                {
                    IdKafedra = k.Id,
                    Name = k.Name,
                    block1 = b1,
                    block2 = b2,
                    block3 = b3,
                    block4 = b4,
                    block5 = b5,
                    blockTotal = bTotal
                });
            }

            FacultyPanelViewModel kpvm = new FacultyPanelViewModel()
            {
                faculty = fac,
                ksvmList = ksvmL
            };

            return View(kpvm);
        }

        public ActionResult FacultyBLocks()
        {
            ViewBag.Blocks = db.Block_Facility.ToList();

            return View();
        }


        public ActionResult FillMarks(int id, int? idCategory)
        {
            ViewBag.CurrentBlockName = db.Block_Facility.Single(b => b.Id == id).Name;

            int idCat;
            if (idCategory == null)
            {
                idCat = db.Category_Facility.First(c => c.Id_block == id && c.Criteria_Facility.Count!=0).Id;
            }
            else
            {
                idCat = idCategory.Value;
            }
            ViewBag.CurrentCategoryName = db.Category_Facility.Single(c => c.Id == idCat).Name;
            ViewBag.IdCategory = idCat;
            var nextCtg = db.Category_Facility.FirstOrDefault(c => c.Id > idCat && c.Id_block == id && c.Criteria_Facility.Count != 0);
            if (nextCtg == null)
            {
                ViewBag.NextIdCategory = null;
            }
            else
            {
                ViewBag.NextIdCategory = db.Category_Facility.FirstOrDefault(c => c.Id > idCat && c.Id_block == id && c.Criteria_Facility.Count != 0).Id;
            }



            var prevCtg = db.Category_Facility.OrderByDescending(a => a.Id).Where(c => c.Id < idCat && c.Id_block == id && c.Criteria_Facility.Count != 0).Take(1);
            if (!prevCtg.Any())
            {
                ViewBag.PrevCategoryId = null;
            }
            else
            {
                ViewBag.PrevCategoryId = prevCtg.Single().Id;
            }

            List<Category_Facility> allCategories = db.Category_Facility.Where(a => a.Id_block == id && a.Criteria_Facility.Count != 0).ToList();

            ViewBag.TotalNumCategory = allCategories.Count;

            ViewBag.CurrentNumCategory = allCategories.IndexOf(db.Category_Facility.Single(c => c.Id == idCat && c.Criteria_Facility.Count != 0)) + 1;


            int idCurrentTeacher = (int)Session["Teacher_ID"];
            int idCurrentFaculty = db.Facilities.Single(k => k.Id_TeacherDecan == idCurrentTeacher).Id;

            List<Mark_Facility> mFaculty =
                db.Mark_Facility.Where(
                    m => m.Id_Facility == idCurrentFaculty && m.Criteria_Facility.Category_Facility.Id == idCat).ToList();

            List<Criteria_Facility> cFaculty = db.Criteria_Facility.Where(c => c.Id_Category == idCat).ToList();

            List<MarkObjectFaculty> marks = new List<MarkObjectFaculty>();

            bool isHaveInDB = false;

            foreach (var item in cFaculty)
            {
                if (mFaculty.Any(mt => mt.Id_Criteria == item.Id))
                {
                    var m = mFaculty.Single(mt => mt.Id_Criteria == item.Id);
                    bool hasDoc = db.Status_Doc_Kafedra.Any(s => s.Id_Mark_Kafedra == m.Id);

                    List<LoadedFilesFaculty> ef = new List<LoadedFilesFaculty>();
                    var sdt = m.Status_Doc_Facility.ToList();
                    List<bool> bools = new List<bool>();
                    foreach (var d in sdt)
                    {
                        ef.Add(new LoadedFilesFaculty()
                        {
                            file = d,
                            IsRemoved = false
                        });
                        bools.Add(false);
                    }

                    if (m.Id_Criteria != null)
                        marks.Add(new MarkObjectFaculty()
                        {
                            IsUsing = true,
                            CriteriaId = (int)m.Id_Criteria,
                            CriteriaName = m.Criteria_Facility.Name,
                            Count = (int)m.Kolvo_ed,
                            HasDoc = hasDoc,
                            ExistingFiles = ef,
                            IsRemoved = bools
                        });
                }
                else
                {
                    marks.Add(new MarkObjectFaculty()
                    {
                        IsUsing = false,
                        CriteriaId = item.Id,
                        CriteriaName = item.Name,
                        Count = 1,
                        HasDoc = false,

                    });
                }

            }

            return View(marks);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateKafedra(List<MarkObjectFaculty> marks, int id, int? idCategory)
        {
            int idCurrentTeacher = (int)Session["Teacher_ID"];
            int idCurrentFaculty = db.Facilities.Single(k => k.Id_TeacherDecan == idCurrentTeacher).Id;

            int idCat = 1;
            if (idCategory != null) idCat = idCategory.Value;

            savingChangesToDB(marks, idCurrentFaculty);

            @ViewBag.IdBLock = id;
            var nextCtgId = db.Category_Facility.FirstOrDefault(c => c.Id_block == id && c.Id > idCat);

            if (idCategory != null)
            {
                @ViewBag.IdCategory = idCat;
                if (nextCtgId != null) @ViewBag.NextIdCategory = nextCtgId.Id; else @ViewBag.NextIdCategory = null;
                return RedirectToAction("FillMarks/" + id, new { idCategory = idCat });
            }

            return RedirectToAction("FacultyBLocks");
        }

        private void savingChangesToDB(List<MarkObjectFaculty> marks, int idCurrentFaculty)
        {
            if (marks != null)
            {
                foreach (var m in marks)
                {
                    Criteria_Facility selectedCriteria = db.Criteria_Facility.Single(c => c.Id == m.CriteriaId);

                    // if the record exists in database
                    if (db.Mark_Facility.Any(mt => mt.Id_Facility == idCurrentFaculty && mt.Id_Criteria == m.CriteriaId))
                    {
                        if (!m.IsUsing)
                        {
                            deleteMarkKafedra(idCurrentFaculty, m);
                        }
                        else
                        {
                            editMarkKafedra(idCurrentFaculty, m, selectedCriteria);
                        }
                    }
                    else //if the record doesn`t exists in database
                        if (m.IsUsing)
                    {
                        createMarkKafedra(idCurrentFaculty, m, selectedCriteria);
                    }
                }
            }
        }
        private void createMarkKafedra(int idCurrentFaculty, MarkObjectFaculty m, Criteria_Facility selectedCriteria)
        {
            Mark_Facility newMarkFaculty = new Mark_Facility();
            newMarkFaculty.Id_Facility = idCurrentFaculty;
            newMarkFaculty.Id_Criteria = m.CriteriaId;
            newMarkFaculty.Kolvo_ed = m.Count;
            if (m.NewFiles == null || m.NewFiles[0] == null) newMarkFaculty.Status = -2;
            else newMarkFaculty.Status = 0;
            newMarkFaculty.Kolvo_mark = (int)selectedCriteria.Mark * m.Count;
            newMarkFaculty.Date = DateTime.Now;

            if (m.NewFiles != null)
            {
                newMarkFaculty.Status_Doc_Facility = new List<Status_Doc_Facility>();
                foreach (var item in m.NewFiles)
                {
                    if (item != null)
                    {
                        var document = new Status_Doc_Facility()
                        {
                            Link_Doc = Guid.NewGuid() + Path.GetFileName(item.FileName),
                            Name = item.FileName,
                            FileType = item.ContentType,
                            FileContent = item.ContentLength
                        };
                        newMarkFaculty.Status_Doc_Facility.Add(document);
                        item.SaveAs(Path.Combine(Server.MapPath("~/documents"), document.Link_Doc));
                    }

                }
            }
            db.Mark_Facility.Add(newMarkFaculty);
            db.SaveChanges();
        }
        private void editMarkKafedra(int idCurrentFaculty, MarkObjectFaculty m, Criteria_Facility selectedCriteria)
        {
            Mark_Facility editMarkF =
                db.Mark_Facility.Single(
                    mt => mt.Id_Facility == idCurrentFaculty && mt.Id_Criteria == m.CriteriaId);
            List<Status_Doc_Facility> oldFiles =
                db.Status_Doc_Facility.Where(d => d.Id_Mark_Facility == editMarkF.Id).ToList();

            if (m.Count != editMarkF.Kolvo_ed)
            {
                editMarkF.Kolvo_ed = m.Count;

                editMarkF.Kolvo_mark = (int)selectedCriteria.Mark * m.Count;
                editMarkF.Date = DateTime.Now;
                db.SaveChanges();
            }
            refreshDocs(oldFiles, m, editMarkF);
        }
        private void refreshDocs(List<Status_Doc_Facility> oldFiles, MarkObjectFaculty m, Mark_Facility editMarkF)
        {

            bool isChanged = false;


            if (oldFiles.Count != 0)
            {
                for (int i = 0; i < m.IsRemoved.Count; i++)
                {
                    if (m.IsRemoved[i])
                    {
                        db.Status_Doc_Facility.Remove(oldFiles[i]);
                        string fullPath = Request.MapPath("~/documents/" + oldFiles[i].Link_Doc);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                        isChanged = true;
                    }
                }
            }

            if (m.NewFiles != null)
            {
                if (oldFiles.Count == 0) editMarkF.Status_Doc_Facility = new List<Status_Doc_Facility>();

                foreach (var nf in m.NewFiles)
                {
                    if (nf != null)
                    {
                        var document = new Status_Doc_Facility()
                        {
                            Link_Doc = Guid.NewGuid() + Path.GetFileName(nf.FileName),
                            Name = nf.FileName,
                            FileType = nf.ContentType,
                            FileContent = nf.ContentLength
                        };
                        editMarkF.Status_Doc_Facility.Add(document);
                        nf.SaveAs(Path.Combine(Server.MapPath("~/documents"), document.Link_Doc));

                        isChanged = true;
                    }

                }
            }

            if (isChanged)
            {

                editMarkF.Date = DateTime.Now;
                int cntRemoved = 0;
                if (m.IsRemoved != null)
                {
                    foreach (var r in m.IsRemoved)
                    {
                        if (r) cntRemoved++;
                    }
                }
                bool bc = false;
                if (m.ExistingFiles != null)
                {
                    if (m.ExistingFiles.Count == cntRemoved && (m.NewFiles == null || m.NewFiles[0] == null))
                    {
                        editMarkF.Status = -2;
                        bc = true;
                    }
                }
                else if (m.NewFiles == null || m.NewFiles[0] == null)
                {
                    editMarkF.Status = -2;
                    bc = true;
                }
                
                if (!bc) editMarkF.Status = 0;
            }

            db.SaveChanges();

        }
        private void deleteMarkKafedra(int idCurrentFaculty, MarkObjectFaculty m)
        {
            Mark_Facility delMarkF =
                db.Mark_Facility.Single(
                    mt => mt.Id_Facility == idCurrentFaculty && mt.Id_Criteria == m.CriteriaId);


            if (m.IsRemoved != null)
            {
                List<Status_Doc_Facility> delDocs =
                    db.Status_Doc_Facility.Where(d => d.Id_Mark_Facility == delMarkF.Id).ToList();
                foreach (var d in delDocs)
                {
                    db.Status_Doc_Facility.Remove(d);
                    string fullPath = Request.MapPath("~/documents/" + d.Link_Doc);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
            }
            db.Mark_Facility.Remove(delMarkF);

            db.SaveChanges();
        }

        public ActionResult ShowMarks(int id, int? idFaculty)
        {
            ViewBag.IdBlock = id;

            ViewBag.Blocks = db.Block_Facility.ToList();


            #region setting ViewBag.Points
            Teacher CurrentTeacher = new Teacher();
            Facility CurrentFaculty = new Facility();
            if (idFaculty == null)
            {
                int idUser = (int)Session["User_ID"];
                CurrentTeacher = db.Teachers.Single(t => t.Id_User == idUser);
                CurrentFaculty = db.Facilities.Single(f => f.Id_TeacherDecan == CurrentTeacher.Id);
            }
            else
            {
                ViewBag.IdFaculty = idFaculty;
                CurrentFaculty = db.Facilities.Single(t => t.Id == idFaculty);
            }
            ViewBag.TeacherName = CurrentTeacher.Name;
            ViewBag.FacultyName = CurrentFaculty.Name;

            if (db.Mark_Facility.Count(a => a.Id_Facility == CurrentFaculty.Id) != 0)
                ViewBag.WaitedTotalPoints = db.Mark_Facility.Where(a => a.Id_Facility == CurrentFaculty.Id).Sum(a => a.Kolvo_mark);
            else ViewBag.WaitedTotalPoints = 0;

            if (db.Mark_Facility.Count(a => a.Id_Facility == CurrentFaculty.Id && a.Status == 1) != 0)
                ViewBag.CheckedTotalPoints =
                    db.Mark_Facility.Where(a => a.Id_Facility == CurrentFaculty.Id && a.Status == 1)
                        .Sum(a => a.Kolvo_mark);
            else ViewBag.CheckedTotalPoints = 0;

            if (
                db.Mark_Facility.Count(
                    a => a.Id_Facility == CurrentFaculty.Id && a.Criteria_Facility.Category_Facility.Id_block == id) != 0)
                ViewBag.WaitedBlockTotalPoints = db.Mark_Facility.Where(
                        a => a.Id_Facility == CurrentFaculty.Id && a.Criteria_Facility.Category_Facility.Id_block == id)
                        .Sum(a => a.Kolvo_mark);
            else ViewBag.WaitedBlockTotalPoints = 0;

            if (
                db.Mark_Facility.Count(
                    a => a.Id_Facility == CurrentFaculty.Id && a.Criteria_Facility.Category_Facility.Id_block == id &&
                        a.Status == 1) != 0)
                ViewBag.CheckedBlockTotalPoints = db.Mark_Facility.Where(a =>
                           a.Id_Facility == CurrentFaculty.Id && a.Criteria_Facility.Category_Facility.Id_block == id &&
                           a.Status == 1).Sum(a => a.Kolvo_mark);
            else ViewBag.CheckedBlockTotalPoints = 0;
            #endregion

            var allCategories = db.Category_Facility.Where(a => a.Id_block == id).ToList();
            var allMarks = db.Mark_Facility.Where(a => a.Criteria_Facility.Category_Facility.Id_block == id && a.Id_Facility == CurrentFaculty.Id).ToList();

            List<ManualMarksFaculty> manualMarks = new List<ManualMarksFaculty>();
            foreach (var c in allCategories)
            {
                if (allMarks.Any(a => a.Criteria_Facility.Id_Category == c.Id))
                {
                    List<Mark_Facility> newMarks = allMarks.Where(a => a.Criteria_Facility.Id_Category == c.Id).ToList();
                    manualMarks.Add(new ManualMarksFaculty()
                    {
                        CategoryMarks = newMarks,
                        CategoryName = c.Name
                    });
                }
            }

            List<CalculatedMarks> calculatedMarks = new List<CalculatedMarks>();

            foreach (var cr in db.Calculation_Rules_Facility)
            {
                switch (cr.TypeOfOperating)
                {
                    case 0:
                        if (
                             db.Mark_Kafedra.Count(
                                 m => m.Id_Criteria == cr.Id_Criteria && m.Kafedra.Id_Facility == CurrentFaculty.Id) != 0)
                        {
                            ViewBag.WaitedTotalPoints += db.Mark_Kafedra.Where(
                                    m => m.Id_Criteria == cr.Id_Criteria && m.Kafedra.Id_Facility == CurrentFaculty.Id)
                                    .Sum(m => m.Kolvo_Mark);
                        }

                        break;
                    case 1:
                        if (
                             db.Mark_Kafedra.Count(
                                 m => m.Id_Criteria == cr.Id_Criteria && m.Kafedra.Id_Facility == CurrentFaculty.Id) != 0)
                        {
                            ViewBag.WaitedTotalPoints += db.Mark_Kafedra.Where(
                                 m => m.Id_Criteria == cr.Id_Criteria && m.Kafedra.Id_Facility == CurrentFaculty.Id)
                                 .Sum(m => m.Kolvo_Mark) / (decimal)db.Mark_Kafedra.Where(
                                 m => m.Id_Criteria == cr.Id_Criteria && m.Kafedra.Id_Facility == CurrentFaculty.Id)
                                 .Sum(m => m.Kolvo_ed);
                        }
                        break;
                    case 2:
                            if (
                                      db.Mark_Teachers.Count(
                                          m => m.Id_Criteria == cr.Calculation_Rules_Kafedra.Id_Criteria && m.Teacher.Kafedra.Id_Facility == CurrentFaculty.Id) != 0)
                            {
                            ViewBag.WaitedTotalPoints += db.Mark_Teachers.Where(
                                        m => m.Id_Criteria == cr.Calculation_Rules_Kafedra.Id_Criteria && m.Teacher.Kafedra.Id_Facility == CurrentFaculty.Id)
                                        .Sum(m => m.Kolvo_Mark);
                            }
                        
                        break;
                }
            }

            foreach (var cr in db.Calculation_Rules_Facility.Where(c => c.Category_Facility.Id_block == id))
            {
                string cmName = "";
                decimal cmPoints = 0;
                int cmCount = 0;
                switch (cr.TypeOfOperating)
                {
                    case 0:
                        cmName = cr.Criteria_Kafedra.Name + " (Сумма)";
                        if (
                            db.Mark_Kafedra.Count(
                                m => m.Id_Criteria == cr.Id_Criteria && m.Kafedra.Id_Facility == CurrentFaculty.Id) != 0)
                        {
                            cmPoints = db.Mark_Kafedra.Where(
                                    m => m.Id_Criteria == cr.Id_Criteria && m.Kafedra.Id_Facility == CurrentFaculty.Id)
                                    .Sum(m => m.Kolvo_Mark);
                            cmCount = (int)db.Mark_Kafedra.Where(
                                m => m.Id_Criteria == cr.Id_Criteria && m.Kafedra.Id_Facility == CurrentFaculty.Id)
                                .Sum(m => m.Kolvo_ed);
                        }

                        break;
                    case 1:
                        cmName = cr.Criteria_Kafedra.Name + " (Среднее значение суммы)";
                        if (
                            db.Mark_Kafedra.Count(
                                m => m.Id_Criteria == cr.Id_Criteria && m.Kafedra.Id_Facility == CurrentFaculty.Id) != 0)
                        {
                            cmPoints = db.Mark_Kafedra.Where(
                                 m => m.Id_Criteria == cr.Id_Criteria && m.Kafedra.Id_Facility == CurrentFaculty.Id)
                                 .Sum(m => m.Kolvo_Mark) / (decimal)db.Mark_Kafedra.Where(
                                 m => m.Id_Criteria == cr.Id_Criteria && m.Kafedra.Id_Facility == CurrentFaculty.Id)
                                 .Sum(m => m.Kolvo_ed);
                            cmCount = (int)db.Mark_Kafedra.Where(
                                m => m.Id_Criteria == cr.Id_Criteria && m.Kafedra.Id_Facility == CurrentFaculty.Id)
                                .Sum(m => m.Kolvo_ed);
                        }
                        break;

                    case 2:
                        cmName = cr.Calculation_Rules_Kafedra.Сriteria_Teachers.Category_Teachers.Name + " || " + cr.Calculation_Rules_Kafedra.Сriteria_Teachers.Name + " (Сумма)";

                        if (
                                  db.Mark_Teachers.Count(
                                      m => m.Id_Criteria == cr.Calculation_Rules_Kafedra.Id_Criteria && m.Teacher.Kafedra.Id_Facility == CurrentFaculty.Id) != 0)
                        {
                            cmPoints += db.Mark_Teachers.Where(
                                        m => m.Id_Criteria == cr.Calculation_Rules_Kafedra.Id_Criteria && m.Teacher.Kafedra.Id_Facility == CurrentFaculty.Id)
                                        .Sum(m => m.Kolvo_Mark);

                        }

                        break;
                }

                ViewBag.WaitedBlockTotalPoints += cmPoints;

                calculatedMarks.Add(new CalculatedMarks()
                {
                    Name = cmName,
                    Points = cmPoints,
                    Count = cmCount,
                    CategoryName = cr.Category_Facility.Name
                });
            }


            ShowMarksObjectFaculty marks = new ShowMarksObjectFaculty()
            {
                ManualMarks = manualMarks,
                CalculatedMarks = calculatedMarks
            };

            return View(marks);
        }

        public ActionResult KafedrasStatusPartial(List<KafedraStatusViewModel> ksvm)
        {
            return View(ksvm);
        }
    }
}