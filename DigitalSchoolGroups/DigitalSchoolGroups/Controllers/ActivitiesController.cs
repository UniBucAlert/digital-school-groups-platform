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
    public class ActivitiesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // ----------READ----------
        public ActionResult Index(int id) // groupId
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            // Order activities.
            var activities = from activity in db.Activities
                             orderby activity.Date descending
                             where activity.GroupId == id
                             select activity;
            
            ViewBag.Activities = activities;
            return View();
        }
        
        // ----------READ ONE----------
        public ActionResult Show(int id)
        {
            Activity activity = db.Activities.Find(id);
            return View(activity);
        }

        [HttpDelete]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Delete(int id)
        {
            Activity activity = db.Activities.Find(id);
            if (activity.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                db.Activities.Remove(activity);
                db.SaveChanges();
                // temp data??? activity deleted
                return RedirectToAction("Index/" + activity.GroupId);
            }
            else
            {
                TempData["message"] = "You do not have the rights to delete the activity!";
                return RedirectToAction("Index/" + activity.GroupId);
            }
        }

        // ----------UPDATE----------
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Edit(int id)
        {
            Activity activity = db.Activities.Find(id);

            if (activity.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                return View(activity);
            }
            else
            {
                TempData["message"] = "You do not have the rights to edit the activity!";
                return RedirectToAction("Index/" + activity.GroupId);
            }
        }

        [HttpPut]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Edit(int id, Activity requestActivity)
        {
            try
            {
                Activity activity = db.Activities.Find(id);

                if (activity.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                {
                    if (TryUpdateModel(activity))
                    {
                        activity = requestActivity;
                        db.SaveChanges();
                        TempData["message"] = "Activity successfully modified!";
                        return RedirectToAction("Index");
                    }

                    return View(requestActivity);
                }
                else
                {
                    TempData["message"] = "You do not have the rights to edit the activity!";
                    return RedirectToAction("Index/" + activity.GroupId);
                }
            }
            catch (Exception e)
            {
                return View(requestActivity);
            }
        }

        // ----------CREATE----------
        public ActionResult New(int id) // groupId
        {
            ViewBag.GroupID = id;
            return View();
        }

        [HttpPost]
        public ActionResult New(Activity activity)
        {
            activity.Date = DateTime.Now;
            try
            {
                if (ModelState.IsValid)
                {
                    db.Activities.Add(activity);
                    db.SaveChanges();
                    TempData["message"] = "Activity successfully added!";
                    return RedirectToAction("Index/" + activity.GroupId);
                }
                else
                {
                    return View(activity);
                }
            }
            catch (Exception e)
            {
                return View(activity);
            }
        }
    }
}