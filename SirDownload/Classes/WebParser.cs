using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace SirDownload
{
    public class WebParser
    {
        public static async Task<List<QuickInfo>> GetTitles(HtmlDocument htmlDoc)
        {
            var programmerLinks = htmlDoc.DocumentNode.SelectNodes("//div[@class='post']");

            List<QuickInfo> titles = new List<QuickInfo>();

            if (programmerLinks.Where(x => x.HasChildNodes).FirstOrDefault().InnerText.Trim().StartsWith("0 search results"))
                return titles;

            await Task.Run(() =>
            {
                int index = 0;
                foreach (var name in programmerLinks)
                {
                    if (programmerLinks.First() == name)
                        continue;
                    QuickInfo info = new QuickInfo(name, index);
                    if (info.Invalid == false)
                        titles.Add(info);

                    index++;
                }
            });

            return titles;

        }

        public static async Task<List<DownloadProvider>> GetLinks(HtmlDocument htmlDoc)
        {

            List<DownloadProvider> result = new List<DownloadProvider>();

            await Task.Run(() =>
            {
                //get all <strong> items
                var prov = htmlDoc.DocumentNode.SelectNodes("//strong");
                foreach (var item in prov)
                {
                    //skip first (not a download link)
                    if (prov.IndexOf(item) == 0)
                        continue;

                    //get link holder
                    var linkItem = item.ParentNode.ParentNode.Descendants()
                                                          .Where(x => x.Name == "a")
                                                          .FirstOrDefault();
                    if (linkItem == null)
                        continue;

                    //get link
                    string link = linkItem.Attributes[0].Value;

                    //add if available
                    if (link.ToLower().Contains("skidrow") == false)
                        result.Add(new DownloadProvider { Link = link, Provider = item.InnerText });

                }
            });
            return result;
        }

        public static int GetPages(HtmlDocument htmlDoc)
        {
            int pages = 0;

            var lastNode = htmlDoc.DocumentNode.SelectSingleNode("//a[@class='last']");
            if (lastNode == null)
                return 1;
            string last = lastNode.Attributes[0].Value;
            string number = last.Remove(0, 37);
            number = number.Remove(number.IndexOf("/"), number.Length - number.IndexOf("/")).Trim();
            int.TryParse(number, out pages);

            return pages;
        }

        public struct DownloadProvider
        {
            public string Provider;
            public string Link;

            public DownloadProvider(string provider, string link)
            {
                Provider = provider;
                Link = link;
            }
        }

        public struct FullInfo
        {
            public string UploadDate;
            public string Description;
            public string CoverURL;
            public string SystemRequirements;

            public FullInfo(HtmlDocument htmlDoc, string coverUrl)
            {
                //Description
                var descParent = htmlDoc.DocumentNode.SelectSingleNode("//h5");
                Description = descParent.ParentNode.ChildNodes.Where(x => x.Name == "p").FirstOrDefault().InnerText;

                //Upload date
                UploadDate = "";

                //Cover URL
                CoverURL = coverUrl;

                //System requirements
                var reqs = htmlDoc.DocumentNode.SelectNodes("//div")
                                               .Where(x => x.HasAttributes)
                                               .Where(x => x.Attributes[0].Value.StartsWith("tabs-"))
                                               .Where(x => x.Attributes[0].Value.EndsWith("-0-1"))
                                               .FirstOrDefault();
                var desc = reqs.InnerText;
                if (desc.Contains("MINIMUM:") || desc.Contains("MAXIMUM:"))
                    SystemRequirements = desc.Trim().Remove(0, 8).Trim();
                else if (desc.ToUpper().Contains("MINIMUM SYSTEM REQUIREMENTS:") || desc.Contains("MAXIMUM SYSTEM REQUIREMENTS:"))
                    SystemRequirements = desc.Trim().Remove(0, 28).Trim();
                else
                    SystemRequirements = desc.Trim();
            }
        }

        public struct QuickInfo
        {
            public string GameName;
            public string CoverURL;
            public string Description;
            public string GamePage;
            public bool Invalid;

            public QuickInfo(HtmlNode node, int index = 1)
            {
                //Get game name
                string html = node.InnerText.Trim();
                GameName = html.Substring(0, html.IndexOf("\n")).Trim();

                //Get cover image link
                var img = node.Descendants("img").Select(e => e.GetAttributeValue("src", null)).Where(s => !string.IsNullOrEmpty(s));

                CoverURL = "";

                foreach (var item in img)
                {
                    if (item.EndsWith(".jpg") || item.EndsWith(".png"))
                    {
                        CoverURL = item;
                        break;
                    }
                }

                //Get game description 
                var desc = node.SelectNodes("//p[@style='text-align: center;']")
                               .Where(x => x.ChildNodes.Count == 1)
                               .Where(x => x.InnerHtml.Contains("<") == false && x.InnerHtml.Contains(">") == false);

                string rawDesc = desc.ElementAt(index).InnerText;

                while (rawDesc.Contains("&#8230;"))
                    rawDesc = rawDesc.Replace("&#8230;", "..");
                while (rawDesc.Contains("&#8217;"))
                    rawDesc = rawDesc.Replace("&#8217;", "\'");
                while (rawDesc.Contains("&#8221;"))
                    rawDesc = rawDesc.Replace("&#8221;", "\"");

                Description = rawDesc;

                //Game URL
                var url = node.Descendants()
                                          .Where(n => n.HasAttributes)
                                          .Where(n => n.Attributes[0].Value.StartsWith("http"))
                                          .Where(n => n.Attributes[0].Value.Contains("#") == false)
                                          .Where(n => n.Attributes[0].Value.EndsWith("request-accepted/") == false)
                                          .Where(n => n.Attributes[0].Value.StartsWith(@"https://www.skidrowreloaded.com/categor") == false);
                GamePage = url.ElementAt(0).Attributes[0].Value;

                //Check if validity
                var val = node.SelectNodes("//div[@class='meta']");

                Invalid = false;
                foreach (string part in "NEWS;TUTORIAL".Split(';'))
                    if (val.ElementAt(index).InnerText.ToUpper().Contains(part))
                        Invalid = true;

            }
        }
    }
}
