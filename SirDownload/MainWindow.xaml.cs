using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace SirDownload
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance;
        private ListPage LastListPage = null;
        public MainWindow()
        {
            InitializeComponent();
            ShowListPage();
            Instance = this;
        }

        #region Window Movement
        private void TitleBarMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                if (TitleLabel.IsMouseOver)
                    DragMove();
        }
        #endregion

        private void Close_Click(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        public async void ShowViewPage(string title, string url, string cover)
        {
            await Task.Run(() =>
            {
                Dispatcher.InvokeAsync(() =>
                {
                    MainFrame.Navigate(new ViewPage(title, url, cover));
                });
            });
        }

        public void ShowListPage()
        {
            if (LastListPage != null)
                MainFrame.Navigate(LastListPage);
            else
            {
                LastListPage = new ListPage();
                MainFrame.Navigate(LastListPage);
            }
        }

        public void ClearHistory()
        {
            if (!this.MainFrame.CanGoBack && !this.MainFrame.CanGoForward)
            {
                return;
            }

            var entry = this.MainFrame.RemoveBackEntry();
            while (entry != null)
            {
                entry = this.MainFrame.RemoveBackEntry();
            }

            this.MainFrame.Navigate(new PageFunction<string>() { RemoveFromJournal = true });
        }

        public void ShowLoad()
        {
            LoadingGrid.Visibility = Visibility.Visible;
        }

        public void HideLoad()
        {
            LoadingGrid.Visibility = Visibility.Hidden;
        }
    }
}
