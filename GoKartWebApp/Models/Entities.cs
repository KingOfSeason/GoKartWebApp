using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoKartWebApp.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int UserID { get; set; }

        public string UserName { get; set; } // <-- Naya UserName property
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }

    [Table("Addresses")]
    public class Address
    {
        [Key]
        public int AddressID { get; set; }
        public int UserID { get; set; }
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Pincode { get; set; }
        public string Country { get; set; }
        public bool IsDefault { get; set; }

        public virtual User User { get; set; }
    }

    [Table("Products")]
    public class Product
    {
        [Key]
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }
        public int DiscountPercent { get; set; }
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; }
        public bool IsActive { get; set; }
    }

    [Table("Orders")]
    public class Order
    {
        [Key]
        public int OrderID { get; set; }
        public string OrderNumber { get; set; }
        public int UserID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }

    [Table("OrderDetails")]
    public class OrderDetail
    {
        [Key]
        public int OrderDetailID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }

    [Table("Wishlist")]
    public class Wishlist
    {
        [Key]
        public int WishlistID { get; set; }
        public int UserID { get; set; }
        public int ProductID { get; set; }

        public virtual User User { get; set; }
        public virtual Product Product { get; set; }
    }

    [Table("Coupons")]
    public class Coupon
    {
        [Key]
        public int CouponID { get; set; }
        public string CouponCode { get; set; }
        public int DiscountPercent { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; }
    }
}