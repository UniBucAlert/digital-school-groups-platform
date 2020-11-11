using DigitalSchoolGroupsPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitalSchoolGroupsPlatform.Controllers
{
    public class MessagesController : Controller
    {
        private DigitalSchoolGroupsPlatform.Models.AppContext db = 
            new DigitalSchoolGroupsPlatform.Models.AppContext();

        // GET: Messages
        public ActionResult Index()
        {
            return View();
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Message message = db.Messages.Find(id);
            db.Messages.Remove(message);
            db.SaveChanges();
            return Redirect("/Groups/Show/" + message.GroupId);
        }

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

        public ActionResult Edit(int id)
        {
            Message message = db.Messages.Find(id);
            ViewBag.Message = message;
            return View();
        }

        [HttpPut]
        public ActionResult Edit(int id, Message requestMessage)
        {
            try
            {
                Message message = db.Messages.Find(id);
                if (TryUpdateModel(message))
                {
                    message.Content = requestMessage.Content;
                    db.SaveChanges();
                }
                return Redirect("/Groups/Show/" + message.GroupId);
            }
            catch (Exception e)
            {
                return View();
            }
        }
    }
}