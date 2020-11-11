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
        private Models.AppContext db = new Models.AppContext();

        // ----------READ----------
        public ActionResult Index()
        {
            var groups = db.Groups.Include("Category");
            ViewBag.Groups = groups;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }

        // ----------CREATE----------
        public ActionResult New()
        {
            Group group = new Group();
            group.Categ = GetAllCategories();

            // Atentie: daca pasam View() => eroare - categoriile null!
            // Trebuie trimise ca argument si categoriile!
            // Se trimite "group" ca param si e okay - contine si categoriile.
            // Se adauga @model DigitalSchoolGroupsPlatform.Models.Group la inceput in View.
            return View(group);
        }

        [HttpPost]
        public ActionResult New(Group group)
        {
            group.Categ = GetAllCategories();
            // Set DateCreated as the current date.
            group.DateCreated = DateTime.Now;

            try
            {
                if (ModelState.IsValid)
                {
                    db.Groups.Add(group);
                    db.SaveChanges();
                    TempData["message"] = "The group has been successfully created.";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(group);
                }
            }
            catch (Exception e)
            {
                return View(group);
                // Obs: nu returnam View() pt a nu mai punee utilizatorul sa scrie 
                // inca o data ceea ce a scris inainte.
                // Starea obiectului este pastrata!
            }
        }

        // ----------READ ONE----------
        public ActionResult Show(int id)
        {
            Group group = db.Groups.Find(id);
            ViewBag.Group = group;
            ViewBag.Category = group.Category;

            return View(group);
        }

        // ----------UPDATE----------
        public ActionResult Edit(int id)
        {
            Group group = db.Groups.Find(id);
            group.Categ = GetAllCategories();
            return View(group);
        }

        [HttpPut]
        public ActionResult Edit(int id, Group requestGroup)
        {
            requestGroup.Categ = GetAllCategories();
            try
            {
                if (ModelState.IsValid)
                {
                    Group group = db.Groups.Find(id);
                    if (TryUpdateModel(group))
                    {
                        group = requestGroup;
                        db.SaveChanges();
                        TempData["message"] = "The group has been successfully modified.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return View(requestGroup);
                    }
                }
                else
                {
                    return View(requestGroup);
                }

            }
            catch (Exception e)
            {
                return View(requestGroup);
            }
        }

        // ----------DELETE----------
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

            // Return the categories list.
            return selectList;
        }
    }
}
