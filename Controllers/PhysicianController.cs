using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClinicPortal.Controllers
{
    [Authorize]
    public class PhysicianController : Controller
    {
        // GET: Physician
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

        public ActionResult Edit(int id)
        {
            var obj = _db.Drugs.Find(id);
            return View(obj);
        }
        [HttpPost]
        public ActionResult Edit(int id, Models.CRDIMgr.Drug dr)
        {
            try
            {
                // TODO: Add update logic here
                if (id != dr.DrugID)
                {
                    return HttpNotFound();
                }
                var drug = _db.Drugs.Find(id);
                drug.Substitutions = dr.Substitutions;
                drug.Uses = dr.Uses;
                drug.SideEffects = dr.SideEffects;
                drug.NotRecommended = dr.NotRecommended;

                _db.SaveChanges();


                return RedirectToAction("Index");
            }
            catch
            {
                return HttpNotFound();
            }
        }


        public JsonResult MsgUserList(string mUserType)
        {


            if (mUserType == "SalesPerson")
            {
                var users = _db.Salesmen.Select(u => new { u.FirstName, MsgUsrId = u.SalesmanId });

                return Json(new SelectList(users.ToArray(), "MsgUsrId", "FirstName"), JsonRequestBehavior.AllowGet);
            }
            if (mUserType == "Patient")
            {
                var users = _db.Patients.Select(u => new { u.FirstName, MsgUsrId = u.PatientID });

                return Json(new SelectList(users.ToArray(), "MsgUsrId", "FirstName"), JsonRequestBehavior.AllowGet);
            }

            return Json(null);
        }

        public ActionResult CreateNewMsg()
        {
            Models.CRDIMgr.TheMessage theMessage = new Models.CRDIMgr.TheMessage();

            List<SelectListItem> lst = new List<SelectListItem>();
            lst.Add(new SelectListItem { Text = "Patient", Value = "Patient" });
            lst.Add(new SelectListItem { Text = "SalesPerson", Value = "SalesPerson" });
            ViewData["MessageToType"] = lst;
            return View(theMessage);
        }
        [HttpPost]
        public ActionResult CreateNewMsg(Models.CRDIMgr.TheMessage theMessage)
        {
            var _usr = Session["CurrentUser"] as Models.CRDIMgr.User;

            if (ModelState.IsValid)
            {
                theMessage.DOM = DateTime.Now;
                theMessage.MessageFromID = _db.Physicians.FirstOrDefault(p => p.UserId == _usr.UserId).PhysicianID;
                theMessage.MessageFromType = _usr.Role;
                theMessage.MessageStatus = "Sent";
                _db.TheMessages.Add(theMessage);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult ViewMsg()
        {
            var _usr = Session["CurrentUser"] as Models.CRDIMgr.User;
            var a = _db.Physicians.FirstOrDefault(p => p.UserId == _usr.UserId);
            int id = a.PhysicianID;
            var obj = _db.TheMessages.Where(t => t.MessageToID == id && t.MessageToType == _usr.Role && t.MessageStatus != "Deleted").ToList();

            return View(obj);
        }

        public ActionResult ReadViewMsg(int id)
        {
            string msgFromName = "";
            var obj = _db.TheMessages.Find(id);
            if (obj.MessageFromType == "SalesPerson")
            {
                msgFromName = _db.Salesmen.FirstOrDefault(s => s.SalesmanId == obj.MessageFromID).FirstName;
                ViewBag.msgFromName = msgFromName;
            }

            if (obj.MessageFromType == "Patient")
            {
                msgFromName = _db.Patients.FirstOrDefault(s => s.PatientID == obj.MessageFromID).FirstName;
                ViewBag.msgFromName = msgFromName;
            }

            obj.MessageStatus = "Read";
            _db.SaveChanges();
            return View(obj);
        }

        public ActionResult DeleteViewMsg(int id)
        {

            return View(_db.TheMessages.Find(id));
        }
        [HttpPost]
        public ActionResult DeleteViewMsg(int id, FormCollection collection)
        {
            var obj = _db.TheMessages.Find(id);
            obj.MessageStatus = "Deleted";
            _db.SaveChanges();
            return RedirectToAction("ViewMsg");
        }
    }
}