using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClinicPortal.Controllers
{
    public class HomeController : Controller
    {
        // GET: HomeController
        Models.CRDIMgr.CASDbEntities2 _db = new Models.CRDIMgr.CASDbEntities2();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewDrugInventory(string option, string search)
        {
            if (option == "Name")
            {
                return View(_db.Drugs.Where(x => x.DrugName == search || search == null).ToList());
            }
            else if (option == "SubName")
            {
                return View(_db.Drugs.Where(x => x.Substitutions == search || search == null).ToList());
            }
            else
            {
                return View(_db.Drugs.Where(x => x.DrugName.StartsWith(search) || search == null).ToList());
            }
            //return View(_db.Drugs.ToList());
        }

        public ActionResult Details(int id)
        {
            return View(_db.Drugs.Find(id));
        }

        public ActionResult contact()
        {

            return View();
        }
        [HttpPost]
        public ActionResult contact(int id)
        {
            return RedirectToAction("Index");
        }
    }
}