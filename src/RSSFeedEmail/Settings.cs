using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RSSFeedEmail
{
    public class Settings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public bool UseSSL { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string ToAddress { get; set; }
        public string FromAddress { get; set; }
        public bool SendAsAttachment { get; set; }
        public List<string> Feeds { get; set; }
        public int NewerThanHours { get; set; }

        public static Settings GetSettings()
        {
            var json = Regex.Replace(File.ReadAllText("config.json"), "\r\n", string.Empty);
            Settings stt = JsonConvert.DeserializeObject<Settings>(json);
            return stt;
        }
    }
}
