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
    /// Логика взаимодействия для WebinarView.xaml
    /// </summary>
    public partial class WebinarView : UserControl
    {
        Webinar selectedWebinar = new Webinar();
        bool isProblem = true;

        public WebinarView()
        {
            InitializeComponent();
            datePick.SelectedDate = DateTime.Now;
        }

        private void AddWebinars_Click(object sender, RoutedEventArgs e)
        {
            isProblem = true;
            NameWebinar.Text = null;
            DescriptionWebinar.Text = null;
            PlaceWebinar.Text = null;
            CalendarWithDate.SelectedDate = null;
            StartTime.SelectedTime = null;
            EndTime.SelectedTime = null;
            FormAddWebinar.Visibility = Visibility.Visible;
        }

        private void DatePick_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime? selectedDate = datePick.SelectedDate;
            if (selectedDate != null)
            {
                var selectDate = selectedDate.Value.Date;
                if (PublicResources.Im.Admin)
                {
                    GetWebinarsOfDateAdmin().GetAwaiter();
                }
                else
                {
                    GetWebinarsOfDateUser().GetAwaiter();
                }
                async Task GetWebinarsOfDateUser()
                {
                    IUser user = new CurrentUser();
                    var webinars = await user.GetWebinarsListOfDate(PublicResources.Im.Id, selectDate);
                    ListWebinars.ItemsSource = webinars;
                }

                async Task GetWebinarsOfDateAdmin()
                {
                    IAdmin admin = new CurrentUser();
                    var webinars = await admin.GetAllWebinarsToday(selectDate);
                    ListWebinars.ItemsSource = webinars;
                }
            }
        }

        private void ListWebinars_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                return;
            }
            selectedWebinar = (Webinar)e.AddedItems[0];
        }

        private void WebinarsList_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListWebinars.SelectedIndex = -1;
        }

        private void ChangeWebinar_Click(object sender, RoutedEventArgs e)
        {
            if (ListWebinars.SelectedIndex != -1)
            {
                isProblem = false;
                NameWebinar.Text = selectedWebinar.NameWebinar;
                DescriptionWebinar.Text = selectedWebinar.Description;
                PlaceWebinar.Text = selectedWebinar.Place;
                CalendarWithDate.SelectedDate = selectedWebinar.DateStart;
                StartTime.SelectedTime = selectedWebinar.DateStart;
                EndTime.SelectedTime = selectedWebinar.DateEnd;
            }
        }

        private void SaveWebinars_Click(object sender, RoutedEventArgs e)
        {
            if (CheckFields())
            {
                if (isProblem)
                {
                    CrateWebinars().GetAwaiter();
                }
                else
                {
                    //ChangeSelectedWebinar().GetAwaiter();
                }
            }
        }

       
        private bool CheckFields()
        {
            if (NameWebinar.Text == "" || NameWebinar.Text == null)
            {
                ShowErrorMessage("Ошибка в названии мероприятия", "Введите название предстоящего мероприятия");
                return false;
            }
            if (PlaceWebinar.Text == "" || PlaceWebinar.Text == null)
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
            return true;
        }

        // Сообщение об ошибке
        private void ShowErrorMessage(string header, string body)
        {
            ErrorGrid.Visibility = Visibility.Visible;
            ErrorWebinarHeader.Text = header;
            ErrorWebinarBody.Text = body;
        }
        private void ErrorOk_Click(object sender, RoutedEventArgs e)
        {
            ErrorGrid.Visibility = Visibility.Hidden;
        }

        private void CalendarWithDate_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CalendarWithDate.SelectedDates.Count >= 31)
            {
                ShowErrorMessage("Ошибка", "Выбрано слишком много дат!");
                CalendarWithDate.SelectedDates.RemoveAt(CalendarWithDate.SelectedDates.Count);
            }
        }





        #region ASYNC metods

        async Task CrateWebinars()
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


                IUser user = new CurrentUser();
                var status = await user.CreateWebinar(new Webinar
                {
                    Date = DateTime.Now,
                    Description = DescriptionWebinar.Text,
                    UserId = PublicResources.Im.Id,
                    DateStart = startDate,
                    DateEnd = endDate,
                    NameWebinar = NameWebinar.Text,
                    Place = PlaceWebinar.Text,
                    IsCanceled = false,
                    PlatformId = 1
                });
                MessageBox.Show(status.ToString());
            }
        }


        #endregion


    }
}
