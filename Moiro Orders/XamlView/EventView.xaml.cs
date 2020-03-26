using Moiro_Orders.Models;
using Moiro_Orders.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Moiro_Orders.XamlView
{
    /// <summary>
    /// Логика взаимодействия для EventView.xaml
    /// </summary>
    public partial class EventView : UserControl
    {
        public EventView()
        {
            InitializeComponent();
            datePick.SelectedDate = DateTime.Now;
        }

        private void AddAllEvents_Click(object sender, RoutedEventArgs e)
        {
            if (PublicResources.Im.Room != 0)
            {
                async Task SetEventsOfDate()
                {
                    IUser user = new CurrentUser();
                    var status = await user.CreateEvent(new Event
                    {
                        Date = DateTime.Now,
                        Description = "Бысл сломан компьютер последством внешнего вмешательства сверхестественных сил",
                        UserId = PublicResources.Im.Id,
                        DateStart = DateTime.Now,
                        DateEnd = DateTime.Now,
                        NameEvent = "Карамба!!!",
                        Place = "Уютный домик",
                        Status = "Работаем"
                    });
                    MessageBox.Show(status.ToString());
                }
                SetEventsOfDate().GetAwaiter();
            }
            else
            {
                MessageBox.Show("Для того, что бы оставить заявку необходимо в настройках", "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void DatePick_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime? selectedDate = datePick.SelectedDate;
            if (selectedDate != null)
            {
                var selectDate = selectedDate.Value.Date;
                if (PublicResources.Im.Admin)
                {
                    GetEventsOfDateAdmin().GetAwaiter();
                }
                else
                {
                    GetEventsOfDateUser().GetAwaiter();
                }
                async Task GetEventsOfDateUser()
                {
                    IUser user = new CurrentUser();
                    var events = await user.GetEventsListOfDate(PublicResources.Im.Id, selectDate);
                    listEvents.ItemsSource = events;
                }

                async Task GetEventsOfDateAdmin()
                {
                    IAdmin admin = new CurrentUser();
                    var events = await admin.GetAllEventsToday(selectDate);
                    listEvents.ItemsSource = events;
                }
            }
        }
    }
}
