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

using System.Windows;
using System.Windows.Data;


namespace Moiro_Orders.ViewModel
{
    public class OrderViewModel : INotifyPropertyChanged
    {


        private Order selectedOrder;

        public List<Order> Orders { get; set; }
        public Order SelectedOrder
        {
            get { return selectedOrder; }
            set
            {
                selectedOrder = value;
                OnPropertyChanged("SelectedOrder");

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
            async Task GetOrder()
            {
                var orders = await user.GetOrdersList(20, PublicResources.Im.Id);
                Orders = orders;
            }
            GetOrder().GetAwaiter();

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
