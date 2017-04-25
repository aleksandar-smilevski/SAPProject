using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SAP.Models;
using SAP.Attributes;
using Microsoft.AspNet.Identity;

namespace SAP.Controllers
{
    [AccessDeniedAuthorize(Roles ="Interviewer")]
    public class UserController : Controller
    {
        private SAPEntities db = new SAPEntities();

        // GET: User
        public ActionResult Index()
        {
            string user = User.Identity.GetUserId();
            var list = db.AddToSurvey.Where(x => x.Id_interviewer == user).Select(x => x.Survey).ToList();
            if(list.Count == 0)
            {
                ViewBag.MessageTitle = "Sorry no surveys :(";
                ViewBag.Message = "You have no surveys assigned. Check back soon :)";
                return View(list);
            }
            return View(list);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
