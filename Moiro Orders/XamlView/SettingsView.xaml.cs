using Moiro_Orders.Models;
using Moiro_Orders.Roles;
using System;
using System.Collections.Generic;
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
            UserOrganizationalUnit.Text.Trim(' ');
            try
            {
                int room = Convert.ToInt32(UserRoom.Text);
                if (room > 500 || room <= 0)
                {
                    throw new Exception("Ошибка в номере кабинета");
                }
                if (UserName.Text.Length > 100 || UserName.Text == "")
                {
                    throw new Exception("Ошибка в ФИО пользователя");
                }
                if (UserOrganizationalUnit.Text.Length > 150 || UserOrganizationalUnit.Text.Length < 5 || UserOrganizationalUnit.Text =="")
                {
                    throw new Exception("Ошибка в поле подразделения");
                }
            }
            catch(Exception ex)
            {
                StatusChange.Content = "Пользователь не был изменён!\n"+ex.Message;
                StatusChange.Foreground = Brushes.Red;
                return;
            }
            if (PublicResources.Im.FullName.Equals(UserName.Text) && PublicResources.Im.Room.ToString().Equals(UserRoom.Text) && PublicResources.Im.OrganizationalUnit.Equals(UserOrganizationalUnit.Text))
            {
                StatusChange.Content = "Пользователь не был изменён!";
                StatusChange.Foreground = Brushes.Red;
            }
            else
            {
                SetUser().GetAwaiter();
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
            currentUser.OrganizationalUnit = UserOrganizationalUnit.Text;

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
