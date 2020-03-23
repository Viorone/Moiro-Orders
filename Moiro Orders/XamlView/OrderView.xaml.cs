using Moiro_Orders.Models;
using Moiro_Orders.Roles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Moiro_Orders.XamlView
{
    /// <summary>
    /// Логика взаимодействия для OrderView.xaml
    /// </summary>
    public partial class OrderView : Page
    {
        public OrderView()
        {
            InitializeComponent();
        }

        public ObservableCollection<Order> Orders { get; private set; }

        private void AddAllOrders_Click(object sender, RoutedEventArgs e)
        {
            async Task SetOrdersOfDate()
            {
                IUser user = new CurrentUser();
                var orders = await user.CreateOrder( new Order {
                    Date = DateTime.Now,
                    Description = "ghjcwf",
                    UserId = PublicResources.Im.Id,
                    Problem= "ldgjsdkhvgksd",
                    Status = "Work"
                });
                MessageBox.Show(orders.ToString());
            }
            SetOrdersOfDate().GetAwaiter();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Отобразить элемент календаря
        }

        private void DatePicker_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime? selectedDate = datePick.SelectedDate;

            var selectDate = selectedDate.Value.Date;
            async Task GetOrdersOfDate()
            {
                IUser user = new CurrentUser();
                var orders = await user.GetOrdersListOfDate(PublicResources.Im.Id, selectDate);
                Orders = new ObservableCollection<Order>(orders);
                foreach (var tmp in Orders)
                {
                    listOrders.Items.Add(tmp);
                    MessageBox.Show(tmp.Status);
                }
            }
             GetOrdersOfDate().GetAwaiter();

        }
        
    }
}
