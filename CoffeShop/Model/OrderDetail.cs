using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeShop.Model
{
    public class OrderDetail
    {
        private int orderID;
        private string? customerID;
        private int? tableID;
        private int totalPrice;
        private DateTime date;


        public virtual Order order { get; set; }

        public int OrderID { get => orderID; set => orderID = value; }
        public string? CustomerID { get => customerID; set => customerID = value; }
        public int? TableID { get => tableID; set => tableID = value; }
        public int TotalPrice { get => totalPrice; set => totalPrice = value; }
        public DateTime Date { get => date; set => date = value; }
    }
}
