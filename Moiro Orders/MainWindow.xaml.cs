
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
                    await currentUser.GetUserNameAsync("gybarev");
                    Title = PublicResources.Im.FullName + " | " + PublicResources.Im.OrganizationalUnit;
                    InitializeComponent();
                   DataContext = new OrderViewModel();
                }
                GetUser().GetAwaiter();
            }
            else
            {
                InitializeComponent();
                DataContext = new OrderViewModel();
            }
        }
    }









    public static class PublicResources
    {
        public static HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:55544/")        //"http://10.10.0.34/"
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

