using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniversityRating.Models;

namespace UniversityRating.Controllers
{
    public class TeachersRoomController : Controller
    {
        private UniversityRatingEntities8 db = new UniversityRatingEntities8();

        public ActionResult ShowMarks(int id, int? idTeacher)
        {
            ViewBag.IdBlock = id;
            if (Session["Teacher_ID"] == null)
            {
                int idCurrentUser = (int)Session["User_ID"];
                Session["Teacher_ID"] = db.Teachers.Single(t => t.Id_User == idCurrentUser).Id;
            }

            ViewBag.Blocks = db.Block_Teachers.ToList();

            List<ShowMarksObject> marks = new List<ShowMarksObject>();

            Teacher CurrentTeacher = new Teacher();
            if (idTeacher == null)
            {
                int idUser = (int)Session["User_ID"];
                CurrentTeacher = db.Teachers.Single(t => t.Id_User == idUser);
            }
            else
            {
                ViewBag.IdTeacher = idTeacher;
                CurrentTeacher = db.Teachers.Single(t => t.Id == idTeacher);
            }
            ViewBag.TeacherName = CurrentTeacher.Name;

            if (db.Mark_Teachers.Count(a => a.Id_teachers == CurrentTeacher.Id) != 0)
                ViewBag.WaitedTotalPoints = db.Mark_Teachers.Where(a => a.Id_teachers == CurrentTeacher.Id).Sum(a => a.Kolvo_Mark);
            else ViewBag.WaitedTotalPoints = 0;

            if (db.Mark_Teachers.Count(a => a.Id_teachers == CurrentTeacher.Id && a.Status == 1) != 0)
                ViewBag.CheckedTotalPoints =
                    db.Mark_Teachers.Where(a => a.Id_teachers == CurrentTeacher.Id && a.Status == 1)
                        .Sum(a => a.Kolvo_Mark);
            else ViewBag.CheckedTotalPoints = 0;

            if (
                db.Mark_Teachers.Count(
                    a => a.Id_teachers == CurrentTeacher.Id && a.Сriteria_Teachers.Category_Teachers.Id_block == id) != 0)
                ViewBag.WaitedBlockTotalPoints = db.Mark_Teachers.Where(
                        a => a.Id_teachers == CurrentTeacher.Id && a.Сriteria_Teachers.Category_Teachers.Id_block == id)
                        .Sum(a => a.Kolvo_Mark);
            else ViewBag.WaitedBlockTotalPoints = 0;

            if (
                db.Mark_Teachers.Count(
                    a => a.Id_teachers == CurrentTeacher.Id && a.Сriteria_Teachers.Category_Teachers.Id_block == id &&
                        a.Status == 1) != 0)
                ViewBag.CheckedBlockTotalPoints = db.Mark_Teachers.Where(a =>
                           a.Id_teachers == CurrentTeacher.Id && a.Сriteria_Teachers.Category_Teachers.Id_block == id &&
                           a.Status == 1).Sum(a => a.Kolvo_Mark);
            else ViewBag.CheckedBlockTotalPoints = 0;

            var allCategories = db.Category_Teachers.Where(a => a.Id_block == id).ToList();
            var allMarks = db.Mark_Teachers.Where(a => a.Сriteria_Teachers.Category_Teachers.Id_block == id && a.Id_teachers == CurrentTeacher.Id).ToList();

            foreach (var c in allCategories)
            {
                if (allMarks.Any(a => a.Сriteria_Teachers.Id_Category == c.Id))
                {
                    List<Mark_Teachers> newMarks = allMarks.Where(a => a.Сriteria_Teachers.Id_Category == c.Id).ToList();
                    marks.Add(new ShowMarksObject()
                    {
                        CategoryMarks = newMarks,
                        CategoryName = c.Name
                    });
                }
            }
            return View(marks);
        }

        public ActionResult TeacherBLocks()
        {
            int idCurrentTeacher = 0;

            if (Session["Teacher_ID"] == null)
            {
                int idCurrentUser = (int)Session["User_ID"];
                Session["Teacher_ID"] = db.Teachers.Single(t => t.Id_User == idCurrentUser).Id;
            }

            ViewBag.Blocks = db.Block_Teachers.ToList();
            return View();
        }

