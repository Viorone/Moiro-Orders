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

        public List<Order> Orders { get; set; }
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
            List<Order> ord = null;
            async Task GetAllOrders()
            {
                ord = await user.GetOrdersList(40, 1);
                Orders = ord;
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
