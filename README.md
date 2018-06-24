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
- User: Your username



### Privacy
Kindle Article Subscriber does NOT log any data, does not call home, check for updates or send/use configuration data to any service. Check it out yourself in the source code.
