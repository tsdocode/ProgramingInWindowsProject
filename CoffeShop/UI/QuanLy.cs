using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CoffeShop.Context;
using LiveCharts;
using LiveCharts.Wpf;

namespace CoffeShop.Model
{
    public partial class QuanLy : Form
    {
        public CoffeShopContext contex;
        public Order currentOrder;
        public Product ModifyProduct;
        public bool add;
        static readonly Random rnd = new Random();
        Stream imageStream;
        public QuanLy()
        {
            InitializeComponent();
            contex = new CoffeShopContext();
            Init();
        }


        ///////////////Form nhập order

        private void btnThanhToan_Click(object sender, EventArgs e)
        {
            PagMain.SetPage(0);
        }
        //Khởi tạo custom
        public void Init()
        {
            //Thêm hay Update 
            add = true;
            //Tắt chức năng tự thêm cột của datagridview 
            dgwOrder.AutoGenerateColumns = false;
            dgwProduct.AutoGenerateColumns = false;
            //Thêm auto complete cho mã Sản phẩm 
            AutoCompleteStringCollection cbAuto = new AutoCompleteStringCollection();
            //Lấy các ID từ bảng Product 
            var productID = contex.Products
                .Select(p => p.ProductID).Distinct().ToList();

            cbID.DataSource = productID;
            cbID.AutoCompleteCustomSource.AddRange(productID.ToArray());
            cbID.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cbID.AutoCompleteSource = AutoCompleteSource.CustomSource;

            //Khởi tạo order rỗng 
            this.currentOrder = new Order();
            this.currentOrder.products = new List<Product>();

            //Thêm size cho form nhập món 
            this.cbProSize.Items.Add("S");
            this.cbProSize.Items.Add("M");
            this.cbProSize.Items.Add("L");
            this.cbProSize.Items.Add("XL");
        }


        //Reload dữ liệu form nhập order 
        public void ReloadData()
        {
            currentOrder = new Order();
            currentOrder.products = new List<Product>();
            dgwOrder.DataSource = currentOrder.products;
        }

        //Random Date để sinh dữ liệu 
        public static DateTime GetRandomDateTime(DateTime? min = null, DateTime? max = null)
        {
            min = min ?? new DateTime(2021, 1, 1);
            max = max ?? new DateTime(2021, 5, 31);

            var range = max.Value - min.Value;
            var randomUpperBound = (Int32)range.TotalSeconds;
            if (randomUpperBound <= 0)
                randomUpperBound = rnd.Next(1, Int32.MaxValue);

            var randTimeSpan = TimeSpan.FromSeconds((Int64)(range.TotalSeconds - rnd.Next(0, randomUpperBound)));
            return min.Value.Add(randTimeSpan);
        }

        //Nút thêm sản phẩm ở form Order
        private void btnAdd_Click(object sender, EventArgs e)
        {
            //Nếu update thì xóa sản phẩm cũ 
            if (!add)
            {
                currentOrder.products.Remove(ModifyProduct);
                add = true;
            }

            //Khởi tạo sản phẩm mới
            var productID = cbID.SelectedItem.ToString();
            var productSize = cbSize.SelectedItem.ToString();

            var product = (from p in contex.Products
                           where (p.ProductID == productID) && (p.Size == productSize)
                           select p
                            ).FirstOrDefault<Product>();
            //Thêm sản phẩm mới vào order hiện tại
            this.currentOrder.products.Add(product);


            dgwOrder.DataSource = currentOrder.products.ToList();


        }


