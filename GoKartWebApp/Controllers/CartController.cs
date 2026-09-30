using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GoKartWebApp.Models;

namespace GoKartWebApp.Controllers
{
    public class CartController : Controller
    {
        private GoKartContext db = new GoKartContext();

        // Helper Method: Session se Cart items nikalne ke liye
        private List<CartItem> GetCart()
        {
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null)
            {
                cart = new List<CartItem>();
                Session["Cart"] = cart;
            }
            return cart;
        }

        // 1. CART VIEW PAGE
        public ActionResult Index()
        {
            var cart = GetCart();
            ViewBag.GrandTotal = cart.Sum(x => x.Total);
            return View(cart);
        }

        // 2. ADD TO CART ACTION
        public ActionResult AddToCart(int id, int quantity = 1)
        {
            var product = db.Products.Find(id);
            if (product != null)
            {
                var cart = GetCart();
                var item = cart.FirstOrDefault(c => c.ProductID == id);

                if (item != null)
                {
                    item.Quantity += quantity;
                }
                else
                {
                    cart.Add(new CartItem
                    {
                        ProductID = product.ProductID,
                        ProductName = product.ProductName,
                        ImageUrl = product.ImageUrl,
                        Price = product.Price,
                        Quantity = quantity
                    });
                }

                // Header ke Cart Badge count ko update karein
                Session["CartCount"] = cart.Sum(x => x.Quantity);
                TempData["Success"] = $"{product.ProductName} added to cart!";
            }

            return RedirectToAction("Index");
        }

        // 3. UPDATE QUANTITY
        [HttpPost]
        public ActionResult UpdateQuantity(int productId, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductID == productId);

            if (item != null)
            {
                if (quantity > 0)
                {
                    item.Quantity = quantity;
                }
                else
                {
                    cart.Remove(item);
                }
            }

            Session["CartCount"] = cart.Sum(x => x.Quantity);
            return RedirectToAction("Index");
        }

        // 4. REMOVE ITEM FROM CART
        public ActionResult RemoveFromCart(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductID == id);

            if (item != null)
            {
                cart.Remove(item);
                Session["CartCount"] = cart.Sum(x => x.Quantity);
                TempData["Success"] = "Item removed from cart.";
            }

            return RedirectToAction("Index");
        }

        // 5. CLEAR ENTIRE CART
        public ActionResult ClearCart()
        {
            Session["Cart"] = null;
            Session["CartCount"] = 0;
            return RedirectToAction("Index");
        }
    }
}