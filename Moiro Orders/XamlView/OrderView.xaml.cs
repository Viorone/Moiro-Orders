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
                    Description = "Бысл сломан компьютер последством внешнего вмешательства сверхестественных сил",
                    UserId = PublicResources.Im.Id,
                    Problem= "Компьютер поднял бунд и устроил революцию",
                    Status = "Работаем"
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

        private void ListOrders_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void ListOrders_Selected(object sender, SelectionChangedEventArgs e)
        {
            var model = (Order)e.AddedItems[0];
            //MessageBox.Show(model.Description);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
