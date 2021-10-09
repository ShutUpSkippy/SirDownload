using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static SirDownload.WebParser;

namespace SirDownload
{
    /// <summary>
    /// Interaction logic for CustomListItem.xaml
    /// </summary>
    public partial class CustomListItem : UserControl
    {
        QuickInfo quickInfo;

        public CustomListItem()
        {
            InitializeComponent();
        }

        public CustomListItem(QuickInfo info)
        {
            InitializeComponent();
            TitleLabel.Content = info.GameName;

            try
            {
                CoverImage.Source = new BitmapImage(new Uri(info.CoverURL, UriKind.Absolute));
            }
            catch
            {

                CoverImage.Source = new BitmapImage(new Uri("SirDownload.png", UriKind.Relative));
            }

            DescriptionBox.Text = info.Description;

            quickInfo = info;
        }

        private async void Item_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MainWindow.Instance.ShowLoad();
            await Task.Delay(50);
            MainWindow.Instance.ShowViewPage(quickInfo.GameName, quickInfo.GamePage, quickInfo.CoverURL);
        }
    }
}
