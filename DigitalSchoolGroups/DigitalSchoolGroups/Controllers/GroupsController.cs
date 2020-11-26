using DigitalSchoolGroups.Models;
using DigitalSchoolGroupsPlatform.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitalSchoolGroupsPlatform.Controllers
{
    public class GroupsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // ----------READ----------
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Index()
        {
            var groups = db.Groups.Include("Category").Include("User");
            ViewBag.Groups = groups;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }

        // ----------CREATE----------
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult New()
        {
            Group group = new Group();

            // Preluam lista de categorii.
            group.Categ = GetAllCategories();

            // Preluam ID-ul utilizatorului curent.
            group.UserId = User.Identity.GetUserId();

            // Atentie: daca pasam View() => eroare - categoriile null!
            // Trebuie trimise ca argument si categoriile!
            // Se trimite "group" ca param si e okay - contine si categoriile.
            // Se adauga @model DigitalSchoolGroupsPlatform.Models.Group la inceput in View.
            return View(group);
        }

        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult New(Group group)
        {
            // Set DateCreated as the current date.
            group.DateCreated = DateTime.Now;
            group.UserId = User.Identity.GetUserId();

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
                    group.Categ = GetAllCategories();
                    return View(group);
                }
            }
            catch (Exception e)
            {
                group.Categ = GetAllCategories();
                return View(group);
                // Obs: nu returnam View() pt a nu mai pune utilizatorul sa scrie 
                // inca o data ceea ce a scris inainte.
                // Starea obiectului este pastrata!
            }
        }

        // ----------READ ONE----------
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Show(int id)
        {
            Group group = db.Groups.Find(id);

            // Setez drepturile de acces.
            SetAccessRights();

            return View(group);
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Show(Message message)
        {
            message.Date = DateTime.Now;
            message.UserId = User.Identity.GetUserId();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Messages.Add(message);
                    db.SaveChanges();
                    return Redirect("/Groups/Show/" + message.GroupId);
                }
                else
                {
                    Group group = db.Groups.Find(message.GroupId);

                    SetAccessRights();

                    return View(group);
                }
            }
            catch (Exception e)
            {
                Group group = db.Groups.Find(message.GroupId);

                SetAccessRights();

                return View(group);
            }
        }

        // ----------UPDATE----------
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult Edit(int id)
        {
            Group group = db.Groups.Find(id);
            group.Categ = GetAllCategories();

            // Un grup poate fi editat doar de utilizatorul care l-a creat
            // sau de admin.
            if (group.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                return View(group);
            }
            else
            {
                TempData["message"] = "You do not have the rights to modify the group!";
                return RedirectToAction("Index");
            }
            
        }

        [HttpPut]
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult Edit(int id, Group requestGroup)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Group group = db.Groups.Find(id);

                    if (group.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                    {
                        if (TryUpdateModel(group))
                        {
                            group = requestGroup;
                            db.SaveChanges();
                            TempData["message"] = "The group has been successfully modified.";
                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["message"] = "You do not have the rights to modify the group!";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    requestGroup.Categ = GetAllCategories();
                    return View(requestGroup);
                }
            }
            catch (Exception e)
            {
                requestGroup.Categ = GetAllCategories();
                return View(requestGroup);
            }
        }

        // ----------DELETE----------
        [HttpDelete]
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult Delete(int id)
        {
            Group group = db.Groups.Find(id);

            if (group.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                db.Groups.Remove(group);
                db.SaveChanges();
                TempData["message"] = "The group has been deleted.";
                return RedirectToAction("Index");
            } else
            {
                TempData["message"] = "You do not have the rights to delete the group.";
                return RedirectToAction("Index");
            }    
        }

        // --------------------------
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

        private void SetAccessRights()
        {
            ViewBag.showButtons = false;

            if (User.IsInRole("Editor") || User.IsInRole("Admin"))
            {
                ViewBag.showButtons = true;
            }

            ViewBag.isAdmin = User.IsInRole("Admin");
            ViewBag.currentUser = User.Identity.GetUserId();
        }
    }
}
