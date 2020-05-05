﻿using Moiro_Orders.Roles;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


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
            IAdmin admin = new CurrentUser();
            admin.GetCountOrdersByStatus(1).GetAwaiter();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
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
        bool refresh = false;
        private void CardOrdersNew_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!refresh)
            {
                //main code
                MessageBox.Show("Card");
            }
            else
            {
                refresh = false;
            }
        }

        private void RefreshOrdersNew_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            refresh = true;
            //code to refresh
            MessageBox.Show("refresh");

        }
    }
}
