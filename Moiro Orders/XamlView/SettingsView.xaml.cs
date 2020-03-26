using Moiro_Orders.Models;
using Moiro_Orders.Roles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Moiro_Orders.XamlView
{
    /// <summary>
    /// Логика взаимодействия для SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
            UserLogin.Text = PublicResources.Im.Login;
            UserName.Text = PublicResources.Im.FullName;
            UserRoom.Text = PublicResources.Im.Room.ToString();
            UserOrganizationalUnit.Text = PublicResources.Im.OrganizationalUnit;
            UserLastLogin.Text = PublicResources.Im.LastLogin.ToString();
        }

        private void UserName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (UserName.IsReadOnly)
            {
                UserName.IsReadOnly = false;
            }
            else
            {
                UserName.IsReadOnly = true;
            }
        }

        private void UserRoom_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (UserRoom.IsReadOnly)
            {
                UserRoom.IsReadOnly = false;
            }
            else
            {
                UserRoom.IsReadOnly = true;
            }
        }

        private void ApplySettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int room = Convert.ToInt32(UserRoom.Text);
                if (room > 500 && room <= 0)
                {
                    throw new Exception();
                }
            }
            catch
            {
                StatusChange.Content = "Пользователь не был изменён!\nОшибка в номере кабинета";
                StatusChange.Foreground = Brushes.Gray;
            }
            if (PublicResources.Im.FullName.Equals(UserName.Text) && PublicResources.Im.Room.ToString().Equals(UserRoom.Text))
            {
                StatusChange.Content = "Пользователь не был изменён!";
                StatusChange.Foreground = Brushes.Gray;
            }
            else
            {
                if (UserName.Text == "" && UserRoom.Text == "")
                {
                    StatusChange.Content = "Пользователь не был изменён!";
                    StatusChange.Foreground = Brushes.Gray;
                }
                else
                {
                    SetUser().GetAwaiter();
                }
            }
        }

        #region ASYNC metods
        async Task SetUser()
        {
            User currentUser = new User();
            IUser user = new CurrentUser();
            currentUser = PublicResources.Im;
            currentUser.Room = Convert.ToInt32(UserRoom.Text);
            currentUser.FullName = UserName.Text;

            var status = await user.UpdateUser(currentUser);
            if (status == System.Net.HttpStatusCode.NoContent)
            {
                
                StatusChange.Foreground = Brushes.Green;
                StatusChange.Content = "Данные успешно обновлены!";
                PublicResources.Im = currentUser;
                //await TimerLabel();
            }
            else
            {
                StatusChange.Foreground = Brushes.Red;
                StatusChange.Content = "HTTP status code " + status;
            }
        }
        //переписать на нормальный таймер
        async Task TimerLabel()
        {
            Thread.Sleep(3000);
            StatusChange.Content = "";
        }
        #endregion

    }
}
