using System.Windows;

namespace Moiro_Orders
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        System.Threading.Mutex mut;
        private void App_Startup(object sender, StartupEventArgs e)
        {
            string mutName = "МОИРО.Заявки";
            mut = new System.Threading.Mutex(true, mutName, out bool createdNew);
            if (!createdNew)
            {
                Shutdown();
            }           
        }
    }
}
