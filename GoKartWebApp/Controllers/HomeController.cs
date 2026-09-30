using System;
using System.Linq;
using System.Web.Mvc;
using GoKartWebApp.Models;

namespace GoKartWebApp.Controllers
{
    public class HomeController : Controller
    {
        private GoKartContext db = new GoKartContext();

        // GET: Home / Products Listing with Category & Search Filter
        public ActionResult Index(string category, string search)
        {
            var products = db.Products.Where(p => p.IsActive == true).AsQueryable();

            // 1. Category Filter Logic
            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category.ToLower().Contains(category.ToLower()));
                ViewBag.SelectedCategory = category;
            }

            // 2. Search Bar Logic
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.ProductName.ToLower().Contains(search.ToLower()) ||
                                               p.Category.ToLower().Contains(search.ToLower()));
                ViewBag.SearchKeyword = search;
            }

            return View(products.ToList());
        }

        // GET: Home/ProductDetails/5
        public ActionResult ProductDetails(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            // Related products load karein (Same category ke, max 4 products)
            ViewBag.RelatedProducts = db.Products
                .Where(p => p.Category == product.Category && p.ProductID != id && p.IsActive == true)
                .Take(4)
                .ToList();

            return View(product);
        }


    }
}