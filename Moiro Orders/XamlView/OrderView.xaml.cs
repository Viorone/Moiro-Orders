using Moiro_Orders.Models;
using Moiro_Orders.Roles;
using Moiro_Orders.ViewModel;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Moiro_Orders.XamlView
{
    /// <summary>
    /// Логика взаимодействия для OrderView.xaml
    /// </summary>
    public partial class OrderView : UserControl
    {
        bool isProblem = true;
        Order selectedOrder = new Order();
        bool click = true;
        List<Order> ordersTmp;

        public OrderView()
        {
            InitializeComponent();
            datePick.SelectedDate = DateTime.Now.Date;
            if (PublicResources.Im.Admin)
            {         
                addOrder.Visibility = Visibility.Hidden;
                List<string> sortList = new List<string>
                {
                    "Сначала новые",
                    "Сначала старые",
                    "По статусу",
                    "В очереди на выполнение",
                    "Выполняются",
                    "Выполнены",
                    "Требуется ремонт/закупка",
                    "Отменено"
                };
                OrderSortBox.Visibility = Visibility.Visible;
                OrderSortBox.IsEnabled = true;
                OrderSortBox.ItemsSource = sortList;
                if (PublicResources.sortCount == -1)
                {
                    OrderSortBox.Text = OrderSortBox.Items[2].ToString();
                }
                else
                {
                    OrderSortBox.Text = OrderSortBox.Items[PublicResources.sortCount].ToString();
                }
            }
        }

        private void DatePicker_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (datePick.SelectedDate != null && click == true)
            {
                var selectDate = datePick.SelectedDate.Value.Date;
                if (PublicResources.Im.Admin) //admin
                {
                    selectedOrder = new Order();
                    click = false;
                    Task.Run(() => ClickSaver());
                    UpdateOrdersListAdmin();

                    AcceptOrder.Visibility = Visibility.Hidden;
                }
                else //user
                {
                    selectedOrder = new Order();
                    click = false;
                    Task.Run(() => ClickSaver());
                    UpdateOrdersListUser();

                    AcceptCompleteOrder.Visibility = Visibility.Hidden;
                    DeleteOrder.Visibility = Visibility.Hidden;
                    changeOrder.Visibility = Visibility.Hidden;
                    addOrder.Visibility = Visibility.Visible;
                }
            }
        }

        private void ListOrders_Selected(object sender, SelectionChangedEventArgs e)
        {
            if (PublicResources.Im.Admin) //admin
            {
                if (e.AddedItems.Count == 0)
                {
                    return;
                }
                selectedOrder = (Order)e.AddedItems[0];
                AcceptOrder.Visibility = Visibility.Hidden;

                if (selectedOrder.StatusId != 3 && selectedOrder.StatusId != 5 && selectedOrder.AdminId == PublicResources.Im.Id || selectedOrder.StatusId != 3 && selectedOrder.StatusId != 5 && selectedOrder.AdminId == null)
                {
                    AcceptOrder.Visibility = Visibility.Visible;
                }
            }
            else //user
            {
                if (e.AddedItems.Count == 0)
                {
                    return;
                }
                problem.Text = null;
                description.Text = null;
                isProblem = true;
                selectedOrder = (Order)e.AddedItems[0];
                var nowTime = DateTime.Now;
                var changeTime = nowTime - selectedOrder.Date;
                DateTime time = Convert.ToDateTime("00:30:00");
                DeleteOrder.Visibility = Visibility.Hidden;
                changeOrder.Visibility = Visibility.Hidden;
                AcceptCompleteOrder.Visibility = Visibility.Hidden;
                if (changeTime < time.TimeOfDay && selectedOrder.StatusId != 3)  //Time chek
                {
                    if (selectedOrder.StatusId != 5)
                    {
                        DeleteOrder.Visibility = Visibility.Visible;
                        changeOrder.Visibility = Visibility.Visible;
                    }
                }
                if (selectedOrder.StatusId == 2)
                {
                    AcceptCompleteOrder.Visibility = Visibility.Visible;
                }
            }
        }

        private void AddOrder_Click(object sender, RoutedEventArgs e) //user
        {
            if (click)
            {
                click = false;
                Task.Run(() => ClickSaver());
                isProblem = false;
                PublicResources.ordersCts.Cancel();
                problem.Text = null;
                description.Text = null;
            }
            OrderDetails.IsEnabled = true;
            OrdersButtonPanel.IsEnabled = false;
            datePick.IsEnabled = false;
            listOrders.IsEnabled = false;
        }

        private void ChangeOrder_Click(object sender, RoutedEventArgs e) //user
        {
            if (click)
            {
                click = false;
                Task.Run(() => ClickSaver());
                isProblem = true;
                problem.Text = selectedOrder.Problem;
                description.Text = selectedOrder.Description;
                PublicResources.ordersCts.Cancel();
            }
            OrderDetails.IsEnabled = true;
            OrdersButtonPanel.IsEnabled = false;
            datePick.IsEnabled = false;
            listOrders.IsEnabled = false;
        }

        private void SaveOrder_Click(object sender, RoutedEventArgs e) //user
        {
                if (click)
                {
                    click = false;
                    Task.Run(() => ClickSaver());
                    if (isProblem)
                    {
                        UpdateOrderAsync().GetAwaiter();
                    }
                    else
                    {
                        CreateNewOrder().GetAwaiter();
                    }
                }
                OrderDetails.IsEnabled = false;
                OrdersButtonPanel.IsEnabled = true;
                datePick.IsEnabled = true;
                listOrders.IsEnabled = true;
        }

        private void AcceptCompleteOrder_Click(object sender, RoutedEventArgs e) //user
        {
            if (click)
            {
                click = false;
                Task.Run(() => ClickSaver());
                CompleteSelectedOrder().GetAwaiter();
            }
        }

        private void DeleteOrder_Click(object sender, RoutedEventArgs e) //user
        {
            if (click)
            {
                click = false;
                Task.Run(() => ClickSaver());
                DeleteSelectedOrder().GetAwaiter();
            }
        }

        private void BackToOrderList_Click(object sender, RoutedEventArgs e) //user
        {
            if (click)
            {
                click = false;
                Task.Run(() => ClickSaver());
                problem.Text = null;
                description.Text = null;
                UpdateOrdersListUser();
            }
            OrderDetails.IsEnabled = false;
            OrdersButtonPanel.IsEnabled = true;
            datePick.IsEnabled = true;
            listOrders.IsEnabled = true;
        }

        private void Problem_KeyUp(object sender, KeyEventArgs e) //user
        {
            if (problem.Text.Trim() != "")
            {
                SaveOrder.IsEnabled = true;
            }
            else
            {
                SaveOrder.IsEnabled = false;
            }
        }

        private void OrdersList_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) //cancel (selected)
        {
            if (click)
            {
                listOrders.SelectedIndex = -1;
                click = false;
                Task.Run(() => ClickSaver());
                if (PublicResources.Im.Admin) //admin
                {
                    AcceptOrder.Visibility = Visibility.Hidden;
                }
                else //user
                {
                    changeOrder.Visibility = Visibility.Hidden;
                    DeleteOrder.Visibility = Visibility.Hidden;
                    AcceptCompleteOrder.Visibility = Visibility.Hidden;
                }
            }
        }

        private void AcceptOrder_Click(object sender, RoutedEventArgs e) //admin
        {
            if (click)
            {
                click = false;
                Task.Run(() => ClickSaver());
                ProblemView.Text = selectedOrder.Problem;
                DescriptionView.Text = selectedOrder.Description;
                UserNameView.Text = selectedOrder.UserName;
                DateView.Text = selectedOrder.Date.ToString();
                LoginView.Text = selectedOrder.UserLogin;
                RoomView.Text = selectedOrder.Room.ToString();
                AdminDescription.Text = selectedOrder.AdminComment;
                PublicResources.ordersCts.Cancel();
                GetStatusesList().GetAwaiter();
            }
            listOrders.IsEnabled = false;
            OrderStatus.IsEnabled = true;
            OrdersButtonPanel.IsEnabled = false;
            OrderSortBox.IsEnabled = false;
        }

        private void SaveOrderAdmin_Click(object sender, RoutedEventArgs e) //admin
        {
            if (click)
            {
                click = false;
                Task.Run(() => ClickSaver());
                ChangeOrderStatusAdmin().GetAwaiter();
            }
            listOrders.IsEnabled = true;
            OrderStatus.IsEnabled = false;
            OrdersButtonPanel.IsEnabled = true;
            OrderSortBox.IsEnabled = true;
        }

        private void BackToOrderAdmin_Click(object sender, RoutedEventArgs e) //admin
        {
            if (click)
            {
                click = false;
                Task.Run(() => ClickSaver());
                UpdateOrdersListAdmin();
            }
            listOrders.IsEnabled = true;
            OrderStatus.IsEnabled = false;
            OrdersButtonPanel.IsEnabled = true;
            OrderSortBox.IsEnabled = true;
        }

        private void OrderSortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)  //Sort selected admin
        {
            PublicResources.sortCount = OrderSortBox.SelectedIndex;
            PublicResources.ordersCts.Cancel();
            listOrders.ItemsSource = null;
            selectedOrder = new Order();
            AcceptOrder.Visibility = Visibility.Hidden;
            UpdateOrdersListAdmin();
        }



        #region Update metods

        void UpdateOrdersListUser()
        {
            PublicResources.ordersCts.Cancel();
            var selectedDate = datePick.SelectedDate.Value;
            PublicResources.ordersCts = new CancellationTokenSource();
            Task.Run(() => AutoUpdateOrdersListUser(PublicResources.ordersCts.Token, selectedDate));
        }

        void UpdateOrdersListAdmin()
        {
            PublicResources.ordersCts.Cancel();
            var selectedDate = datePick.SelectedDate.Value;
            PublicResources.ordersCts = new CancellationTokenSource();
            Task.Run(() => AutoUpdateOrdersListAdmin(PublicResources.ordersCts.Token, selectedDate));
        }

        #endregion


        #region ASYNC metods

        //User Metods

        async Task UpdateOrderAsync()
        {           
            IUser user = new CurrentUser();
            var order = await user.GetOrderById(selectedOrder.Id);
            try
            {
                order.Problem = problem.Text;
                order.Description = description.Text;
            }
            catch
            {

            }
            var status = await user.EditOrder(order);
            problem.Text = null;
            description.Text = null;
            ordersTmp.Remove(selectedOrder);
            listOrders.ItemsSource = ordersTmp;
            UpdateOrdersListUser();
        }

        async Task CreateNewOrder()
        {
            IUser user = new CurrentUser();
            try
            {
                var status = await user.CreateOrder(new Order
                {
                    Date = DateTime.Now,
                    Description = description.Text,
                    UserId = PublicResources.Im.Id,
                    Problem = problem.Text,
                    StatusId = 1
                });
            }
            catch
            {
               
                
            }
            problem.Text = null;
            description.Text = null;
            datePick.SelectedDate = DateTime.Now;
            UpdateOrdersListUser();
            //MessageBox.Show(status.ToString());
        }

        async Task DeleteSelectedOrder()
        {
            IUser user = new CurrentUser();
            selectedOrder.StatusId = 5;                              //Отмена заявки пользователем
            var status = await user.EditOrder(selectedOrder);
            ordersTmp.Remove(selectedOrder);
            listOrders.ItemsSource = ordersTmp;
            UpdateOrdersListUser();
            //MessageBox.Show(status.ToString());
        }

        async Task CompleteSelectedOrder()
        {
            IUser user = new CurrentUser();
            selectedOrder.StatusId = 3;                              //Подтверждение выполнения заявки пользователем
            selectedOrder.CompletionDate = DateTime.Now;
            var status = await user.EditOrder(selectedOrder);
            ordersTmp.Remove(selectedOrder);
            listOrders.ItemsSource = ordersTmp;
            UpdateOrdersListUser();
            //MessageBox.Show(status.ToString());
        }


        //Admin Metods

        async Task GetStatusesList()
        {
            IAdmin admin = new CurrentUser();
            var statusesTmp = await admin.GetStatuses();
            StatusList.DisplayMemberPath = "Name";
            var statuses = statusesTmp.Where(x => x.Id != 1 && x.Id != 3 && x.Id != 5);
            StatusList.ItemsSource = statuses;
            StatusList.SelectedIndex = 0;
        }

        async Task ChangeOrderStatusAdmin()
        {
            IAdmin admin = new CurrentUser();
            var order = await admin.GetOrderById(selectedOrder.Id);
            if (selectedOrder.StatusId == order.StatusId)
            {
                order.StatusId = ((Status)StatusList.SelectedItem).Id;
                try
                {
                    order.AdminComment = AdminDescription.Text;
                    order.AdminId = PublicResources.Im.Id;
                }
                catch
                {

                }                              
                var status = await admin.EditOrder(order);
                ordersTmp.Remove(selectedOrder);
                listOrders.ItemsSource = ordersTmp;
            }
            UpdateOrdersListAdmin();
        }

        #endregion


        async void AutoUpdateOrdersListAdmin(CancellationToken cancellationToken, DateTime selectedDate)
        {
            IAdmin admin = new CurrentUser();
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var orders = await admin.GetAllOrdersToday(selectedDate);
                    ordersTmp = orders;
                    Action action = () =>
                    {
                        var ordersChange = orders;
                        List<Order> tmpList = new List<Order>();
                        var tmp1 = listOrders.Items.Cast<Order>();
                        tmpList.AddRange(tmp1);

                        if (tmpList.Count != orders.Count)
                        {
                            var sortOrd = OrdersSort(orders);
                            listOrders.ItemsSource = sortOrd;
                            if (selectedOrder != null)
                            {
                                var tmp = sortOrd.FirstOrDefault(a => a.Id == selectedOrder.Id);
                                listOrders.SelectedItem = tmp;
                                PublicResources.messengerChecker = true;
                            }
                        }
                        else
                        {
                            var except = ordersChange.Except(tmpList, new DBComparer()).ToList();
                            if (except.Count != 0)
                            {
                                var sortOrd = OrdersSort(orders);
                                listOrders.ItemsSource = sortOrd;
                                if (selectedOrder != null)
                                {
                                    var tmp = sortOrd.FirstOrDefault(a => a.Id == selectedOrder.Id);
                                    listOrders.SelectedItem = tmp;
                                    PublicResources.messengerChecker = true;
                                }
                            }
                        }
                    };
                    await listOrders.Dispatcher.BeginInvoke(action);
                    await Task.Delay(5000, cancellationToken);
                }
            }
            catch (OperationCanceledException) { }
        }

        async void AutoUpdateOrdersListUser(CancellationToken cancellationToken, DateTime selectedDate)
        {
            IUser user = new CurrentUser();
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var orders = await user.GetOrdersListOfDate(PublicResources.Im.Id, selectedDate);
                    ordersTmp = orders;
                    Action action = () =>
                    {
                        var ordersChange = orders;
                        List<Order> tmpList = new List<Order>();
                        var tmp1 = listOrders.Items.Cast<Order>();
                        tmpList.AddRange(tmp1);

                        if (tmpList.Count != orders.Count)
                        {
                            listOrders.ItemsSource = orders;
                            if (selectedOrder != null)
                            {
                                var tmp = orders.FirstOrDefault(a => a.Id == selectedOrder.Id);
                                listOrders.SelectedItem = tmp;
                                PublicResources.messengerChecker = true;
                            }
                        }
                        else
                        {
                            var except = ordersChange.Except(tmpList, new DBComparer()).ToList();
                            if (except.Count != 0)
                            {
                                listOrders.ItemsSource = orders;
                                if (selectedOrder != null)
                                {
                                    var tmp = orders.FirstOrDefault(a => a.Id == selectedOrder.Id);
                                    listOrders.SelectedItem = tmp;
                                    PublicResources.messengerChecker = true;
                                }
                            }
                        }
                    };
                    await listOrders.Dispatcher.BeginInvoke(action);
                    await Task.Delay(10000, cancellationToken);
                }
            }
            catch (OperationCanceledException) { }
        }

        async void ClickSaver()
        {
            await Task.Delay(200);
            click = true;
        }

        IEnumerable<Order> OrdersSort(List<Order> ord)
        {
            IEnumerable<Order> sortOrd;
            switch (PublicResources.sortCount)
            {
                case 0:
                    sortOrd = ord.Reverse<Order>();
                    break;
                case 1:
                    sortOrd = ord;
                    break;
                case 2:
                    sortOrd = ord.OrderBy(a => a.StatusId);
                    break;
                case 3:
                    sortOrd = ord.Where(a => a.StatusId == 1);
                    break;
                case 4:
                    sortOrd = ord.Where(a => a.StatusId == 2);
                    break;
                case 5:
                    sortOrd = ord.Where(a => a.StatusId == 3);
                    break;
                case 6:
                    sortOrd = ord.Where(a => a.StatusId == 4);
                    break;
                case 7:
                    sortOrd = ord.Where(a => a.StatusId == 5);
                    break;
                default:
                    sortOrd = ord.OrderBy(a => a.StatusId);
                    break;
            }
            return sortOrd;
        }

        
    }
    // вынести в отдельный класс!!!
    public class DBComparer : IEqualityComparer<Order>
    {
        public bool Equals(Order x, Order y)
        {
            if (ReferenceEquals(x, y)) return true;
            return x != null && y != null
                && x.Id.Equals(y.Id)
                && x.Problem.Equals(y.Problem)
                && x.Description.Equals(y.Description)
                && x.AdminId.Equals(y.AdminId)
                && x.StatusId.Equals(y.StatusId);
        }
        public int GetHashCode(Order obj)
        {
            return obj.AdminComment == null ? 0 : obj.AdminComment.GetHashCode();
        }
    }
}