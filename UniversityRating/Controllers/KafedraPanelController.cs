using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniversityRating.Models;

namespace UniversityRating.Controllers
{
    public class KafedraPanelController : Controller
    {
        private UniversityRatingEntities8 db = new UniversityRatingEntities8();
        // GET: KafedraPanel
        public ActionResult Index()
        {
            int idCurrentTeacher = (int) Session["Teacher_Id"];
            Kafedra kaf = db.Kafedras.Single(k => k.Id_TeacherZav == idCurrentTeacher);
            List<Teacher> kafTeachers = db.Teachers.Where(t => t.Id_Kafedra == kaf.Id).ToList();
            List<TeacherStatusViewModel> tsvmL = new List<TeacherStatusViewModel>();

            foreach (var t in kafTeachers)
            {
                int b1 = 0, b2 = 0, b3 = 0, b4 = 0, b5 = 0, bTotal = 0;
                if (db.Mark_Teachers.Any(m => m.Id_teachers == t.Id && m.Сriteria_Teachers.Category_Teachers.Block_Teachers.Id == 1))
                {
                    b1 = db.Mark_Teachers.Where(
                            m => m.Id_teachers == t.Id && m.Сriteria_Teachers.Category_Teachers.Block_Teachers.Id == 1)
                            .Sum(m => m.Kolvo_Mark);
                }
                if (db.Mark_Teachers.Any(m => m.Id_teachers == t.Id && m.Сriteria_Teachers.Category_Teachers.Block_Teachers.Id == 2))
                {
                    b2 = db.Mark_Teachers.Where(
                            m => m.Id_teachers == t.Id && m.Сriteria_Teachers.Category_Teachers.Block_Teachers.Id == 2)
                            .Sum(m => m.Kolvo_Mark);
                }
                if (db.Mark_Teachers.Any(m => m.Id_teachers == t.Id && m.Сriteria_Teachers.Category_Teachers.Block_Teachers.Id == 3))
                {
                    b3 = db.Mark_Teachers.Where(
                            m => m.Id_teachers == t.Id && m.Сriteria_Teachers.Category_Teachers.Block_Teachers.Id == 3)
                            .Sum(m => m.Kolvo_Mark);
                }
                if (db.Mark_Teachers.Any(m => m.Id_teachers == t.Id && m.Сriteria_Teachers.Category_Teachers.Block_Teachers.Id == 4))
                {
                    b4 = db.Mark_Teachers.Where(
                            m => m.Id_teachers == t.Id && m.Сriteria_Teachers.Category_Teachers.Block_Teachers.Id == 4)
                            .Sum(m => m.Kolvo_Mark);
                }
                if (db.Mark_Teachers.Any(m => m.Id_teachers == t.Id && m.Сriteria_Teachers.Category_Teachers.Block_Teachers.Id == 5))
                {
                    b5 = db.Mark_Teachers.Where(
                            m => m.Id_teachers == t.Id && m.Сriteria_Teachers.Category_Teachers.Block_Teachers.Id == 5)
                            .Sum(m => m.Kolvo_Mark);
                }
                if (db.Mark_Teachers.Any(m => m.Id_teachers == t.Id))
                {
                    bTotal = db.Mark_Teachers.Where(
                            m => m.Id_teachers == t.Id).Sum(m => m.Kolvo_Mark);
                }
                tsvmL.Add(new TeacherStatusViewModel()
                    {
                        IdTeacher = t.Id,
                        FIO = t.Name,
                        block1 = b1,
                        block2 = b2,
                        block3 = b3,
                        block4 = b4,
                        block5 = b5,
                        blockTotal = bTotal
                    });
            }
            
            KafedraPanelViewModel kpvm = new KafedraPanelViewModel()
            {
                kafedra = kaf,
                tsvmList = tsvmL
            };

            return View(kpvm);
        }

        public ActionResult KafedraBLocks()
        {
            ViewBag.Blocks = db.Block_Kafedra.ToList();

            return View();
        }


