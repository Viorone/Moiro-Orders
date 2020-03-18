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
using System.Windows.Input;
using static Moiro_Orders.MainWindow;

namespace Moiro_Orders.ViewModel
{
    public class OrderViewModel : INotifyPropertyChanged
    {

        public ObservableCollection<Order> _orders = new ObservableCollection<Order>();
        public ObservableCollection<Order> Orders
        {
            get { return _orders; }
            set
            {
                _orders= value;
                RaisePropertyChanged();
            }
        }
        public OrderViewModel()
        { }

        #region ICommand and external set data
        private AsyncDelegateCommand _commandGetAllOrders;
        private AsyncDelegateCommand _commandGetAllOrders1;
        public ICommand CommandGetAllOrders
        {
            get
            {
                if (_commandGetAllOrders == null)
                {
                    _commandGetAllOrders = new AsyncDelegateCommand(GetAllOrdres);
                }
                return _commandGetAllOrders;
            }
        }
        public ICommand CommandGetOrdersOfDate
        {
            get
            {
                if (_commandGetAllOrders1 == null)
                {
                    _commandGetAllOrders1 = new AsyncDelegateCommand(GetOrdersOfDate);
                }
                return _commandGetAllOrders1;
            }
        }
        private async Task GetAllOrdres(object o)
        {
            if(PublicResources.Im.Admin)
            {
                IAdmin admin = new CurrentUser();
                var orders = await admin.GetAllOrdersToday(DateTime.Today);
                Orders = new ObservableCollection<Order>(orders);
            }
            else
            {
                IUser user = new CurrentUser();
                var orders = await user.GetOrdersList(20, PublicResources.Im.Id);
                Orders = new ObservableCollection<Order>(orders);            
            }                   
        }
        private async Task GetOrdersOfDate(object o)
        {
            IUser user = new CurrentUser(); 
            var orders = await user.GetOrdersListOfDate( PublicResources.Im.Id, new DateTime(2020,3,12));
            Orders = new ObservableCollection<Order>(orders);
        }
        #endregion
        #region MVVM related        
        private void RaisePropertyChanged([CallerMemberName]string propertyName = "") // волшебство .NET 4.5
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }

}

