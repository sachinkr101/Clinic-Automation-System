using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClinicPortal.Controllers
{
    [Authorize]
    public class PatientController : Controller
    {
        Models.CRDIMgr.CASDbEntities2 _db = new Models.CRDIMgr.CASDbEntities2();
        // GET: Patient
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult EditProfile(int id)
        {
            var obj = _db.Patients.Find(id);
            return View(obj);
        }
        [HttpPost]
        public ActionResult EditProfile(int id, Models.CRDIMgr.Patient pat)
        {


            try
            {
                if (id != pat.PatientID)
                {
                    return HttpNotFound();
                }
                var patient = _db.Patients.Find(id);
                patient.FirstName = pat.FirstName.Trim();
                patient.LastName = pat.LastName.Trim();
                patient.nationality = pat.nationality.Trim();
                patient.ContactDetails = pat.ContactDetails.Trim();
                patient.EmgContactName = pat.EmgContactName.Trim();
                patient.EmgContactNumber = pat.EmgContactNumber.Trim();
                patient.Address = pat.Address;
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
            if (mUserType == "Physician")
            {
                var users = _db.Physicians.Select(u => new { u.FirstName, MsgUsrId = u.PhysicianID });

                return Json(new SelectList(users.ToArray(), "MsgUsrId", "FirstName"), JsonRequestBehavior.AllowGet);
            }

            return Json(null);
        }

        public ActionResult CreateNewMsg()
        {
            Models.CRDIMgr.TheMessage theMessage = new Models.CRDIMgr.TheMessage();

            List<SelectListItem> lst = new List<SelectListItem>();
            lst.Add(new SelectListItem { Text = "Physician", Value = "Physician" });
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
                theMessage.MessageFromID = _db.Patients.FirstOrDefault(p => p.UserId == _usr.UserId).PatientID;
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
            var a = _db.Patients.FirstOrDefault(p => p.UserId == _usr.UserId);
            int id = a.PatientID;
            var obj = _db.TheMessages.Where(t => t.MessageToID == id && t.MessageToType == _usr.Role && t.MessageStatus != "Deleted").ToList();

            return View(obj);
        }

        //public ActionResult DeleteViewMsg(int id)
        //{
        //    return View(_db.Physicians.Find(id));
        //}
        //[HttpPost]
        //public ActionResult DeleteViewMsg(int id, Models.CRDIMgr.Physician physician)
        //{
        //    try
        //    {
        //        physician.CurrentStatus = "Deleted";
        //        _db.Entry(physician);
        //        _db.SaveChanges();
        //        return RedirectToAction("ViewMsg");
        //    }
        //    catch
        //    {

        //        return RedirectToAction("ViewMsg");
        //    }


        //}



        public ActionResult ReadViewMsg(int id)
        {
            string msgFromName = "";
            var obj = _db.TheMessages.Find(id);

            if (obj.MessageFromType == "SalesPerson")
            {
                msgFromName = _db.Salesmen.FirstOrDefault(s => s.SalesmanId == obj.MessageFromID).FirstName;
                ViewBag.msgFromName = msgFromName;
            }

            if (obj.MessageFromType == "Physician")
            {
                msgFromName = _db.Physicians.FirstOrDefault(s => s.PhysicianID == obj.MessageFromID).FirstName;
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

    }
}