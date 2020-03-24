using Moiro_Orders.Models;
using Moiro_Orders.Roles;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Moiro_Orders.XamlView
{
    /// <summary>
    /// Логика взаимодействия для OrderView.xaml
    /// </summary>
    public partial class OrderView : UserControl
    {
        public OrderView()
        {
            InitializeComponent();
            datePick.SelectedDate = DateTime.Now;
        }





        private void AddAllOrders_Click(object sender, RoutedEventArgs e)   //Рабочий метод
        {
            async Task SetOrdersOfDate()
            {
                IUser user = new CurrentUser();
                var status = await user.CreateOrder( new Order {
                    Date = DateTime.Now,
                    Description = "ghjcwf",
                    UserId = PublicResources.Im.Id,
                    Problem= "ldgjsdkhvgksd",
                    Status = "Work"
                });
                MessageBox.Show(status.ToString());
            }
            SetOrdersOfDate().GetAwaiter();
        }




        private void DatePicker_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)    //Рабочий метод (Не удалять!)
        {
            DateTime? selectedDate = datePick.SelectedDate;
            if (selectedDate != null)
            {
                var selectDate = selectedDate.Value.Date;
                async Task GetOrdersOfDate()
                {
                    IUser user = new CurrentUser();
                    var orders = await user.GetOrdersListOfDate(PublicResources.Im.Id, selectDate);
                    listOrders.ItemsSource = orders;
                }
                GetOrdersOfDate().GetAwaiter();
            }

        }


    }
}
