using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoffeShop.Context;
using CoffeShop.Model;
using System.Windows.Forms;

namespace CoffeShop
{
    class Program
    {
       [STAThread]
        static void Main(string[] args)
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new QuanLy());
        }
    }
}
