using DigitalSchoolGroups.Models;
using DigitalSchoolGroupsPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitalSchoolGroupsPlatform.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // ----------READ----------
        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            // Order categories in alphabetical order.
            var categories = from category in db.Categories
                             orderby category.CategoryName
                             select category;
            ViewBag.Categories = categories;
            return View();
        }

        // ----------READ ONE----------
        public ActionResult Show(int id)
        {
            Category category = db.Categories.Find(id);
            ViewBag.Category = category;
            return View();  // de modificat....
        }

        // ----------CREATE----------
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        public ActionResult New(Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Categories.Add(category);
                    db.SaveChanges();
                    TempData["message"] = "Category successfully added!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View();  //(category)
                }
            }
            catch (Exception e)
            {
                return View();
            }
        }

        // ----------UPDATE----------
        public ActionResult Edit(int id)
        {
            Category category = db.Categories.Find(id);
            ViewBag.Category = category;
            return View();
        }

        [HttpPut]
        public ActionResult Edit(int id, Category requestCategory)
        {
            try
            {
                Category category = db.Categories.Find(id);
                if (TryUpdateModel(category))
                {
                    category.CategoryName = requestCategory.CategoryName;
                    TempData["message"] = "Categoria a fost modificata!";
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View();
            }
        }

        // ----------DELETE----------
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();
            TempData["message"] = "Categoria a fost stearsa!";
            return RedirectToAction("Index");
        }
    }
}
