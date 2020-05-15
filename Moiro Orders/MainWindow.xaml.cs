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
        bool check = true;
        public MainWindow()
        {
            InitializeComponent();
            GetUser().GetAwaiter();
            Task.Run(() => Messenger());           
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
        //для перехода с формы AdminView
        public void Orders_FromAdmin()
        {
            if (click)
            {
                click = false;
                Task.Run(() => MainClickSaver());
                SwitchScreen(new OrderView());
            }
        }

        private void Admins_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (PublicResources.Im.Admin)
            {
                if (click)
                {
                    click = false;
                    Task.Run(() => MainClickSaver());
                    PublicResources.ordersCts.Cancel();
                    SwitchScreen(new AdminView());
                }
            }
        }
        private void Webinars_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (click)
            {
                click = false;
                Task.Run(() => MainClickSaver());
                PublicResources.ordersCts.Cancel();
                SwitchScreen(new WebinarView()); 
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
        private void Info_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            if (click)
            {
                click = false;
                Task.Run(() => MainClickSaver());
                PublicResources.ordersCts.Cancel();
                SwitchScreen(new InfoView());
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
                UpdateUserNameHeader();
                mainView.Children.Clear();
                mainView.Children.Add(clicl);
            }
        }

        internal void SwitchScreen(OrderView sender,DateTime date, int sortId)
        {
            if (sender != null)
            {
                UpdateUserNameHeader();
                mainView.Children.Clear();
                mainView.Children.Add(sender);
                sender.datePick.SelectedDate = date;
                sender.OrderSortBox.SelectedIndex = sortId;
            }
        }

        #region ASYNC metods
        async Task GetUser()
        {
            UsersController currentUser = new UsersController();
            var user = await currentUser.GetUserAsync(Environment.UserName);
            //var user = await currentUser.GetUserAsync("gybarev2"); 
            user.LastLogin = DateTime.Now;
            await currentUser.UpdateUserAsync(user);
            HeaderText.Text = PublicResources.Im.FullName + " | " + PublicResources.Im.OrganizationalUnit;
            Admins.Visibility = Visibility.Visible;
            SwitchScreen(new OrderView());
            loadingGrid.Visibility = Visibility.Hidden;
            if (PublicResources.Im.Admin == false)
            {
                Admins.Visibility = Visibility.Hidden;
            }    
        }

        async void MainClickSaver()
        {
            await Task.Delay(200);
            click = true;
        }

        async void Messenger()
        {
            while(check)
            {
                if (PublicResources.messengerChecker == true)
                {
                    void action()
                    {
                        if (WindowState == WindowState.Minimized)
                        {
                            WindowState = WindowState.Normal;
                        }
                        Activate();
                        Topmost = true;
                        Topmost = false;
                        Focus();
                    }
                    Dispatcher.Invoke(action);
                    PublicResources.messengerChecker = false;
                }
                await Task.Delay(1000);               
            }           
        }


        #endregion

        #region Обработчики на форму для замены стандартных контролов

        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            //сворачиваем что бы не закрывали
            WindowState = WindowState.Minimized;
            //Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void MaximaizedApp_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                FullScreenImage.Kind = MaterialDesignThemes.Wpf.PackIconKind.WindowMaximize;
                WindowState = WindowState.Normal;
            }
            else
            {
                FullScreenImage.Kind = MaterialDesignThemes.Wpf.PackIconKind.WindowRestore;
                WindowState = WindowState.Maximized;
            }
        }

        private void MinimazedApp_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        #endregion

        public void UpdateUserNameHeader()
        {
            HeaderText.Text = null;
            HeaderText.Text = PublicResources.Im.FullName + " | " + PublicResources.Im.OrganizationalUnit;
        }
    }







    public static class PublicResources
    {
        internal static User Im = new User();
        internal static int sortCount = -1;
        internal static string version = "0.41 beta";
        internal static CancellationTokenSource ordersCts = new CancellationTokenSource();
        internal static bool messengerChecker = false;

        public static HttpClient client = new HttpClient()
        {
            //BaseAddress = new Uri("http://localhost:55544/")
            BaseAddress = new Uri("http://10.10.0.34/")
        };


        static PublicResources()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}

