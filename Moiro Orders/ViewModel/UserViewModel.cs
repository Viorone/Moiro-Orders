using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Moiro_Orders.ViewModel
{
    public class UserViewModel : INotifyPropertyChanged
    {

        private string _synchronizedText;
        public string SynchronizedText
        {
            get => _synchronizedText;
            set
            {
                _synchronizedText = value;
                OnPropertyChanged(nameof(SynchronizedText));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
 
   
}

