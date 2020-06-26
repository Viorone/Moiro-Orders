using Moiro_Orders.Roles;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Moiro_Orders.Models;
using System;
using System.Net.Http;

namespace Moiro_Orders.XamlView
{
    /// <summary>
    /// Логика взаимодействия для User.xaml
    /// </summary>
    public partial class AdminView : UserControl
    {
        public AdminView()
        {
            GetAdmins().GetAwaiter();
            InitializeComponent();
            for (int i = 1; i < 6; ++i)
            {
                GetCountOrders(i).GetAwaiter();
            }
            DateTime dTime = DateTime.Now;
            DateTime dTime2 = DateTime.Now;
            dTime = dTime.AddMonths(-1);
            dateStart.SelectedDate = dTime;
            dateEnd.SelectedDate = DateTime.Now.Date;
            dTime2 = dTime.AddMonths(2);
            EventsDateStart.SelectedDate = DateTime.Now.Date;
            EventsDateEnd.SelectedDate = dTime2;
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
        private void CompletePastOrders_Click(object sender, RoutedEventArgs e)
        {
            //событие на подтверждение всех неподверждённых заявок за прошлый месяц и более
        }

        #region Для вкладки "Мероприятия"
        private void EventsDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            GetEventsByAdmin();
        }
      
        private void EventLog_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EventLog.SelectedIndex == 0 && ListViewEvent.Items.Count == 0)
            {
                GetEventsByAdmin();
            }
        }
        #endregion

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

        async void GetEventsAll()
        {
            List<Order> events = new List<Order>();
            DateTime tmpDateEnd = DateTime.Now;
            DateTime tmpDateStart = DateTime.Now;

            Action action = () =>
            {
                tmpDateStart = dateStart.SelectedDate.Value;
                tmpDateEnd = dateEnd.SelectedDate.Value;
            };
            Action action1 = () =>
            {
                ListGettingOrders.ItemsSource = events;
            };
            await dateStart.Dispatcher.BeginInvoke(action);

            IAdmin admin = new CurrentUser();
            await ListGettingOrders.Dispatcher.BeginInvoke(action1);
        }

        async void GetEventsByAdmin()
        {
            List<Event> events = new List<Event>();
            DateTime tmpDateEnd = DateTime.Now;
            DateTime tmpDateStart = DateTime.Now;

            Action action = () =>
            {
                tmpDateStart = EventsDateStart.SelectedDate.Value;
                tmpDateEnd = EventsDateEnd.SelectedDate.Value;
            };
            Action action1 = () =>
            {
                events = events.OrderBy(a => a.DateStart).ToList();
                ListViewEvent.ItemsSource = events;
            };
            await dateStart.Dispatcher.BeginInvoke(action);

            IAdmin admin = new CurrentUser();
            events = await admin.GetEventsForStatistic(tmpDateStart, tmpDateEnd);
            await ListGettingOrders.Dispatcher.BeginInvoke(action1);
        }

        async Task GetAdmins()
        {
            IEnumerable<User> users = null;
            IAdmin admin = new CurrentUser();
            users = await admin.GetAdminsList();
            adminSelect.DisplayMemberPath = "FullName";
            adminSelect.ItemsSource = users;
        }
        async Task SetAdminToEvent()
        {
            Event tmpEvent = (Event)ListViewEvent.SelectedItem;
            tmpEvent.AdminId = ((User)adminSelect.SelectedItem).Id;
            IAdmin admin = new CurrentUser();
            var response = await admin.EditEvent(tmpEvent);
            addingPanel.Visibility = Visibility.Collapsed;
            ListViewEvent.SelectedItem = -1;
            adminSelect.SelectedIndex = -1;
            if (response == System.Net.HttpStatusCode.NoContent)
            {
                GetEventsByAdmin();
            }
        }

        #endregion

        private void ListViewEvent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewEvent.SelectedIndex == -1) { return; }
            addingPanel.Visibility = Visibility.Visible;
        }

        private void AdminSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (adminSelect.SelectedIndex == -1)
            {
                return;
            }
            SetAdminToEvent().GetAwaiter();

        }
    }
}
