using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoffeShop.Model;
using System.Data.Entity.ModelConfiguration;


namespace CoffeShop.Map
{
    public class OrderDetailMap : EntityTypeConfiguration<OrderDetail>
    {
        public OrderDetailMap()
        {
            HasKey(o => o.OrderID);
        }
    }
}
