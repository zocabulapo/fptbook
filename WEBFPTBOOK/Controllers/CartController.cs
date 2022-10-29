using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEBFPTBOOK.Models;

namespace WEBFPTBOOK.Controllers
{
    public class CartController : Controller
    {
        SqlDataContext data = new SqlDataContext();
        // GET: Cart
        public List<Cart> GetCart()
        {
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart ==null)
            {
                lstCart = new List<Cart>();
                Session["Cart"] = lstCart;
            }
            return lstCart;
        }
        public ActionResult AddCart(int iBookID, string strURL)
        {
            //GET : SESSION
            List<Cart> lstCart = GetCart();
            // Check if the book has been added to the cart
            Cart product = lstCart.Find(n => n.IBookID == iBookID);
            if (product == null)
            {
                product = new Cart(iBookID);
                lstCart.Add(product);
                return Redirect(strURL);
            }
            else
            {
                product.IQuatity++;
                return Redirect(strURL);
            }
        }
        // Total quantity
        private int TotalQuantity()
        {
            int iTotalQuantity = 0;
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart!=null)
            {
                iTotalQuantity = lstCart.Sum(n => n.IQuatity);

            }
            return iTotalQuantity;
        }
        // Total price
        private double TotalPrice()
        {
            double iTotalPrice = 0;
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if(lstCart!=null){
                iTotalPrice = lstCart.Sum(n => n.ITotal);

            }
            return iTotalPrice;

        }
        // Create cart
        public ActionResult Cart()
        {
            List<Cart> lstCart = GetCart();
            if(lstCart.Count==0)
            {
                return RedirectToAction("Index", "FPTBook");
            }
            ViewBag.TotalQuantity = TotalQuantity();
            ViewBag.TotalPrice = TotalPrice();
            return View(lstCart);
        }
        public ActionResult CartPartial()
        {
            ViewBag.TotalQuantity = TotalQuantity();
            ViewBag.TotalPrice = TotalPrice();
            return PartialView();
        }
        // Delete cart
        public ActionResult DeleteCart(int iBookID)
        {
            List<Cart> lstCart = GetCart();
            Cart product = lstCart.SingleOrDefault(n => n.IBookID == iBookID);
            if (lstCart != null)
            {
                lstCart.RemoveAll(n => n.IBookID == iBookID);
                return RedirectToAction("Cart");
            }
            if (lstCart.Count == 0)
            {
                return RedirectToAction("Index", "FPTBook");
            }
            return RedirectToAction("Cart");
        }
        // update cart
        public ActionResult UpdateCart(int iBookID,FormCollection f)
        {
            List<Cart> lstCart = GetCart();
            Cart product = lstCart.SingleOrDefault(n => n.IBookID == iBookID);
            if (product != null)
            {
                product.IQuatity = int.Parse(f["txtQuatity"].ToString());
            }
            return RedirectToAction("Cart");
        }
        // Order
        [HttpGet]
        public ActionResult Order()
        {
            // check login
            if (Session["Username"] == null || Session["Username"].ToString() == "")
            {
                return RedirectToAction("Login", "Username");
            }
            if(Session["Cart"] == null)
            {
                return RedirectToAction("Index", "FPTBook");
            }
            // get: cart
            List<Cart> lstCart = GetCart();
            ViewBag.TotalQuantity = TotalQuantity();
            ViewBag.TotalPrice = TotalPrice();
            return View(lstCart);
        }
        // agree to order
        public ActionResult Order(FormCollection collection)
        {
            //add order
            Order ord = new Order();
            Customer cus = (Customer)Session["Username"];
            List<Cart> gh = GetCart();
            ord.CustomerID = cus.CustomerID;
            ord.OrderDate = DateTime.Now;
            var DeliveryDate = string.Format("{0:MM/dd/yyyy}", collection["DeliveryDate"]);
            ord.DeliDate = DateTime.Parse(DeliveryDate);
            ord.DeliStatus = false;
            ord.ComplePay = false;
            data.Orders.InsertOnSubmit(ord);
            data.SubmitChanges();
            // Add order details
            foreach (var item in gh)
            {
                OrderDetail ctdh = new OrderDetail();
                ctdh.OrderID = ord.OrderID;
                ctdh.BookID = item.IBookID;
                ctdh.Quality = item.IQuatity;
                ctdh.Price = (decimal) item.IPrice;
                data.OrderDetails.InsertOnSubmit(ctdh);            
            }
            data.SubmitChanges();
            Session["Cart"] = null;
            return RedirectToAction("AgreeToOrder","Cart");
        }
        public ActionResult AgreeToOrder()
        {
            return View();
        }
    }
}