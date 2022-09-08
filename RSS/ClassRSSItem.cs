using NLog;
using System.Net;
using System.Xml;

namespace RSSrider.RSS
{
    public class ClassRSSItem
    {
        private string title;
        private string link;
        private string description;
        private string pubdate;

        Logger LOG = LogManager.GetCurrentClassLogger();

        public string Title { get { return title; } }
        public string Link { get { return link; } }
        public string Description { get { return description; } }
        public string Pubdate { get { return pubdate; } }
        public ClassRSSItem(string title, string link, string description, string date)
        {
            this.title = title;
            this.link = link;
            this.description = description;
            this.pubdate = date;
        }
        public ClassRSSItem(XmlNode item)
        {
            try
            {
                this.title = "No node";
                this.link = "";
                this.description = "";
                this.pubdate = "";
                foreach (XmlNode teg in item.ChildNodes)
                {
                    switch (teg.Name)
                    {
                        case "title":
                            {
                                this.title = teg.InnerText;
                                break;
                            }
                        case "link":
                            {
                                this.link = teg.InnerText;
                                break;
                            }
                        case "description":
                            {
                                this.description = teg.InnerText;
                                break;
                            }
                        case "pubDate":
                            {
                                this.pubdate = teg.InnerText;
                                break;
                            }
                    }
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                LOG.Error("Статусный код ошибки: {0} - {1}",
                (int)httpResponse.StatusCode, httpResponse.StatusCode);
            }
            
        }
    }
}
