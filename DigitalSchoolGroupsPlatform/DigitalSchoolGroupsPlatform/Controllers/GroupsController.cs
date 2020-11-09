using DigitalSchoolGroupsPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitalSchoolGroupsPlatform.Controllers
{
    public class GroupsController : Controller
    {
        // Am acces la elementele din BD prin instanta ArticleDBContext.
        private Models.AppContext db = new Models.AppContext();

        // GET: Groups
        public ActionResult Index()
        {
            // JOIN facut de framework in spate (pt fiecare articol, join cu categorii si ia categoria)
            var groups = db.Groups.Include("Category");
            ViewBag.Groups = groups;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }

        // GET implicit
        public ActionResult New()
        {
            Group group = new Group();
            group.Categ = GetAllCategories();

            /* // before:
            var categories = from category in db.Categories
                             select category;

            ViewBag.Categories = categories;
            */
            // Atentie daca pasam View() o sa dea eroare ca are categoriile null!
            // Trebuie sa trimit ca argument si categoriile.
            // Trimit article si e ok ca are si categoriile!
            // adaugam @model ArticlesApp.Models.Article la inceput in View.
            return View(group);
        }

        [HttpPost]
        public ActionResult New(Group group)
        {
            group.Date = DateTime.Now;

            try
            {
                db.Groups.Add(group);
                db.SaveChanges();
                TempData["message"] = "Articolul a fost adaugat.";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View(group);   // nu mai punem utilizatorul sa scrie inca o data ce a scris inainte
                // starea obiectului este pastrata!
            }
        }

        public ActionResult Show(int id)
        {
            Group group = db.Groups.Find(id);
            ViewBag.Group = group;
            ViewBag.Category = group.Category;

            return View();
        }

        public ActionResult Edit(int id)
        {
            Group group = db.Groups.Find(id);
            group.Categ = GetAllCategories();
            /*
            ViewBag.Article = article;
            ViewBag.Category = article.Category;

            var categories = from category in db.Categories
                             select category;
            ViewBag.Categories = categories;
            */
            return View(group);
        }

        [HttpPut]
        public ActionResult Edit(int id, Group requestGroup)
        {
            try
            {
                Group group = db.Groups.Find(id);
                if (TryUpdateModel(group))
                {
                    group = requestGroup;
                    /* article.Title = requestArticle.Title;
                    article.Content = requestArticle.Content;
                    article.Date = requestArticle.Date;
                    article.CategoryId = requestArticle.CategoryId; */
                    db.SaveChanges();
                    TempData["message"] = "Articolul a fost editat";
                    return RedirectToAction("Index");
                }
                return View(requestGroup);
            }
            catch (Exception e)
            {
                return View(requestGroup);
            }
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Group group = db.Groups.Find(id);
            db.Groups.Remove(group);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Prin [NonAction] ne asiguram ca metoda poate fi accesata de alte metode,
        [NonAction] // dar nu poate fi accesata niciodata prin URL!
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            // generam o lista goala
            var selectList = new List<SelectListItem>();
            // extragem toate categoriile din baza de date
            var categories = from cat in db.Categories
                             select cat;
            // iteram prin categorii
            foreach (var category in categories)
            {
                // adaugam in lista elementele necesare pentru dropdown
                selectList.Add(new SelectListItem
                {
                    Value = category.CategoryId.ToString(),
                    Text = category.CategoryName.ToString()
                });
            }

            /*
            foreach (var category in categories)
            {
                var itemList = new SelectListItem();
                itemList.Value = category.CategoryId.ToString();
                itemList.Text = category.CategoryName.ToString();

                selectList.Add(itemList);
            }
            */

            // returnam lista de categorii
            return selectList;
        }
    }
}