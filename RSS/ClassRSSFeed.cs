using NLog;
using System.Net;
using System.Xml;

namespace RSSrider.RSS
{
    public class ClassRSSFeed
    {
        private string title;
        private string link;
        private string status;
        private string description;
        private string rssUrl;
        private string timer;
        private List<ClassRSSItem> items;

        Logger LOG = LogManager.GetCurrentClassLogger();

        public string Title { get { return title; } }
        public string Link { get { return link; } }
        public string Status { get { return status; } }
        public string Description { get { return description; } }
        public string RssUrl { get { return rssUrl; } }
        public string Timer { get { return timer; } }
        public List<ClassRSSItem> Items { get { return items; } }

        public ClassRSSFeed(
            string title, 
            string link, 
            string timer, 
            string status, 
            string description, 
            string rssUrl, 
            List<ClassRSSItem> items)
        {
            this.status = status;
            this.title = title;
            this.link = link;
            this.rssUrl = rssUrl;
            this.timer = timer;
            this.description = description;
            this.items = items;
        }
        public ClassRSSFeed(string url, string feedstatus)
        {
            items = new();
            this.title = "No channel";
            this.link = "";
            this.status = feedstatus;
            this.rssUrl = url;
            this.timer = "";
            this.description = "";         
            try
            {
                XmlTextReader xmlTextReader = new(url);
                XmlDocument xmlDoc = new();
                xmlDoc.Load(xmlTextReader);
                xmlTextReader.Close();
                XmlNode? channelRootNode = xmlDoc.GetElementsByTagName("channel")[0];
                if (channelRootNode == null) { throw new WebException("Ошибка XML файла! Канал не найден"); }
                foreach (XmlNode channelNode in channelRootNode.ChildNodes)
                {
                    switch (channelNode.Name)
                    {
                        case "title":
                            {
                                this.title = channelNode.InnerText;
                                break;
                            }
                        case "description":
                            {
                                this.description = channelNode.InnerText;
                                break;
                            }
                        case "link":
                            {
                                this.link = channelNode.InnerText;
                                break;
                            }
                        case "item":
                            {
                                if(feedstatus == "deactivate") { continue; }
                                ClassRSSItem channelItem = new(channelNode);
                                this.Items.Add(channelItem);
                                break;
                            }
                    }
                }
                xmlTextReader.Close();
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
