using HtmlAgilityPack;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static SirDownload.WebParser;

namespace SirDownload
{
    /// <summary>
    /// Interaction logic for ViewPage.xaml
    /// </summary>
    public partial class ViewPage : Page
    {
        public ViewPage(string title, string url, string cover)
        {
            InitializeComponent();
            TitleText.Content = title;

            GetData(url, cover);
        }

        private async void GetData(string url, string cover)
        {
            HtmlWeb web = new HtmlWeb();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var htmlDoc = await web.LoadFromWebAsync(url);

            GetLinks(htmlDoc);
            GetInfo(htmlDoc, cover);
        }

        private async void GetInfo(HtmlDocument htmlDoc, string cover)
        {
            await Task.Run(() =>
            {
                FullInfo info = new FullInfo(htmlDoc, cover);

                Dispatcher.InvokeAsync(() =>
                {
                    DescriptionBox.Text = info.Description;

                    if (!string.IsNullOrEmpty(cover))
                        CoverImage.Source = new BitmapImage(new Uri(info.CoverURL));

                    RequirementsBox.Text = info.SystemRequirements;
                });
            });

            MainWindow.Instance.HideLoad();
        }

        private async void GetLinks(HtmlDocument htmlDoc)
        {
            foreach (var item in await WebParser.GetLinks(htmlDoc))
            {
                int i = DownloadPanel.Children.Add(new DownloadItem(item.Provider, item.Link));
                DownloadPanel.Children[i].MouseDown += Download_Click;
            }
        }

        private void Back_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MainWindow.Instance.ShowListPage();
        }

        private void Download_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DownloadItem item = (DownloadItem)sender;

            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "cmd";
            info.Arguments = "/c start " + item.DownloadLink;
            info.CreateNoWindow = true;
            info.WindowStyle = ProcessWindowStyle.Hidden;

            Process.Start(info);
        }
    }
}
