// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppMailRelay
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;

            _options = configuration.Get<AppSettings>();

            string usageFile = Path.Combine(env.ContentRootPath, _options.Relay.UsageFile);
            if (File.Exists(usageFile))
                _usage = File.ReadAllText(usageFile);
        }

        AppSettings _options { get; }
        IConfiguration Configuration { get; }
        string _usage = "Usage: unavailable.";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCache(() => _options.Redis);
            services.AddAppMailRelay(() => _options);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var forwardOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            };
            forwardOptions.KnownNetworks.Clear();
            forwardOptions.KnownProxies.Clear();
            app.UseForwardedHeaders(forwardOptions);

            app.UseAppMailRelay();

            app.Run(async (context) =>
            {
                string url = $"{context.Request.Scheme}://{context.Request.Host}{_options.Relay.RequestPath}";
                await context.Response.WriteAsync($"<pre>{_usage.Replace("##URL##", url)}</pre>");
            });
        }
    }
}
