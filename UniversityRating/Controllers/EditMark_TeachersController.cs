using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Web;
using System.Web.Mvc;
using UniversityRating.Models;

namespace UniversityRating.Controllers
{
    public class EditMark_TeachersController : Controller
    {
        private UniversityRatingEntities8 db = new UniversityRatingEntities8();

        // GET: EditMark_Teachers
        [HttpGet]
        public ActionResult Index()
        {
           int idCurrentTeacher=0;

            if (Session["Teacher_ID"] == null)
            {
                int idCurrentUser = (int) Session["User_ID"];
                Session["Teacher_ID"] = db.Teachers.Single(t => t.Id_User == idCurrentUser).Id;
            }

            ViewBag.Blocks = db.Block_Teachers.ToList();

            return View();
        }

        [HttpPost]
        public ActionResult Index(CriteriaListViewModel model)
        {
            int idCategory = Convert.ToInt32(model.SelectedCategoryId);
            int idCurrentTeacher = (int)Session["Teacher_ID"];
            List<Mark_Teachers> currentTeachersMarks = db.Mark_Teachers.Where(m => m.Id_teachers == idCurrentTeacher).ToList();
            List<Mark_Teachers> mark_Teachers =
                currentTeachersMarks.Where(m => m.Сriteria_Teachers.Id_Category == idCategory).ToList();

            List<Block_Teachers> block_teachers = db.Block_Teachers.ToList();
            List<Category_Teachers> category_teachers = db.Category_Teachers.ToList();
            CriteriaListViewModel clvm = new CriteriaListViewModel
            {
                MarkTeachers = mark_Teachers,
                Categories = new SelectList(category_teachers, "Id", "Name"),
                SelectedCategoryId = model.SelectedCategoryId,
            };
            ViewBag.Blocks = db.Block_Teachers.ToList();
            return View(clvm);
        }

        //Method GetCategories returns Categories for Block 
        public ActionResult GetCategories(int id)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            List<Category_Teachers> categoryTeachers = db.Category_Teachers.Where(c => c.Id_block == id).ToList();
            foreach (var ct in categoryTeachers)
            {
                items.Add(new SelectListItem() { Text = ct.Name, Value = ct.Id.ToString() });
            }
            // you may replace the above code with data reading from database based on the id
            if(items.Count==0) items.Add(new SelectListItem() { Text = "Нет категорий", Value = "0" });
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        //Method GetCriterias returns Criterias for Category 
        public ActionResult GetCriterias(int id)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            List<Сriteria_Teachers> criteriaTeachers = db.Сriteria_Teachers.Where(c => c.Id_Category == id).ToList();
            foreach (var ct in criteriaTeachers)
            {
                items.Add(new SelectListItem() { Text = ct.Name, Value = ct.Id.ToString() });
            }
            // you may replace the above code with data reading from database based on the id
            if (items.Count == 0) items.Add(new SelectListItem() { Text = "Нет категорий", Value = "0" });
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        // GET: EditMark_Teachers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mark_Teachers mark_Teachers = db.Mark_Teachers.Find(id);
            Mark_Teachers mark_Teachers1 = db.Mark_Teachers.Include(i => i.Status_Doc_Teacher).SingleOrDefault(i => i.Id == id);
            if (mark_Teachers == null)
            {
                return HttpNotFound();
            }
            return View(mark_Teachers);
        }

        // GET: EditMark_Teachers/Create
        public ActionResult Create(int id, int? idCategory)
        {
            ViewBag.CurrentBlockName = db.Block_Teachers.Single(b=>b.Id==id).Name;  
         
            int idCat;
            if (idCategory == null)
            {
                idCat = db.Category_Teachers.First(c => c.Id_block == id).Id;
            }
            else
            {
                idCat = idCategory.Value;
            }
            ViewBag.CurrentCategoryName = db.Category_Teachers.Single(c=>c.Id==idCat).Name;
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



            var prevCtg = db.Category_Teachers.OrderByDescending(a=>a.Id).Where(c => c.Id < idCat && c.Id_block == id).Take(1);
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
                            CriteriaId = (int) m.Id_Criteria,
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

        // POST: EditMark_Teachers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(List<MarkObject> marks, int id, int? idCategory)
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
                return RedirectToAction("Create/" + id, new { idCategory = idCat });
            }

