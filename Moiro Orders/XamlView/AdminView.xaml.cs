using System;
using Moiro_Orders.Roles;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Moiro_Orders.Models;
using System.Windows.Media;
using Word = Microsoft.Office.Interop.Word;
using System.Reflection;

namespace Moiro_Orders.XamlView
{
    /// <summary>
    /// Логика взаимодействия для User.xaml
    /// </summary>
    public partial class AdminView : UserControl
    {
        public List<Order> tmpOrd;
        public AdminView()
        {
            GetAdmins().GetAwaiter();
            InitializeComponent();
            for (int i = 1; i < 6; ++i)
            {
                GetCountOrders(i).GetAwaiter();
            }
            DateTime dTime = DateTime.Now;
            DateTime dTime2 = DateTime.Now;
            dTime = dTime.AddMonths(-1);
            dateStart.SelectedDate = dTime;
            dateEnd.SelectedDate = DateTime.Now.Date;
            dTime2 = dTime.AddMonths(2);
            EventsDateStart.SelectedDate = DateTime.Now.Date;
            EventsDateEnd.SelectedDate = dTime2;
        }

        private void ListViewEvent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewEvent.SelectedIndex == -1) { return; }
            addingPanel.Visibility = Visibility.Visible;
        }
        private void AdminSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (adminSelect.SelectedIndex == -1)
            {
                return;
            }
            SetAdminToEvent().GetAwaiter();

        }
        private void Button_Click(object sender, RoutedEventArgs e) //Гигантская кнопка
        {
            async Task UpdateUsersDb()
            {
                logList.Items.Clear();
                IAdmin admin = new CurrentUser();
                var tmp1 = admin.GetNewADUsersList();
                var tmp2 = await admin.GetAllUserName();
                var result = tmp1.Join(tmp2, ok => ok.Login, ik => ik.Login, (one, two) => new { one, two }).ToList();
                tmp1.RemoveAll(x => result.Any(r => x == r.one));
                tmp2.RemoveAll(x => result.Any(r => x == r.two));

                List<string> responses = new List<string>();

                foreach (var var1 in tmp1)
                {
                    var response = await admin.UpdateUsersDb(var1);
                    responses.Add(response.ToString());
                    logList.Items.Add(var1.FullName + " - " + response.ToString());
                }
            }
            UpdateUsersDb().GetAwaiter();
        }
        private void CompletePastOrders_Click(object sender, RoutedEventArgs e)
        {
            //событие на подтверждение всех неподверждённых заявок за прошлый месяц и более
        }

        #region Для вкладки "Мероприятия"
        private void EventsDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            GetEventsByAdmin();
        }
        private void EventLog_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EventLog.SelectedIndex == 0 && ListViewEvent.Items.Count == 0)
            {
                GetEventsByAdmin();
            }
            if (EventLog.SelectedIndex == 2)
            {
                dateEndReport.SelectedDate = DateTime.Now.Date;
                dateStartReport.SelectedDate = DateTime.Now.AddDays(-7);
                dateEndReportEvent.SelectedDate = DateTime.Now.Date;
                dateStartReportEvent.SelectedDate = DateTime.Now.AddDays(-7);
            }
        }
        #endregion

        #region Для вкладки "Статистика"

        #region Обработчики событий на карточки
        private void CardOrdersNew_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Task.Run(() => GetOrdersByStatus(1));
        }
        private void CardOrdersInProgress_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Task.Run(() => GetOrdersByStatus(2));
        }
        private void CardOrdersComplete_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Task.Run(() => GetOrdersByStatus(3));
        }
        private void CardOrdersNeedRepair_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Task.Run(() => GetOrdersByStatus(4));
        }
        private void CardOrdersCancel_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Task.Run(() => GetOrdersByStatus(5));
        }
        #endregion

        #region Обработчики событий на обновление карточки
        private void RefreshOrdersNew_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GetCountOrders(1).GetAwaiter();
        }
        private void RefreshOrdersinProgress_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GetCountOrders(2).GetAwaiter();
        }
        private void RefreshOrdersComplete_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GetCountOrders(3).GetAwaiter();
        }
        private void RefreshOrdersNeedRepair_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GetCountOrders(4).GetAwaiter();
        }
        private void RefreshOrdersCancel_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GetCountOrders(5).GetAwaiter();
        }
        #endregion
        #endregion статистика



        #region Для вкладки отчётов
        private void SaveDocxFile_Click(object sender, RoutedEventArgs e)
        {
            if (dateEndReport.Text != "" && dateStartReport.Text != "")
            {
                if (dateEndReport.SelectedDate.Value < dateStartReport.SelectedDate.Value)
                {
                    logText.Foreground = Brushes.Red;
                    logText.Text = "Ошибка. Дата окончания не может быть меньше даты начала!";
                }
                else
                {
                    logText.Foreground = Brushes.Gray;
                    logText.Text = "Формирование отчёта... (Ждите пока не появится окно сохранения)";
                    CreateDocx();
                }
            }
            else
            {
                logText.Text = "Пожалуйста, заполните даты начала о окончания очёта";
                logText.Foreground = Brushes.Red;
            }
        }
        private void SaveDocxFileEvent_Click(object sender, RoutedEventArgs e)
        {
            if (dateEndReportEvent.Text != "" && dateStartReportEvent.Text != "")
            {
                if (dateEndReportEvent.SelectedDate.Value < dateStartReportEvent.SelectedDate.Value)
                {
                    logTextEvent.Foreground = Brushes.Red;
                    logTextEvent.Text = "Ошибка. Дата окончания не может быть меньше даты начала!";
                }
                else
                {
                    logTextEvent.Foreground = Brushes.Gray;
                    logTextEvent.Text = "Формирование отчёта... (Ждите пока не появится окно сохранения)";
                    CreateDocxEvent();
                }
            }
            else
            {
                logTextEvent.Text = "Пожалуйста, заполните даты начала о окончания очёта";
                logTextEvent.Foreground = Brushes.Red;
            }
        }
        #endregion

        #region ASYNC metods
        async Task<int> GetCountOrders(int statusId)
        {
            IAdmin admin = new CurrentUser();
            var count = await admin.GetCountOrdersByStatus(statusId);
            switch (statusId)
            {
                case 1:
                    CountOrdersNew.Text = count.ToString();
                    break;
                case 2:
                    CountOrdersInProgress.Text = count.ToString();
                    break;
                case 3:
                    CountOrdersComplete.Text = count.ToString();
                    break;
                case 4:
                    CountOrdersNeedRepair.Text = count.ToString();
                    break;
                case 5:
                    CountOrdersCancel.Text = count.ToString();
                    break;

            }
            return count;
        }
        async void GetOrdersByStatus(int statusId)
        {
            List<Order> orders = new List<Order>();
            DateTime tmpDateEnd = DateTime.Now;
            DateTime tmpDateStart = DateTime.Now;

            Action action = () =>
            {
                tmpDateStart = dateStart.SelectedDate.Value;
                tmpDateEnd = dateEnd.SelectedDate.Value;
            };
            Action action1 = () =>
            {
                ListGettingOrders.ItemsSource = orders;
            };
            await dateStart.Dispatcher.BeginInvoke(action);

            IAdmin admin = new CurrentUser();
            orders = await admin.GetOrdersByStatus(statusId, tmpDateStart, tmpDateEnd);
            await ListGettingOrders.Dispatcher.BeginInvoke(action1);
        }
        async void GetOrdersByStatusOnTime(int statusId)
        {
            List<Order> orders = new List<Order>();
            DateTime tmpDateEnd = DateTime.Now;
            DateTime tmpDateStart = DateTime.Now;

            Action action = () =>
            {
                tmpDateStart = dateStartReport.SelectedDate.Value;
                tmpDateEnd = dateEndReport.SelectedDate.Value;
            };
            Action action1 = () =>
            {
                tmpOrd = orders;
            };
            await dateStart.Dispatcher.BeginInvoke(action);

            IAdmin admin = new CurrentUser();
            orders = await admin.GetOrdersByStatus(statusId, tmpDateStart, tmpDateEnd);
            await ListGettingOrders.Dispatcher.BeginInvoke(action1);
        }
        async void GetEventsAll()
        {
            List<Order> events = new List<Order>();
            DateTime tmpDateEnd = DateTime.Now;
            DateTime tmpDateStart = DateTime.Now;

            Action action = () =>
            {
                tmpDateStart = dateStart.SelectedDate.Value;
                tmpDateEnd = dateEnd.SelectedDate.Value;
            };
            Action action1 = () =>
            {
                ListGettingOrders.ItemsSource = events;
            };
            await dateStart.Dispatcher.BeginInvoke(action);

            IAdmin admin = new CurrentUser();
            await ListGettingOrders.Dispatcher.BeginInvoke(action1);
        }
        async void GetEventsByAdmin()
        {
            List<Event> events = new List<Event>();
            DateTime tmpDateEnd = DateTime.Now;
            DateTime tmpDateStart = DateTime.Now;

            Action action = () =>
            {
                tmpDateStart = EventsDateStart.SelectedDate.Value;
                tmpDateEnd = EventsDateEnd.SelectedDate.Value;
            };
            Action action1 = () =>
            {
                events = events.OrderBy(a => a.DateStart).ToList();
                ListViewEvent.ItemsSource = events;
            };
            await dateStart.Dispatcher.BeginInvoke(action);

            IAdmin admin = new CurrentUser();
            events = await admin.GetEventsForStatistic(tmpDateStart, tmpDateEnd);
            await ListGettingOrders.Dispatcher.BeginInvoke(action1);
        }
        async Task GetAdmins()
        {
            IEnumerable<User> users = null;
            IAdmin admin = new CurrentUser();
            users = await admin.GetAdminsList();
            adminSelect.DisplayMemberPath = "FullName";
            adminSelect.ItemsSource = users;
        }
        async Task SetAdminToEvent()
        {
            Event tmpEvent = (Event)ListViewEvent.SelectedItem;
            tmpEvent.AdminId = ((User)adminSelect.SelectedItem).Id;
            IAdmin admin = new CurrentUser();
            var response = await admin.EditEvent(tmpEvent);
            addingPanel.Visibility = Visibility.Collapsed;
            ListViewEvent.SelectedItem = -1;
            adminSelect.SelectedIndex = -1;
            if (response == System.Net.HttpStatusCode.NoContent)
            {
                GetEventsByAdmin();
            }
        }
        async void CreateDocx()
        {
            int r = 0;
            DateTime start = dateStartReport.SelectedDate.Value;
            DateTime end = dateEndReport.SelectedDate.Value;
            var arr = new List<Order>[6];
            List<Order> orders = new List<Order>();
            IAdmin admin = new CurrentUser();
            orders = await admin.GetOrdersByStatus(3, start, end);
            arr[3] = orders;
            orders = await admin.GetOrdersByStatus(1, start, end);
            arr[1] = orders;
            orders = await admin.GetOrdersByStatus(2, start, end);
            arr[2] = orders;
            orders = await admin.GetOrdersByStatus(4, start, end);
            arr[4] = orders;
            orders = await admin.GetOrdersByStatus(5, start, end);
            arr[5] = orders;

            #region Create .Docx File
            // неведомая хрень
            object oMissing = Missing.Value;
            object oEndOfDoc = "\\endofdoc";
            Word.Application app;
            Word.Document docx;
            Word.Range wrdRng;
            // открытие приложния
            try
            {
                app = new Word.Application
                {
                    Visible = false, // отображение хода заполнения
                };
                //object fileName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Отчёт о заявках от " + DateTime.Now.ToLongDateString();
                docx = app.Documents.Add();
            }
            catch (Exception ex)
            {
                logText.Foreground = Brushes.Red;
                logText.Text = "Ошибка генерации! " + ex.Message; //исправить вывод и сделать номально увеличение текста
                //docx.Close(Word.WdSaveOptions.wdDoNotSaveChanges, Word.WdOriginalFormat.wdOriginalDocumentFormat, false);
                // app.Quit();
                return;
            }

            //изменение ориентации листа

            docx.PageSetup.Orientation = Word.WdOrientation.wdOrientLandscape;
            docx.PageSetup.TopMargin = 15;
            docx.PageSetup.BottomMargin = 15;
            docx.PageSetup.RightMargin = 25;
            docx.PageSetup.LeftMargin = 25;

            // начинаем создавать док
            Word.Paragraph para1;
            para1 = docx.Content.Paragraphs.Add(ref oMissing);
            para1.Range.Font.Name = "Times New Roman";
            para1.Range.Font.Size = 16;
            para1.Range.Text = "Отчёт о заявках от " + DateTime.Now.ToShortDateString();
            para1.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            para1.Range.Font.Bold = 1;
            para1.Range.InsertParagraphAfter();
            para1.CloseUp();

            //Insert a paragraph at the end of the document.
            Word.Paragraph para2;
            object oRng = docx.Bookmarks.get_Item(ref oEndOfDoc).Range;
            para2 = docx.Content.Paragraphs.Add(ref oRng);
            for (int i = 0; i < 6; i++)
            {
                //каждый раз после вставки нового предложения заново применяем форматирование
                para2.Range.Font.Bold = 0;
                para2.Range.Font.Name = "Times New Roman";
                para2.Range.Font.Size = 14;
                para2.Format.SpaceAfter = 0;
                para2.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                if (i == 0)
                {
                    para2.Range.Text = "Выбранный диапазон с " + start.Date.ToShortDateString() + " по " + end.Date.ToShortDateString() + ".";
                }
                if (i == 1)
                {
                    para2.Range.Text = "Заявки ожидающие принятия специалистом - " + arr[1].Count + ".";
                }
                if (i == 2)
                {
                    para2.Range.Text = "Заявки в процессе выполнения - " + arr[2].Count + ".";
                }
                if (i == 3)
                {
                    para2.Range.Text = "Выполнено заявок - " + arr[3].Count + ".";
                }
                if (i == 4)
                {
                    para2.Range.Text = "Заявки, требующие ремонта или закупки - " + arr[4].Count + ".";
                }
                if (i == 5)
                {
                    para2.Range.Text = "Отменено заявок пользователями - " + arr[5].Count + ".";
                }
                para2.Range.InsertParagraphAfter();
            }
            #region Таблица выполненных заявок
            if (arr[3].Count == 0)
            {
                para2.Range.InsertParagraphAfter();
                para2.Range.Text = "Выполненные заявки - Отсутствуют.";
                para2.Range.InsertParagraphBefore();
                para2.CloseUp();
            }
            else
            {
                para2.Range.InsertParagraphAfter();
                para2.Range.Text = "Таблица 1 - Выполненные заявки.";
                para2.CloseUp();


                Word.Table oTable;
                wrdRng = docx.Bookmarks.get_Item(ref oEndOfDoc).Range;
                oTable = docx.Tables.Add(wrdRng, arr[3].Count + 1, 6, ref oMissing, ref oMissing);
                oTable.Range.Font.Size = 12;
                //границы таблицы
                oTable.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                oTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                oTable.Columns[1].Width = 20;
                oTable.Columns[2].Width = 80;
                oTable.Columns[3].Width = 100;
                oTable.Columns[4].Width = 100;
                oTable.Columns[6].Width = 95;
                oTable.AllowAutoFit = true;  //выравнивание по ширине???
                oTable.AutoFitBehavior(Word.WdAutoFitBehavior.wdAutoFitWindow); // вот это точно выравнивание по ширине окна ворда
                for (r = 1; r <= arr[3].Count + 1; r++)
                {

                    if (r == 1)
                    {
                        oTable.Cell(r, 1).FitText = true; //расширение при добавлении текста
                        oTable.Cell(r, 1).Range.Text = "№";
                        oTable.Cell(r, 2).Range.Text = "Дата отправки";
                        oTable.Cell(r, 3).Range.Text = "Заявитель";
                        oTable.Cell(r, 4).Range.Text = "Исполнитель";
                        oTable.Cell(r, 5).Range.Text = "Проблема";
                        oTable.Cell(r, 6).Range.Text = "Дата подтверждения";
                        for (int tmp = 1; tmp <= 6; tmp++)
                        {
                            oTable.Cell(r, tmp).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                        }
                        continue;
                    }
                    oTable.Rows[r - 1].Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    oTable.Cell(r, 1).Range.Text = arr[3][r - 2].Id.ToString();
                    oTable.Cell(r, 2).Range.Text = arr[3][r - 2].Date.ToString();
                    oTable.Cell(r, 3).Range.Text = arr[3][r - 2].UserName.ToString();
                    oTable.Cell(r, 4).Range.Text = arr[3][r - 2].AdminName.ToString();
                    oTable.Cell(r, 5).Range.Text = arr[3][r - 2].Problem.ToString();
                    oTable.Cell(r, 6).Range.Text = arr[3][r - 2].CompletionDate.ToString();
                }
                oTable.Rows[1].Range.Font.Bold = 1;
                oTable.Rows[1].Alignment = Word.WdRowAlignment.wdAlignRowCenter;
            }
            #endregion

            #region Таблица ожидающих заявок
            //параграф (заголовок)
            if (arr[2].Count == 0)
            {
                Word.Paragraph para;
                oRng = docx.Bookmarks.get_Item(ref oEndOfDoc).Range;
                para = docx.Content.Paragraphs.Add(ref oRng);
                para.Range.InsertParagraphBefore();
                para.Range.InsertParagraphBefore();
                para.Range.Text = "Заявки, в процессе выпоолнения и ожидания подтверждения - Отсутствуют.";
                para.Range.Font.Bold = 0;
                para.Range.Font.Name = "Times New Roman";
                para.Range.Font.Size = 14;
                para.Format.SpaceAfter = 0;
                para.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para.Range.InsertParagraphAfter();
            }
            else
            {
                Word.Paragraph para;
                oRng = docx.Bookmarks.get_Item(ref oEndOfDoc).Range;
                para = docx.Content.Paragraphs.Add(ref oRng);
                para.Range.InsertParagraphBefore();
                para.Range.InsertParagraphBefore();
                para.Range.Text = "Таблица 2 - Заявки, в процессе выполнения и ожидания подтверждения.";
                para.Range.Font.Bold = 0;
                para.Range.Font.Name = "Times New Roman";
                para.Range.Font.Size = 14;
                para.Format.SpaceAfter = 0;
                para.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;

                //сама таблица
                Word.Table oTable;
                wrdRng = docx.Bookmarks.get_Item(ref oEndOfDoc).Range;
                oTable = docx.Tables.Add(wrdRng, arr[2].Count + 1, 6, ref oMissing, ref oMissing);
                oTable.Range.Font.Size = 12;
                //границы таблицы
                oTable.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                oTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                oTable.Columns[1].Width = 20;
                oTable.Columns[2].Width = 80;
                oTable.Columns[3].Width = 100;
                oTable.Columns[4].Width = 100;
                oTable.AllowAutoFit = true;  //выравнивание по ширине???
                oTable.AutoFitBehavior(Word.WdAutoFitBehavior.wdAutoFitWindow); // вот это точно выравнивание по ширине окна ворда
                for (r = 1; r <= arr[2].Count + 1; r++)
                {
                    if (r == 1)
                    {
                        oTable.Cell(r, 1).FitText = true; //расширение при добавлении текста
                        oTable.Cell(r, 1).Range.Text = "№";
                        oTable.Cell(r, 2).Range.Text = "Дата отправки";
                        oTable.Cell(r, 3).Range.Text = "Заявитель";
                        oTable.Cell(r, 4).Range.Text = "Исполнитель";
                        oTable.Cell(r, 5).Range.Text = "Проблема";
                        oTable.Cell(r, 6).Range.Text = "Описание";
                        for (int tmp = 1; tmp <= 6; tmp++)
                        {
                            oTable.Cell(r, tmp).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                        }
                        continue;
                    }
                    oTable.Rows[r - 1].Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    oTable.Cell(r, 1).Range.Text = arr[2][r - 2].Id.ToString();
                    oTable.Cell(r, 2).Range.Text = arr[2][r - 2].Date.ToString();
                    oTable.Cell(r, 3).Range.Text = arr[2][r - 2].UserName.ToString();
                    oTable.Cell(r, 4).Range.Text = arr[2][r - 2].AdminName.ToString();
                    oTable.Cell(r, 5).Range.Text = arr[2][r - 2].Problem.ToString();
                    oTable.Cell(r, 6).Range.Text = arr[2][r - 2].Description.ToString();
                }
                oTable.Rows[1].Range.Font.Bold = 1;
                oTable.Rows[1].Alignment = Word.WdRowAlignment.wdAlignRowCenter;
            }
            #endregion

            #region Таблица заявок в процесе подтверждения специалистом
            //параграф (заголовок)
            if (arr[1].Count == 0)
            {
                Word.Paragraph para;
                oRng = docx.Bookmarks.get_Item(ref oEndOfDoc).Range;
                para = docx.Content.Paragraphs.Add(ref oRng);
                para.Range.InsertParagraphBefore();
                para.Range.InsertParagraphBefore();
                para.Range.Text = "Заявки, в процессе подтверждения специалистом - Отсутствуют.";
                para.Range.Font.Bold = 0;
                para.Range.Font.Name = "Times New Roman";
                para.Range.Font.Size = 14;
                para.Format.SpaceAfter = 0;
                para.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para.Range.InsertParagraphAfter();
            }
            else
            {
                Word.Paragraph para;
                oRng = docx.Bookmarks.get_Item(ref oEndOfDoc).Range;
                para = docx.Content.Paragraphs.Add(ref oRng);
                para.Range.InsertParagraphBefore();
                para.Range.InsertParagraphBefore();
                para.Range.Text = "Таблица 3 - Заявки, в процессе принятия специалистом.";
                para.Range.Font.Bold = 0;
                para.Range.Font.Name = "Times New Roman";
                para.Range.Font.Size = 14;
                para.Format.SpaceAfter = 0;
                para.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;

                //сама таблица
                Word.Table oTable;
                wrdRng = docx.Bookmarks.get_Item(ref oEndOfDoc).Range;
                oTable = docx.Tables.Add(wrdRng, arr[1].Count + 1, 5, ref oMissing, ref oMissing);
                oTable.Range.Font.Size = 12;
                //границы таблицы
                oTable.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                oTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                oTable.Columns[1].Width = 20;
                oTable.Columns[2].Width = 80;
                oTable.Columns[3].Width = 100;
                oTable.Columns[4].Width = 100;
                oTable.Columns[5].Width = 300;
                oTable.AllowAutoFit = true;  //выравнивание по ширине???
                oTable.AutoFitBehavior(Word.WdAutoFitBehavior.wdAutoFitWindow); // вот это точно выравнивание по ширине окна ворда
                for (r = 1; r <= arr[1].Count + 1; r++)
                {
                    if (r == 1)
                    {
                        oTable.Cell(r, 1).FitText = true; //расширение при добавлении текста
                        oTable.Cell(r, 1).Range.Text = "№";
                        oTable.Cell(r, 2).Range.Text = "Дата отправки";
                        oTable.Cell(r, 3).Range.Text = "Заявитель";
                        oTable.Cell(r, 4).Range.Text = "Проблема";
                        oTable.Cell(r, 5).Range.Text = "Описание";
                        for (int tmp = 1; tmp <= 5; tmp++)
                        {
                            oTable.Cell(r, tmp).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                        }
                        continue;
                    }
                    oTable.Rows[r - 1].Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    oTable.Cell(r, 1).Range.Text = arr[1][r - 2].Id.ToString();
                    oTable.Cell(r, 2).Range.Text = arr[1][r - 2].Date.ToString();
                    oTable.Cell(r, 3).Range.Text = arr[1][r - 2].UserName.ToString();
                    oTable.Cell(r, 4).Range.Text = arr[1][r - 2].Problem.ToString();
                    oTable.Cell(r, 5).Range.Text = arr[1][r - 2].Description.ToString();
                }
                oTable.Rows[1].Range.Font.Bold = 1;
                oTable.Rows[1].Alignment = Word.WdRowAlignment.wdAlignRowCenter;
            }
            #endregion

            #region Таблица заявок требующие ремонт или закупку
            //параграф (заголовок)
            if (arr[1].Count == 0)
            {
                Word.Paragraph para;
                oRng = docx.Bookmarks.get_Item(ref oEndOfDoc).Range;
                para = docx.Content.Paragraphs.Add(ref oRng);
                para.Range.InsertParagraphBefore();
                para.Range.InsertParagraphBefore();
                para.Range.Text = "Заявки, требующие ремонт или закупку - Отсутствуют.";
                para.Range.Font.Bold = 0;
                para.Range.Font.Name = "Times New Roman";
                para.Range.Font.Size = 14;
                para.Format.SpaceAfter = 0;
                para.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para.Range.InsertParagraphAfter();
            }
            else
            {
                Word.Paragraph para;
                oRng = docx.Bookmarks.get_Item(ref oEndOfDoc).Range;
                para = docx.Content.Paragraphs.Add(ref oRng);
                para.Range.InsertParagraphBefore();
                para.Range.InsertParagraphBefore();
                para.Range.Text = "Таблица 4 - Заявки, требующие ремонт или закупку.";
                para.Range.Font.Bold = 0;
                para.Range.Font.Name = "Times New Roman";
                para.Range.Font.Size = 14;
                para.Format.SpaceAfter = 0;
                para.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                //сама таблица
                Word.Table oTable;
                wrdRng = docx.Bookmarks.get_Item(ref oEndOfDoc).Range;
                oTable = docx.Tables.Add(wrdRng, arr[4].Count + 1, 7, ref oMissing, ref oMissing);
                oTable.Range.Font.Size = 12;
                //границы таблицы
                oTable.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                oTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                oTable.Columns[1].Width = 20;
                oTable.Columns[2].Width = 80;
                oTable.Columns[3].Width = 100;
                oTable.Columns[6].Width = 100;
                oTable.AllowAutoFit = true;  //выравнивание по ширине???
                oTable.AutoFitBehavior(Word.WdAutoFitBehavior.wdAutoFitWindow); // вот это точно выравнивание по ширине окна ворда
                for (r = 1; r <= arr[4].Count + 1; r++)
                {
                    if (r == 1)
                    {
                        oTable.Cell(r, 1).FitText = true; //расширение при добавлении текста
                        oTable.Cell(r, 1).Range.Text = "№";
                        oTable.Cell(r, 2).Range.Text = "Дата отправки";
                        oTable.Cell(r, 3).Range.Text = "Заявитель";
                        oTable.Cell(r, 4).Range.Text = "Проблема";
                        oTable.Cell(r, 5).Range.Text = "Описание";
                        oTable.Cell(r, 6).Range.Text = "Исполнитель";
                        oTable.Cell(r, 7).Range.Text = "Комментарий";
                        for (int tmp = 1; tmp <= 7; tmp++)
                        {
                            oTable.Cell(r, tmp).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                        }
                        continue;
                    }
                    oTable.Rows[r - 1].Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    oTable.Cell(r, 1).Range.Text = arr[4][r - 2].Id.ToString();
                    oTable.Cell(r, 2).Range.Text = arr[4][r - 2].Date.ToString();
                    oTable.Cell(r, 3).Range.Text = arr[4][r - 2].UserName.ToString();
                    oTable.Cell(r, 4).Range.Text = arr[4][r - 2].Problem.ToString();
                    oTable.Cell(r, 5).Range.Text = arr[4][r - 2].Description.ToString();
                    oTable.Cell(r, 6).Range.Text = arr[4][r - 2].AdminName.ToString();
                    oTable.Cell(r, 7).Range.Text = arr[4][r - 2].AdminComment.ToString();
                }
                oTable.Rows[1].Range.Font.Bold = 1;
                oTable.Rows[1].Alignment = Word.WdRowAlignment.wdAlignRowCenter;
            }
            #endregion

            #region Таблица заявок отменённые пользователем
            //параграф (заголовок)
            if (arr[1].Count == 0)
            {
                Word.Paragraph para;
                oRng = docx.Bookmarks.get_Item(ref oEndOfDoc).Range;
                para = docx.Content.Paragraphs.Add(ref oRng);
                para.Range.InsertParagraphBefore();
                para.Range.InsertParagraphBefore();
                para.Range.Text = "Заявки, отменённые пользователем - Отсутствуют.";
                para.Range.Font.Bold = 0;
                para.Range.Font.Name = "Times New Roman";
                para.Range.Font.Size = 14;
                para.Format.SpaceAfter = 0;
                para.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para.Range.InsertParagraphAfter();
            }
            else
            {
                Word.Paragraph para;
                oRng = docx.Bookmarks.get_Item(ref oEndOfDoc).Range;
                para = docx.Content.Paragraphs.Add(ref oRng);
                para.Range.InsertParagraphBefore();
                para.Range.InsertParagraphBefore();
                para.Range.Text = "Таблица 4 - Заявки отменённые пользователем.";
                para.Range.Font.Bold = 0;
                para.Range.Font.Name = "Times New Roman";
                para.Range.Font.Size = 14;
                para.Format.SpaceAfter = 0;
                para.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                //сама таблица
                Word.Table oTable;
                wrdRng = docx.Bookmarks.get_Item(ref oEndOfDoc).Range;
                oTable = docx.Tables.Add(wrdRng, arr[5].Count + 1, 5, ref oMissing, ref oMissing);
                oTable.Range.Font.Size = 12;
                //границы таблицы
                oTable.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                oTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                oTable.Columns[1].Width = 20;
                oTable.Columns[2].Width = 80;
                oTable.Columns[3].Width = 100;
                oTable.AllowAutoFit = true;  //выравнивание по ширине???
                oTable.AutoFitBehavior(Word.WdAutoFitBehavior.wdAutoFitWindow); // вот это точно выравнивание по ширине окна ворда
                for (r = 1; r <= arr[5].Count + 1; r++)
                {
                    if (r == 1)
                    {
                        oTable.Cell(r, 1).FitText = true; //расширение при добавлении текста
                        oTable.Cell(r, 1).Range.Text = "№";
                        oTable.Cell(r, 2).Range.Text = "Дата отправки";
                        oTable.Cell(r, 3).Range.Text = "Заявитель";
                        oTable.Cell(r, 4).Range.Text = "Проблема";
                        oTable.Cell(r, 5).Range.Text = "Описание";
                        for (int tmp = 1; tmp <= 5; tmp++)
                        {
                            oTable.Cell(r, tmp).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                        }
                        continue;
                    }
                    oTable.Rows[r - 1].Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    oTable.Cell(r, 1).Range.Text = arr[5][r - 2].Id.ToString();
                    oTable.Cell(r, 2).Range.Text = arr[5][r - 2].Date.ToString();
                    oTable.Cell(r, 3).Range.Text = arr[5][r - 2].UserName.ToString();
                    oTable.Cell(r, 4).Range.Text = arr[5][r - 2].Problem.ToString();
                    oTable.Cell(r, 5).Range.Text = arr[5][r - 2].Description.ToString();
                }
                oTable.Rows[1].Range.Font.Bold = 1;
                oTable.Rows[1].Alignment = Word.WdRowAlignment.wdAlignRowCenter;
            }
            #endregion
            //Close this form.
            try
            {
                docx.Close();
                logText.Foreground = Brushes.Green;
                logText.Text = "Документ успешно сохранён!";
            }
            catch
            {
                docx.Close(Word.WdSaveOptions.wdDoNotSaveChanges, Word.WdOriginalFormat.wdOriginalDocumentFormat, false);
                logText.Foreground = Brushes.Gray;
                logText.Text = "Отмена сохранения документа!";
            }
            app.Quit();
        }
        async void CreateDocxEvent()
        {
            DateTime start = dateStartReportEvent.SelectedDate.Value;
            DateTime end = dateEndReportEvent.SelectedDate.Value;
            var events = new List<Event>();
            IAdmin admin = new CurrentUser();
            events = await admin.GetEventsForStatistic(start, end);
            if (events.Count == 0)
            {
                logTextEvent.Foreground = Brushes.Red;
                logTextEvent.Text = "За выбранный промежуток времени мероприятий НЕТ!";
                return;
            }
            if (!cancelEvent.IsChecked.Value) //удаляем отменённые мероприятия если отменили
            {
                for (int i = 0; i < events.Count; ++i)
                {
                    if (events[i].IsCanceled)
                    {
                        events.Remove(events[i]);
                        --i;
                    }
                }
            }
            #region Create .Docx File for event
            // неведомая хрень
            object oMissing = Missing.Value;
            object oEndOfDoc = "\\endofdoc";
            Word.Application app;
            Word.Document docx;
            Word.Range wrdRng;
            // открытие приложния
            try
            {
                app = new Word.Application
                {
                    Visible = false, // отображение хода заполнения
                };
                docx = app.Documents.Add();
            }
            catch (Exception ex)
            {
                logTextEvent.Foreground = Brushes.Red;
                logTextEvent.Text = "Ошибка генерации! \n" + ex.Message;
                return;
            }
            //изменение ориентации листа
            docx.PageSetup.Orientation = Word.WdOrientation.wdOrientLandscape;
            docx.PageSetup.TopMargin = 15;
            docx.PageSetup.BottomMargin = 15;
            docx.PageSetup.RightMargin = 25;
            docx.PageSetup.LeftMargin = 25;

            // начинаем создавать док
            Word.Paragraph para1;
            para1 = docx.Content.Paragraphs.Add(ref oMissing);
            para1.Range.Font.Name = "Times New Roman";
            para1.Range.Font.Size = 16;
            para1.Range.Text = "Отчёт о мероприятиях от " + DateTime.Now.ToShortDateString();
            para1.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            para1.Range.Font.Bold = 1;
            para1.Range.InsertParagraphAfter();
            para1.Range.Font.Name = "Times New Roman";
            para1.Range.Font.Size = 16;
            para1.Range.Font.Bold = 0;
            if ((bool)cancelEvent.IsChecked)
            {
                para1.Range.Text = "Выбранный диапазон с " + start.Date.ToShortDateString() + " по " + end.Date.ToShortDateString() + ". (С учётом отменённых заявок)";
            }
            else
            {
                para1.Range.Text = "Выбранный диапазон с " + start.Date.ToShortDateString() + " по " + end.Date.ToShortDateString() + ".";
            }
            para1.Range.InsertParagraphAfter();
            para1.CloseUp();

            //Insert a paragraph at the end of the document.
            Word.Paragraph para2;
            object oRng = docx.Bookmarks.get_Item(ref oEndOfDoc).Range;
            para2 = docx.Content.Paragraphs.Add(ref oRng);
            para2.Range.Font.Bold = 0;
            para2.Range.Font.Name = "Times New Roman";
            para2.Range.Font.Size = 14;
            para2.Format.SpaceAfter = 0;
            para2.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            para2.Range.InsertParagraphAfter();
            para2.Range.Text = "Таблица Мероприятий.";

            Word.Table oTable;
            wrdRng = docx.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oTable = docx.Tables.Add(wrdRng, events.Count, 6, ref oMissing, ref oMissing);
            oTable.Range.Font.Size = 12;
            oTable.Range.Font.Bold = 0;
            //границы таблицы
            oTable.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            oTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            oTable.Columns[1].Width = 20;
            oTable.Columns[2].Width = 60;
            oTable.Columns[3].Width = 80;
            oTable.Columns[5].Width = 150; // пробуем ограничить длинну
            oTable.Columns[6].Width = 60;
            oTable.AllowAutoFit = true;  //выравнивание по ширине???
            oTable.AutoFitBehavior(Word.WdAutoFitBehavior.wdAutoFitWindow); // вот это точно выравнивание по ширине окна ворда
            int r = 0;
          
            foreach (var eve in events)
            {
                ++r;
                if (r == 1)
                {
                    oTable.Cell(r, 1).FitText = true; //расширение при добавлении текста
                    oTable.Cell(r, 1).Range.Text = "№";
                    oTable.Cell(r, 2).Range.Text = "Дата мероприятия";
                    oTable.Cell(r, 3).Range.Text = "Заявитель";
                    oTable.Cell(r, 4).Range.Text = "Название";
                    oTable.Cell(r, 5).Range.Text = "Описание";
                    oTable.Cell(r, 6).Range.Text = "Дата заявки";
                    for (int tmp = 1; tmp <= 6; tmp++)
                    {
                        oTable.Cell(r, tmp).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    }
                    oTable.Rows[r].Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    continue;
                }
               
                oTable.Rows[r].Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                oTable.Cell(r, 1).Range.Text = eve.Id.ToString();
                oTable.Cell(r, 2).Range.Text = eve.DateStart.ToShortDateString() + "\n c " + eve.DateStart.TimeOfDay.ToString() + " по " + eve.DateEnd.TimeOfDay.ToString();
                oTable.Cell(r, 3).Range.Text = eve.UserName.ToString();
                oTable.Cell(r, 4).Range.Text = eve.NameEvent.ToString();
                oTable.Cell(r, 5).Range.Text = eve.Description.ToString();
                oTable.Cell(r, 6).Range.Text = eve.Date.ToString();
                if (eve.IsCanceled)
                {
                    oTable.Cell(r, 6).Range.Text += "Отменено";
                }
            }
            oTable.Rows[1].Range.Font.Bold = 1;
            oTable.Rows[1].Alignment = Word.WdRowAlignment.wdAlignRowCenter;
            //Close this form.
            try
            {
                docx.Close();
                logTextEvent.Foreground = Brushes.Green;
                logTextEvent.Text = "Документ успешно сохранён!";
            }
            catch
            {
                docx.Close(Word.WdSaveOptions.wdDoNotSaveChanges, Word.WdOriginalFormat.wdOriginalDocumentFormat, false);
                logTextEvent.Foreground = Brushes.Gray;
                logTextEvent.Text = "Отмена сохранения документа!";
            }
            app.Quit();
        }
        #endregion
    }
    #endregion
}
    #endregion

  