
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

namespace Moiro_Orders
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            //get User
            UsersController currentUser = new UsersController();
            currentUser.GetUserAsync("gybarev").GetAwaiter();
        }
        private void Send_Click(object sender, RoutedEventArgs e)
        {                
            // Create a new event
            Event @event = new Event
            {
                UserId = 1,
                NameEvent = "Прикол",
                Description = "100",
                Status = "Widgets",
                DateStart = DateTime.Now,
                DateEnd = DateTime.Now,
                Place = "General place"
            };
            IUser user = new CurrentUser();
            //user.CrateEvent(@event).GetAwaiter(); 
            async Task GetEvent()
            {
                var events = await user.GetEventsList(20, PublicResources.Im.Id);
                MessageBox.Show(events[0].NameEvent);
            }
            GetEvent().GetAwaiter();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Create a new order
            Order order = new Order
            {
                UserId = 1,
                Problem = "Gizmo",
                Description = "100",
                Status = "Widgets",
            };
            IUser user = new CurrentUser();
            //user.CreateOrder(order).GetAwaiter();
            async Task GetOrder()
            {
                var orders = await user.GetOrdersList(20, PublicResources.Im.Id);
                MessageBox.Show(orders[0].Problem, "Это проблемма");
            }
            GetOrder().GetAwaiter();
        }

        private void Chat_Click(object sender, RoutedEventArgs e)
        {
            // Create a new publicChat
            PublicChat publicChat = new PublicChat
            {
                UserId = 1,
                Message = "Hello World"
            };
            IAdmin admin = new CurrentUser();
            //admin.CreatePublicChatMessage(publicChat).GetAwaiter();
            async Task GetMessage()
            {
                var messages = await admin.GetPublicChatMessagesList(20, PublicResources.Im.Id);
                MessageBox.Show(messages[0].Message);
            }
            GetMessage().GetAwaiter();
        }
    }











    public static class PublicResources
    {
        public static HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:55544/")        //"http://10.10.0.34/"
        };

        internal static User Im  = new User();

        static PublicResources()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }



    }
}

