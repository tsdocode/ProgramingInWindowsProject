using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeShop.Model
{
    public class Order
    {
        private int orderID;

        public virtual OrderDetail OrderDetails { get; set; }
        public virtual ICollection<Product> products { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderID { get => orderID; set => orderID = value; }

    }
}
