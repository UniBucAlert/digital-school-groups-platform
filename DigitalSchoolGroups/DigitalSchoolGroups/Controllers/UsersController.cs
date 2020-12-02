using DigitalSchoolGroups.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitalSchoolGroups.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Users
        public ActionResult Index()
        {
            var users = from user in db.Users
                        orderby user.UserName
                        select user;
            ViewBag.UsersList = users;
            return View();
        }

        public ActionResult Show(string id)
        {
            ApplicationUser user = db.Users.Find(id);

            ViewBag.currentUser = User.Identity.GetUserId();

            //var userRole = roles.Where(j => j.Id == user.Roles.FirstOrDefault().RoleId).
            //               Select(a => a.Name).FirstOrDefault();

            /*
            Pentru afisarea rolului unui utilizator, trebuie tinut cont de modul in care Identity 
            stocheaza rolurile. Intre utilizatori si roluri este o relatie many-to-many, utilizatorul 
            avand astfel unul sau mai multe roluri. 
            Avand in vedere ca in aplicatia noastra folosim un singur rol pentru un utilizator, 
            se poate obtine rolul acestuia folosind user.Roles.FirstOrDefault().
            */
            string currentRole = user.Roles.FirstOrDefault().RoleId;

            var userRoleName = (from role in db.Roles
                                where role.Id == currentRole
                                select role.Name).First();

            ViewBag.roleName = userRoleName;

            return View(user);
        }

        
        public ActionResult Edit(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            // Incarc toate rolurile existente.
            user.AllRoles = GetAllRoles();

            // Rolul curent
            var userRole = user.Roles.FirstOrDefault();
            ViewBag.userRole = userRole.RoleId;
            // user ca param! -> voi avea si toate rolurile transmise catre View.
            return View(user);
        }

        [HttpPut]
        public ActionResult Edit(string id, ApplicationUser newData)
        {
            ApplicationUser user = db.Users.Find(id);
            // In IdentityModels.cs am definit proprietatea AllRoles
            user.AllRoles = GetAllRoles();
            var userRole = user.Roles.FirstOrDefault();
            ViewBag.userRole = userRole.RoleId;

            try
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                if (TryUpdateModel(user))
                {
                    user.UserName = newData.UserName;
                    user.Email = newData.Email;
                    user.PhoneNumber = newData.PhoneNumber;

                    var roles = from role in db.Roles select role;
                    foreach (var role in roles)
                    {
                        UserManager.RemoveFromRole(id, role.Name);
                    }

                    var selectedRole = db.Roles.Find(HttpContext.Request.Params.Get("newRole"));
                    UserManager.AddToRole(id, selectedRole.Name);

                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Response.Write(e.Message);
                newData.Id = id;
                return View(newData);
            }
        }

        /* Returneaza lista de roluri - pereche value-text: id-nume rol */
        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles()
        {
            var selectList = new List<SelectListItem>();

            var roles = from role in db.Roles select role;
            foreach (var role in roles)
            {
                selectList.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name.ToString()
                });
            }
            return selectList;
        }

        [HttpDelete]
        public ActionResult Delete(string id)
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var user = UserManager.Users.FirstOrDefault(u => u.Id == id);

            var groups = db.Groups.Where(a => a.UserId == id);
            foreach (var group in groups)
            {
                db.Groups.Remove(group);
            }

            var messages = db.Messages.Where(message => message.UserId == id);
            foreach (var message in messages)
            {
                db.Messages.Remove(message);
            }

            // Commit pe groups
            db.SaveChanges();
            UserManager.Delete(user);
            return RedirectToAction("Index");
        }
    }
}