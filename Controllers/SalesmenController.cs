using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClinicPortal.Controllers
{
    [Authorize]
    public class SalesmenController : Controller
    {
        // GET: Salesmen
        Models.CRDIMgr.CASDbEntities2 _db = new Models.CRDIMgr.CASDbEntities2();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddOrderheader()
        {
            var obj = new Models.CRDIMgr.OrderHeader();
            SelectList lst = new SelectList(_db.Drugs.Select(s => new { s.DrugID, s.DrugName }).ToList(), "DrugID", "DrugName");
            ViewData["Drugs"] = lst;
            SelectList lstSupplier = new SelectList(_db.Suppliers.Select(s => new { s.SupplierID, s.FirstName }).ToList(), "SupplierID", "FirstName");

            ViewData["Suppliers"] = lstSupplier;
            return View(obj);
        }
        [HttpPost]
        public ActionResult AddOrderheader(Models.CRDIMgr.OrderHeader orderHeader, List<Models.CRDIMgr.OrderProductLine> prds, [Bind(Prefix = "Drugs")] List<int> DrugID, List<int> Quantity, List<int> Rate, List<int> GrossAmount, List<int> DiscountAmount, List<int> NetAmounta, int Suppliers)
        {

            var _usr = Session["CurrentUser"] as Models.CRDIMgr.User;

            List<Models.CRDIMgr.OrderProductLine> lst = new List<Models.CRDIMgr.OrderProductLine>();


            for (int i = 0; i < DrugID.Count; i++)
            {
                Models.CRDIMgr.OrderProductLine obj = new Models.CRDIMgr.OrderProductLine { Drug = _db.Drugs.Find(DrugID[i]), Quantity = Quantity[i], Rate = Rate[i], GrossAmount = GrossAmount[i], DiscountAmount = DiscountAmount[i], NetAmount = NetAmounta[i] };
                orderHeader.OrderProductLines.Add(obj);
            }
            orderHeader.Supplier = _db.Suppliers.Find(Suppliers);
            orderHeader.SalesPerson = _usr.UserName;
           
            _db.OrderHeaders.Add(orderHeader);

            _db.SaveChanges();
            ViewBag.message = String.Format($"Hello, {0} You have sucessfully placed an order.", _usr.UserName);
            return RedirectToAction("Index");
        }


        public ActionResult ViewPlacedOrders(Models.CRDIMgr.OrderHeader orderHeader)
        {
            var _usr = Session["CurrentUser"] as Models.CRDIMgr.User;
            var obj = _db.OrderHeaders.Where(o => o.SalesPerson == _usr.UserName);


            return View(obj.OrderByDescending(o => o.OrderId));
        }


        public ActionResult ViewPlacedDrugs(int id)
        {

            var obj = _db.OrderProductLines.Where(o => o.OrderId == id);
            ViewBag.orderid = id;
            return View(obj);
        }


        public JsonResult MsgUserList(string mUserType)
        {


            if (mUserType == "Physician")
            {
                var users = _db.Physicians.Select(u => new { u.FirstName, MsgUsrId = u.PhysicianID });

                return Json(new SelectList(users.ToArray(), "MsgUsrId", "FirstName"), JsonRequestBehavior.AllowGet);
            }
            if (mUserType == "Patient")
            {
                var users = _db.Patients.Select(u => new { u.FirstName, MsgUsrId = u.PatientID });

                return Json(new SelectList(users.ToArray(), "MsgUsrId", "FirstName"), JsonRequestBehavior.AllowGet);
            }

            if (mUserType == "Supplier")
            {
                var users = _db.Suppliers.Select(u => new { u.FirstName, MsgUsrId = u.SupplierID });

                return Json(new SelectList(users.ToArray(), "MsgUsrId", "FirstName"), JsonRequestBehavior.AllowGet);
            }



            return Json(null);
        }

        public ActionResult CreateNewMsg()
        {
            Models.CRDIMgr.TheMessage theMessage = new Models.CRDIMgr.TheMessage();

            List<SelectListItem> lst = new List<SelectListItem>();
            lst.Add(new SelectListItem { Text = "Patient", Value = "Patient" });
            lst.Add(new SelectListItem { Text = "Physician", Value = "Physician" });
            lst.Add(new SelectListItem { Text = "Supplier", Value = "Supplier" });
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
                theMessage.MessageFromID = _db.Salesmen.FirstOrDefault(p => p.UserId == _usr.UserId).SalesmanId;
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
            var a = _db.Salesmen.FirstOrDefault(p => p.UserId == _usr.UserId);
            int id = a.SalesmanId;
            var obj = _db.TheMessages.Where(t => t.MessageToID == id && t.MessageToType == _usr.Role && t.MessageStatus != "Deleted").ToList();

            return View(obj);
        }

        public ActionResult ReadViewMsg(int id)
        {
            string msgFromName = "";
            var obj = _db.TheMessages.Find(id);
            if (obj.MessageFromType == "Patient")
            {
                msgFromName = _db.Patients.FirstOrDefault(s => s.PatientID == obj.MessageFromID).FirstName;
                ViewBag.msgFromName = msgFromName;
            }

            if (obj.MessageFromType == "Supplier")
            {
                msgFromName = _db.Suppliers.FirstOrDefault(s => s.SupplierID == obj.MessageFromID).FirstName;
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