﻿using Moiro_Orders.Models;
using Moiro_Orders.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        private bool isProblem = true;
        public Order selectedOrder;
        bool click = true;
        CancellationTokenSource cts = new CancellationTokenSource();
        int sortCount = -1;

        public OrderView()
        {
            InitializeComponent();
            datePick.SelectedDate = DateTime.Now.Date;
            if (PublicResources.Im.Admin) //admin
            {
                addOrder.Visibility = Visibility.Hidden;
                List<string> sortList = new List<string>();
                sortList.Add("Сначала новые");
                sortList.Add("Сначала старые");
                sortList.Add("По статусу");
                sortList.Add("В очереди на выполнение");
                sortList.Add("Выполняются");
                sortList.Add("Выполнены");
                sortList.Add("Требуется ремонт/закупка");
                sortList.Add("Отменено");
                OrderSortBox.Visibility = Visibility.Visible;
                OrderSortBox.ItemsSource = sortList;
                OrderSortBox.Text = OrderSortBox.Items[2].ToString();
            }
        }

        private void DatePicker_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (datePick.SelectedDate != null && click == true)
            {
                var selectDate = datePick.SelectedDate.Value.Date;
                if (PublicResources.Im.Admin) //admin
                {
                    if (datePick.SelectedDate == DateTime.Now.Date)
                    {
                        cts.Cancel();
                        click = false;
                        Task.Run(() => ClickSaver());
                        cts = new CancellationTokenSource();
                        Task.Run(() => AutoUpdateOrdersListAdmin(cts.Token));
                    }
                    else
                    {
                        cts.Cancel();
                        GetOrdersOfDateAdmin(selectDate).GetAwaiter();
                    }
                }
                else //user
                {
                    if (datePick.SelectedDate == DateTime.Now.Date)
                    {
                        cts.Cancel();
                        click = false;
                        Task.Run(() => ClickSaver());
                        cts = new CancellationTokenSource();
                        Task.Run(() => AutoUpdateOrdersListUser(cts.Token));
                    }
                    else
                    {
                        cts.Cancel();
                        GetOrdersOfDateUser(selectDate).GetAwaiter();
                    }
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
                cts.Cancel();
                selectedOrder = (Order)e.AddedItems[0];
                AcceptOrder.Visibility = Visibility.Hidden;
                Cancel.Visibility = Visibility.Visible;
                datePick.Visibility = Visibility.Hidden;
                DateText.Visibility = Visibility.Hidden;
                OrderSortBox.Visibility = Visibility.Hidden;

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
                cts.Cancel();
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
                //если разница в пол часа, изменять - нельзя 
                if (changeTime < time.TimeOfDay && selectedOrder.StatusId != 3)
                {
                    if (selectedOrder.StatusId != 5) // если заявка не отменена
                    {
                        DeleteOrder.Visibility = Visibility.Visible;
                        changeOrder.Visibility = Visibility.Visible;
                    }
                }
                if (selectedOrder.StatusId == 2)  //если выполняется заявка, то пользователь может подтверить выполненние 
                {
                    AcceptCompleteOrder.Visibility = Visibility.Visible;
                }
                addOrder.Visibility = Visibility.Hidden;
                Cancel.Visibility = Visibility.Visible;
                datePick.Visibility = Visibility.Hidden;
                DateText.Visibility = Visibility.Hidden;
            }
        }

        private void AddOrder_Click(object sender, RoutedEventArgs e) //user
        {
            if (click)
            {
                click = false;
                Task.Run(() => ClickSaver());
                isProblem = false;
                cts.Cancel();
            }
            addOrder.Visibility = Visibility.Hidden;
            datePick.Visibility = Visibility.Hidden;
            DateText.Visibility = Visibility.Hidden;
            listOrders.Visibility = Visibility.Hidden;
            backToOrderList.Visibility = Visibility.Visible;
            SaveOrder.Visibility = Visibility.Visible;
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
                cts.Cancel();
            }
            listOrders.Visibility = Visibility.Hidden;
            changeOrder.Visibility = Visibility.Hidden;
            DeleteOrder.Visibility = Visibility.Hidden;
            addOrder.Visibility = Visibility.Hidden;
            Cancel.Visibility = Visibility.Hidden;
            datePick.Visibility = Visibility.Hidden;
            DateText.Visibility = Visibility.Hidden;
            AcceptCompleteOrder.Visibility = Visibility.Hidden;
            backToOrderList.Visibility = Visibility.Visible;
            SaveOrder.Visibility = Visibility.Visible;
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
            addOrder.Visibility = Visibility.Visible;
            listOrders.Visibility = Visibility.Visible;
            datePick.Visibility = Visibility.Visible;
            DateText.Visibility = Visibility.Visible;
            backToOrderList.Visibility = Visibility.Hidden;
            SaveOrder.Visibility = Visibility.Hidden;
            AcceptCompleteOrder.Visibility = Visibility.Hidden;
        }

        private void AcceptCompleteOrder_Click(object sender, RoutedEventArgs e) //user
        {
            if (click)
            {
                click = false;
                Task.Run(() => ClickSaver());
                CompleteSelectedOrder().GetAwaiter();
            }
            changeOrder.Visibility = Visibility.Hidden;
            DeleteOrder.Visibility = Visibility.Hidden;
            addOrder.Visibility = Visibility.Visible;
            Cancel.Visibility = Visibility.Hidden;
            AcceptCompleteOrder.Visibility = Visibility.Hidden;
            datePick.Visibility = Visibility.Visible;
            DateText.Visibility = Visibility.Visible;
        }

        private void DeleteOrder_Click(object sender, RoutedEventArgs e) //user
        {
            if (click)
            {
                click = false;
                Task.Run(() => ClickSaver());
                DeleteSelectedOrder().GetAwaiter();
            }
            changeOrder.Visibility = Visibility.Hidden;
            DeleteOrder.Visibility = Visibility.Hidden;
            addOrder.Visibility = Visibility.Visible;
            Cancel.Visibility = Visibility.Hidden;
            AcceptCompleteOrder.Visibility = Visibility.Hidden;
            datePick.Visibility = Visibility.Visible;
            DateText.Visibility = Visibility.Visible;
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
            listOrders.Visibility = Visibility.Visible;
            addOrder.Visibility = Visibility.Visible;
            Cancel.Visibility = Visibility.Hidden;
            datePick.Visibility = Visibility.Visible;
            DateText.Visibility = Visibility.Visible;
            backToOrderList.Visibility = Visibility.Hidden;
            SaveOrder.Visibility = Visibility.Hidden;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            listOrders.Visibility = Visibility.Visible;
            if (click)
            {
                listOrders.SelectedIndex = -1;
                click = false;
                Task.Run(() => ClickSaver());
                if (PublicResources.Im.Admin) //admin
                {
                    AcceptOrder.Visibility = Visibility.Hidden;
                    UpdateOrdersListAdmin();
                }
                else //user
                {
                    addOrder.Visibility = Visibility.Visible;
                    changeOrder.Visibility = Visibility.Hidden;
                    DeleteOrder.Visibility = Visibility.Hidden;
                    AcceptCompleteOrder.Visibility = Visibility.Hidden;
                    UpdateOrdersListUser();
                }
            }
            Cancel.Visibility = Visibility.Hidden;
            datePick.Visibility = Visibility.Visible;
            DateText.Visibility = Visibility.Visible;
            OrderSortBox.Visibility = Visibility.Visible;
        }

        private void AcceptOrder_Click(object sender, RoutedEventArgs e) //admin
        {
            if (click)
            {
                click = false;
                Task.Run(() => ClickSaver());
                ProblemView.Text = selectedOrder.Problem;
                DescriptionView.Text = selectedOrder.Description;
                UserView.Text = selectedOrder.UserName;
                DateView.Text = selectedOrder.Date.ToString();
                LoginView.Text = selectedOrder.UserLogin;
                RoomView.Text = selectedOrder.Room.ToString();
                AdminDescription.Text = selectedOrder.AdminComment;
                cts.Cancel();
                GetStatusesList().GetAwaiter();
            }
            Cancel.Visibility = Visibility.Hidden;
            listOrders.IsEnabled = false;
            AcceptOrder.Visibility = Visibility.Hidden;
            datePick.Visibility = Visibility.Hidden;
            DateText.Visibility = Visibility.Hidden;
            BackToOrderAdmin.Visibility = Visibility.Visible;
            SaveOrderAdmin.Visibility = Visibility.Visible;
        }

        private void SaveOrderAdmin_Click(object sender, RoutedEventArgs e) //admin
        {
            if (click)
            {
                click = false;
                Task.Run(() => ClickSaver());
                ChangeOrderStatusAdmin().GetAwaiter();
            }
            listOrders.Visibility = Visibility.Visible;
            datePick.Visibility = Visibility.Visible;
            DateText.Visibility = Visibility.Visible;
            BackToOrderAdmin.Visibility = Visibility.Hidden;
            SaveOrderAdmin.Visibility = Visibility.Hidden;
        }

        private void BackToOrderAdmin_Click(object sender, RoutedEventArgs e) //admin
        {
            if (click)
            {
                listOrders.SelectedIndex = -1;
                click = false;
                Task.Run(() => ClickSaver());
               // UpdateOrdersListAdmin();
            }
            Cancel.Visibility = Visibility.Hidden;
            AcceptOrder.Visibility = Visibility.Hidden;
            listOrders.IsEnabled = true;
            datePick.Visibility = Visibility.Visible;
            DateText.Visibility = Visibility.Visible;
            BackToOrderAdmin.Visibility = Visibility.Hidden;
            SaveOrderAdmin.Visibility = Visibility.Hidden;
        }

        private void OrderSortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)  //Sort selected
        {
            sortCount = OrderSortBox.SelectedIndex;
            cts.Cancel();
            listOrders.ItemsSource = null;
            UpdateOrdersListAdmin();
        }


        #region Update metods

        void UpdateOrdersListUser()
        {
            if (datePick.SelectedDate == DateTime.Now.Date)
            {
                cts = new CancellationTokenSource();
                Task.Run(() => AutoUpdateOrdersListUser(cts.Token));
            }
            else
            {
                //datePick.SelectedDate = DateTime.Now.Date;
                Task.Run(() => GetOrdersOfDateUser(selectedOrder.Date));
            }
        }

        void UpdateOrdersListAdmin()
        {
            if (datePick.SelectedDate == DateTime.Now.Date)
            {
                cts = new CancellationTokenSource();
                Task.Run(() => AutoUpdateOrdersListAdmin(cts.Token));
            }
            else
            {
                Task.Run(() => GetOrdersOfDateAdmin(selectedOrder.Date));
            }
        }

        #endregion


        #region ASYNC metods

        //User Metods
        async Task GetOrdersOfDateUser(DateTime selectDate)
        {
            IUser user = new CurrentUser();
            var orders = await user.GetOrdersListOfDate(PublicResources.Im.Id, selectDate);
            if (orders != null)
            {
                await listOrders.Dispatcher.BeginInvoke(new Action(() => listOrders.ItemsSource = orders));
            }
        }

        async Task UpdateOrderAsync()
        {
            selectedOrder.Problem = problem.Text;
            selectedOrder.Description = description.Text;

            IUser user = new CurrentUser();
            var status = await user.EditOrder(selectedOrder);
            problem.Text = null;
            description.Text = null;
            datePick.SelectedDate = selectedOrder.Date;
            UpdateOrdersListUser();
            //MessageBox.Show(status.ToString());
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
                StatusId = 1
            });
            problem.Text = null;
            description.Text = null;
            datePick.SelectedDate = DateTime.Now;
            cts = new CancellationTokenSource();
            await Task.Run(() => AutoUpdateOrdersListUser(cts.Token));
            //MessageBox.Show(status.ToString());
        }

        async Task DeleteSelectedOrder()
        {
            IUser user = new CurrentUser();
            selectedOrder.StatusId = 5;                              //Отмена заявки пользователем
            var status = await user.EditOrder(selectedOrder);
            datePick.SelectedDate = selectedOrder.Date;
            UpdateOrdersListUser();
            //MessageBox.Show(status.ToString());
        }

        async Task CompleteSelectedOrder()
        {
            IUser user = new CurrentUser();
            selectedOrder.StatusId = 3;                              //Подтверждение выполнения заявки пользователем
            selectedOrder.CompletionDate = DateTime.Now;
            var status = await user.EditOrder(selectedOrder);
            datePick.SelectedDate = selectedOrder.Date;
            UpdateOrdersListUser();
            //MessageBox.Show(status.ToString());
        }


        //Admin Metods
        async Task GetOrdersOfDateAdmin(DateTime selectDate)
        {
            IAdmin admin = new CurrentUser();
            var orders = await admin.GetAllOrdersToday(selectDate);
            if (orders != null)
            {
                var sortOrd = OrdersSort(orders);
                listOrders.ItemsSource = sortOrd;
                await listOrders.Dispatcher.BeginInvoke( new Action(()=> listOrders.ItemsSource = sortOrd));
            }
        }

        async Task GetStatusesList()
        {
            IAdmin admin = new CurrentUser();
            var statusesTmp = await admin.GetStatuses();
            StatusList.DisplayMemberPath = "Name";
            var statuses = statusesTmp.Where(x => x.Id != 1 && x.Id != 3 && x.Id != 5);
            StatusList.ItemsSource = statuses;
            StatusList.SelectedItem = StatusList.Items[0];
        }

        async Task ChangeOrderStatusAdmin()
        {
            IAdmin admin = new CurrentUser();
            var order = await admin.GetOrderById(selectedOrder.Id);
            if (selectedOrder.StatusId == order.StatusId)
            {
                selectedOrder.StatusId = ((Status)StatusList.SelectedItem).Id;
                selectedOrder.AdminComment = AdminDescription.Text;
                selectedOrder.AdminId = PublicResources.Im.Id;
                var status = await admin.EditOrder(selectedOrder);
                listOrders.ItemsSource = null;
            }
            UpdateOrdersListAdmin();
        }

        #endregion

        async void AutoUpdateOrdersListAdmin(CancellationToken cancellationToken)
        {
            IAdmin admin = new CurrentUser();
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var orders = await admin.GetAllOrdersToday(DateTime.Now);

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
                        }
                        else
                        {
                            var except = ordersChange.Except(tmpList, new DBComparer()).ToList();
                            if (except.Count != 0)
                            {
                                var sortOrd = OrdersSort(orders);
                                listOrders.ItemsSource = sortOrd;
                            }
                        }
                    };
                    await listOrders.Dispatcher.BeginInvoke(action);
                    //MessageBox.Show("REWQ");
                    await Task.Delay(5000, cancellationToken);
                }
            }
            catch (OperationCanceledException) { }
        }

        async void AutoUpdateOrdersListUser(CancellationToken cancellationToken)
        {
            IUser user = new CurrentUser();
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var orders = await user.GetOrdersListOfDate(PublicResources.Im.Id, DateTime.Now.Date);
                    Action action = () =>
                    {
                        var ordersChange = orders;
                        List<Order> tmpList = new List<Order>();
                        var tmp1 = listOrders.Items.Cast<Order>();
                        tmpList.AddRange(tmp1);

                        if (tmpList.Count != orders.Count)
                        {
                            listOrders.ItemsSource = orders;
                        }
                        else
                        {
                            var except = ordersChange.Except(tmpList, new DBComparer()).ToList();
                            if (except.Count != 0)
                            {                              
                                listOrders.ItemsSource = orders;
                            }
                        }
                    };
                    await listOrders.Dispatcher.BeginInvoke(action);
                    //MessageBox.Show("QWER");
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
            switch (sortCount)
            {
                case 0:
                    sortOrd = ord; 
                    break;
                case 1:
                    sortOrd = ord.Reverse<Order>();
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
                default: sortOrd = ord.OrderBy(a => a.StatusId);
                    break;
            }
            return sortOrd;
        }
    }

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