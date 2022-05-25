using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ClinicPortal.Controllers
{
    public class SecGateWayController : Controller
    {
        // GET: SecGateWay
        public ActionResult Login()
        {
            return View(new Models.CRDIMgr.User());
        }

        [HttpPost]
        public ActionResult Login(Models.CRDIMgr.User user)
        {

            Models.CRDIMgr.CASDbEntities2 _db = new Models.CRDIMgr.CASDbEntities2();

            var _usr = _db.Users.SingleOrDefault(u => u.UserName == user.UserName && u.Password == user.Password);



            if (_usr != null)
            {
                FormsAuthentication.SetAuthCookie(_usr.UserName + _usr.Role, false);

                var rst = GotoLandingPage(_usr.Role);


                return rst;
            }
            ModelState.AddModelError("", "invalid Username or Password");
            return View(user);
        }

        private ActionResult GotoLandingPage(string role)
        {
            //Admin      Patient   Physician Supplier Salesmen
            //if (role.ToLower()=="admin")
            //{
            return RedirectToAction("Index", role);

            //}

        }


        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}