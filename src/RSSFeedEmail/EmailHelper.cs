using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace RSSFeedEmail
{
    class EmailHelper
    {
        SmtpClient MailServer
        {
            get;
            set;
        }

        public EmailHelper(string server, int port, bool useSSL, NetworkCredential credentials)
        {
            MailServer = new SmtpClient(server);
            MailServer.Port = port;
            MailServer.EnableSsl = useSSL;
            MailServer.Credentials = credentials;
        }

        public bool SendEmail(List<string> to, string from, string subject, string body, string attachmentPath)
        {
            try
            {
                MailMessage mail = new MailMessage();

                mail.From = new MailAddress(from);
                to.ForEach(toAddress => mail.To.Add(toAddress));

                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = !string.IsNullOrEmpty(body);

                if (!string.IsNullOrEmpty(attachmentPath))
                {
                    System.Net.Mail.Attachment attachment;
                    attachment = new System.Net.Mail.Attachment(attachmentPath);
                    mail.Attachments.Add(attachment);
                }

                MailServer.Send(mail);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
