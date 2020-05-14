// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

using System;

namespace AppMailRelay.Models
{
    public class MailMessage
    {
        public string MessageId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public string Html { get; set; }
        public bool BccSender { get; set; }
    }

    public class TaggedMessage : MailMessage
    {
        public string ReferenceId { get; set; }
        public string ClientName { get; set; } = "unknown-client";

    }

        public class MailMessageStatus
    {
        public string ReferenceId { get; set; }
        public string MessageId { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public enum MessageStatus
    {
        unknown,
        pending,
        success,
        failure
    }
}
