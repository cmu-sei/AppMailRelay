# AppMailRelay

Proxies web-api messages to smtp emails so apps don't have to mess with SMTP.  Just fire api messages to the AppMailRelay, and let it worry about SMTP.

A typical scenario is where you've got apps running in various clouds, but want to pipe email messages back to your on-premissis mail server.  So you run AppMailRelay on-prem, and any distributed apps send messages via web-requests to it's API.  In turn, the AppMailRelay sends those message to the on-prem mail server via SMTP.

For apps that insist on talking SMTP, you can host an instance of AppSmtpRelay, which accepts SMTP messages and relays them to your on-prem AppMailRelay.  The AppSmtpRelay is dumb, no auth, etc.; designed for accepting traffic from the local network, so don't expose it publicly.

## SysAdmins

Run the application (`dotnet AppMailRelay.dll`) to start proxying messages between clients and a mail server.

See the [src/AppMailRelay/appsettings.json](src/AppMailRelay/appsettings.json) for settings.

Add client keys to the Relay:ClientKeys array in the form of `client-name#randomsecret`.

When running the docker image, mount a custom `appsettings.Production.json` into `/app`.  Or set environment variables à la AspNetCore.

## Developers
The API is pretty simple for clients; see the [Readme](src/AppMailRelay/Readme.md) for API documentation.

The AppMailClient is prebuilt to drop in to an app.  See the [Readme](src/AppMailClient/Readme.md).

### Acknowlegements

This project relies on the great work of these projects:
* [ASP.NET Core](https://github.com/aspnet)
* [MailKit](https://github.com/jstedfast/MailKit)
* [netDumbster](https://github.com/cmendible/netDumbster)
