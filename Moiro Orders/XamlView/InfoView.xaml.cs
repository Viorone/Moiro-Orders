using System.Windows.Controls;


namespace Moiro_Orders.XamlView
{
    /// <summary>
    /// Логика взаимодействия для Info.xaml
    /// </summary>
    public partial class InfoView : UserControl
    {
        public InfoView()
        {
            InitializeComponent();
            Version.Text = PublicResources.version;
        }
    }
}
