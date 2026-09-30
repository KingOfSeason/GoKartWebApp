using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GoKartWebApp.Models;

namespace GoKartWebApp.Controllers
{
    public class OrdersController : Controller
    {
        private GoKartContext db = new GoKartContext();

        // 1. MY ORDERS PAGE (GET: Orders/Index)
        public ActionResult Index()
        {
            // User logged in hai ya nahi check karein
            if (Session["UserID"] == null)
            {
                TempData["Error"] = "Please login to view your orders.";
                return RedirectToAction("Login", "Account");
            }

            int userId = Convert.ToInt32(Session["UserID"]);

            // Single user ke saare orders datewise (Latest First) fetch karein
            var userOrders = db.Orders
                               .Where(o => o.UserID == userId)
                               .OrderByDescending(o => o.OrderDate)
                               .ToList();

            return View(userOrders);
        }

        // 2. ORDER DETAILS PAGE (GET: Orders/Details/5)
        public ActionResult Details(int id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }
    }
}