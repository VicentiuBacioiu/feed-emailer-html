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

            SyndicationFeed feed = DownloadFeed(feedURL);

            foreach (SyndicationItem item in feed.Items)
            {
                DateTimeOffset date = GetFeedDate(item);

                //check if item is older than the specified period
                if ((DateTime.Now - date).TotalMinutes > config.NewerThanMinutes)
                {
                    break;
                }

                string feedContent = ExtractContent(item);
                if (feedContent == null)
                {
                    continue;
                }

                feedItems.Add(new FeedItem
                    {
                        Title = item.Title.Text,
                        Content = feedContent
                    });
            }

            return feedItems;
        }

        private static SyndicationFeed DownloadFeed(string feedURL)
        {
            try
            {
                XmlReader reader = XmlReader.Create(feedURL);
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                reader.Close();
                return feed;
            }
            catch (WebException)
            {
                throw;
            }
            catch (Exception)
            {
                return new SyndicationFeed();
            }
        }

        private static DateTimeOffset GetFeedDate(SyndicationItem item)
        {
            try
            {
                var date = item.PublishDate;
                if (date == DateTimeOffset.MinValue)
                {
                    date = item.ElementExtensions.ReadElementExtensions<DateTime>("date", "http://purl.org/dc/elements/1.1/").FirstOrDefault();
                }
                return date;
            }
            catch
            {
                return DateTimeOffset.MinValue;
            }
        }

        private static string ExtractContent(SyndicationItem item)
        {
            try
            {
                string feedContent = item.ElementExtensions.ReadElementExtensions<string>("encoded", "http://purl.org/rss/1.0/modules/content/").FirstOrDefault();

                if (item.Content != null)
                {
                    feedContent = (item.Content as TextSyndicationContent).Text;
                }
                else if (item.Summary != null)
                {
                    feedContent = item.Summary.Text;
                }

                if (string.IsNullOrEmpty(feedContent))
                {
                    return null;
                }

                feedContent = ProcessUnsupportedCharacters(feedContent);
                feedContent = ProcessImages(feedContent);
                return feedContent;
            }
            catch
            {
                return null;
            }
        }

        static string ProcessImages(string feedContent)
        {
            var imgRegEx = @"<img[^>]*src=""([^""]*)""";
            var regEx = new Regex(imgRegEx);
            var matches = regEx.Matches(feedContent);

            var images = matches.Cast<Match>().Select(match => match.Success ? match.Groups[1].Value : null).Where(img => img != null);

            foreach (var image in images)
            {
                feedContent = feedContent.Replace(image, ImageProcessor.ConvertImageToBase64(image));
            }

            return feedContent;
        }

        static string ProcessUnsupportedCharacters(string feedContent)
        {
            var unsupportedRegEx = @"&#x[^;]+;";
            var regEx = new Regex(unsupportedRegEx);

            return regEx.Replace(feedContent, string.Empty);
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
