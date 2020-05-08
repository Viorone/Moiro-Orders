using Moiro_Orders.Roles;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Moiro_Orders.Models;
using System;

namespace Moiro_Orders.XamlView
{
    /// <summary>
    /// Логика взаимодействия для User.xaml
    /// </summary>
    public partial class AdminView : UserControl
    {
        public AdminView()
        {
            InitializeComponent();
            for (int i = 1; i < 6; ++i)
            {
                GetCountOrders(i).GetAwaiter();
            }
            DateTime dTime = DateTime.Now;
            dTime = dTime.AddMonths(-1);
            dateStart.SelectedDate = dTime;
            dateEnd.SelectedDate = DateTime.Now.Date;
        }

        private void Button_Click(object sender, RoutedEventArgs e) //Гигантская кнопка
        {
            async Task UpdateUsersDb()
            {
                logList.Items.Clear();
                IAdmin admin = new CurrentUser();
                var tmp1 = admin.GetNewADUsersList();
                var tmp2 = await admin.GetAllUserName();
                var result = tmp1.Join(tmp2, ok => ok.Login, ik => ik.Login, (one, two) => new { one, two }).ToList();
                tmp1.RemoveAll(x => result.Any(r => x == r.one));
                tmp2.RemoveAll(x => result.Any(r => x == r.two));

                List<string> responses = new List<string>();

                foreach (var var1 in tmp1)
                {
                    var response = await admin.UpdateUsersDb(var1);
                    responses.Add(response.ToString());
                    logList.Items.Add(var1.FullName +" - "+ response.ToString());
                }
            }
            UpdateUsersDb().GetAwaiter();
        }

        #region Для вкладки "Статистика"

        #region Обработчики событий на карточки

        private void CardOrdersNew_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Task.Run(() => GetOrdersByStatus(1));
        }
        private void CardOrdersInProgress_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Task.Run(() => GetOrdersByStatus(2));
        }
        private void CardOrdersComplete_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Task.Run(() => GetOrdersByStatus(3));
        }
        private void CardOrdersNeedRepair_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Task.Run(() => GetOrdersByStatus(4));
        }
        private void CardOrdersCancel_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Task.Run(() => GetOrdersByStatus(5));
        }

        #endregion

        #region Обработчики событий на обновление карточки
        private void RefreshOrdersNew_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GetCountOrders(1).GetAwaiter();
        }
        private void RefreshOrdersinProgress_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GetCountOrders(2).GetAwaiter();
        }
        private void RefreshOrdersComplete_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GetCountOrders(3).GetAwaiter();
        }
        private void RefreshOrdersNeedRepair_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GetCountOrders(4).GetAwaiter();
        }
        private void RefreshOrdersCancel_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        { 
            GetCountOrders(5).GetAwaiter();
        }
        #endregion


        #endregion

        #region ASYNC metods
        async Task<int> GetCountOrders(int statusId)
        {
            IAdmin admin = new CurrentUser();
            var count = await admin.GetCountOrdersByStatus(statusId);
            switch (statusId)
            {
                case 1:
                    CountOrdersNew.Text = count.ToString();
                    break;
                case 2:
                    CountOrdersInProgress.Text = count.ToString();
                    break;
                case 3:
                    CountOrdersComplete.Text = count.ToString();
                    break;
                case 4:
                    CountOrdersNeedRepair.Text = count.ToString();
                    break;
                case 5:
                    CountOrdersCancel.Text = count.ToString();
                    break;

            }
            return count;
        }

        async void GetOrdersByStatus(int statusId)
        {
            List<Order> orders = new List<Order>();
            DateTime tmpDateEnd = DateTime.Now;
            DateTime tmpDateStart = DateTime.Now;

            Action action = () =>
            {
                tmpDateStart = dateStart.SelectedDate.Value;
                tmpDateEnd = dateEnd.SelectedDate.Value;
            };
            Action action1 = () =>
            {
                ListGettingOrders.ItemsSource = orders;
            };
            await dateStart.Dispatcher.BeginInvoke(action);

            IAdmin admin = new CurrentUser();
            orders = await admin.GetOrdersByStatus(statusId, tmpDateStart, tmpDateEnd);
            await ListGettingOrders.Dispatcher.BeginInvoke(action1);
        }
        #endregion
    }
}
