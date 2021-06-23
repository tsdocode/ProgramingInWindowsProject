using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeShop.Model
{
    public class Product
    {
        private string productID;
        private string name;
        private byte[] image;

        private string? size;
        private int price;


        public virtual ICollection<Order> Orders { get; set; }
        public string ProductID { get => productID; set => productID = value; }
        public string Name { get => name; set => name = value; }
        public byte[] Image { get => image; set => image = value; }
        public string Size { get => size; set => size = value; }
        public int Price { get => price; set => price = value; }
    }
}
