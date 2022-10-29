using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WEBFPTBOOK.Models
{
    public class Cart
    {
        SqlDataContext data = new SqlDataContext();
        public int IBookID { get; set; }
        public string IBookName { get; set; }
        public string IBookPic { get; set; }
        public Double IPrice { get; set; }
        public int IQuatity { get; set; }
        public Double ITotal {
            get { return IQuatity * IPrice; }       
        }
        public Cart(int BookID)
        {
            IBookID = BookID;
            Book book = data.Books.Single(n => n.BookID == IBookID);
            IBookName = book.BookName;
            IBookPic = book.BookPic;
            IPrice = double.Parse(book.Price.ToString());
            IQuatity = 1;
        }

    }
}