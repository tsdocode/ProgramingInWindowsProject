using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using CoffeShop.Model;
using CoffeShop.Map;
using CoffeShop.Migrations;

namespace CoffeShop.Context 
{
    public class CoffeShopContext : DbContext
    {
        public CoffeShopContext() : base("name=CoffeShop")
        {
            Database.SetInitializer<CoffeShopContext>(new MigrateDatabaseToLatestVersion<CoffeShopContext, Configuration>());
        }

     
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new OrderMap());
            modelBuilder.Configurations.Add(new OrderDetailMap());
            modelBuilder.Configurations.Add(new ProductMap());


        }
    }
}
