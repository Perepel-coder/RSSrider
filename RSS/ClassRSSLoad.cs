using NLog;
using System.Net;
using System.Xml;

namespace RSSrider.RSS
{
    public static class ClassRSSLoad
    {
        private static Logger LOG = LogManager.GetCurrentClassLogger();
        public static List<ClassRSSFeed> RSSFeed(List<string[]> urls)
        {
            List<ClassRSSFeed> feeds = new();
            try
            {
                for(int i = 0; i < urls.Count; i++)
                {
                    feeds.Add(new(urls[i][0], urls[i][1]));
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                LOG.Error("Статусный код ошибки: {0} - {1}",
                (int)httpResponse.StatusCode, httpResponse.StatusCode);
            }
            return feeds;
        }

        public static async Task<IEnumerable<ClassRSSFeed>> RssUrls()
        {
            var swc = Task.Factory.StartNew(() =>
            {
                List<ClassRSSFeed> feeds = new();
                try
                {
                    List<string[]> rssUrls = new();
                    XmlDocument xmlDoc = new();
                    xmlDoc.Load(@"settings.xml");
                    XmlNode? feedsRootNode = xmlDoc.GetElementsByTagName("feeds")[0];
                    if (feedsRootNode == null) { throw new Exception("Файл настроек не найден"); }

                    for (int i = 0; i < feedsRootNode.ChildNodes.Count; i++)
                    {
                        rssUrls.Add(new string[2]);
                        var feedNode = feedsRootNode.ChildNodes[i];
                        if (feedNode.Name == "feed")
                        {
                            string url = feedNode.Attributes["url"].InnerText;
                            if (url == null) { throw new Exception("RSS ленты не найдены"); }
                            rssUrls[i][0] = url;
                            rssUrls[i][1] = feedNode.Attributes["status"].InnerText;
                        }
                    }
                    feeds = RSSFeed(rssUrls);
                }
                catch (WebException ex)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                    LOG.Error("Статусный код ошибки: {0} - {1}",
                    (int)httpResponse.StatusCode, httpResponse.StatusCode);
                }
                return feeds;
            });
            return await swc;
        }

        public static async Task<ClassRSSFeed> GetFeed(string url)
        {
            var swc = Task.Factory.StartNew(() =>
            {
                ClassRSSFeed feed = new(url, "activate");
                return feed;
            });
            return await swc;
        }

        public static void AddFeed(string url)
        {
            try
            {
                XmlTextReader xmlTextReader = new(url);
                XmlDocument xmlDoc = new();
                xmlDoc.Load(xmlTextReader);
                xmlTextReader.Close();

                xmlDoc.Load(@"settings.xml");
                XmlNode? feedsRootNode = xmlDoc.GetElementsByTagName("feeds")[0];
                if (feedsRootNode == null) { throw new Exception("Файл настроек не найден"); }

                foreach (XmlNode feedNode in feedsRootNode.ChildNodes)
                {
                    if (feedNode.Name == "feed" && feedNode.Attributes["url"].Value == url)
                    {
                        throw new Exception("Канал уже записан");
                    }
                }

                XmlElement feedElem = xmlDoc.CreateElement("feed");
                XmlAttribute urlAttr = xmlDoc.CreateAttribute("url");
                XmlAttribute statusAttr = xmlDoc.CreateAttribute("status");

                urlAttr.AppendChild(xmlDoc.CreateTextNode(url));
                statusAttr.AppendChild(xmlDoc.CreateTextNode("activate"));
                feedElem.Attributes.Append(urlAttr);
                feedElem.Attributes.Append(statusAttr);
                feedsRootNode.AppendChild(feedElem);
                xmlDoc.Save(@"settings.xml");
            }
            catch (WebException ex)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                LOG.Error("Статусный код ошибки: {0} - {1}",
                (int)httpResponse.StatusCode, httpResponse.StatusCode);
            }
        }

        public static async Task<bool> DelFeed(string[] urls)
        {
            var swc = Task.Factory.StartNew(() =>
            {
                try
                {
                    XmlDocument xmlDoc = new();
                    xmlDoc.Load(@"settings.xml");
                    XmlNode? feedsRootNode = xmlDoc.GetElementsByTagName("feeds")[0];
                    if (feedsRootNode == null) { throw new Exception("Файл настроек не найден"); }
                    foreach (string url in urls)
                    {
                        for (int i = 0; i < feedsRootNode.ChildNodes.Count; i++)
                        {
                            var feed = feedsRootNode.ChildNodes[i];
                            if (feed.Name == "feed" && feed.Attributes["url"].Value == url)
                            {
                                feedsRootNode.RemoveChild(feed); break;
                            }
                        }
                    }
                    xmlDoc.Save(@"settings.xml");
                    return true;
                }
                catch (WebException ex)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                    LOG.Error("Статусный код ошибки: {0} - {1}",
                    (int)httpResponse.StatusCode, httpResponse.StatusCode);
                    return false;
                }

            });
            return await swc;

            
        }

        public static async Task<bool> ChangeStatus(string[] urls)
        {
            var swc = Task.Factory.StartNew(() =>
            {
                try
                {
                    XmlDocument xmlDoc = new();
                    xmlDoc.Load(@"settings.xml");
                    XmlNode? feedsRootNode = xmlDoc.GetElementsByTagName("feeds")[0];
                    if (feedsRootNode == null) { throw new Exception("Файл настроек не найден"); }
                    foreach (string url in urls)
                    {
                        for (int i = 0; i < feedsRootNode.ChildNodes.Count; i++)
                        {
                            var feed = feedsRootNode.ChildNodes[i];
                            if (feed.Name == "feed" && feed.Attributes["url"].Value == url)
                            {
                                if (feed.Attributes["status"].Value == "activate")
                                {
                                    feed.Attributes["status"].Value = "deactivate";
                                    continue;
                                }
                                if (feed.Attributes["status"].Value == "deactivate")
                                {
                                    feed.Attributes["status"].Value = "activate";
                                }
                            }
                        }
                    }
                    xmlDoc.Save(@"settings.xml");
                    return true;
                }
                catch (WebException ex)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                    LOG.Error("Статусный код ошибки: {0} - {1}",
                    (int)httpResponse.StatusCode, httpResponse.StatusCode);
                    return false;
                }

            });
            return await swc;
        }

        public static async Task<string> RSSTimer()
        {
            var swc = Task.Factory.StartNew(() =>
            {
                string timer = "";
                XmlDocument xmlDoc = new();
                xmlDoc.Load(@"settings.xml");
                XmlNode? timerRootNode = xmlDoc.GetElementsByTagName("timers")[0];
                foreach (XmlNode timerNode in timerRootNode.ChildNodes)
                {
                    switch (timerNode.Name)
                    {
                        case "timer":
                            {
                               timer =  timerNode.Attributes["value"].InnerText;
                               break;
                            }
                       
                    }
                }
                return timer;
            });
            return await swc;
        }

    }
}
