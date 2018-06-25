# Kindle Article Subscriber

A simple executable that scans RSS feeds and sends updates directly to your Kindle (or to your inbox).

### Prerequisites
The program needs a SMTP server to be able to send emails.  
Don't have one? You can use your Gmail settings (emails will be sent from your Gmail account)  
Doesn't sound secure? Use two-step verification and generate an app password.  
Feeling uncomfortable using your Gmail credentials? You can create a new account, and use it just for this.

### Kindle Configuration
Download the Release package, unzip it, and open the config.json file using any text editor. Below is each option explained:
- Server: The SMTP email server. Example: mail.google.com
- Port: The email port, default 587
- UseSSL: Set to **true** if you want to use a secure connection, otherwise **false**
- User: The email account username
- Password: The email account password
- ToAddress: Your Kindle email address ([find it here|https://www.amazon.com/gp/help/customer/display.html?nodeId=201974240]) 
- FromAddress: The email address for the sender field
- SendAsAttachment: Must be true to work on Kindle
- NewerThanMinutes: Timeout period to check for new feeds. Only feeds newer than the specified period will be sent via email. Make sure you're using a number big enough to get feeds.
- Feeds: A string array of RSS feed URLs.

**Run the program and wait.**  
The first feeds will start coming after 30 seconds (if the published date is newer than the specified interval).  
The next feeds will come after the specified interval has elapsed.

### Alternate Configuration - Regular Article via Email
The program can be configured to deliver the article to any e-mail address, the content being readable on any device that supports HTML.
Follow the configuration above, but set the "SendAsAttachment" option to false.

### Privacy
Kindle Article Subscriber does NOT log any data, does not call home, check for updates or send/use configuration data to any service.  
You can also take the source code and compile it yourself.

### Issues
If you find any issues, please open an issue in Github, including as many details (and pictures) as possible.

### Contributing
If you want to contribute to this repository, open a pull request.
