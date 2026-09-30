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
                // Check if Email already exists
                var existingEmail = db.Users.FirstOrDefault(u => u.Email == user.Email);
                if (existingEmail != null)
                {
                    ViewBag.Error = "Email ID pehle se registered hai!";
                    return View(user);
                }

                // Check if UserName already exists
                if (!string.IsNullOrEmpty(user.UserName))
                {
                    var existingUserName = db.Users.FirstOrDefault(u => u.UserName == user.UserName);
                    if (existingUserName != null)
                    {
                        ViewBag.Error = "Username pehle se liya gaya hai! Kripya doosra username chunein.";
                        return View(user);
                    }
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

        // POST: Login (Email YA Username dono se login chalega)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string usernameOrEmail, string password)
        {
            // Check karein ki entered value Email se match hoti hai YA Username se
            var user = db.Users.FirstOrDefault(u =>
                (u.Email == usernameOrEmail || u.UserName == usernameOrEmail) && u.Password == password);

            if (user != null)
            {
                Session["UserID"] = user.UserID;
                // Session me UserName set karein (agar empty hai toh FullName)
                Session["UserName"] = string.IsNullOrEmpty(user.UserName) ? user.FullName : user.UserName;
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

            ViewBag.Error = "Invalid Username/Email or Password!";
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