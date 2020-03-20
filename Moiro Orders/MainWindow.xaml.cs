
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

            

            async Task UpdateUsersDb()
            {
                IAdmin admin = new CurrentUser();

                var tmp1 = admin.GetNewADUsersList();
                var tmp2 = await admin.GetAllUserName();
                
                var result = tmp1.Join(tmp2, ok => ok.Login, ik => ik.Login,(one, two) => new { one, two }).ToList();
                    tmp1.RemoveAll(x => result.Any(r => x == r.one));
                    tmp2.RemoveAll(x => result.Any(r => x == r.two));

                List<string> responses = new List<string>();
                foreach (var var1 in tmp1)
                {
                    var response = await admin.UpdateUsersDb(var1);
                    responses.Add(response.ToString());
                }

               
            }

            UpdateUsersDb().GetAwaiter();
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            //допилить и разобраться


           // frameOrder.Navigate(typeofOrderView), "переход из MainPage");
           
            frameOrder.NavigationService.Navigate(new Uri("XamlView.UserView.xaml", UriKind.Relative));
            
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

