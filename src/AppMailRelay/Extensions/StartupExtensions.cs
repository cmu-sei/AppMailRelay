// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System;
using AppMailRelay;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AppMailRelayMiddlewareExtensions
    {
        public static IServiceCollection AddAppMailRelay(
            this IServiceCollection services,
            Func<AppSettings> config
        )
        {
            var opt = config();
            services
                .AddSingleton(svc => opt.Relay)
                .AddSingleton(svc => opt.Mail);

            if (string.IsNullOrEmpty(opt.Mail.Host))
                services.AddSingleton<IMailer, MailerMock>();
            else
                services.AddSingleton<IMailer, Mailer>();

            return services;
        }

        public static IServiceCollection AddCache(this IServiceCollection services, Func<RedisOptions> configure = null)
        {
            var options = (configure != null)
                ? configure()
                : new RedisOptions();

            if (System.String.IsNullOrWhiteSpace(options?.Url))
            {
                services.AddDistributedMemoryCache();
            }
            else
            {
                services.AddStackExchangeRedisCache(opt => {
                    opt.Configuration = options.Url;
                    opt.InstanceName = options.Name;
                });
            }

            return services;
        }

    }
}
