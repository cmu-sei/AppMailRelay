// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AppMailRelay.Models;
using AppMailRelay.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AppMailRelay
{
    public class AppMailRelayMiddleware
    {
        public AppMailRelayMiddleware(
            RequestDelegate next,
            IMailer mailer,
            ILogger<AppMailRelayMiddleware> logger,
            RelayOptions options
        )
        {
            _next = next;
            _mailer = mailer;
            _logger = logger;
            _options = options;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        private readonly RequestDelegate _next;
        private readonly RelayOptions _options;
        private readonly IMailer _mailer;
        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _jsonOptions;
        public async Task Invoke(HttpContext context)
        {
            if (!ValidPath(context.Request))
            {
                await _next(context);
                return;
            }

            string authClient = ValidClient(context.Request);

            if (string.IsNullOrEmpty(authClient))
            {
                await OnForbidden(context);
                return;
            }

            if (context.Request.Method == HttpMethods.Post)
            {
                try
                {
                    var msg = await JsonSerializer.DeserializeAsync<TaggedMessage>(context.Request.Body, _jsonOptions);

                    msg.ClientName = authClient;
                    msg.ReferenceId = Guid.NewGuid().ToString("N");

                    var status = await _mailer.Enqueue(msg);

                    await OnSuccess(context, status);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to enqueue message.");
                    await OnError(context, ex.Message);
                }
            }

            if (context.Request.Method == HttpMethods.Get)
            {
                string id = context.Request.Query["id"].ToString();
                if (!id.HasValue())
                {
                    id = context.Request.Path.Value
                        .Replace(_options.RequestPath, "")
                        .Split('/')
                        .LastOrDefault();
                }

                await OnSuccess(context, await _mailer.GetStatus(id));
            }
        }

        private async Task OnSuccess(HttpContext context, MailMessageStatus status)
        {
            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(status, _jsonOptions));
        }

        private async Task OnForbidden(HttpContext context)
        {
            context.Response.StatusCode = 401;
            context.Response.Headers.Add("WWW-Authenticate", "x-api-key");
            await context.Response.WriteAsync("");
        }

        private async Task OnError(HttpContext context, string msg)
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync(msg);
        }

        private bool ValidPath(HttpRequest request)
        {
            return request.Path.Value.StartsWith(_options.RequestPath);
        }

        private string ValidClient(HttpRequest request)
        {
            string client = "";
            string apiKey = request.Headers["X-API-KEY"];

            if (apiKey.HasValue() && _options.ClientKeys.Contains(apiKey))
            {
                client = apiKey.Untagged();
            }

            return client;
        }

    }
}
