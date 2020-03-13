# AppMailClient

AppMailClient is a light-weight client for sending mail to an AppMailRelay server.  If you don't want to mess with SMTP, just post web api calls to the AppMailRelay, which handles the smtp bit.

`Send()` calls are returned with a *pending* status, since the Relay server queues the messages.

That is good enough for many apps, but if confirmation is important, `Status()` can be used to check for a status up to 10 minutes or so after sending.


### Usage

```
# Add package to project
dotnet add package AppMailClient -s <internal-artifactory>
```

```csharp
// Startup.cs -- configure dependency
public void ConfigureServices(IServiceCollection services)
{
    //services.AddAppMailClient(() => Configuration.GetSection("AppMail"); // scoped options
    services.AddAppMailClient(() => Configuration.GetSection("AppMail").Get<AppMailClient.Options>());
}

// Service.cs -- inject dependency
public SomeService(IAppMailClient mailClient) { ... }
```

### Settings
```json
"AppMail": {
  "Url": "https://appmail.relay.local/msg",  # relay endpoint
  "Key": "client#secret", # obtain from relay admin
  "From": "Sender <sender@this.ws>", # optional, default at relay
  "CcRecipients": "one@this.ws; two@this.ws", #optional, additional cc's for every message
  "BccRecipients": "three@this.ws; four@this.ws" #optional, additional bcc's for every message
}
```

Email addresses can be `Display Name <name@this.ws>`  or just `name@this.ws` and are separated by `;`.

Sometimes an app doesn't know/care who the sender is, but still wants to Bcc them.  *MailMessage.BccSender* can be set to true to achieve this.
