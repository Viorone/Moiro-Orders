
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
            List<User> allUsers = new List<User>();
            DirectoryEntry searchRoot;

            searchRoot = new DirectoryEntry("LDAP://" + "moiro.bel", null, null, AuthenticationTypes.Secure);
            DirectorySearcher search = new DirectorySearcher(searchRoot);
            search.Filter = "(&(objectClass=user)(objectCategory=person))";
            search.PropertiesToLoad.Add("distinguishedname");
            SearchResult result, result2;
            SearchResultCollection resultCol = search.FindAll();
            search.PropertiesToLoad.Add("samaccountname");
            SearchResultCollection resultCol2 = search.FindAll();

            if (resultCol == null)
            {
                return;
            }
            for (int counter = 0; counter < resultCol.Count; counter++)
            {
                result = resultCol[counter];
                result2 = resultCol2[counter];
                if (result.Properties.Contains("distinguishedname"))
                {
                    string ulongData = result.Properties["distinguishedname"][0].ToString();
                    string login = result2.Properties["samaccountname"][0].ToString();
                    // allUsers.Add(ulongData);
                    int startIndex = ulongData.IndexOf("OU=", 1) + 3; //+3 for  length of "OU="
                    int endIndex = ulongData.IndexOf(",", startIndex);
                    int startIndexCN = ulongData.IndexOf("CN=", 1) + 3;
                    int endIndexCN = ulongData.IndexOf(",", startIndexCN);
                    var group = ulongData.Substring((startIndex), (endIndex - startIndex));
                    var name = ulongData.Substring(startIndexCN, endIndexCN - startIndexCN).Trim('=');
                    if (name[0] >= 0x0400 && name[0] <= 0x04FF)
                    {
                        allUsers.Add(new User
                        {
                            FullName = name,
                            Login = login,
                            OrganizationalUnit = group
                        });
                    }
                    // ОНО работает!!!!!!
                }
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

