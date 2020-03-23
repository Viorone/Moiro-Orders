
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
                UsersController currentUser = new UsersController();
                async Task GetUser()
                {
                    await currentUser.GetUserNameAsync(Environment.UserName);
                    Title = PublicResources.Im.FullName + " | " + PublicResources.Im.OrganizationalUnit;
                    InitializeComponent();
                }
                GetUser().GetAwaiter();
            }
            else
            {
                InitializeComponent();
            }
        }

        private void AddOrder_Click(object sender, RoutedEventArgs e)
        {
            //Order tmp = new Order
            //{
            //    UserId = 1,
            //    Date = DateTime.Now,
            //    Description = "Хьюстон у нас проблемы",
            //    Problem = "Жопа",
            //    Status = "могло бы быить и лучше"
            //};
            //IUser user = new CurrentUser();
            //async Task CreateOrder()
            //{
            //    var status =  await user.CreateOrder(tmp);
            //    MessageBox.Show(status.ToString());
            //}
            //CreateOrder().GetAwaiter();        
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

        private void Orders_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SwitchScreen(new OrderView());
            //frameOrder.NavigationService.Navigate(new Uri("XamlView/OrderView.xaml", UriKind.Relative));
        }

        private void Users_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SwitchScreen(new UserView());
            //frameOrder.NavigationService.Navigate(new Uri("XamlView/UserView.xaml", UriKind.Relative));
        }

        private void CloseMenuButton_MouseEnter(object sender, MouseEventArgs e)
        {
            
           
        }
        internal void SwitchScreen(object sender)
        {
            var clicl = (UserControl)sender;
            if (clicl!=null)
            {
                mainView.Children.Clear();
                mainView.Children.Add(clicl);
            }
        }
    }
















    public static class PublicResources
    {
        public static HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri("http://10.10.0.34/")        //"http://10.10.0.34/"
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

