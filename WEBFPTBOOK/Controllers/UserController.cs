using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using WEBFPTBOOK.Models;

namespace WEBFPTBOOK.Controllers
{
    public class UserController : Controller
    {
        // create object manage data
        SqlDataContext data = new SqlDataContext();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        // GET : USER
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(Customer cus)
        {
            cus.Password = EncodePassword(cus.Password);
            // add value to form
            data.Customers.InsertOnSubmit(cus);
            data.SubmitChanges();
            return RedirectToAction("Index", "FPTBook");

        }
        public static string EncodePassword(string originalPassword)
        {
            //Declarations
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;

            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            md5 = new MD5CryptoServiceProvider();
            originalBytes = System.Text.ASCIIEncoding.Default.GetBytes(originalPassword);
            encodedBytes = md5.ComputeHash(originalBytes);

            //Convert encoded bytes back to a 'readable' string
            return BitConverter.ToString(encodedBytes);
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var username = collection["UserName"];
            var password = collection["Password"];
            if (string.IsNullOrEmpty(username))
            {
                ViewData["Error1"] = "Please ente username!!!";
            }
            if (string.IsNullOrEmpty(password))
            {
                ViewData["Error2"] = "Please enter password!!!";
            }
            else
            {
                Customer cus = data.Customers.SingleOrDefault(n => n.UserName == username && n.Password == password);
                if (cus != null)
                {
                    ViewBag.Notify = "Login successfully";
                    Session["Username"] = cus.UserName;
                    Session["id"] = cus.CustomerID;
                    return RedirectToAction("Index", "FPTBook");
                }
                else
                {
                    ViewBag.Notify = "Username and Password is incorrect";
                }
            }
            return View();
        }
        //GET : /User/EditUser
        public ActionResult EditCus(int id)
        {
            return View(data.Customers.SingleOrDefault(n => n.CustomerID == id));

        }
        [HttpPost]
        public ActionResult EditCus(Customer cus)
        {
            UpdateModel(cus);
            data.SubmitChanges();
            return RedirectToAction("Edit");
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "FPTBook");
        }
    }
       
}