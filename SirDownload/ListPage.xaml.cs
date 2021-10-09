using HtmlAgilityPack;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static SirDownload.WebParser;

namespace SirDownload
{
    /// <summary>
    /// Interaction logic for ListPage.xaml
    /// </summary>
    public partial class ListPage : Page
    {
        public ObservableCollection<CustomListItem> ListItems { get; set; }
        private int PreviousPage = 1;
        private int CurrentPage = 1;
        private int TotalPages = 1;
        public ListPage()
        {
            ListItems = new ObservableCollection<CustomListItem>();
            this.DataContext = this;
            InitializeComponent();
        }

        private void Search_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Search(SearchBox.Text);
                Keyboard.ClearFocus();
            }
        }

        public async void Search(string text, int page = -1, bool update = false)
        {
            ListItems.Clear();
            MainWindow.Instance.ShowLoad();

            //Get htmldoc
            HtmlWeb web = new HtmlWeb();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HtmlDocument htmlDoc;

            if (page == -1)
                htmlDoc = await web.LoadFromWebAsync($@"https://www.skidrowreloaded.com/?s={text}&x=0&y=0");
            else
                htmlDoc = await web.LoadFromWebAsync($@"https://www.skidrowreloaded.com/page/{page}/?s={text}&x=0&y=0");


            //add results
            foreach (QuickInfo s in await WebParser.GetTitles(htmlDoc))
                ListItems.Add(new CustomListItem(s));

            //used for getting button color
            var converter = new System.Windows.Media.BrushConverter();

            //show number of pages
            if (update == false)
            {
                TotalPages = WebParser.GetPages(htmlDoc);
                PagePanel.Visibility = Visibility.Visible;
                PageLabel.Content = "of " + TotalPages.ToString();
            }

            if (ListItems.Count > 0)
                MainListView.ScrollIntoView(ListItems[0]);
            MainWindow.Instance.HideLoad();
        }

        private void Search_Click(object sender, MouseButtonEventArgs e)
        {
            Search(SearchBox.Text);
        }

        private void PageUp(object sender, MouseButtonEventArgs e)
        {
            CurrentPage++;
            UpdatePage();
        }

        private void PageDown(object sender, MouseButtonEventArgs e)
        {
            CurrentPage--;
            UpdatePage();
        }

        private void UpdatePage()
        {
            CurrentPage = Math.Clamp(CurrentPage, 1, TotalPages);
            if (PreviousPage != CurrentPage)
            {
                PageBox.Text = CurrentPage.ToString();
                PreviousPage = CurrentPage;
                Search(SearchBox.Text, CurrentPage, true);
            }
        }

        private void PageBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int result = 1;
                if (int.TryParse(PageBox.Text.Trim(), out result))
                    CurrentPage = result;

                UpdatePage();
                Keyboard.ClearFocus();
            }
        }
    }
}
