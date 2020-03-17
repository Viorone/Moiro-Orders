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
            set {
                _orders= value;
                RaisePropertyChanged();
            }
        }

        private AsyncDelegateCommand _longAddCommand;
        public ICommand BTNclick
        {
            get
            {
                if (_longAddCommand == null)
                {
                    _longAddCommand = new AsyncDelegateCommand(LongAdd);
                }
                return _longAddCommand;
            }
        }

        private async Task LongAdd(object o)
        {
            IUser user = new CurrentUser();
            //user.CreateOrder(order).GetAwaiter();
            async Task GetOrder()
            {
                var orders = await user.GetOrdersList(20, PublicResources.Im.Id);
                foreach (var tmp in orders)
                {
                    Orders.Add(tmp);
                }

            } 
            GetOrder().GetAwaiter();
        }



        public  OrderViewModel()
        {
           

        }


        #region MVVM related        
        private void RaisePropertyChanged([CallerMemberName]string propertyName = "") // волшебство .NET 4.5
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }

}

