using Moiro_Orders.Models;
using Moiro_Orders.Roles;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


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

        private void ApplySettings_Click(object sender, RoutedEventArgs e)
        {
            StatusChange.Opacity = 1;
            StatusChange.Content = "";
            try
            {
                int room = Convert.ToInt32(UserRoom.Text);
                if (room > 500 || room <= 0)
                {
                    throw new Exception();
                }
                if (UserName.Text.Length > 100)
                {
                    throw new Exception();
                }
            }
            catch
            {
                StatusChange.Content = "Пользователь не был изменён!\nОшибка в номере кабинета";
                StatusChange.Foreground = Brushes.Red;
                return;
            }
            if (PublicResources.Im.FullName.Equals(UserName.Text) && PublicResources.Im.Room.ToString().Equals(UserRoom.Text))
            {
                StatusChange.Content = "Пользователь не был изменён!";
                StatusChange.Foreground = Brushes.Red;
            }
            else
            {
                if (UserName.Text == "" || UserRoom.Text == "")
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

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
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
            }
            else
            {
                StatusChange.Foreground = Brushes.Red;
                StatusChange.Content = "HTTP status code " + status;
            }
        }


        #endregion

    }
}
