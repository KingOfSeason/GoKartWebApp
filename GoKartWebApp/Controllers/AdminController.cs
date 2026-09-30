using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using GoKartWebApp.Models;
using System.Collections.Generic;

namespace GoKartWebApp.Controllers
{
    // Strongly-typed model for Categories to prevent Dynamic Exception
    public class CategoryViewModel
    {
        public string CategoryName { get; set; }
        public int TotalProducts { get; set; }
    }

    public class AdminController : Controller
    {
        private GoKartContext db = new GoKartContext();

        // 1. DASHBOARD
        public ActionResult Index()
        {
            if (Session["UserRole"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Account");

            ViewBag.TotalOrders = db.Orders.Count();
            ViewBag.TotalSales = db.Orders.Where(o => o.Status == "Delivered").Select(o => (decimal?)o.TotalAmount).Sum() ?? 0;
            ViewBag.TotalCustomers = db.Users.Count(u => u.Role == "Customer");
            ViewBag.TotalProducts = db.Products.Count();

            var products = db.Products.ToList();
            return View(products);
        }

        // 2. ADD PRODUCT
        public ActionResult AddProduct()
        {
            if (Session["UserRole"]?.ToString() != "Admin") return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProduct(Product product, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                    string extension = Path.GetExtension(ImageFile.FileName);
                    fileName = fileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;

                    string folderPath = Server.MapPath("~/Images/Products/");
                    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                    string savePath = Path.Combine(folderPath, fileName);
                    ImageFile.SaveAs(savePath);

                    product.ImageUrl = "/Images/Products/" + fileName;
                }
                else
                {
                    product.ImageUrl = "/Images/Products/default.jpg";
                }

                product.IsActive = true;
                db.Products.Add(product);
                db.SaveChanges();

                TempData["Success"] = "Product added successfully!";
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // EDIT PRODUCT
        public ActionResult EditProduct(int id)
        {
            if (Session["UserRole"]?.ToString() != "Admin") return RedirectToAction("Login", "Account");

            var product = db.Products.Find(id);
            if (product == null) return HttpNotFound();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProduct(Product product, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = db.Products.Find(product.ProductID);
                if (existingProduct != null)
                {
                    existingProduct.ProductName = product.ProductName;
                    existingProduct.Category = product.Category;
                    existingProduct.Price = product.Price;
                    existingProduct.OriginalPrice = product.OriginalPrice;
                    existingProduct.DiscountPercent = product.DiscountPercent;
                    existingProduct.StockQuantity = product.StockQuantity;
                    existingProduct.IsActive = product.IsActive;

                    if (ImageFile != null && ImageFile.ContentLength > 0)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                        string extension = Path.GetExtension(ImageFile.FileName);
                        fileName = fileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;

                        string folderPath = Server.MapPath("~/Images/Products/");
                        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                        string savePath = Path.Combine(folderPath, fileName);
                        ImageFile.SaveAs(savePath);

                        existingProduct.ImageUrl = "/Images/Products/" + fileName;
                    }

                    db.SaveChanges();
                    TempData["Success"] = "Product updated successfully!";
                    return RedirectToAction("Index");
                }
            }
            return View(product);
        }

        // DELETE PRODUCT
        public ActionResult DeleteProduct(int id)
        {
            if (Session["UserRole"]?.ToString() != "Admin") return RedirectToAction("Login", "Account");

            var product = db.Products.Find(id);
            if (product != null)
            {
                db.Products.Remove(product);
                db.SaveChanges();
                TempData["Success"] = "Product deleted successfully!";
            }
            return RedirectToAction("Index");
        }

        // 3. CATEGORIES (FIXED: Dynamic Binding Error)
        public ActionResult Categories()
        {
            if (Session["UserRole"]?.ToString() != "Admin") return RedirectToAction("Login", "Account");

            var categories = db.Products
                               .GroupBy(p => p.Category)
                               .Select(g => new CategoryViewModel
                               {
                                   CategoryName = g.Key,
                                   TotalProducts = g.Count()
                               })
                               .ToList();

            return View(categories);
        }

        // 4. ORDERS (Handles both Order.cshtml and Orders.cshtml)
        public ActionResult Orders()
        {
            if (Session["UserRole"]?.ToString() != "Admin") return RedirectToAction("Login", "Account");

            var orders = db.Orders.Include(o => o.User).OrderByDescending(o => o.OrderDate).ToList();

            if (System.IO.File.Exists(Server.MapPath("~/Views/Admin/Order.cshtml")))
            {
                return View("Order", orders);
            }
            return View("Orders", orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateOrderStatus(int orderId, string status)
        {
            if (Session["UserRole"]?.ToString() != "Admin") return RedirectToAction("Login", "Account");

            var order = db.Orders.Find(orderId);
            if (order != null)
            {
                order.Status = status;
                db.SaveChanges();
                TempData["Success"] = "Order status updated!";
            }
            return RedirectToAction("Orders");
        }

        // 5. CUSTOMERS (Handles both Customer.cshtml and Customers.cshtml)
        public ActionResult Customers()
        {
            if (Session["UserRole"]?.ToString() != "Admin") return RedirectToAction("Login", "Account");

            var customers = db.Users.Where(u => u.Role == "Customer").ToList();

            if (System.IO.File.Exists(Server.MapPath("~/Views/Admin/Customer.cshtml")))
            {
                return View("Customer", customers);
            }
            return View("Customers", customers);
        }

        public ActionResult EditCustomer(int id)
        {
            if (Session["UserRole"]?.ToString() != "Admin") return RedirectToAction("Login", "Account");

            var user = db.Users.Find(id);
            if (user == null) return HttpNotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCustomer(User user)
        {
            if (Session["UserRole"]?.ToString() != "Admin") return RedirectToAction("Login", "Account");

            var existingUser = db.Users.Find(user.UserID);
            if (existingUser != null)
            {
                existingUser.FullName = user.FullName;
                existingUser.Email = user.Email;
                existingUser.Role = user.Role;

                db.SaveChanges();
                TempData["Success"] = "Customer details updated!";
                return RedirectToAction("Customers");
            }
            return View(user);
        }

        public ActionResult DeleteCustomer(int id)
        {
            if (Session["UserRole"]?.ToString() != "Admin") return RedirectToAction("Login", "Account");

            var user = db.Users.Find(id);
            if (user != null)
            {
                db.Users.Remove(user);
                db.SaveChanges();
                TempData["Success"] = "Customer account deleted!";
            }
            return RedirectToAction("Customers");
        }
    }
}