        public ActionResult CreateKafedra(int id, int? idCategory)
        {
            ViewBag.CurrentBlockName = db.Block_Teachers.Single(b => b.Id == id).Name;

            int idCat;
            if (idCategory == null)
            {
                idCat = db.Category_Kafedra.First(c => c.Id_Block == id && c.Criteria_Kafedra.Count!=0).Id;
            }
            else
            {
                idCat = idCategory.Value;
            }
            ViewBag.CurrentCategoryName = db.Category_Kafedra.Single(c => c.Id == idCat).Name;
            ViewBag.IdCategory = idCat;
            var nextCtg = db.Category_Kafedra.FirstOrDefault(c => c.Id > idCat && c.Id_Block == id && c.Criteria_Kafedra.Count != 0);
            if (nextCtg == null)
            {
                ViewBag.NextIdCategory = null;
            }
            else
            {
                ViewBag.NextIdCategory = db.Category_Kafedra.FirstOrDefault(c => c.Id > idCat && c.Id_Block == id && c.Criteria_Kafedra.Count != 0).Id;
            }



            var prevCtg = db.Category_Kafedra.OrderByDescending(a => a.Id).Where(c => c.Id < idCat && c.Id_Block == id && c.Criteria_Kafedra.Count != 0).Take(1);
            if (!prevCtg.Any())
            {
                ViewBag.PrevCategoryId = null;
            }
            else
            {
                ViewBag.PrevCategoryId = prevCtg.Single().Id;
            }

            List<Category_Kafedra> allCategories = db.Category_Kafedra.Where(a => a.Id_Block == id && a.Criteria_Kafedra.Count != 0).ToList();

            ViewBag.TotalNumCategory = allCategories.Count;

            ViewBag.CurrentNumCategory = allCategories.IndexOf(db.Category_Kafedra.Single(c => c.Id == idCat && c.Criteria_Kafedra.Count != 0)) + 1;


            int idCurrentTeacher = (int)Session["Teacher_ID"];
            int idCurrentKafedra = db.Kafedras.Single(k => k.Id_TeacherZav == idCurrentTeacher).Id;

            List<Mark_Kafedra> mKafedra =
                db.Mark_Kafedra.Where(
                    m => m.Id_Kafedra == idCurrentKafedra && m.Criteria_Kafedra.Category_Kafedra.Id == idCat).ToList();

            List<Criteria_Kafedra> cKafedra = db.Criteria_Kafedra.Where(c => c.Id_Category == idCat).ToList();

            List<MarkObjectKafedra> marks = new List<MarkObjectKafedra>();

            bool isHaveInDB = false;

            foreach (var item in cKafedra)
            {
                if (mKafedra.Any(mt => mt.Id_Criteria == item.Id))
                {
                    var m = mKafedra.Single(mt => mt.Id_Criteria == item.Id);
                    bool hasDoc = db.Status_Doc_Kafedra.Any(s => s.Id_Mark_Kafedra == m.Id);
                    List<LoadedFilesKafedra> ef = new List<LoadedFilesKafedra>();
                    var sdt = m.Status_Doc_Kafedra.ToList();
                    List<bool> bools = new List<bool>();
                    foreach (var d in sdt)
                    {
                        ef.Add(new LoadedFilesKafedra()
                        {
                            file = d,
                            IsRemoved = false
                        });
                        bools.Add(false);
                    }

                    if (m.Id_Criteria != null)
                        marks.Add(new MarkObjectKafedra()
                        {
                            IsUsing = true,
                            CriteriaId = (int)m.Id_Criteria,
                            CriteriaName = m.Criteria_Kafedra.Name,
                            Count = m.Kolvo_ed,
                            HasDoc = hasDoc,
                            ExistingFiles = ef,
                            IsRemoved = bools
                        });
                }
                else
                {
                    marks.Add(new MarkObjectKafedra()
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
        public ActionResult CreateKafedra(List<MarkObjectKafedra> marks, int id, int? idCategory)
        {
            int idCurrentTeacher = (int)Session["Teacher_ID"];
            int idCurrentKafedra = db.Kafedras.Single(k => k.Id_TeacherZav == idCurrentTeacher).Id;

            int idCat = 1;
            if (idCategory != null) idCat = idCategory.Value;

            savingChangesToDB(marks, idCurrentKafedra);

            @ViewBag.IdBLock = id;
            var nextCtgId = db.Category_Teachers.FirstOrDefault(c => c.Id_block == id && c.Id > idCat);

            if (idCategory != null)
            {
                @ViewBag.IdCategory = idCat;
                if (nextCtgId != null) @ViewBag.NextIdCategory = nextCtgId.Id; else @ViewBag.NextIdCategory = null;
                return RedirectToAction("CreateKafedra/" + id, new { idCategory = idCat });
            }

            return RedirectToAction("KafedraBLocks");
        }

        private void savingChangesToDB(List<MarkObjectKafedra> marks, int idCurrentKafedra)
        {
            if (marks != null)
            {
                foreach (var m in marks)
                {
                    Criteria_Kafedra selectedCriteria = db.Criteria_Kafedra.Single(c => c.Id == m.CriteriaId);

                    // if the record exists in database
                    if (db.Mark_Kafedra.Any(mt => mt.Id_Kafedra == idCurrentKafedra && mt.Id_Criteria == m.CriteriaId))
                    {
                        if (!m.IsUsing)
                        {
                            deleteMarkKafedra(idCurrentKafedra, m);
                        }
                        else
                        {
                            editMarkKafedra(idCurrentKafedra, m, selectedCriteria);
                        }
                    }
                    else //if the record doesn`t exists in database
                        if (m.IsUsing)
                    {
                        createMarkKafedra(idCurrentKafedra, m, selectedCriteria);
                    }
                }
            }
        }
        private void createMarkKafedra(int idCurrentKafedra, MarkObjectKafedra m, Criteria_Kafedra selectedCriteria)
        {
            Mark_Kafedra newMarkKafedra = new Mark_Kafedra();
            newMarkKafedra.Id_Kafedra = idCurrentKafedra;
            newMarkKafedra.Id_Criteria = m.CriteriaId;
            newMarkKafedra.Kolvo_ed = m.Count;
            if (m.NewFiles == null || m.NewFiles[0] == null) newMarkKafedra.Status = -2;
            else newMarkKafedra.Status = 0;
            newMarkKafedra.Kolvo_Mark = (int)selectedCriteria.Mark * m.Count;
            newMarkKafedra.Date = DateTime.Now;

            if (m.NewFiles != null)
            {
                newMarkKafedra.Status_Doc_Kafedra = new List<Status_Doc_Kafedra>();
                foreach (var item in m.NewFiles)
                {
                    if (item != null)
                    {
                        var document = new Status_Doc_Kafedra()
                        {
                            Link_Doc = Guid.NewGuid() + Path.GetFileName(item.FileName),
                            Name = item.FileName,
                            FileType = item.ContentType,
                            FileContent = item.ContentLength
                        };
                        newMarkKafedra.Status_Doc_Kafedra.Add(document);
                        item.SaveAs(Path.Combine(Server.MapPath("~/documents"), document.Link_Doc));
                    }

                }
            }
            db.Mark_Kafedra.Add(newMarkKafedra);
            db.SaveChanges();
        }
        private void editMarkKafedra(int idCurrentKafedra, MarkObjectKafedra m, Criteria_Kafedra selectedCriteria)
        {
            Mark_Kafedra editMarkK =
                db.Mark_Kafedra.Single(
                    mt => mt.Id_Kafedra == idCurrentKafedra && mt.Id_Criteria == m.CriteriaId);
            List<Status_Doc_Kafedra> oldFiles =
                db.Status_Doc_Kafedra.Where(d => d.Id_Mark_Kafedra == editMarkK.Id).ToList();

            if (m.Count != editMarkK.Kolvo_ed)
            {
                editMarkK.Kolvo_ed = m.Count;

                editMarkK.Kolvo_Mark = (int)selectedCriteria.Mark * m.Count;
                editMarkK.Date = DateTime.Now;
                db.SaveChanges();
            }
            refreshDocs(oldFiles, m, editMarkK);
        }
        private void refreshDocs(List<Status_Doc_Kafedra> oldFiles, MarkObjectKafedra m, Mark_Kafedra editMarkK)
        {

            bool isChanged = false;


            if (oldFiles.Count != 0)
            {
                for (int i = 0; i < m.IsRemoved.Count; i++)
                {
                    if (m.IsRemoved[i])
                    {
                        db.Status_Doc_Kafedra.Remove(oldFiles[i]);
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
                if (oldFiles.Count == 0) editMarkK.Status_Doc_Kafedra = new List<Status_Doc_Kafedra>();

                foreach (var nf in m.NewFiles)
                {
                    if (nf != null)
                    {
                        var document = new Status_Doc_Kafedra()
                        {
                            Link_Doc = Guid.NewGuid() + Path.GetFileName(nf.FileName),
                            Name = nf.FileName,
                            FileType = nf.ContentType,
                            FileContent = nf.ContentLength
                        };
                        editMarkK.Status_Doc_Kafedra.Add(document);
                        nf.SaveAs(Path.Combine(Server.MapPath("~/documents"), document.Link_Doc));

                        isChanged = true;
                    }

                }
            }

            if (isChanged)
            {

                editMarkK.Date = DateTime.Now;
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
                        editMarkK.Status = -2;
                        bc = true;
                    }
                }
                else if (m.NewFiles == null || m.NewFiles[0] == null)
                {
                    editMarkK.Status = -2;
                    bc = true;
                }

                if (!bc) editMarkK.Status = 0;
            }

            db.SaveChanges();

        }
        private void deleteMarkKafedra(int idCurrentKafedra, MarkObjectKafedra m)
        {
            Mark_Kafedra delMarkK =
                db.Mark_Kafedra.Single(
                    mt => mt.Id_Kafedra == idCurrentKafedra && mt.Id_Criteria == m.CriteriaId);
            

            if (m.IsRemoved != null)
            {
                List<Status_Doc_Kafedra> delDocs =
                    db.Status_Doc_Kafedra.Where(d => d.Id_Mark_Kafedra == delMarkK.Id).ToList();
                foreach (var d in delDocs)
                {
                    db.Status_Doc_Kafedra.Remove(d);
                    string fullPath = Request.MapPath("~/documents/" + d.Link_Doc);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
            }
            db.Mark_Kafedra.Remove(delMarkK);

            db.SaveChanges();
        }

        public ActionResult ShowMarks(int id, int? idKafedra)
        {
            ViewBag.IdBlock = id;

            ViewBag.Blocks = db.Block_Kafedra.ToList();


            #region setting ViewBag.Points
            Teacher CurrentTeacher = new Teacher();
            Kafedra CurrentKafedra = new Kafedra();
            if (idKafedra == null)
            {
                int idUser = (int)Session["User_ID"];
                CurrentTeacher = db.Teachers.Single(t => t.Id_User == idUser);
                CurrentKafedra = db.Kafedras.Single(t => t.Id_TeacherZav == CurrentTeacher.Id);
            }
            else
            {
                ViewBag.IdTeacher = idKafedra;
                CurrentKafedra = db.Kafedras.Single(t => t.Id == idKafedra);
            }
            ViewBag.TeacherName = CurrentTeacher.Name;
            ViewBag.KafedraName = CurrentKafedra.Name;

            if (db.Mark_Kafedra.Count(a => a.Id_Kafedra == CurrentKafedra.Id) != 0)
                ViewBag.WaitedTotalPoints = db.Mark_Kafedra.Where(a => a.Id_Kafedra == CurrentKafedra.Id).Sum(a => a.Kolvo_Mark);
            else ViewBag.WaitedTotalPoints = 0;

            if (db.Mark_Kafedra.Count(a => a.Id_Kafedra == CurrentKafedra.Id && a.Status == 1) != 0)
                ViewBag.CheckedTotalPoints =
                    db.Mark_Kafedra.Where(a => a.Id_Kafedra == CurrentKafedra.Id && a.Status == 1)
                        .Sum(a => a.Kolvo_Mark);
            else ViewBag.CheckedTotalPoints = 0;

            if (
                db.Mark_Kafedra.Count(
                    a => a.Id_Kafedra == CurrentKafedra.Id && a.Criteria_Kafedra.Category_Kafedra.Id_Block == id) != 0)
                ViewBag.WaitedBlockTotalPoints = db.Mark_Kafedra.Where(
                        a => a.Id_Kafedra == CurrentKafedra.Id && a.Criteria_Kafedra.Category_Kafedra.Id_Block == id)
                        .Sum(a => a.Kolvo_Mark);
            else ViewBag.WaitedBlockTotalPoints = 0;

            if (
                db.Mark_Kafedra.Count(
                    a => a.Id_Kafedra == CurrentKafedra.Id && a.Criteria_Kafedra.Category_Kafedra.Id_Block == id &&
                        a.Status == 1) != 0)
                ViewBag.CheckedBlockTotalPoints = db.Mark_Kafedra.Where(a =>
                           a.Id_Kafedra == CurrentKafedra.Id && a.Criteria_Kafedra.Category_Kafedra.Id_Block == id &&
                           a.Status == 1).Sum(a => a.Kolvo_Mark);
            else ViewBag.CheckedBlockTotalPoints = 0;
            #endregion

            var allCategories = db.Category_Kafedra.Where(a => a.Id_Block == id).ToList();
            var allMarks = db.Mark_Kafedra.Where(a => a.Criteria_Kafedra.Category_Kafedra.Id_Block == id && a.Id_Kafedra == CurrentKafedra.Id).ToList();

            List<ManualMarks> manualMarks = new List<ManualMarks>();
            foreach (var c in allCategories)
            {
                if (allMarks.Any(a => a.Criteria_Kafedra.Id_Category == c.Id))
                {
                    List<Mark_Kafedra> newMarks = allMarks.Where(a => a.Criteria_Kafedra.Id_Category == c.Id).ToList();
                    manualMarks.Add(new ManualMarks()
                    {
                        CategoryMarks = newMarks,
                        CategoryName = c.Name
                    });
                }
            }

            List<CalculatedMarks> calculatedMarks = new List<CalculatedMarks>();

            foreach (var cr in db.Calculation_Rules_Kafedra)
            {
                switch (cr.TypeOfOperating)
                {
                    case 0:
                       if (
                            db.Mark_Teachers.Count(
                                m => m.Id_Criteria == cr.Id_Criteria && m.Teacher.Id_Kafedra == CurrentKafedra.Id) != 0)
                        {
                            ViewBag.WaitedTotalPoints += db.Mark_Teachers.Where(
                                    m => m.Id_Criteria == cr.Id_Criteria && m.Teacher.Id_Kafedra == CurrentKafedra.Id)
                                    .Sum(m => m.Kolvo_Mark);
                        }

                        break;
                    case 1:
                       if (
                            db.Mark_Teachers.Count(
                                m => m.Id_Criteria == cr.Id_Criteria && m.Teacher.Id_Kafedra == CurrentKafedra.Id) != 0)
                        {
                            ViewBag.WaitedTotalPoints = db.Mark_Teachers.Where(
                                 m => m.Id_Criteria == cr.Id_Criteria && m.Teacher.Id_Kafedra == CurrentKafedra.Id)
                                 .Sum(m => m.Kolvo_Mark) / (decimal)db.Mark_Teachers.Where(
                                 m => m.Id_Criteria == cr.Id_Criteria && m.Teacher.Id_Kafedra == CurrentKafedra.Id)
                                 .Sum(m => m.Kolvo_ed);
                         }


                        break;
                }
            }

            foreach (var cr in db.Calculation_Rules_Kafedra.Where(c=>c.Category_Kafedra.Id_Block==id))
            {
                string cmName="";
                decimal cmPoints = 0;
                int cmCount=0;
                switch (cr.TypeOfOperating)
                {
                    case 0: cmName = cr.Сriteria_Teachers.Name + " (Сумма)";
                        if (
                            db.Mark_Teachers.Count(
                                m => m.Id_Criteria == cr.Id_Criteria && m.Teacher.Id_Kafedra == CurrentKafedra.Id) != 0)
                        {
                            cmPoints = db.Mark_Teachers.Where(
                                    m => m.Id_Criteria == cr.Id_Criteria && m.Teacher.Id_Kafedra == CurrentKafedra.Id)
                                    .Sum(m => m.Kolvo_Mark);
                            cmCount = (int)db.Mark_Teachers.Where(
                                m => m.Id_Criteria == cr.Id_Criteria && m.Teacher.Id_Kafedra == CurrentKafedra.Id)
                                .Sum(m => m.Kolvo_ed);
                        }
                            
                        break;
                    case 1: cmName = cr.Сriteria_Teachers.Name + " (Среднее значение суммы)";
                        if (
                            db.Mark_Teachers.Count(
                                m => m.Id_Criteria == cr.Id_Criteria && m.Teacher.Id_Kafedra == CurrentKafedra.Id) != 0)
                        {
                            cmPoints = db.Mark_Teachers.Where(
                                 m => m.Id_Criteria == cr.Id_Criteria && m.Teacher.Id_Kafedra == CurrentKafedra.Id)
                                 .Sum(m => m.Kolvo_Mark) / (decimal)db.Mark_Teachers.Where(
                                 m => m.Id_Criteria == cr.Id_Criteria && m.Teacher.Id_Kafedra == CurrentKafedra.Id)
                                 .Sum(m => m.Kolvo_ed);
                            cmCount = (int)db.Mark_Teachers.Where(
                                m => m.Id_Criteria == cr.Id_Criteria && m.Teacher.Id_Kafedra == CurrentKafedra.Id)
                                .Sum(m => m.Kolvo_ed);
                        }
                         

                        break;
                }

                ViewBag.WaitedBlockTotalPoints += cmPoints;

                calculatedMarks.Add(new CalculatedMarks()
                {
                    CategoryName = cr.Category_Kafedra.Name,
                        Name = cmName,
                        Count = cmCount,
                        Points = cmPoints}
                );
            }


            ShowMarksObjectKafedra marks = new ShowMarksObjectKafedra()
            {
                ManualMarks = manualMarks,
                CalculatedMarks = calculatedMarks
            };

            return View(marks);
        }

        public ActionResult TeachersStatusPartial(List<TeacherStatusViewModel> tsvm)
        {
            return View(tsvm);
        }
    }
}