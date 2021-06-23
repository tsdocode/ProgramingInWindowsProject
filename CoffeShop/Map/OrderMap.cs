using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoffeShop.Model;

namespace CoffeShop.Map
{
    public class OrderMap : EntityTypeConfiguration<Order>
    {
        public OrderMap() {
            // Primary Key
            HasKey(o => o.OrderID);
            //Property ProductID
            HasRequired(o => o.OrderDetails)
                .WithRequiredPrincipal(o => o.order);
            //M-N relation Order-Product
            HasMany<Product>(o => o.products)
                .WithMany(p => p.Orders)
                .Map(
                    op => {
                        op.MapLeftKey("OrderRefID");
                        op.MapRightKey("ProductRefID", "ProductRefSize");
                        op.ToTable("Order_Product");
                    }
                );
        }
    }
}
