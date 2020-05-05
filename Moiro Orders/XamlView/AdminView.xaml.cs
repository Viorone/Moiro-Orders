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
        }
        
        private void Button_Click(object sender, RoutedEventArgs e) //Гигантская кнопка
        {
            async Task UpdateUsersDb()
            {
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
                    logList.Items.Add(response.ToString());
                }
            }
            UpdateUsersDb().GetAwaiter();
        }

        #region Для вкладки "Статистика"

        #region Обработчики событий на карточки
        bool refresh = false; //нужно для измежания нажатия на карточку в которую входит обновление цифр
        private void CardOrdersNew_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!refresh)
            {
                GetOrdersByStatus(1, dateStart.SelectedDate.Value, dateEnd.SelectedDate.Value).GetAwaiter();
            }
            else
            {
                refresh = false;
            }
        }
        private void CardOrdersCancel_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!refresh)
            {
                GetOrdersByStatus(2, dateStart.SelectedDate.Value, dateEnd.SelectedDate.Value).GetAwaiter();
            }
            else
            {
                refresh = false;
            }
        }
        private void CardOrdersComplete_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!refresh)
            {
                //main code
                MessageBox.Show("CardComplete");
            }
            else
            {
                refresh = false;
            }
        }
        private void CardOrdersNeedRepair_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!refresh)
            {
                //main code
                MessageBox.Show("CardNeedRepair");
            }
            else
            {
                refresh = false;
            }
        }
        private void CardOrdersInProgress_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!refresh)
            {
                //main code
                MessageBox.Show("CardШтЗкщпкуыы");
            }
            else
            {
                refresh = false;
            }
        }
        #endregion

        #region Обработчики событий на обновление карточки
        private void RefreshOrdersNew_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            refresh = true;
            GetCountOrders(1).GetAwaiter();
        }
        private void RefreshOrdersinProgress_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            refresh = true;
            GetCountOrders(2).GetAwaiter();
        }
        private void RefreshOrdersComplete_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            refresh = true;
            GetCountOrders(3).GetAwaiter();
        }
        private void RefreshOrdersNeedRepair_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            refresh = true;
            GetCountOrders(4).GetAwaiter();
        }
        private void RefreshOrdersCancel_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            refresh = true;
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

        async Task GetOrdersByStatus(int statusId, DateTime dateStart, DateTime dateEnd)
        {
            IAdmin admin = new CurrentUser();
            var orders = await admin.GetOrdersByStatus(statusId, dateStart, dateEnd);
            ListGettingOrders.ItemsSource = orders;
        }

        #endregion

       
    }
}
