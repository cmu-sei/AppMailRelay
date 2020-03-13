// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Tasks;
using AppMailRelay.Extensions;
using AppMailRelay.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace AppMailRelay
{

    public class MailerMock : IMailer
    {
        public MailerMock(
            MailOptions options,
            IDistributedCache cache,
            ILogger<MailerMock> logger
        )
        {
            _logger = logger;
            _options = options;
            _queue = new BlockingCollection<TaggedMessage>();
            _cache = cache;
            _cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = new TimeSpan(0, _options.StatusCacheMinutes, 0)
            };
            Task.Run(() => ProcessQueue());
        }

        private readonly IDistributedCache _cache;
        private readonly DistributedCacheEntryOptions _cacheOptions;
        private readonly ILogger _logger;
        private readonly MailOptions _options;
        private readonly BlockingCollection<TaggedMessage> _queue;
        private string logMsg = "Mail mocked to {0} on behalf of {1} {2}. {3}";

        public async Task<MailMessageStatus> GetStatus(string referenceId)
        {
            string status = await _cache.GetStringAsync(referenceId);

            return status.HasValue()
                ? JsonSerializer.Deserialize<MailMessageStatus>(status)
                : new MailMessageStatus
                {
                    ReferenceId = referenceId,
                    Timestamp = DateTime.UtcNow,
                    Status = MessageStatus.unknown.ToString()
                };

        }

        public async Task<MailMessageStatus> Enqueue(TaggedMessage message)
        {
            if (string.IsNullOrWhiteSpace(message.To))
                throw new Exception("invalid-recipient");

            var status = new MailMessageStatus
            {
                ReferenceId = message.ReferenceId,
                MessageId = message.MessageId,
                Timestamp = DateTime.UtcNow,
                Status = MessageStatus.pending.ToString()
            };

            string errorMessage = "";
            if (!_queue.TryAdd(message))
            {
                status.Status = MessageStatus.failure.ToString();
                errorMessage = "Unable to add message to queue.";
            }

            await _cache.SetStringAsync(
                status.ReferenceId,
                JsonSerializer.Serialize(status),
                _cacheOptions
            );

            _logger.LogDebug(logMsg, message.To, message.ClientName, status.Status, errorMessage);
            return status;
        }

        public void ProcessQueue()
        {
            foreach (var source in _queue.GetConsumingEnumerable())
            {
                var status = new MailMessageStatus
                {
                    ReferenceId = source.ReferenceId,
                    MessageId = source.MessageId
                };


                if (!source.From.HasValue())
                    source.From = _options.Sender;

                source.Cc += $";{_options.CcRecipients}";

                source.Bcc += $";{_options.BccRecipients}";

                if (source.BccSender)
                    source.Bcc += $";{source.From}";

                var message = source.ToMimeMessage();

                try
                {
                    // client.Send(FormatOptions.Default, message);
                    status.Status = MessageStatus.success.ToString();
                    _logger.LogInformation(logMsg, source.To, source.ClientName, "succeeded", "");
                }
                catch (Exception ex)
                {
                    status.Status = MessageStatus.failure.ToString();
                    _logger.LogError(logMsg, source.To, source.ClientName, "failed", ex.Message);
                }


                status.Timestamp = DateTime.UtcNow;

                _cache.SetString(
                    status.ReferenceId,
                    JsonSerializer.Serialize(status),
                    _cacheOptions
                );

                if (_queue.Count == 0)
                {
                    // client.Disconnect(true);
                    _logger.LogDebug("Queue empty; disconnected mock smtp client.");
                }
            }
        }

    }
}