        public ActionResult MainMenu()
        {
            int idCurrentUser = (int)Session["User_ID"];

            if (Session["Teacher_ID"] == null)
            {
                Session["Teacher_ID"] = db.Teachers.Single(t => t.Id_User == idCurrentUser).Id;
            }
            User curUser = db.Users.Single(u => u.Id == idCurrentUser);
            if (curUser.IsFirstTimeLogin) return RedirectToAction("ChangePassword");


            int id = (int) Session["User_ID"];
            Teacher CurrentTeacher = db.Teachers.Single(t => t.Id_User == id);

            if (db.Kafedras.Any(k => k.Id_TeacherZav == CurrentTeacher.Id)) ViewBag.IsZav = true;
            else ViewBag.IsZav = false;

            if (db.Facilities.Any(k => k.Id_TeacherDecan == CurrentTeacher.Id)) ViewBag.IsDec = true;
            else ViewBag.IsDec = false;

           string varIsVypusk = null, varIsState = null;
            if (CurrentTeacher.Is_Staff != null)
            {
                varIsState = (bool) CurrentTeacher.Is_Staff ? "Штатный" : "По совместительству";
            }
            if (CurrentTeacher.Kafedra != null && CurrentTeacher.Kafedra.Is_Released != null)
            {
                varIsVypusk = CurrentTeacher.Kafedra.Is_Released ? "Выпускающий" : "Не выпускающий";
            }

            AnketaTeacher anketa = new AnketaTeacher()
            {
                teacher = CurrentTeacher,
                isVypusk = varIsVypusk,
                isState = varIsState,
            };

            return View(anketa);
        }
        [HttpGet]
        public ActionResult ChangePassword()
        {
            if (checkSessionIsCorrect())
            {
                int idCurrentUser = (int)Session["User_ID"];

                ChangePasswordViewModel cpvm = new ChangePasswordViewModel()
                {
                    Login = db.Users.Single(u=>u.Id==idCurrentUser).Login
                };
                return View(cpvm);
            }
            return RedirectToAction("Logoff", "Home");
        }

        private bool checkSessionIsCorrect()
        {
            if (Session["User_Id"] != null)
            {
                return true;
            }
            return false;
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (checkSessionIsCorrect())
            {

                int idCurrentUser = (int)Session["User_ID"];

                User curUser = db.Users.Single(u => u.Id == idCurrentUser);
                curUser.Password = model.Password;
                curUser.IsFirstTimeLogin = false;
                db.SaveChanges();

                return RedirectToAction("Anketa");
            }
            return RedirectToAction("Logoff", "Home");
        }

        [HttpGet]
        public ActionResult Anketa()
        {
            int idUser = (int)Session["User_ID"];
            Teacher currentTeacher = db.Teachers.Single(t => t.Id_User == idUser);
            if (currentTeacher.Id_Kafedra != null)
            {
                ViewBag.Id_Kafedra = new SelectList(db.Kafedras.Where(k=>k.Id_Facility==currentTeacher.Kafedra.Id_Facility), "Id", "Name", currentTeacher.Id_Kafedra);
                ViewBag.Id_Facility = new SelectList(db.Facilities, "Id", "Name", currentTeacher.Kafedra.Id_Facility);
            }
            else
            {
                ViewBag.Id_Kafedra = new SelectList(db.Kafedras.Where(k=>k.Id_Facility==1), "Id", "Name", 0);
                ViewBag.Id_Facility = new SelectList(db.Facilities, "Id", "Name", 0);
            }
            if(currentTeacher.Id_Position!=null)
                ViewBag.Id_Position = new SelectList(db.Position_Teachers, "Id", "Name", currentTeacher.Id_Position);
            else
                ViewBag.Id_Position = new SelectList(db.Position_Teachers, "Id", "Name", 0);

            return View(currentTeacher);
        }

        [HttpPost]
        public ActionResult Anketa(Teacher model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int idUser = (int)Session["User_ID"];
            Teacher currentTeacher = db.Teachers.Single(t => t.Id_User == idUser);

            currentTeacher.Name = model.Name;
            currentTeacher.Id_Kafedra = model.Id_Kafedra;
            currentTeacher.Id_Position = model.Id_Position;
            currentTeacher.Is_Staff = model.Is_Staff;
            currentTeacher.Staj = model.Staj;
            db.SaveChanges();

            return RedirectToAction("MainMenu");
        }

