
using Moiro_Orders.Models;
using Moiro_Orders.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
using Moiro_Orders.Roles;
using Moiro_Orders.ViewModel;
using System.Net;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using Moiro_Orders.XamlView;

namespace Moiro_Orders
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            if (PublicResources.Im.FullName == null)
            {
                GetUser().GetAwaiter();
            }
            else
            {
                InitializeComponent();
            }
        }


        private void OpenMenuButton_Click(object sender, RoutedEventArgs e)
        {
            OpenMenuButton.Visibility = Visibility.Collapsed;
            CloseMenuButton.Visibility = Visibility.Visible;
        }

        private void CloseMenuButton_Click(object sender, RoutedEventArgs e)
        {
            OpenMenuButton.Visibility = Visibility.Visible;
            CloseMenuButton.Visibility = Visibility.Collapsed;
        }

        private void Orders_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SwitchScreen(new OrderView());
        }

        private void Users_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (PublicResources.Im.Admin)
            {
                SwitchScreen(new UserView());
            }
            else
            {
                MessageBox.Show("У Вас совсем нет прав! Грустно, но такова жизнь...");
            }

        }

        private void Events_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SwitchScreen(new EventView());
        }
        private void UsersSettings_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SwitchScreen(new SettingsView());
        }

        internal void SwitchScreen(object sender)
        {
            var clicl = (UserControl)sender;
            if (clicl != null)
            {
                mainView.Children.Clear();
                mainView.Children.Add(clicl);
            }
        }
        #region ASYNC metods
        async Task GetUser()
        {
            UsersController currentUser = new UsersController();
            //await currentUser.GetUserAsync(Environment.UserName);
            await currentUser.GetUserAsync("gybarev");
            Title = PublicResources.Im.FullName + " | " + PublicResources.Im.OrganizationalUnit;
            Users.Visibility = Visibility.Visible;
            loadingGrid.Visibility = Visibility.Hidden;
            InitializeComponent();
        }
        #endregion

    }
















    public static class PublicResources
    {

        public static HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:55544/")        //"http://10.10.0.34//"
        };

        internal static User Im = new User();

        static PublicResources()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
    
}

