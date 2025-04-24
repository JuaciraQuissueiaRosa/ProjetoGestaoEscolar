using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApiGestaoEscolar.Controllers
{
    public class HorariosController : Controller
    {
      
        // GET: Horarios
        public ActionResult Index()
        {
            return View();
        }

        // GET: Horarios/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Horarios/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Horarios/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Horarios/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Horarios/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Horarios/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Horarios/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
