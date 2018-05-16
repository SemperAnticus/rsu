using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniversityRating.Models;

namespace UniversityRating.Controllers
{
    public class RatingReportsController : Controller
    {
        UniversityRatingEntities8 db = new UniversityRatingEntities8();

        public ActionResult Teachers()
        {
            List<Teacher> profs = db.Teachers.Where(t => t.Id_Position == 1).ToList();
            List<Teacher> docents = db.Teachers.Where(t => t.Id_Position == 2).ToList();
            List<Teacher> seniorPis = db.Teachers.Where(t => t.Id_Position == 3).ToList();
            List<Teacher> pis = db.Teachers.Where(t => t.Id_Position == 4).ToList();

            ReportTeachers rp = new ReportTeachers()
            {
                ProfessorsList = profs,
                DocentsList = docents,
                SeniorPiList = seniorPis,
                PiList = pis
            };
            return View(rp);
        }

        public ActionResult Departments(bool IsRealesed)
        {
            ViewBag.IsRealesed = IsRealesed;
            List<KafedraReportElement> nominal = new List<KafedraReportElement>();

            if (IsRealesed)
            {
                foreach (var item in db.Kafedras.Where(k => k.Is_Released))
                {
                    nominal.Add(new KafedraReportElement()
                    {
                        KafedraName = item.Name,
                        FacultyName = item.Facility.Name,
                        BTotal = (double) item.TotalMark,
                        B1 = item.MarkB1,
                        B2 = item.MarkB2,
                        B3 = item.MarkB3,
                        B4 = item.MarkB4,
                        B5 = item.MarkB5
                    });
                }
            }
            else
            {
                foreach (var item in db.Kafedras.Where(k => !k.Is_Released))
                {
                    nominal.Add(new KafedraReportElement()
                    {
                        KafedraName = item.Name,
                        FacultyName = item.Facility.Name,
                        BTotal = (double)item.TotalMark,
                        B1 = item.MarkB1,
                        B2 = item.MarkB2,
                        B3 = item.MarkB3,
                        B4 = item.MarkB4,
                        B5 = item.MarkB5
                    });
                }
            }

            List<KafedraReportElement> kTotal = nominal.OrderByDescending(n => n.BTotal).ToList();
            List<KafedraReportElement> kB1 = nominal.OrderByDescending(n => n.B1).ToList();
            List<KafedraReportElement> kB2 = nominal.OrderByDescending(n => n.B2).ToList();
            List<KafedraReportElement> kB3 = nominal.OrderByDescending(n => n.B3).ToList();
            List<KafedraReportElement> kB4 = nominal.OrderByDescending(n => n.B4).ToList();
            List<KafedraReportElement> kB5 = nominal.OrderByDescending(n => n.B5).ToList();

            ReportKafedra rk = new ReportKafedra()
            {
                kafsByTotal = kTotal,
                kafsByB1 = kB1,
                kafsByB2 = kB2,
                kafsByB3 = kB3,
                kafsByB4 = kB4,
                kafsByB5 = kB5
            };

            return View(rk);
        }

        public ActionResult Faculties(bool isAverage, bool withUnRealesed)
        {
            switch (isAverage)
            {
                case true:
                    switch (withUnRealesed)
                    {
                        case true:
                            ViewBag.Regim = 1;
                            break;
                        case false:
                            ViewBag.Regim = 2;
                            break;
                    }
                    break;
                case false:
                    switch (withUnRealesed)
                    {
                        case true:
                            ViewBag.Regim = 3;
                            break;
                        case false:
                            ViewBag.Regim = 4;
                            break;
                    }
                    break;
            }
            List<Kafedra> kafedras = new List<Kafedra>();
           
            
            switch (withUnRealesed)
            {
                case true:
                    kafedras = db.Kafedras.ToList();
                    break;
                case false:
                    kafedras = db.Kafedras.Where(k => k.Is_Released).ToList();
                    break;
            }

            List<FacultyReportElement> nominal = new List<FacultyReportElement>();
             List<FacultyReportElement> fbt = new List<FacultyReportElement>();
             List<FacultyReportElement> fb1 = new List<FacultyReportElement>();
             List<FacultyReportElement> fb2 = new List<FacultyReportElement>();
             List<FacultyReportElement> fb3 = new List<FacultyReportElement>();
             List<FacultyReportElement> fb4 = new List<FacultyReportElement>();

            switch (isAverage)
            {
                case true:
                    foreach (var fac in db.Facilities)
                    {
                        nominal.Add(new FacultyReportElement()
                        {
                            FacultyName = fac.Name,
                            bTotal = (double)fac.TotalMark / kafedras.Count(k=>k.Id_Facility==fac.Id),
                            b1 = (double)fac.MarkB1 / kafedras.Count(k => k.Id_Facility == fac.Id),
                            b2 = (double)fac.MarkB2 / kafedras.Count(k => k.Id_Facility == fac.Id),
                            b3 = (double)fac.MarkB3 / kafedras.Count(k => k.Id_Facility == fac.Id),
                            b4 = (double)fac.MarkB4 / kafedras.Count(k => k.Id_Facility == fac.Id)
                        });
                    }                
                    break;
                case false:
                    foreach (var fac in db.Facilities)
                    {
                        nominal.Add(new FacultyReportElement()
                        {
                            FacultyName = fac.Name,
                            bTotal = (double)fac.TotalMark,
                            b1 = fac.MarkB1,
                            b2 = fac.MarkB2,
                            b3 = fac.MarkB3,
                            b4 = fac.MarkB4
                        });
                    }
                    break;
            }

            fbt = nominal.OrderByDescending(n => n.bTotal).ToList();
            fb1 = nominal.OrderByDescending(n => n.b1).ToList();
            fb2 = nominal.OrderByDescending(n => n.b2).ToList();
            fb3 = nominal.OrderByDescending(n => n.b3).ToList();
            fb4 = nominal.OrderByDescending(n => n.b4).ToList();


            ReportFaculty rf = new ReportFaculty()
            {
                facsByTotal = fbt,
                facsByB1 = fb1,
                facsByB2 = fb2,
                facsByB3 = fb3,
                facsByB4 = fb4
            };

            return View(rf);
        }
    }
}