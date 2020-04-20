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
            FormAddEvent.Visibility = Visibility.Visible;
            // при добавлении должен быть указан кабинет у пользователя!
            FormAddEvent.Visibility = Visibility.Visible;
         
           
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

        private void SelectedDatesShow_Click(object sender, RoutedEventArgs e)
        {
            if (CheckFields())
            {
                var collections = CalendarWhithDate.SelectedDates;
                foreach (var tmpDate in collections)
                {
                    async Task SetEventsOfDate()
                    {
                        DateTime startDate, endDate;
                        startDate = tmpDate;
                        startDate = startDate.AddHours(StartTime.SelectedTime.Value.Hour);
                        startDate = startDate.AddMinutes(StartTime.SelectedTime.Value.Minute);
                        endDate = tmpDate;
                        endDate = startDate.AddHours(StartTime.SelectedTime.Value.Hour);
                        endDate = startDate.AddMinutes(StartTime.SelectedTime.Value.Minute);
                        IUser user = new CurrentUser();
                        var status = await user.CreateEvent(new Event
                        {
                            Date = DateTime.Now,
                            Description = DescriptionEvent.Text,
                            UserId = PublicResources.Im.Id,
                            DateStart = startDate,
                            DateEnd = endDate,
                            NameEvent = NameEvent.Text,
                            Place = PlaceEvent.Text,
                            StatusId = 1
                        });
                        MessageBox.Show(status.ToString());
                    }
                    SetEventsOfDate().GetAwaiter();
                }
            }
        }
        //проверка на заполненость полей для мероприятия
        private bool CheckFields()
        {
            if (NameEvent.Text == "" || NameEvent.Text == null)
            {
                ShowErrorMessage("Ошибка в названии мероприятия", "Введите название предстоящего мероприятия");
                return false;
            }
            if (PlaceEvent.Text == "" || PlaceEvent.Text == null)
            {
                ShowErrorMessage("Ошибка в месте проведения", "Введите место проведения предстоящего мероприятия");
                return false;
            }
            if (CalendarWhithDate.SelectedDates.Count == 0)
            {
                ShowErrorMessage("Ошибка в дате мероприятий", "Выберите дату или даты проведения данного мероприятия");
                return false;
            }
            if (!StartTime.SelectedTime.HasValue && !EndTime.SelectedTime.HasValue)
            {
                ShowErrorMessage("Ошибка во времени проведения", "Выберите время начала и окончания проведения данного мероприятия");
                return false;
            }
            return true;
        }
        // Сообщение об ошибке
        private void ShowErrorMessage(string header, string body)
        {
            ErrorGrid.Visibility = Visibility.Visible;
            ErrorEventHeader.Text = header;
            ErrorEventBody.Text = body;
        }
        private void ErrorOk_Click(object sender, RoutedEventArgs e)
        {
            ErrorGrid.Visibility = Visibility.Hidden;
        }

        private void CalendarWhithDate_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CalendarWhithDate.SelectedDates.Count >= 31)
            {
                MessageBox.Show("Выбрано слишком много дат!");
            }
        }

       
    }
}
