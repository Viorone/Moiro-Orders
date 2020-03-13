
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
            EventsController eventsController = new EventsController();
            //eventsController.CreateEvent().GetAwaiter();
            eventsController.GetEvents(20).GetAwaiter();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OrdersController currentOrder = new OrdersController();
            currentOrder.GetOrders(20).GetAwaiter();                    
        }

        private void Chat_Click(object sender, RoutedEventArgs e)
        {
            PublicChatsController publicChatsController = new PublicChatsController();
            //publicChatsController.CreatePublicChat().GetAwaiter();
            publicChatsController.GetPublicChat(20).GetAwaiter();
        }

    }











    public static class PublicResources
    {
        public static HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri("http://10.10.0.34/")
        };

        internal static User Im { get; set; } = new User();

        static PublicResources()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }
        
    }
}

