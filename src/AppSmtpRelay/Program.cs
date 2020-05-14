// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using netDumbster.smtp;

namespace AppSmtpRelay
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // compose options
            var options = new AppMailClient.Options();
            options.Url = Environment.GetEnvironmentVariable("APPRELAY_URL");
            options.Key = Environment.GetEnvironmentVariable("APPRELAY_KEY");
            options.CcRecipients = Environment.GetEnvironmentVariable("APPRELAY_CC");
            options.BccRecipients = Environment.GetEnvironmentVariable("APPRELAY_BCC");
            Int32.TryParse(Environment.GetEnvironmentVariable("APPRELAY_PORT"), out int port);

            // command line args: -u --url, -k --key, -p --port
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-u":
                    case "--url":
                    options.Url = args[i+1];
                    break;

                    case "-k":
                    case "--key":
                    options.Key = args[i+1];
                    break;

                    case "-p":
                    case "--port":
                    Int32.TryParse(args[i+1], out port);
                    break;
                }
            }

            if (port == 0) port = 25;

            if (string.IsNullOrEmpty(options.Url) || string.IsNullOrEmpty(options.Key))
            {
                Console.WriteLine("You must supply a url and api-key for the destination AppMailRelay.");
                return;
            }

            // set up logging
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Default", LogLevel.Information)
                    .AddConsole();
            });
            ILogger logger = loggerFactory.CreateLogger<Program>();

            // set up stmp listener
            var _queue = new BlockingCollection<SmtpMessage>();

            var server = SimpleSmtpServer.Start(port);

            server.MessageReceived += (sender, args) =>
            {
                _queue.Add(args.Message);
            };

            logger.LogInformation($"SmtpServer is listening on port {port}.");

            // set up relay client
            var http = new HttpClient();
            http.BaseAddress = new Uri(options.Url);
            http.DefaultRequestHeaders.Add("x-api-key", options.Key);

            var mailer = new AppMailClient.Mailer(
                loggerFactory.CreateLogger<AppMailClient.Mailer>(),
                options,
                http
            );

            // process incoming messages
            foreach(var source in _queue.GetConsumingEnumerable())
            {
                try
                {
                    var msg = new AppMailClient.MailMessage();
                    msg.From = source.Headers["From"];
                    msg.To = source.Headers["To"].Replace(",", ";");
                    msg.Cc = source.Headers["Cc"]?.Replace(",", ";") + ";" + options.CcRecipients;
                    msg.Bcc = source.Headers["Bcc"]?.Replace(",", ";") + ";" + options.BccRecipients;

                    msg.Subject = source.Headers["Subject"];

                    msg.Text = source.MessageParts
                        .Where(p => p.HeaderData.Contains("text/plain"))
                        .Select(p => p.BodyData)
                        .FirstOrDefault();

                    msg.Html = source.MessageParts
                        .Where(p => p.HeaderData.Contains("text/html"))
                        .Select(p => p.BodyData)
                        .FirstOrDefault();

                    await mailer.Send(msg);

                    logger.LogInformation($"relayed {msg.To} [{msg.Subject}]");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Relay failed.");
                }
            }

        }
    }
}