            return RedirectToAction("Index");
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
            if(m.NewFiles==null || m.NewFiles[0]==null) newMarkTeachers.Status = -2;
            else newMarkTeachers.Status = 0;
            newMarkTeachers.Kolvo_Mark = selectedCriteria.Mark*m.Count;
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

            if (m.IsRemoved!=null)
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

                editMarkT.Kolvo_Mark = selectedCriteria.Mark*m.Count;
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

        // GET: EditMark_Teachers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mark_Teachers mark_Teachers = db.Mark_Teachers.Find(id);

            if (mark_Teachers == null)
            {
                return HttpNotFound();
            }

            List<Category_Teachers> cts = db.Category_Teachers.Where(c => c.Id_block == mark_Teachers.Сriteria_Teachers.Category_Teachers.Id_block).ToList();

            List<Сriteria_Teachers> сrts =
                db.Сriteria_Teachers.Where(c => c.Id_Category == mark_Teachers.Сriteria_Teachers.Id_Category).ToList();

            MarkTeacherViewModel mtvm = new MarkTeacherViewModel()
            {
                Categories = new SelectList(cts, "Id", "Name"),
                SelectedCategoryId = mark_Teachers.Сriteria_Teachers.Id_Category.ToString(),
                Criterias = new SelectList(сrts, "Id", "Name"),
                SelectedCriteriaId = mark_Teachers.Id_Criteria.ToString(),
                Kolvo_ed = (int)mark_Teachers.Kolvo_ed
            };

           return View(mtvm);
        }

        // POST: EditMark_Teachers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(CriteriaListViewModel clvm)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(clvm).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //     return View(clvm);
        //}

        // GET: EditMark_Teachers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mark_Teachers mark_Teachers = db.Mark_Teachers.Find(id);
            ViewBag.IdBlock = mark_Teachers.Сriteria_Teachers.Category_Teachers.Block_Teachers.Id;

            if (mark_Teachers == null)
            {
                return HttpNotFound();
            }

            List<Category_Teachers> cts = db.Category_Teachers.Where(c => c.Id_block == mark_Teachers.Сriteria_Teachers.Category_Teachers.Id_block).ToList();

            List<Сriteria_Teachers> сrts =
                db.Сriteria_Teachers.Where(c => c.Id_Category == mark_Teachers.Сriteria_Teachers.Id_Category).ToList();

            MarkTeacherViewModel mtvm = new MarkTeacherViewModel();

            ViewBag.BlockStr = mark_Teachers.Сriteria_Teachers.Category_Teachers.Block_Teachers.Name;
            ViewBag.CategoryStr = mark_Teachers.Сriteria_Teachers.Category_Teachers.Name;
            ViewBag.CriteriaStr = mark_Teachers.Сriteria_Teachers.Name;
            ViewBag.Count = mark_Teachers.Kolvo_ed;

            return View(mtvm);
        }

        // POST: EditMark_Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int idBlock)
        {
            Status_Doc_Teacher doc = db.Status_Doc_Teacher.Single(d => d.Id_Mark_Teacher == id);
            db.Status_Doc_Teacher.Remove(doc);

            string fullPath = Request.MapPath("~/documents/" + doc.Link_Doc);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }

            Mark_Teachers mark_Teachers = db.Mark_Teachers.Find(id);
            db.Mark_Teachers.Remove(mark_Teachers);
            db.SaveChanges();
            return RedirectToAction("Index/" + 1);
        }

        public FileResult GetFile(string fileFullName, string fileType, string fileName)
        {     // Путь к файлу 
            string file_path = Server.MapPath("~/documents/"+fileFullName);
            // Тип файла - content-type    
            string file_type=fileType;
            // Имя файла 
            string file_name = fileName;
            return File(file_path,file_type, file_name);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

