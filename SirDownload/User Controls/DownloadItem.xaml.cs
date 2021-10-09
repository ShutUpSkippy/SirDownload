using System.Windows.Controls;

namespace SirDownload
{
    /// <summary>
    /// Interaction logic for DownloadItem.xaml
    /// </summary>
    public partial class DownloadItem : UserControl
    {
        public string DownloadLink;
        public DownloadItem(string title, string link)
        {
            InitializeComponent();
            NameLabel.Content = title;
            DownloadLink = link;
        }
    }
}
