using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RSSFeedEmail
{
    static class ImageProcessor
    {
        static string[] IGNORED_IMAGES = new[] { ".svg" };
        const string B64_SRC = "data:image/png;base64,";
        const string EMPTY_IMG = "data:image/gif;base64,R0lGODlhAQABAIABAP///wAAACH5BAEKAAEALAAAAAABAAEAAAICTAEAOw==";

        public static string ConvertImageToBase64(string imageUrl)
        {
            // ignore unsupported images
            if (IGNORED_IMAGES.Any(img => imageUrl.EndsWith(img)))
            {
                return EMPTY_IMG;
            }

            // download image
            var imgBytes = DownloadImage(imageUrl);
            var b64Image = GetBase64(imgBytes);
            return string.Concat(B64_SRC, b64Image);
        }

        static byte[] DownloadImage(string imageUrl)
        {
            var client = new WebClient();
            var stream = client.OpenRead(imageUrl);
            var bitmap = new Bitmap(stream);

            var destStream = new System.IO.MemoryStream();
            bitmap.Save(destStream, System.Drawing.Imaging.ImageFormat.Png);

            stream.Flush();
            stream.Close();
            destStream.Flush();
            destStream.Close();
            client.Dispose();

            return destStream.ToArray();
        }

        static string GetBase64(byte[] imageBytes)
        {           
            return Convert.ToBase64String(imageBytes);
        }
    }
}