        //Button xác nhận đơn
        private void bunifuFlatButton2_Click(object sender, EventArgs e)
        {

            // Khai báo biến traloi
            DialogResult traloi;
            // Hiện hộp thoại hỏi đáp
            traloi = MessageBox.Show("Xác nhận đơn ?", "Trả lời",
            MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            // Kiểm tra có nhắp chọn nút Ok không?
            if (traloi == DialogResult.OK) {
                int total = dgwOrder.Rows.Cast<DataGridViewRow>()
                   .Sum(t => Convert.ToInt32(t.Cells[4].Value));

                //Lấy random date để tạo dữ liệu
                currentOrder.OrderDetails = new OrderDetail()
                {
                    TotalPrice = total,
                    Date = GetRandomDateTime()
                };

                contex.Orders.Add(currentOrder);
                contex.SaveChanges();
                printBillPreview.PrintPreviewControl.Zoom = 1.5;
                printBillPreview.ClientSize = new Size(900, 800);
                if (printBillPreview.ShowDialog() == DialogResult.OK)
                {
                    printBill.Print();
                }
                ReloadData();
            }


            


        }


        //Thay đổi textbox Tên sản phẩm theo ID
        private void cbID_SelectedIndexChanged(object sender, EventArgs e)
        {
            var productID = cbID.SelectedItem.ToString();
            var query = from product in contex.Products
                        where product.ProductID == productID
                        select product.Name;
            var productName = query.FirstOrDefault();
            tbName.Text = productName.ToString();

            var sizeQuery = from p in contex.Products
                            where p.ProductID == productID
                            select p.Size;
            var productSize = sizeQuery;
            cbSize.DataSource = productSize.ToList();
        }

        //Rest button 
        private void btnReset_Click(object sender, EventArgs e)
        {
            ReloadData();
        }


        //Sửa sanr phẩm trong đơn
        private void btnEdit_Click(object sender, EventArgs e)
        {
            add = false;
            int idx = dgwOrder.CurrentCell.RowIndex;
            var productID = cbID.SelectedItem.ToString();
            var productSize = cbSize.SelectedItem.ToString();

            this.cbID.SelectedItem = dgwOrder.Rows[idx].Cells[0].Value.ToString();
            this.cbSize.SelectedItem = dgwOrder.Rows[idx].Cells[3].Value.ToString();
            var removeProduct = (from p in contex.Products
                                 where (p.ProductID == productID) && (p.Size == productSize)
                                 select p
                               ).FirstOrDefault<Product>();
            ModifyProduct = removeProduct;
        }

        //Xóa sản phẩm
        private void btnDelete_Click(object sender, EventArgs e)
        {
            int idx = dgwOrder.CurrentCell.RowIndex;
            var productID = cbID.SelectedItem.ToString();
            var productSize = cbSize.SelectedItem.ToString();

            this.cbID.SelectedItem = dgwOrder.Rows[idx].Cells[0].Value.ToString();
            var removeProduct = (from p in contex.Products
                                 where (p.ProductID == productID) && (p.Size == productSize)
                                 select p
                               ).FirstOrDefault<Product>();
            currentOrder.products.Remove(removeProduct);
            dgwOrder.DataSource = currentOrder.products;
        }


        //In hóa đơn
        private void printBill_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Bitmap bm = new Bitmap(this.dgwOrder.Width, this.dgwOrder.Height);
            dgwOrder.DrawToBitmap(bm, new Rectangle(0, 0, this.dgwOrder.Width, this.dgwOrder.Height));
            e.Graphics.DrawImage(bm, 0, 0);
            e.Graphics.DrawString("Tổng đơn = " + currentOrder.OrderDetails.TotalPrice.ToString(), new Font("Arial", 15, FontStyle.Regular), Brushes.Black, new Point(0, 400));
        }

        ////////Form Nhập món 
        //Chuyển sang form thêm món 
        private void btnForgot_Click(object sender, EventArgs e)
        {
            PagMain.SetPage(1);
            //Khởi tạo datagridview 
            dgwProduct.DataSource = contex.Products.ToList();
            add = true;
            //Gán ảnh mặc định 
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            picProduct.Image = Image.FromFile(path + "\\Assets\\logo.png");
            tbProID.Text = tbProName.Text = tbPrice.Text = "";
        }

