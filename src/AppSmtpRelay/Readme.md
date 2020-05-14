# AppSmtpRelay

Proxies smtp emails to an AppMailRelay, so you don't have to mess with network plumbing.  Just point your apps that support SMTP to your AppSmtpRelay.

A typical scenario is where you've got apps running in various clouds, but want to pipe email messages back to your on-premises mail server.  So you run AppMailRelay on-prem, and AppSmtpRelay send messages via web-requests to it's API.  In turn, the AppMailRelay sends those message to the on-prem mail server via SMTP.

### Settings

You'll need to set the url and apikey for the destination AppMailRelay.  Settings can be environment variables or command line.

* -u, --url, APPRELAY_URL
* -k, --key, APPRELAY_KEY
* -p, --port, APPRELAY_PORT (default is to listen on tcp/25)
* APPRELAY_CC | always append these mailto addresses ( delimit with `;`)
* APPRELAY_BCC | always append these mailto addresses ( delimit with `;`)
