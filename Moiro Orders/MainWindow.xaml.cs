
using Moiro_Orders.Models;
using Moiro_Orders.Controller;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Moiro_Orders.XamlView;
using System.Threading;

namespace Moiro_Orders
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool click = true;
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
            if (click)
            {
                click = false;
                Task.Run(() => MainClickSaver());
                SwitchScreen(new OrderView());
            }
        }

        private void Users_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (PublicResources.Im.Admin)
            {
                if (click)
                {
                    click = false;
                    Task.Run(() => MainClickSaver());
                    PublicResources.ordersCts.Cancel();
                    SwitchScreen(new UserView());
                }
            }
        }

        private void Events_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (click)
            {             
                click = false;
                Task.Run(() => MainClickSaver());
                PublicResources.ordersCts.Cancel();
                SwitchScreen(new EventView());
            }
        }
        private void UsersSettings_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (click)
            {
                click = false;
                Task.Run(() => MainClickSaver());
                PublicResources.ordersCts.Cancel();
                SwitchScreen(new SettingsView());
            }
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
            HeaderText.Text = PublicResources.Im.FullName + " | " + PublicResources.Im.OrganizationalUnit;
            Users.Visibility = Visibility.Visible;
            loadingGrid.Visibility = Visibility.Hidden;
            if (PublicResources.Im.Admin == false)
            {
                Users.Visibility = Visibility.Hidden;
            }
            InitializeComponent();
        }

        async void MainClickSaver()
        {
            await Task.Delay(500);
            click = true;
        }

        #endregion

        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }






    public static class PublicResources
    {
        public static HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:55544/")
            //BaseAddress = new Uri("http://10.10.0.34/")
        };

        internal static User Im = new User();
        internal static int sortCount = -1;
        internal static CancellationTokenSource ordersCts = new CancellationTokenSource();
        static PublicResources()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}

