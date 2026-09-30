using System.Linq;
using System.Web.Mvc;
using GoKartWebApp.Models;

namespace GoKartWebApp.Controllers
{
    public class UserController : Controller
    {
        private GoKartContext db = new GoKartContext();
        private int currentUserId = 1; // Logged in user session ID

        // 1. Dashboard
        public ActionResult Dashboard()
        {
            var user = db.Users.Find(currentUserId);
            var orders = db.Orders.Where(o => o.UserID == currentUserId).OrderByDescending(o => o.OrderDate).Take(5).ToList();

            ViewBag.UserName = user.FullName;
            ViewBag.UserEmail = user.Email;
            ViewBag.TotalOrders = db.Orders.Count(o => o.UserID == currentUserId);
            ViewBag.TotalSpent = db.Orders.Where(o => o.UserID == currentUserId && o.Status != "Cancelled").Sum(o => (decimal?)o.TotalAmount) ?? 0;
            ViewBag.WishlistCount = db.Wishlists.Count(w => w.UserID == currentUserId);
            ViewBag.AvailableCoupons = db.Coupons.Count(c => c.IsActive);
            ViewBag.Recommended = db.Products.Take(5).ToList();

            return View(orders);
        }

        // 2. My Orders
        public ActionResult MyOrders()
        {
            var orders = db.Orders.Where(o => o.UserID == currentUserId).OrderByDescending(o => o.OrderDate).ToList();
            return View(orders);
        }

        // 3. My Wishlist
        public ActionResult MyWishlist()
        {
            var wishlist = db.Wishlists.Where(w => w.UserID == currentUserId).Select(w => w.Product).ToList();
            return View(wishlist);
        }

        // 4. My Cart
        public ActionResult MyCart()
        {
            // Dummy Cart fetching from DB/Session
            var cartItems = db.Products.Take(3).ToList();
            return View(cartItems);
        }

        // 5. My Profile
        public ActionResult MyProfile()
        {
            var user = db.Users.Find(currentUserId);
            return View(user);
        }

        // 6. Address Book
        public ActionResult AddressBook()
        {
            var addresses = db.Addresses.Where(a => a.UserID == currentUserId).ToList();
            return View(addresses);
        }

        // 7. Payment Methods
        public ActionResult PaymentMethods()
        {
            return View();
        }

        // 8. Reviews & Ratings
        public ActionResult Reviews()
        {
            return View();
        }

        // 9. Help & Support
        public ActionResult HelpSupport()
        {
            return View();
        }
    }
}