        public ActionResult GetKafedras(int id)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            List<Kafedra> categoryTeachers = db.Kafedras.Where(c => c.Id_Facility == id).ToList();
            foreach (var ct in categoryTeachers)
            {
                items.Add(new SelectListItem() { Text = ct.Name, Value = ct.Id.ToString() });
            }
            // you may replace the above code with data reading from database based on the id
            if (items.Count == 0) items.Add(new SelectListItem() { Text = "Нет кафедры", Value = "0" });
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillMarks(int id, int? idCategory)
        {
            ViewBag.CurrentBlockName = db.Block_Teachers.Single(b => b.Id == id).Name;

            int idCat;
            if (idCategory == null)
            {
                idCat = db.Category_Teachers.First(c => c.Id_block == id).Id;
            }
            else
            {
                idCat = idCategory.Value;
            }
            ViewBag.CurrentCategoryName = db.Category_Teachers.Single(c => c.Id == idCat).Name;
            ViewBag.IdCategory = idCat;
            var nextCtg = db.Category_Teachers.FirstOrDefault(c => c.Id > idCat && c.Id_block == id);
            if (nextCtg == null)
            {
                ViewBag.NextIdCategory = null;
            }
            else
            {
                ViewBag.NextIdCategory = db.Category_Teachers.FirstOrDefault(c => c.Id > idCat && c.Id_block == id).Id;
            }

            var prevCtg = db.Category_Teachers.OrderByDescending(a => a.Id).Where(c => c.Id < idCat && c.Id_block == id).Take(1);
            if (!prevCtg.Any())
            {
                ViewBag.PrevCategoryId = null;
            }
            else
            {
                ViewBag.PrevCategoryId = prevCtg.Single().Id;
            }

            List<Category_Teachers> allCategories = db.Category_Teachers.Where(a => a.Id_block == id).ToList();

            ViewBag.TotalNumCategory = allCategories.Count;

            ViewBag.CurrentNumCategory = allCategories.IndexOf(db.Category_Teachers.Single(c => c.Id == idCat)) + 1;


            int idCurrentTeacher = (int)Session["Teacher_ID"];

            List<Mark_Teachers> mTeacher =
                db.Mark_Teachers.Where(
                    m => m.Id_teachers == idCurrentTeacher && m.Сriteria_Teachers.Category_Teachers.Id == idCat).ToList();

            List<Сriteria_Teachers> cTeacher = db.Сriteria_Teachers.Where(c => c.Id_Category == idCat).ToList();

            List<MarkObject> marks = new List<MarkObject>();

            bool isHaveInDB = false;

            foreach (var item in cTeacher)
            {
                if (mTeacher.Any(mt => mt.Id_Criteria == item.Id))
                {
                    var m = mTeacher.Single(mt => mt.Id_Criteria == item.Id);
                    bool hasDoc = db.Status_Doc_Teacher.Any(s => s.Id_Mark_Teacher == m.Id);
                    List<LoadedFiles> ef = new List<LoadedFiles>();
                    var sdt = m.Status_Doc_Teacher.ToList();
                    List<bool> bools = new List<bool>();
                    foreach (var d in sdt)
                    {
                        ef.Add(new LoadedFiles()
                        {
                            file = d,
                            IsRemoved = false
                        });
                        bools.Add(false);
                    }

                    if (m.Id_Criteria != null)
                        marks.Add(new MarkObject()
                        {
                            IsUsing = true,
                            CriteriaId = (int)m.Id_Criteria,
                            CriteriaName = m.Сriteria_Teachers.Name,
                            Count = m.Kolvo_ed.Value,
                            HasDoc = hasDoc,
                            ExistingFiles = ef,
                            IsRemoved = bools
                        });
                }
                else
                {
                    marks.Add(new MarkObject()
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
        public ActionResult FillMarks(List<MarkObject> marks, int id, int? idCategory)
        {
            int idCurrentTeacher = (int)Session["Teacher_ID"];
            int idCat = 1;
            if (idCategory != null) idCat = idCategory.Value;

            savingChangesToDB(marks, idCurrentTeacher);

            @ViewBag.IdBLock = id;
            var nextCtgId = db.Category_Teachers.FirstOrDefault(c => c.Id_block == id && c.Id > idCat);

            if (idCategory != null)
            {
                @ViewBag.IdCategory = idCat;
                if (nextCtgId != null) @ViewBag.NextIdCategory = nextCtgId.Id; else @ViewBag.NextIdCategory = null;
                return RedirectToAction("FillMarks/" + id, new { idCategory = idCat });
            }

            return RedirectToAction("TeacherBLocks");
        }

        private void savingChangesToDB(List<MarkObject> marks, int idCurrentTeacher)
        {
            if (marks != null)
            {
                foreach (var m in marks)
                {
                    Сriteria_Teachers selectedCriteria = db.Сriteria_Teachers.Single(c => c.Id == m.CriteriaId);

                    // if the record exists in database
                    if (db.Mark_Teachers.Any(mt => mt.Id_teachers == idCurrentTeacher && mt.Id_Criteria == m.CriteriaId))
                    {
                        if (!m.IsUsing)
                        {
                            deleteMarkTeacher(idCurrentTeacher, m);
                        }
                        else
                        {
                            editMarkTeacher(idCurrentTeacher, m, selectedCriteria);
                        }
                    }
                    else //if the record doesn`t exists in database
                        if (m.IsUsing)
                    {
                        createMarkTeacher(idCurrentTeacher, m, selectedCriteria);
                    }
                }
            }
        }

        private void createMarkTeacher(int idCurrentTeacher, MarkObject m, Сriteria_Teachers selectedCriteria)
        {
            Mark_Teachers newMarkTeachers = new Mark_Teachers();
            newMarkTeachers.Id_teachers = idCurrentTeacher;
            newMarkTeachers.Id_Criteria = m.CriteriaId;
            newMarkTeachers.Kolvo_ed = m.Count;
            if (m.NewFiles == null || m.NewFiles[0] == null) newMarkTeachers.Status = -2;
            else newMarkTeachers.Status = 0;
            newMarkTeachers.Kolvo_Mark = selectedCriteria.Mark * m.Count;
            newMarkTeachers.Date = DateTime.Now;

            if (m.NewFiles != null)
            {
                newMarkTeachers.Status_Doc_Teacher = new List<Status_Doc_Teacher>();
                foreach (var item in m.NewFiles)
                {
                    if (item != null)
                    {
                        var document = new Status_Doc_Teacher()
                        {
                            Link_Doc = Guid.NewGuid() + Path.GetFileName(item.FileName),
                            Name = item.FileName,
                            FileType = item.ContentType,
                            FileContent = item.ContentLength
                        };
                        newMarkTeachers.Status_Doc_Teacher.Add(document);
                        item.SaveAs(Path.Combine(Server.MapPath("~/documents"), document.Link_Doc));
                    }

                }
            }
            db.Mark_Teachers.Add(newMarkTeachers);
            db.SaveChanges();
        }

        private void deleteMarkTeacher(int idCurrentTeacher, MarkObject m)
        {
            Mark_Teachers delMarkT =
                db.Mark_Teachers.Single(
                    mt => mt.Id_teachers == idCurrentTeacher && mt.Id_Criteria == m.CriteriaId);

            if (m.IsRemoved != null)
            {
                List<Status_Doc_Teacher> delDocs =
                    db.Status_Doc_Teacher.Where(d => d.Id_Mark_Teacher == delMarkT.Id).ToList();
                foreach (var d in delDocs)
                {
                    db.Status_Doc_Teacher.Remove(d);
                    string fullPath = Request.MapPath("~/documents/" + d.Link_Doc);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
            }
            db.Mark_Teachers.Remove(delMarkT);

            db.SaveChanges();
        }

        private void editMarkTeacher(int idCurrentTeacher, MarkObject m, Сriteria_Teachers selectedCriteria)
        {
            Mark_Teachers editMarkT =
                db.Mark_Teachers.Single(
                    mt => mt.Id_teachers == idCurrentTeacher && mt.Id_Criteria == m.CriteriaId);
            List<Status_Doc_Teacher> oldFiles =
                db.Status_Doc_Teacher.Where(d => d.Id_Mark_Teacher == editMarkT.Id).ToList();

            if (m.Count != editMarkT.Kolvo_ed.Value)
            {
                editMarkT.Kolvo_ed = m.Count;

                editMarkT.Kolvo_Mark = selectedCriteria.Mark * m.Count;
                editMarkT.Date = DateTime.Now;
                db.SaveChanges();
            }
            refreshDocs(oldFiles, m, editMarkT);
        }

        private void refreshDocs(List<Status_Doc_Teacher> oldFiles, MarkObject m, Mark_Teachers editMarkT)
        {

            bool isChanged = false;


            if (oldFiles.Count != 0)
            {
                for (int i = 0; i < m.IsRemoved.Count; i++)
                {
                    if (m.IsRemoved[i])
                    {
                        db.Status_Doc_Teacher.Remove(oldFiles[i]);
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
                if (oldFiles.Count == 0) editMarkT.Status_Doc_Teacher = new List<Status_Doc_Teacher>();

                foreach (var nf in m.NewFiles)
                {
                    if (nf != null)
                    {
                        var document = new Status_Doc_Teacher()
                        {
                            Link_Doc = Guid.NewGuid() + Path.GetFileName(nf.FileName),
                            Name = nf.FileName,
                            FileType = nf.ContentType,
                            FileContent = nf.ContentLength
                        };
                        editMarkT.Status_Doc_Teacher.Add(document);
                        nf.SaveAs(Path.Combine(Server.MapPath("~/documents"), document.Link_Doc));

                        isChanged = true;
                    }

                }
            }

            if (isChanged)
            {

                editMarkT.Date = DateTime.Now;
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
                        editMarkT.Status = -2;
                        bc = true;
                    }
                }
                else if (m.NewFiles == null || m.NewFiles[0] == null)
                {
                    editMarkT.Status = -2;
                    bc = true;
                }

                if (!bc) editMarkT.Status = 0;
            }

            db.SaveChanges();

        }
    }
}