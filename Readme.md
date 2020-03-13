# AppMailRelay

Proxies web-api messages to smtp emails so apps don't have to mess with SMTP.  Just fire api messages to the AppMailRelay, and let it worry about SMTP.

## SysAdmins

Run the application (`dotnet AppMailRelay.dll`) to start proxying messages between clients and a mail server.

See the [src/AppMailRelay/appsettings.json](src/AppMailRelay/appsettings.json) for settings.

Add client keys to the Relay:ClientKeys array in the form of `client-name#randomsecret`.

When running the docker image, mount a custom `appsettings.Production.json` into `/app`.

## Developers
The API is pretty simple for clients; see the [Readme](src/AppMailRelay/Readme.md) for API documentation.

The AppMailClient is prebuilt to drop in to an app.  See the [Readme](src/AppMailClient/Readme.md).
