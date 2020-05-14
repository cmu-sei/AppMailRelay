// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Threading.Tasks;
using AppMailClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {

            var services = new ServiceCollection();

            services.AddLogging(opt => {
                // opt.AddConsole();
            });

            services.AddAppMailClient(opt => {
                opt.Url = "http://localhost:5000/msg";
                opt.Key = "dev#1234";
            });

            var sp = services.BuildServiceProvider();

            RunTest(sp).Wait();

        }

        static async Task RunTest(IServiceProvider sp)
        {
            var client = sp.GetService<AppMailClient.IAppMailClient>();

            var status = await client.Send(new MailMessage
            {
                Subject = "test message",
                Text = "This is an email text body.",
                Html = "<html><body>This is an email html body.</body></html>",
                To = "You <you@that.local>",
                Cc = "",
                Bcc = "",
                From = "Me <me@this.local>"

            });
            Console.WriteLine(status.ReferenceId);
        }
    }

}
