using Moiro_Orders.Models;
using Moiro_Orders.Roles;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
            // при добавлении должен быть указан кабинет у пользователя!
            FormAddEvent.Visibility = Visibility.Visible;
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
                        StatusId = 1
                    });
                    MessageBox.Show(status.ToString());
                }
               // SetEventsOfDate().GetAwaiter();
               // отключил верхний таск что бы не спамили
           
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

        private void BackToEvent_Click(object sender, RoutedEventArgs e)
        {
            FormAddEvent.Visibility = Visibility.Hidden;
        }

        private void SelectedDatesShow_Click(object sender, RoutedEventArgs e)
        {
            var collections = CalendarWhithDate.SelectedDates;
            foreach (var tmpDate in collections)
            {
               // stackTrace.Text += tmpDate.Date+" ";
            }
        }

        // 
        private void CalendarWhithDate_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CalendarWhithDate.SelectedDates.Count >= 31)
            {
                MessageBox.Show("Выбрано слишком много дат!");
            }
        }
    }
}