        //Thay đổi ảnh sản phẩm
        private void bunifuPictureBox1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "JPEG|*.jpg", ValidateNames = true, Multiselect = false })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var filename = ofd.FileName;
                    /*MessageBox.Show(filename);*/
                    picProduct.Image = Image.FromFile(filename);
                }
            }
        }


        //Chuyển ảnh thành mảng byte
        byte[] ImageToBytes(Image img)
        {
            using (var ms = new MemoryStream())
            {
                Bitmap bmp = new Bitmap(img);
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        //Thêm sanr phẩm
        private void btnProAdd_Click(object sender, EventArgs e)
        {
            var new_product = new Product();
            try
            {
                if (!add)
                {
                    contex.Products.Remove(ModifyProduct);
/*                    contex.SaveChanges();*/
                    add = true;
                }
                var ProductID = this.tbProID.Text;
                var ProductName = this.tbProName.Text;
                int ProductPrice = int.Parse(this.tbPrice.Text);
                Image test = this.picProduct.Image;
                var ProductImage = ImageToBytes(test);
                var ProductSize = cbProSize.SelectedItem.ToString();
                new_product = new Product()
                {
                    ProductID = ProductID,
                    Name = ProductName,
                    Size = ProductSize,
                    Price = ProductPrice,
                    Image = ProductImage
                };
                contex.Products.Add(new_product);
                contex.SaveChanges();
                dgwProduct.DataSource = contex.Products.ToList();
            }
            catch (Exception ex) {
                contex.Products.Remove(new_product);
                MessageBox.Show(ex.Message, "Lỗi",
    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        //Chuyển byte[] thành ảnh
        Image bytesToImage(byte[] b)
        {
            using (var ms = new MemoryStream(b))
            {
                return Image.FromStream(ms);
            }
        }

        //Nút sửa sản phẩm
        private void bunifuButton3_Click(object sender, EventArgs e)
        {
            add = false;
            int idx = dgwProduct.CurrentCell.RowIndex;
            var ProductID = dgwProduct.Rows[idx].Cells[0].Value.ToString();
            var ProductName = dgwProduct.Rows[idx].Cells[2].Value.ToString();
            var ProductPrice = dgwProduct.Rows[idx].Cells[4].Value.ToString();
            var imageBytes = (byte[])dgwProduct.Rows[idx].Cells[1].Value;
            var ProductImage = bytesToImage(imageBytes);
            var ProductSize = dgwProduct.Rows[idx].Cells[3].Value.ToString();

            picProduct.Image = bytesToImage(imageBytes);
            tbProID.Text = ProductID;
            tbProName.Text = ProductName;
            cbProSize.SelectedItem = ProductSize;
            cbProSize.Text = ProductSize;
            tbPrice.Text = ProductPrice;
            /* MessageBox.Show(ProductImage);*/

            var removeProduct = (from p in contex.Products
                                 where (p.ProductID == ProductID) && (p.Size == ProductSize)
                                 select p
                               ).FirstOrDefault<Product>();
            ModifyProduct = removeProduct;
        }


        //Nút xóa sản phẩm
        private void bunifuButton2_Click(object sender, EventArgs e)
        {
            int idx = dgwProduct.CurrentCell.RowIndex;


            var ProductID = dgwProduct.Rows[idx].Cells[0].Value.ToString();
            var ProductSize = dgwProduct.Rows[idx].Cells[3].Value.ToString();

            var removeProduct = (from p in contex.Products
                                 where (p.ProductID == ProductID) && (p.Size == ProductSize)
                                 select p
                               ).FirstOrDefault<Product>();

            contex.Products.Remove(removeProduct);
            contex.SaveChanges();
            dgwProduct.DataSource = contex.Products.ToList();
        }


        //Chuyển sang form Thêm hàng
        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            picProduct.Image = Image.FromFile(path + "\\Assets\\logo.png");
            tbProID.Text = tbProName.Text = tbPrice.Text = "";
        }

        //Chỉ nhập số
        private void tbPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            if (((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        //////////Form Báo cáo

        //Chuyển sang form báo cáo 
        private void btnReport_Click(object sender, EventArgs e)
        {
            PagMain.SetPage(2);
            DrawChart();


        }

        //Vẽ biểu đồ 
        public void DrawChart() {
            moneyByDate();
            drawWeek();
            drawProduct();
        }


        //Biểu đồ doanh thu theo ngày 
        public void moneyByDate() {
            //Query tổng doanh thu theo ngày 
            var OD = contex.OrderDetails.ToList();
            var income = OD
                .GroupBy(od => od.Date.Date)
                .OrderBy(od => od.Key.Date)
                .Select(od => new OrderDetail
                {
                    Date = od.Key.Date,
                    TotalPrice = od.Sum(c => c.TotalPrice)

                }).ToList();
            List<int> incomes = new List<int>();
            List<string> dates  = new List<string>();

            foreach (var ic in income) {
                incomes.Add(ic.TotalPrice);
                dates.Add(ic.Date.Date.ToShortDateString());
            }


            //Thêm dữ liệu vào livechart
            chartIncome.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Thu nhập ",
                    Values = new ChartValues<int>{ },
                },
            };

            chartIncome.AxisX.Add(new Axis
            {
                Title = "Date",
                Labels = dates
            });

            foreach (int ic in incomes)
            {
                chartIncome.Series[0].Values.Add(ic);
            }
        }

        //Biểu đồ thu nhập trong các ngày trong tuần
        public void drawWeek() {
            //Query doanh thu theo thứ trong tuần
            var OD = contex.OrderDetails.ToList();
            var income = OD
                .GroupBy(od => od.Date)
                .Select(od => new OrderDetail
                {
                    Date = od.Key.Date,
                    TotalPrice = od.Sum(c => c.TotalPrice)
                }).ToList();

            var dates = income
                .OrderBy(ic => ic.Date.DayOfWeek)
                .Select(ic => ic.Date.DayOfWeek.ToString()).Distinct().ToList();
            income = income
                .OrderBy(ic => ic.Date.DayOfWeek)
                .GroupBy(ic => ic.Date.DayOfWeek)
                .Select(ic => new OrderDetail
                {
                    TotalPrice = ic.Sum(c => c.TotalPrice)
                }).ToList();
            List<int> incomes = new List<int>();

            foreach (var ic in income)
            {
                incomes.Add(ic.TotalPrice);
            }


            //Thêm dữ liệu vào chart
            chartWeekIncome.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Thu nhập ",
                    Values = new ChartValues<int>{ },
                },
            };

            chartWeekIncome.AxisX.Add(new Axis
            {
                Title = "Thứ",
                Labels = dates,
                LabelFormatter = value => value.ToString("N"),
                Separator = new Separator // force the separator step to 1, so it always display all the labels
                {
                    Step = 1, // if you don't force the separator, it will be calculated automatically, and could skip some labels
                    IsEnabled = false //disable it to make it invisible.
                }
            });

            

            foreach (int ic in incomes)
            {
                chartWeekIncome.Series[0].Values.Add(ic);
            }
        }

        //Biểu đồ sản phẩm ưa thích 
        public void drawProduct() {
            //Query count(số đơn ) theo sản phẩm 
            var OD = contex.Orders;
            var product_order = OD
                .Where(od => od.products.Count() > 0)
                .Select(od => od.products
                    .Select(p => p.ProductID)
                )
                .ToList();

            List<string> flattenedList = product_order.SelectMany(x => x).ToList();
            var frequency = flattenedList.GroupBy(x => x).OrderByDescending(x => x.Count()).ToDictionary(x => x.Key, x => x.Count()).Take(10);

            Func<ChartPoint, string> labelPoint = chartPoint =>
                string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);

            foreach (var item in frequency) {
                charProductPrefe.Series.Add(
                     new PieSeries
                     {
                         Title = item.Key,
                         Values = new ChartValues<double> { item.Value },
                         PushOut = 2,
                         DataLabels = false,
                         LabelPoint = labelPoint
                     }
                    );
            }

            charProductPrefe.LegendLocation = LegendLocation.Bottom;

        }


        //Thoát khỏi chương trình
        private void btnExit_Click(object sender, EventArgs e)
        {
            // Khai báo biến traloi
            DialogResult traloi;
            // Hiện hộp thoại hỏi đáp
            traloi = MessageBox.Show("Thoát ?", "Trả lời",
            MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            // Kiểm tra có nhắp chọn nút Ok không?
            if (traloi == DialogResult.OK) Application.Exit();
        }


        
    }
}
