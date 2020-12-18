using DigitalSchoolGroups.Models;
using DigitalSchoolGroupsPlatform.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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

        // Number of groups to show on one page.
        private int _perPage = 6;

        // ----------READ----------
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Index()
        {
            var groups = db.Groups.Include("Category").Include("GroupAdmin").OrderBy(a => a.DateCreated);
            var totalItems = groups.Count();
            var currentPage = Convert.ToInt32(Request.Params.Get("page"));

            var offset = 0;

            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * this._perPage;
            }

            var paginatedGroups = groups.Skip(offset).Take(this._perPage);

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"].ToString();
            } 

            //ViewBag.perPage = this._perPage;
            ViewBag.total = totalItems;
            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)this._perPage);

            ViewBag.Groups = paginatedGroups;
            ViewBag.currentUserObject = db.Users.Find(User.Identity.GetUserId());
            ViewBag.IsAdmin = User.IsInRole("Admin");

            return View();
        }

        private ApplicationUser GetCurrentUser()
        {
            return db.Users.Find(User.Identity.GetUserId());
        }

        // ----------CREATE----------
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult New()
        {
            Group group = new Group();

            // Preluam lista de categorii.
            group.Categ = GetAllCategories();

            // Preluam ID-ul utilizatorului curent.
            group.GroupAdmin = GetCurrentUser();

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
            group.GroupAdmin = GetCurrentUser();

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


        // Accepts a join request
        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult JoinRequest(int groupId, String userId)
        {
            Group group = db.Groups.Find(groupId);
            ApplicationUser user = db.Users.Find(userId);
            if ((GetCurrentUser().IsModeratorOf(group) && user.RequestedToJoin(group)) || User.IsInRole("Admin"))
            {
                user.DeleteJoinRequest(group);

                user.Groups.Add(group);
                group.Users.Add(user);

                db.SaveChanges();
            }
            else
            {
                TempData["message"] = "You do not have the rights to accept this join request!";
                return RedirectToAction("Index");
            }

            return RedirectToAction("Show/" + groupId);
        }

        // Declines a join request
        [HttpDelete]
        [ActionName("JoinRequest")]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult DeleteJoinRequest(int groupId, String userId)
        {
            Group group = db.Groups.Find(groupId);
            ApplicationUser user = db.Users.Find(userId);
            if ((GetCurrentUser().IsModeratorOf(group) && user.RequestedToJoin(group)) || User.IsInRole("Admin"))
            {
                user.DeleteJoinRequest(group);
                db.SaveChanges();
            }
            else
            {
                TempData["message"] = "You do not have the rights to delete this join request!";
                return RedirectToAction("Index");
            }

            return RedirectToAction("Show/" + groupId);
        }

        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Members(int id)
        {
            Group group = db.Groups.Find(id);

            if (GetCurrentUser().IsInGroup(group) || User.IsInRole("Admin"))
            {
                // Setez drepturile de acces.
                SetAccessRights();

                return View(group);
            }
            else
            {
                TempData["message"] = "You do not have the rights to view these members!";
                return RedirectToAction("Index");
            }
        }

        [HttpDelete]
        [ActionName("Members")]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult KickMember(int groupId, String userId)
        {
            Group group = db.Groups.Find(groupId);
            ApplicationUser user = db.Users.Find(userId);

            if ((GetCurrentUser().IsModeratorOf(group) && user.IsInGroup(group)) || User.IsInRole("Admin"))
            {
                user.Groups.Remove(group);
                group.Users.Remove(user);
                db.SaveChanges();
            }
            else
            {
                TempData["message"] = "You do not have the rights to kick this member!";
                return RedirectToAction("Index");
            }

            return RedirectToAction("Members/" + groupId);
        }

        [HttpPut]
        [ActionName("Members")]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult ChangeGroupRole(int groupId, String userId)
        {
            Group group = db.Groups.Find(groupId);
            ApplicationUser user = db.Users.Find(userId);

            if ((GetCurrentUser().IsModeratorOf(group) && user.IsInGroup(group)) || User.IsInRole("Admin"))
            {
                if (user.IsModeratorOf(group))
                {
                    user.Groups.Add(group);
                    group.Users.Add(user);
                    user.ModeratorOf.Remove(group);
                    group.Moderators.Remove(user);
                }
                else
                {
                    user.Groups.Remove(group);
                    group.Users.Remove(user);
                    user.ModeratorOf.Add(group);
                    group.Moderators.Add(user);
                }

                db.SaveChanges();
            }
            else
            {
                TempData["message"] = "You do not have the rights to edit this role!";
                return RedirectToAction("Index");
            }

            return RedirectToAction("Members/" + groupId);
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

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Join(int id)
        {
            Group group = db.Groups.Find(id);
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            if (TryUpdateModel(group))
            {
                user.GroupsRequests.Add(group);
                group.Requests.Add(user);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // ----------UPDATE----------
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult Edit(int id)
        {
            Group group = db.Groups.Find(id);
            group.Categ = GetAllCategories();

            // Un grup poate fi editat doar de moderator
            // sau de admin.
            if (GetCurrentUser().IsModeratorOf(group) || User.IsInRole("Admin"))
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

                    if (GetCurrentUser().IsModeratorOf(group) || User.IsInRole("Admin"))
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

            if (GetCurrentUser().IsModeratorOf(group) || User.IsInRole("Admin"))
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
            ViewBag.currentUserObject = GetCurrentUser();
        }
    }
}
