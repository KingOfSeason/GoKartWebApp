using System;
using System.Linq;
using System.Web.Mvc;
using GoKartWebApp.Models;

namespace GoKartWebApp.Controllers
{
    public class AccountController : Controller
    {
        private GoKartContext db = new GoKartContext();

        // GET: Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = db.Users.FirstOrDefault(u => u.Email == user.Email);
                if (existingUser != null)
                {
                    ViewBag.Error = "Email ID pehle se registered hai!";
                    return View(user);
                }

                if (string.IsNullOrEmpty(user.Role))
                {
                    user.Role = "Customer";
                }

                db.Users.Add(user);
                db.SaveChanges();

                TempData["Success"] = "Registration successful! Please login.";
                return RedirectToAction("Login");
            }
            return View(user);
        }

        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Login (YEH CODE YAHA AAYEGA)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            var user = db.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

            // -------- YEH RAHA AAPKA CODE --------
            if (user != null)
            {
                Session["UserID"] = user.UserID;
                Session["UserName"] = user.FullName;
                Session["UserRole"] = user.Role;

                // Case-insensitive role comparison (Admin/admin dono chalega)
                if (string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            // -------------------------------------

            ViewBag.Error = "Invalid Email or Password!";
            return View();
        }

        // Logout
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
        }
    }
}