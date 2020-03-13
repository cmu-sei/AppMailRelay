// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AppMailClient
{
    public class MailerMock : IAppMailClient
    {
        private readonly ILogger _logger;
        private readonly AppMailClient.Options _options;

        public MailerMock (
            ILogger<MailerMock> logger,
            AppMailClient.Options options
        ){
            _logger = logger;
            _options = options;
        }

        List<MailMessageStatus> _store = new List<MailMessageStatus>();

        public Task<MailMessageStatus> Send(MailMessage message)
        {
            _logger.LogInformation($"Logged message [{message.Subject}] to ${message.To}");

            var status = new MailMessageStatus
            {
                ReferenceId = Guid.NewGuid().ToString("N"),
                MessageId = message.MessageId,
                Timestamp = DateTime.UtcNow,
                Status = "pending"
            };

            _store.Add(status);

            // cleanup
            var expired = _store.OrderByDescending(s => s.Timestamp).Skip(20).ToArray();
            foreach(var s in expired)
                _store.Remove(s);

            return Task.FromResult(status);
        }

        public Task<MailMessageStatus> Status(string referenceId)
        {
            _logger.LogInformation($"Checking mock status for {referenceId}");

            var status = _store.Where(s => s.ReferenceId == referenceId).SingleOrDefault()
            ?? new MailMessageStatus
            {
                ReferenceId = referenceId,
                Timestamp = DateTime.UtcNow,
                Status = "unknown"
            };

            return Task.FromResult(status);
        }
    }
}
