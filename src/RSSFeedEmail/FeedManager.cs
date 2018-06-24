using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace RSSFeedEmail
{
    static class FeedManager
    {
        const string FEED_FILE = "FeedTemplate.html";
        static Settings config = Settings.GetSettings();
        static EmailHelper emailHelper = new EmailHelper(config.Server, config.Port, config.UseSSL, new NetworkCredential(config.User, config.Password));
        static string feedTemplate;

        public static void RunFeeds()
        {
            feedTemplate = File.ReadAllText(FEED_FILE);

            foreach (var feedURL in config.Feeds)
            {
                var feedItems = DownloadFeedAsHTML(feedURL);
                feedItems.ForEach(SendFeedToRecipient);
            }
        }

        static List<FeedItem> DownloadFeedAsHTML(string feedURL)
        {
            List<FeedItem> feedItems = new List<FeedItem>();

            XmlReader reader = XmlReader.Create(feedURL);
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            reader.Close();

            foreach (SyndicationItem item in feed.Items)
            {
                var date = item.PublishDate;

                //check if item is older than the specified period
                if ((DateTime.Now - date).TotalMinutes > config.NewerThanMinutes)
                {
                    break;
                }

                var feedContent = item.ElementExtensions.ReadElementExtensions<string>("encoded", "http://purl.org/rss/1.0/modules/content/").FirstOrDefault();

                feedContent = ProcessImages(feedContent);

                feedItems.Add(new FeedItem
                    {
                        Title = item.Title.Text,
                        Content = string.IsNullOrEmpty(feedContent) ? item.Summary.Text : feedContent
                    });
            }

            return feedItems;
        }

        static string ProcessImages(string feedContent)
        {
            var imgRegEx = @"<img[^>]*src=""([^""]*)""";
            var regEx = new Regex(imgRegEx);
            var matches = regEx.Matches(feedContent);
           
            var images = matches.Cast<Match>().Select(match => match.Success ? match.Groups[1].Value : null).Where(img => img != null);

            foreach(var image in images){
                feedContent = feedContent.Replace(image, ImageProcessor.ConvertImageToBase64(image));
            }

            return feedContent;
        }

        static void SendFeedToRecipient(FeedItem feed)
        {
            string body;
            string attachmentPath;

            string content = string.Format(feedTemplate, feed.Title, feed.Content);

            if (config.SendAsAttachment)
            {
                body = string.Empty;
                attachmentPath = Path.Combine(Path.GetTempPath(), GetSanitizedFileName(feed.Title) + ".html");
                File.WriteAllText(attachmentPath, content, Encoding.UTF8);
            }
            else
            {
                body = content;
                attachmentPath = null;
            }

            emailHelper.SendEmail(new List<string>() { config.ToAddress }, config.FromAddress, feed.Title, body, attachmentPath);
        }

        static string GetSanitizedFileName(string path)
        {
            var invalids = Path.GetInvalidFileNameChars();
            var newName = string.Join("_", path.Split(invalids, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
            return newName;
        }
    }
}
