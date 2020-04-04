using Moiro_Orders.Models;
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

        public OrderView()
        {
            InitializeComponent();

            datePick.SelectedDate = DateTime.Now.Date;
            if (PublicResources.Im.Admin) //admin
            {
                addOrder.Visibility = Visibility.Hidden;
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

                if (selectedOrder.StatusId != 3 && selectedOrder.StatusId != 6)
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
                isProblem = true;
                selectedOrder = (Order)e.AddedItems[0];
                var nowTime = DateTime.Now;
                var changeTime = nowTime - selectedOrder.Date;
                DateTime time = Convert.ToDateTime("00:30:00");
                DeleteOrder.Visibility = Visibility.Hidden;
                changeOrder.Visibility = Visibility.Hidden;
                AcceptCompleteOrder.Visibility = Visibility.Hidden;
                if (changeTime < time.TimeOfDay && selectedOrder.StatusId != 3)
                {
                    if (selectedOrder.StatusId != 6)
                    {
                        DeleteOrder.Visibility = Visibility.Visible;
                    }
                    changeOrder.Visibility = Visibility.Visible;
                }
                if (selectedOrder.StatusId == 2)
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
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            listOrders.Visibility = Visibility.Visible;
            if (click)
            {              
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
                cts.Cancel();
                GetStatusesList().GetAwaiter();
            }
            Cancel.Visibility = Visibility.Hidden;
            listOrders.Visibility = Visibility.Hidden;
            AcceptOrder.Visibility = Visibility.Hidden;
            datePick.Visibility = Visibility.Hidden;
            DateText.Visibility = Visibility.Hidden;
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
        }

        private void BackToOrderAdmin_Click(object sender, RoutedEventArgs e) //admin
        {         
            if (click)
            {
                click = false;
                Task.Run(() => ClickSaver());


                UpdateOrdersListAdmin();                
            }
            Cancel.Visibility = Visibility.Visible;
            AcceptOrder.Visibility = Visibility.Visible;
            listOrders.Visibility = Visibility.Visible;
            datePick.Visibility = Visibility.Visible;
            DateText.Visibility = Visibility.Visible;
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
                GetOrdersOfDateUser(selectedOrder.Date).GetAwaiter();
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
                GetOrdersOfDateAdmin(selectedOrder.Date).GetAwaiter();
            }
        }

        #endregion


        #region ASYNC metods

        //User Metods
        async Task GetOrdersOfDateUser(DateTime selectDate)
        {
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
                StatusId = 1,
                CompletionDate = Convert.ToDateTime("1900 - 01 - 01 00:00:00.000")
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
            selectedOrder.StatusId = 6;                              //Отмена заявки пользователем
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
            listOrders.ItemsSource = orders;
        }

        async Task GetStatusesList()
        {
            IAdmin admin = new CurrentUser();
            var statusesTmp = await admin.GetStatuses();
            StatusList.DisplayMemberPath = "Name";
            var statuses = statusesTmp.Where(x => x.Id != 1 && x.Id != 3 && x.Id != 6);
            StatusList.ItemsSource = statuses;
            StatusList.SelectedItem = StatusList.Items[0];
        }

        async Task ChangeOrderStatusAdmin()
        {
            IAdmin admin = new CurrentUser();
            selectedOrder.StatusId = ((Status)StatusList.SelectedItem).Id;
            selectedOrder.AdminComment = AdminDescription.Text;
            var status = await admin.EditOrder(selectedOrder);
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
                    Action action = () => listOrders.ItemsSource = orders;
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
                    Action action = () => listOrders.ItemsSource = orders;
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
    }
}