using Moiro_Orders.Models;
using Moiro_Orders.Roles;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Moiro_Orders.XamlView
{
    /// <summary>
    /// Логика взаимодействия для OrderView.xaml
    /// </summary>
    public partial class OrderView : UserControl
    {
        private bool isProblem = true;
        public Order selectedOrder;


        public OrderView()
        {
            InitializeComponent();
            datePick.SelectedDate = DateTime.Now;
            if (PublicResources.Im.Admin)
            {
                
                addOrder.Visibility = Visibility.Hidden;
            }
        }


        private void DatePicker_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime? selectedDate = datePick.SelectedDate;
            if (selectedDate != null)
            {
                var selectDate = selectedDate.Value.Date;
                if (PublicResources.Im.Admin)
                {
                    GetOrdersOfDateAdmin(selectDate).GetAwaiter();
                }
                else
                {
                    GetOrdersOfDateUser(selectDate).GetAwaiter();
                }
            }
        }

        private void ListOrders_Selected(object sender, SelectionChangedEventArgs e)
        {
            if (PublicResources.Im.Admin)
            {
                selectedOrder = (Order)e.AddedItems[0];
                AcceptOrder.Visibility = Visibility.Visible;
                Cancel.Visibility = Visibility.Visible;
            }
            else
            {
                isProblem = true;
                selectedOrder = (Order)e.AddedItems[0];
                changeOrder.Visibility = Visibility.Visible;
                DeleteOrder.Visibility = Visibility.Visible;
                addOrder.Visibility = Visibility.Hidden;
                Cancel.Visibility = Visibility.Visible;
            }          
        }

        private void AddOrder_Click(object sender, RoutedEventArgs e) //Кнопка добавить заявку
        {
            addOrder.Visibility = Visibility.Hidden;
            listOrders.Visibility = Visibility.Hidden;
            isProblem = false;
        }

        private void ChangeOrder_Click(object sender, RoutedEventArgs e)
        {
            isProblem = true;
            listOrders.Visibility = Visibility.Hidden;
            problem.Text = selectedOrder.Problem;
            description.Text = selectedOrder.Description;
            changeOrder.Visibility = Visibility.Hidden;
            DeleteOrder.Visibility = Visibility.Hidden;
            addOrder.Visibility = Visibility.Hidden;
            Cancel.Visibility = Visibility.Hidden;
        }

        private void SaveOrder_Click(object sender, RoutedEventArgs e)
        {
            
            if (isProblem == true)
            {
                UpdateOrderAsync().GetAwaiter();
            }
            else
            {
                CreateNewOrder().GetAwaiter();
            }
        }

        private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedOrder().GetAwaiter();
        }

        private void BackToOrderList_Click(object sender, RoutedEventArgs e)
        {
            addOrder.Visibility = Visibility.Visible;
            Cancel.Visibility = Visibility.Hidden;
            problem.Text = null;
            description.Text = null;
            if (selectedOrder != null)
            {
                GetOrdersOfDateUser(DateTime.Now).GetAwaiter();
                datePick.SelectedDate = DateTime.Now;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (PublicResources.Im.Admin)
            {
                GetOrdersOfDateAdmin(selectedOrder.Date).GetAwaiter();
                AcceptOrder.Visibility = Visibility.Hidden;
                Cancel.Visibility = Visibility.Hidden;
            }
            else
            {
                GetOrdersOfDateUser(selectedOrder.Date).GetAwaiter();
                datePick.SelectedDate = selectedOrder.Date;
                addOrder.Visibility = Visibility.Visible;
                changeOrder.Visibility = Visibility.Hidden;
                DeleteOrder.Visibility = Visibility.Hidden;
                Cancel.Visibility = Visibility.Hidden;
            }        
        }

        private void AcceptOrder_Click(object sender, RoutedEventArgs e)
        {
            ProblemView.Text = selectedOrder.Problem;
            DescriptionView.Text = selectedOrder.Description;
            UserView.Text = selectedOrder.UserName;
            DateView.Text = selectedOrder.Date.ToString();
            LoginView.Text = selectedOrder.UserId.ToString();
            RoomView.Text = selectedOrder.Room.ToString();

            // Метод ниже не работает, не присваевается значение)))
            //StatusView.Text = selectedOrder.Status +"rl;gnklsdfnv";
            //ChangeOrderStatusAdmin().GetAwaiter();  
        }




        #region ASYNC metods

        //User Metods
        async Task GetOrdersOfDateUser(DateTime selectDate)
        {
            listOrders.Visibility = Visibility.Visible;
            IUser user = new CurrentUser();
            var orders = await user.GetOrdersListOfDate(PublicResources.Im.Id, selectDate);
            listOrders.ItemsSource = orders;
        }

        async Task UpdateOrderAsync()
        {
            selectedOrder.Problem = problem.Text;
            selectedOrder.Description = description.Text;

            IUser user = new CurrentUser();
            var status = await user.EditOrder(selectedOrder);
            problem.Text = null;
            description.Text = null;
            addOrder.Visibility = Visibility.Visible;
            GetOrdersOfDateUser(selectedOrder.Date).GetAwaiter();
            MessageBox.Show(status.ToString());
        }

        async Task CreateNewOrder()
        {
            IUser user = new CurrentUser();
            var status = await user.CreateOrder(new Order
            {
                Date = DateTime.Now,
                Description = description.Text,
                UserId = PublicResources.Im.Id,
                Problem = problem.Text,
                StatusId = 2
            });
            problem.Text = null;
            description.Text = null;
            addOrder.Visibility = Visibility.Visible;
            GetOrdersOfDateUser(DateTime.Now).GetAwaiter();
            datePick.SelectedDate = DateTime.Now;
            MessageBox.Show(status.ToString());
        }

        async Task DeleteSelectedOrder()
        {
            IUser user = new CurrentUser();
            var status = await user.DeleteOrder(selectedOrder.Id);
            GetOrdersOfDateUser(selectedOrder.Date).GetAwaiter();
            changeOrder.Visibility = Visibility.Hidden;
            DeleteOrder.Visibility = Visibility.Hidden;
            addOrder.Visibility = Visibility.Visible;
            Cancel.Visibility = Visibility.Hidden;
            MessageBox.Show(status.ToString());
        }

        //Admin Metods
        async Task GetOrdersOfDateAdmin(DateTime selectDate)
        {
            Order order = new Order();
            IAdmin admin = new CurrentUser();
            var orders = await admin.GetAllOrdersToday(selectDate);
            listOrders.ItemsSource = orders;
        }

        //async Task ChangeOrderStatusAdmin() 
        //{
        //    IAdmin admin = new CurrentUser();
        //    var status = await admin.EditOrder(selectedOrder);
        //    problem.Text = null;
        //    description.Text = null;
        //    addOrder.Visibility = Visibility.Visible;
        //    GetOrdersOfDateAdmin(selectedOrder.Date).GetAwaiter();
        //    MessageBox.Show(status.ToString());
        //}




        #endregion

        private void SaveOrderAdmin_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BackToOrderAdmin_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
