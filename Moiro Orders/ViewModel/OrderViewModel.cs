using Moiro_Orders.Models;
using Moiro_Orders.Roles;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Moiro_Orders.ViewModel
{
    public class OrderViewModel : INotifyPropertyChanged
    {
        public OrderViewModel() { }

        public ObservableCollection<Order> _orders = new ObservableCollection<Order>();
        public ObservableCollection<Order> Orders
        {
            get { return _orders; }
            set
            {
                _orders = value;
                RaisePropertyChanged();
            }
        }

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

        //private RelayCommand doubleCommand;
        //public RelayCommand DoubleCommand
        //{
        //    get
        //    {
        //        return doubleCommand;
        //    }
        //}




        private async Task GetAllOrdres(object a)
        {


            IUser user = new CurrentUser();
            Order order = new Order();
            var status = await user.CreateOrder(order);
         
                 
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

