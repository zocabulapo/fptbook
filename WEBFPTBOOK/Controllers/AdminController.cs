using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEBFPTBOOK.Models;
using System.IO;
namespace WEBFPTBOOK.Controllers
{
    public class AdminController : Controller
    {
        // create object manage data
        SqlDataContext data = new SqlDataContext();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult BookManage()
        {
            return View(data.Books.ToList());
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var username = collection["Username"];
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
                Admin admin = data.Admins.SingleOrDefault(n => n.UserAdmin == username && n.PassAdmin == password);
                if (admin != null)
                {
                    ViewBag.Notify = "Login successfully";
                    Session["Username"] = admin;
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ViewBag.Notify = "Username and Password is incorrect";
                }
            }
            return View();
        }
        [HttpGet]
        public ActionResult AddBook()
        {
            ViewBag.TopicID = new SelectList(data.Topics.ToList().OrderBy(n => n.TopicName), "TopicID", "TopicName");
            ViewBag.PubID = new SelectList(data.Publishers.ToList().OrderBy(n => n.PubName), "PubID", "PubName");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddBook(Book bookpic,Book book,HttpPostedFileBase fileupload)
        {
            data.Books.InsertOnSubmit(book);
            data.SubmitChanges();
            ViewBag.TopicID = new SelectList(data.Topics.ToList().OrderBy(n => n.TopicName), "TopicID", "TopicName");
            ViewBag.PubID = new SelectList(data.Publishers.ToList().OrderBy(n => n.PubName), "PubID", "PubName");
            
            if (fileupload == null)
            {
                ViewBag.Notify = "Select Image input";
                return View();
            }
            // add to database
            else
            {
                if (ModelState.IsValid)
                {
                    //save file image
                    var fileName = Path.GetFileName(fileupload.FileName);
                    // save link file
                    var path = Path.Combine(Server.MapPath("~/product_imgs"), Path.GetFileName(fileName));
                    if (System.IO.File.Exists(path))
                        ViewBag.Notify = "image already exists";
                    else
                    {
                        fileupload.SaveAs(path);
                    }
                    bookpic.BookPic = fileName;
                    // Save File
                    data.Books.InsertOnSubmit(bookpic);
                    data.SubmitChanges();
                }
                return RedirectToAction("BookManage");
            }

        }
        // Create delete all
        [HttpGet]
        public ActionResult DeleteAll(int id )
        {
            // Get object to delete
            Book book = data.Books.SingleOrDefault(n => n.BookID == id);
            ViewBag.BookID = book.BookID;
            if (book == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(book);
        }
        [HttpPost, ActionName("DeleteAll")]
        public ActionResult AgreeDelete(int id)
        {
            // Get object to delete
            Book book = data.Books.SingleOrDefault(n => n.BookID == id);
            ViewBag.BookID = book.BookID;
            if (book == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.Books.DeleteOnSubmit(book);
            data.SubmitChanges();
            return RedirectToAction("BookManage");
        }
        public ActionResult EditBook(int id)
        {
            using (var context = new SqlDataContext())
            {
                var data = context.Books.Where(x => x.BookID == id).SingleOrDefault();
                return View(data);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBook(int id, Book model)

        {
            using (var context = new SqlDataContext())
            {
                var data = context.Books.FirstOrDefault(x => x.BookID == id);
              
                if (data == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                else
                {
                    
                    data.BookName = model.BookName;
                    data.Price = model.Price;
                    data.BookDesc = model.BookDesc;
                    data.BookPic = model.BookPic;
                    data.DayUpdate = model.DayUpdate;
                    data.Quality = model.Quality;
                    data.TopicID = model.TopicID;
                    data.PubID = model.PubID;
                    context.SubmitChanges();

                }
                return View();

            }
        }

        //Create Publisher manage
        [HttpGet]
        public ActionResult Publisher()
        {
            return View(data.Publishers.ToList());
        }
        public ActionResult AddPublisher()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddPublisher(Publisher pub)
        {
            data.Publishers.InsertOnSubmit(pub);
            data.SubmitChanges();
            return RedirectToAction("Publisher");
        }

        public ActionResult DeletePubliser( int id )
        {
            var dtl = data.Publishers.SingleOrDefault(n => n.PubID == id);
            data.Publishers.DeleteOnSubmit(dtl);
            data.SubmitChanges();
            return RedirectToAction("Publisher");
        }

        public ActionResult EditPublisher(int id)
        {
            return View(data.Publishers.SingleOrDefault(n => n.PubID == id));
        }
        [HttpPost]
        public ActionResult EditPublisher(Publisher pub)
        {
            UpdateModel(pub);
            data.SubmitChanges();
            return RedirectToAction("Publisher");
        }

        [HttpGet]
        public ActionResult Topic()
        {
            return View(data.Topics.ToList());
        }
        public ActionResult AddTopic()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddTopic(Topic tp)
        {
            data.Topics.InsertOnSubmit(tp);
            data.SubmitChanges();
            return RedirectToAction("Topic");
        }
        public ActionResult DeleteTopic(int id)
        {
            var dtl = data.Topics.SingleOrDefault(n => n.TopicID == id);
            data.Topics.DeleteOnSubmit(dtl);
            data.SubmitChanges();
            return RedirectToAction("Topic");
        }
        public ActionResult EditTopic(int id)
        {
            return View(data.Topics.SingleOrDefault(n => n.TopicID == id));
        }
        [HttpPost]
        public ActionResult EditTopic(Topic tp)
        {
            UpdateModel(tp);
            data.SubmitChanges();
            return RedirectToAction("Topic");
        }
        public ActionResult CustomerManage()
        {
            return View(data.Customers.ToList());
        }
        public ActionResult DeleteCustomer(int id)
        {
            var dcus = data.Customers.SingleOrDefault(n => n.CustomerID == id);
            data.Customers.DeleteOnSubmit(dcus);
            data.SubmitChanges();
            return RedirectToAction("CustomerManage");
        }

    }
}