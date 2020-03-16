using Moiro_Orders.Models;
using Moiro_Orders.Roles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Moiro_Orders.ViewModel
{
    public class OrderViewModel : INotifyPropertyChanged
    {

        private Order selectedPhone;

        public ObservableCollection<Order> Orders { get; set; }
        public Order SelectedPhone
        {
            get { return selectedPhone; }
            set
            {
                selectedPhone = value;
                OnPropertyChanged("SelectedPhone");
            }
        }

        public  OrderViewModel()
        {
            IUser user = new CurrentUser();
            async Task GetAllOrders()
            {
                var orders = await user.GetOrdersList(40, 1);

                foreach (var tmp in orders)
                {
                    Orders.Add(tmp);
                }
            }
            GetAllOrders().GetAwaiter();

        }
    

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

    }
}
