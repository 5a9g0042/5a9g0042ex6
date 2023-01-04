using CsvHelper;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HW6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Drink> drinks = new List<Drink>();
        List<OrderItem> order = new List<OrderItem>();
        string takeout;
        public MainWindow()
        {
            InitializeComponent();

            AddNewDrink(drinks);

            DisplayDrinks(drinks);
        }

        private void DisplayDrinks(List<Drink> drinks)
        {
            foreach (Drink d in drinks)
            {
                StackPanel sp = new StackPanel();
                CheckBox cb = new CheckBox();
                Slider sl = new Slider();
                Label lb = new Label();
                sp.Orientation = Orientation.Horizontal;
                cb.Content = d.Name + d.Size + d.Price;
                cb.Margin = new Thickness(5);
                cb.Width = 150;
                cb.Height = 25;
                sl.Value = 0;
                sl.Width = 100;
                sl.Minimum = 0;
                sl.Maximum = 10;
                sl.TickPlacement = TickPlacement.TopLeft;
                sl.TickFrequency = 1;
                sl.IsSnapToTickEnabled = true;
                lb.Width = 50;
                Binding myBinding = new Binding("Value");
                myBinding.Source = sl;
                lb.SetBinding(ContentProperty, myBinding);
                sp.Children.Add(cb);
                sp.Children.Add(sl);
                sp.Children.Add(lb);
                stackpanel_drinkmenu.Children.Add(sp);
            }
        }

        private void AddNewDrink(List<Drink> mydrinks)
        {
            //mydrinks.Add(new Drink() { Name = "咖啡", Size = "大杯", Price = 60 });
            //mydrinks.Add(new Drink() { Name = "咖啡", Size = "中杯", Price = 50 });
            //mydrinks.Add(new Drink() { Name = "紅茶", Size = "大杯", Price = 30 });
            //mydrinks.Add(new Drink() { Name = "紅茶", Size = "中杯", Price = 20 });
            //mydrinks.Add(new Drink() { Name = "綠茶", Size = "大杯", Price = 25 });
            //mydrinks.Add(new Drink() { Name = "綠茶", Size = "中杯", Price = 20 });
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "CSV檔案|*.csv|文字檔案|*.txt|所有檔案|*.*";
            dialog.DefaultExt = "*.csv";
            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;
                StreamReader sr = new StreamReader(path, Encoding.Default);
                CsvReader csv = new CsvReader(sr, CultureInfo.InvariantCulture);
                csv.Read();
                csv.ReadHeader();
                while (csv.Read() == true)
                {
                    Drink d = new Drink() { Name = csv.GetField("Name"), Size = csv.GetField("Size"), Price = csv.GetField<int>("Price") };
                    mydrinks.Add(d);
                }

            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb.IsChecked == true)
            {
                takeout = rb.Content.ToString();
            }
        }

        private void orderbutton_Click(object sender, RoutedEventArgs e)
        {
            displayTextBlock.Text = "";
            PlaceOrder(order);
            DisplayOrderDetail(order);
        }

        private void PlaceOrder(List<OrderItem> order)
        {
            order.Clear();
            for (int i = 0; i < stackpanel_drinkmenu.Children.Count; i++)
            {
                StackPanel sp = stackpanel_drinkmenu.Children[i] as StackPanel;
                CheckBox cb = sp.Children[0] as CheckBox;
                Slider sl = sp.Children[1] as Slider;
                int quantity = Convert.ToInt32(sl.Value);
                if (cb.IsChecked == true && quantity != 0)
                {
                    int price = drinks[i].Price;
                    int subtotal = price * quantity;
                    order.Add(new OrderItem() { Index = i, Quantity = quantity, SubTotal = subtotal });
                }
            }
        }

        private void DisplayOrderDetail(List<OrderItem> order)
        {
            StreamWriter sw = new StreamWriter("C:\\5a9g0001\\HW6\\Order.txt");
            int total = 0, num;
            displayTextBlock.Text = "";
            int i = 1;
            sw.Write( $"您所訂購的飲品{takeout}，訂購細項如下:\n");
            foreach (OrderItem item in order)
            {
                total += item.SubTotal;
                Drink drinkItem = drinks[item.Index];
                sw.WriteLine( $"訂購品項{i}:{drinkItem.Name}{drinkItem.Size}，單價{drinkItem.Price}元 X {item.Quantity}，小計{item.SubTotal}元。\n");
                i++;
            }
            sw.WriteLine("\n");
            sw.WriteLine( $"訂單彙總如下:\n");
            if (total >= 500)
            {
                num = (int)(total * 0.8);
                sw.WriteLine( $"原價{total}元，訂購總額滿500元以上，售價為8折，售價為{num}元");
            }
            else if (total >= 300)
            {
                num = (int)(total * 0.85);
                sw.WriteLine( $"原價{total}元，訂購總額滿300元以上，售價為85折，售價為{num}元");
            }
            else if (total >= 200)
            {
                num = (int)(total * 0.9);
                sw.WriteLine($"原價{total}元，訂購總額滿200元以上，售價為9折，售價為{num}元");
            }
            else
            {
                sw.WriteLine( "\n");
            }
            sw.Close();
        }
    }
}
