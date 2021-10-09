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


        #region Window
        public MainWindow()
        {
            InitializeComponent();
            ShowListPage();
            Instance = this;
        }

        private void TitleBarMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                if (TitleLabel.IsMouseOver)
                    DragMove();
        }

        private void Close_Click(object sender, MouseButtonEventArgs e) => Close();
        #endregion

        #region Navigation 
        public async void ShowViewPage(string title, string url, string cover)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                MainFrame.Navigate(new ViewPage(title, url, cover));
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

        /// <summary>
        /// Clears the MainFrame from previous pages.
        /// </summary>
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
        #endregion

        #region Loading Screen
        public void ShowLoad()
           => LoadingGrid.Visibility = Visibility.Visible;

        public void HideLoad()
           => LoadingGrid.Visibility = Visibility.Hidden;
        #endregion
    }
}
