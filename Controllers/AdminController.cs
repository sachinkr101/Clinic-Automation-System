using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClinicPortal.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        // GET: Admin

        Models.CRDIMgr.CASDbEntities2 _db = new Models.CRDIMgr.CASDbEntities2();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult InBox()
        {
            return View();
        }

        #region AddPatient
        public ActionResult AddPatient()
        {
            Models.CRDIMgr.Patient pat = new Models.CRDIMgr.Patient { User = new Models.CRDIMgr.User() };
            return View(pat);
        }
        [HttpPost]
        public ActionResult AddPatient(Models.CRDIMgr.Patient pat)
        {
            if (ModelState.IsValid)
            {
                pat.User.Role = "Patient";
                pat.User.IsActive = true;
                pat.User.IsEmailVerified = false;
                pat.User.IsLocked = false;
                pat.User.LastLogDate = null;

                _db.Patients.Add(pat);
                _db.SaveChanges();
            }
            return View();
        }
        #endregion

        #region AddPhysician
        public ActionResult AddPhysician()
        {
            Models.CRDIMgr.Physician ph = new Models.CRDIMgr.Physician { User = new Models.CRDIMgr.User() };
            return View(ph);
        }
        [HttpPost]
        public ActionResult AddPhysician(Models.CRDIMgr.Physician ph)
        {
            if (ModelState.IsValid)
            {
                ph.User.Role = "Physician";
                ph.User.IsActive = true;
                ph.User.IsEmailVerified = false;
                ph.User.IsLocked = false;
                ph.User.LastLogDate = null;

                _db.Physicians.Add(ph);
                _db.SaveChanges();
            }

            return View();
        }
        #endregion


        public ActionResult AddSupplier()
        {
            Models.CRDIMgr.Supplier sp = new Models.CRDIMgr.Supplier { User = new Models.CRDIMgr.User() };
            return View(sp);
        }
        [HttpPost]
        public ActionResult AddSupplier(Models.CRDIMgr.Supplier sp)
        {
            if (ModelState.IsValid)
            {
                sp.User.Role = "Supplier";
                sp.User.IsActive = true;
                sp.User.IsEmailVerified = false;
                sp.User.LastLogDate = null;
                sp.User.IsLocked = false;
                _db.Suppliers.Add(sp);
                _db.SaveChanges();
            }
            return View();
        }

        public ActionResult AddSalesman()
        {
            Models.CRDIMgr.Salesman slp = new Models.CRDIMgr.Salesman { User = new Models.CRDIMgr.User() };
            return View(slp);
        }
        [HttpPost]
        public ActionResult AddSalesman(Models.CRDIMgr.Salesman slp)
        {
            if (ModelState.IsValid)
            {
                slp.User.Role = "SalesPerson";
                slp.User.IsActive = true;
                slp.User.LastLogDate = null;
                slp.User.IsLocked = false;
                slp.User.IsEmailVerified = false;
                _db.Salesmen.Add(slp);
                _db.SaveChanges();
            }

            return View();
        }


        public ActionResult ModifyPatient()
        {
            var obj = _db.Patients;
            return View(obj);
        }
        public ActionResult ModifyPhysician()
        {
            var obj = _db.Physicians;
            return View(obj);
        }
        public ActionResult ModifySalesman()
        {
            var obj = _db.Salesmen;
            return View(obj);
        }
        public ActionResult ModifySupplier()
        {
            var obj = _db.Suppliers;
            return View(obj);
        }

        public ActionResult EditPatientInfo(int id)
        {
            var pat = _db.Patients.Find(id);
            return View(pat);

        }
        [HttpPost]
        public ActionResult EditPatientInfo(int id, Models.CRDIMgr.Patient patient)
        {
            if (id != patient.PatientID)
            {
                return HttpNotFound();
            }
            var pat = _db.Patients.Find(id);
            pat.Address = patient.Address;
            pat.ContactDetails = patient.ContactDetails;
            pat.EmgContactName = patient.EmgContactName;
            pat.EmgContactNumber = patient.EmgContactNumber;
            //pat.User.IsActive = patient.User.IsActive;
            pat.User.IsLocked = patient.User.IsLocked;
            _db.SaveChanges();
            return RedirectToAction("ModifyPatient");
        }
        public ActionResult EditPhysicianInfo(int id)
        {

            return View(_db.Physicians.Find(id));
        }
        [HttpPost]
        public ActionResult EditPhysicianInfo(int id, Models.CRDIMgr.Physician physician)
        {
            if (id != physician.PhysicianID)
            {
                return HttpNotFound();
            }
            var phy = _db.Physicians.Find(id);
            phy.SpecialistIn = physician.SpecialistIn;
            phy.AboutPhysician = physician.AboutPhysician;
            phy.CurrentStatus = physician.CurrentStatus;
            phy.User.IsLocked = physician.User.IsLocked;
            _db.SaveChanges();
            return RedirectToAction("ModifyPhysician");
        }
        public ActionResult EditSalesmanInfo(int id)
        {

            return View(_db.Salesmen.Find(id));
        }

        [HttpPost]
        public ActionResult EditSalesmanInfo(int id, Models.CRDIMgr.Salesman salesPerson)
        {
            if (id != salesPerson.SalesmanId)
            {
                return HttpNotFound();
            }
            var sal = _db.Salesmen.Find(id);
            sal.Address = salesPerson.Address;
            sal.AboutSalesMan = salesPerson.AboutSalesMan;
            sal.CurrentStatus = salesPerson.CurrentStatus;
            sal.User.IsLocked = salesPerson.User.IsLocked;
            _db.SaveChanges();


            return RedirectToAction("ModifySalesPerson");
        }
        public ActionResult EditSupplierInfo(int id)
        {

            return View(_db.Suppliers.Find(id));
        }
        [HttpPost]
        public ActionResult EditSupplierInfo(int id, Models.CRDIMgr.Supplier supplier)
        {
            var sup = _db.Suppliers.Find(id);
            sup.CurrentStatus = supplier.CurrentStatus;
            sup.Address = supplier.Address;
            sup.AboutSupplier = supplier.AboutSupplier;
            sup.User.IsLocked = supplier.User.IsLocked;
            _db.SaveChanges();
            return RedirectToAction("ModifySupplier");
        }

    }
}
