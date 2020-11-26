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
    public class MessagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Messages
        public ActionResult Index()
        {
            return View();
        }

        [HttpDelete]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Delete(int id)
        {
            Message message = db.Messages.Find(id);
            if (message.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                db.Messages.Remove(message);
                db.SaveChanges();
                return Redirect("/Groups/Show/" + message.GroupId);
            }
            else
            {
                TempData["message"] = "You do not have the rights to delete the message!";
                return RedirectToAction("Index", "Groups");
            }
        }

        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Edit(int id)
        {
            Message message = db.Messages.Find(id);

            if (message.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                return View(message);
            }
            else
            {
                TempData["message"] = "You do not have the rights to edit the message!";
                return RedirectToAction("Index", "Groups");
            }  
        }

        [HttpPut]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Edit(int id, Message requestMessage)
        {
            try
            {
                Message message = db.Messages.Find(id);

                if (message.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                {
                    if (TryUpdateModel(message))
                    {
                        message.Content = requestMessage.Content;
                        db.SaveChanges();
                    }

                    return Redirect("/Groups/Show/" + message.GroupId);
                }
                else
                {
                    TempData["message"] = "You do not have the rights to edit the message!";
                    return RedirectToAction("Index", "Groups");
                }
            }
            catch (Exception e)
            {
                return View(requestMessage);
            }
        }


        /*
         * Metoda "mutata" in GroupsController, in Show(Message message)
         * pentru a evita un redirect care nu ar fi pastrat starea curenta a obiectului.
         * In acest fel, nu pierdem validarile!
        
        [HttpPost]
        public ActionResult New(Message message)
        {
            message.Date = DateTime.Now;
            try
            {
                db.Messages.Add(message);
                db.SaveChanges();
                return Redirect("/Groups/Show/" + message.GroupId);
            }
            catch (Exception e)
            {
                return Redirect("/Groups/Show/" + message.GroupId);
            }
        }
        */
    }
}