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

        private async Task GetAllOrdres(object o)
        {
            DateTime date = new DateTime(2020, 3, 15);

            if(PublicResources.Im.Admin == true)
            {
                IAdmin admin = new CurrentUser();
                var orders = await admin.GetOrdersListOfDate(1, date);
                Orders = new ObservableCollection<Order>(orders);
            }
            else
            {
                IUser user = new CurrentUser();
                var orders = await user.GetOrdersList(20, PublicResources.Im.Id);
                Orders = new ObservableCollection<Order>(orders);            
            }                   
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

