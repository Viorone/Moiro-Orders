using Moiro_Orders.Models;
using Moiro_Orders.Roles;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Moiro_Orders.XamlView
{
    /// <summary>
    /// Логика взаимодействия для EventView.xaml
    /// </summary>
    public partial class EventView : UserControl
    {
        Event selectedEvent = new Event();
        bool isProblem = true;

        public EventView()
        {
            InitializeComponent();
            datePick.SelectedDate = DateTime.Now;
        }

        private void AddEvents_Click(object sender, RoutedEventArgs e)
        {
            isProblem = true;
            NameEvent.Text = null;
            DescriptionEvent.Text = null;
            PlaceEvent.Text = null;
            CalendarWithDate.SelectedDate = null;
            StartTime.SelectedTime = null;
            EndTime.SelectedTime = null;
            CalendarWithDate.SelectionMode = CalendarSelectionMode.MultipleRange;
            FormAddEvent.Visibility = Visibility.Visible;
        }

        private void CancelEvent_Click(object sender, RoutedEventArgs e)
        {
            CancelSelectedEvent().GetAwaiter();
            ListEvents.SelectedIndex = -1;
            ChangeEvent.Visibility = Visibility.Hidden;
            CancelEvent.Visibility = Visibility.Hidden;
        }

        private void ChangeEvent_Click(object sender, RoutedEventArgs e)
        {
            if (ListEvents.SelectedIndex != -1)
            {
                isProblem = false;
                NameEvent.Text = selectedEvent.NameEvent;
                DescriptionEvent.Text = selectedEvent.Description;
                PlaceEvent.Text = selectedEvent.Place;
                CalendarWithDate.SelectedDate = selectedEvent.DateStart;
                CalendarWithDate.SelectionMode = CalendarSelectionMode.SingleDate;
                StartTime.SelectedTime = selectedEvent.DateStart;
                EndTime.SelectedTime = selectedEvent.DateEnd;
            }
        }

        private void DatePick_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime? selectedDate = datePick.SelectedDate;
            if (selectedDate != null)
            {              
                GetEventsOfDate().GetAwaiter();
            }
        }

        private void ListEvents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (e.AddedItems.Count == 0)
            {
                return;
            }
            ChangeEvent.Visibility = Visibility.Hidden;
            CancelEvent.Visibility = Visibility.Hidden;
            selectedEvent = (Event)e.AddedItems[0];
            var nowTime = DateTime.Now;
            var changeTime = nowTime - selectedEvent.Date;
            DateTime time = Convert.ToDateTime("23:59:59");
            if (selectedEvent.UserId == PublicResources.Im.Id && changeTime < time.TimeOfDay && !selectedEvent.IsCanceled)
            {
                ChangeEvent.Visibility = Visibility.Visible;
                CancelEvent.Visibility = Visibility.Visible;
            }
        }

        private void EventsList_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListEvents.SelectedIndex = -1;
            ChangeEvent.Visibility = Visibility.Hidden;
            CancelEvent.Visibility = Visibility.Hidden;
        }        

        private void SelectedDatesShow_Click(object sender, RoutedEventArgs e)
        {
            if (CheckFields())
            {
                if (isProblem)
                {
                    
                    CrateEvents().GetAwaiter();
                }
                else
                {
                    ChangeSelectedEvent().GetAwaiter();
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
            if (CalendarWithDate.SelectedDates.Count == 0)
            {
                ShowErrorMessage("Ошибка в дате мероприятий", "Выберите дату или даты проведения данного мероприятия");
                return false;
            }
            if (!StartTime.SelectedTime.HasValue && !EndTime.SelectedTime.HasValue)
            {
                ShowErrorMessage("Ошибка во времени проведения", "Выберите время начала и окончания проведения данного мероприятия");
                return false;
            }
            if (CalendarWithDate.SelectedDates.Count >= 31)
            {
                ShowErrorMessage("Ошибка", "Выбрано слишком много дат!");
                CalendarWithDate.SelectedDates.RemoveAt(30);
                return false;
            }
            foreach (var date in CalendarWithDate.SelectedDates)
            {
                var tmbSub = date - DateTime.Now.Date;
                if (tmbSub.Days < 0)
                {
                    ShowErrorMessage("Ошибка в дате (датах) проведения", "Неверная дата "+ tmbSub.ToString() +"\nНельзя выберать дату или даты мероприятия раньше текущего дня");
                    return false;
                }
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

       bool CheckDates(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                ShowErrorMessage("Ошибка", "Дата начала не может быть больше даты окончания");
                return false;
            }
            if(startDate < DateTime.Now)
            {
                ShowErrorMessage("Ошибка", "Вы не можете выбирать прошедшие даты и время");
                return false;
            }
            if(endDate < DateTime.Now)
            {
                ShowErrorMessage("Ошибка", "Вы не можете выбирать прошедшие даты и время");
                return false;
            }
            
            return true;
        }

        private void CalendarWithDate_MouseMove(object sender, MouseEventArgs e)
        {
            Mouse.Capture(null);
        }

        #region ASYNC metods

        async Task GetEventsOfDate()
        {
            DateTime selectDate = datePick.SelectedDate.Value.Date;
            IAdmin admin = new CurrentUser();
            var events = await admin.GetAllEventsToday(selectDate);
            ListEvents.ItemsSource = events;
        }

        async Task CrateEvents()
        {
            var collections = CalendarWithDate.SelectedDates;
            foreach (var tmpDate in collections)
            {               
                DateTime startDate, endDate;
                startDate = tmpDate;
                startDate = startDate.AddHours(StartTime.SelectedTime.Value.Hour);
                startDate = startDate.AddMinutes(StartTime.SelectedTime.Value.Minute);
                endDate = tmpDate;
                endDate = endDate.AddHours(EndTime.SelectedTime.Value.Hour);
                endDate = endDate.AddMinutes(EndTime.SelectedTime.Value.Minute);
                var tmpCheck = CheckDates(startDate, endDate);
                if (tmpCheck)
                {
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
                        IsCanceled = false
                    });
                }
                await GetEventsOfDate();
            }
        }


        async Task ChangeSelectedEvent()
        {
            var tmpDate = CalendarWithDate.SelectedDate.Value;
            DateTime startDate, endDate;
            startDate = tmpDate;
            startDate = startDate.AddHours(StartTime.SelectedTime.Value.Hour);
            startDate = startDate.AddMinutes(StartTime.SelectedTime.Value.Minute);
            endDate = tmpDate;
            endDate = endDate.AddHours(EndTime.SelectedTime.Value.Hour);
            endDate = endDate.AddMinutes(EndTime.SelectedTime.Value.Minute);

            var tmpCheck = CheckDates(startDate, endDate);
            if (tmpCheck)
            {
                IUser user = new CurrentUser();
                var status = await user.EditEvent(new Event
                {
                    Id = selectedEvent.Id,
                    Description = DescriptionEvent.Text,
                    UserId = selectedEvent.UserId,
                    DateStart = startDate,
                    DateEnd = endDate,
                    NameEvent = NameEvent.Text,
                    Place = PlaceEvent.Text,
                    IsCanceled = false
                });
            }
            await GetEventsOfDate();
        }

        async Task CancelSelectedEvent()
        {
            IUser user = new CurrentUser();
            Event @event = new Event();
            @event = selectedEvent;
            @event.IsCanceled = true;
            var status = await user.EditEvent(@event);
            await GetEventsOfDate();
        }


        #endregion

    }
}