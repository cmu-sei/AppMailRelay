// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System;
using System.Linq;
using AppMailRelay.Models;
using MimeKit;

namespace AppMailRelay.Extensions
{
    public static class MailExtensions
    {
        public static MimeMessage ToMimeMessage(this TaggedMessage source)
        {
            return ((MailMessage)source).ToMimeMessage();
        }

        public static MimeMessage ToMimeMessage(this MailMessage source)
        {
            var message = new MimeMessage();
            message.Subject = source.Subject ?? "Message Subject";
            message.From.Add(source.From.ToMailAddress());
            message.To.AddRange(source.To.Split(';').Select(x => x.ToMailAddress()));

            var cc = source.Cc.Trim().Split(';', StringSplitOptions.RemoveEmptyEntries);
            if (cc.Length > 0)
                message.Cc.AddRange(
                    cc.Select(x => x.Trim())
                    .Distinct()
                    .Select(x => x.ToMailAddress())
                );

            var bcc = source.Bcc.Trim().Split(';', StringSplitOptions.RemoveEmptyEntries);
            if (bcc.Length > 0)
                message.Bcc.AddRange(
                    bcc.Select(x => x.Trim())
                    .Distinct()
                    .Select(x => x.ToMailAddress())
                );

            var parts = new Multipart("alternative");
            if (source.Text.HasValue()) parts.Add(new TextPart("plain") { Text = source.Text });
            if (source.Html.HasValue()) parts.Add(new TextPart("html") { Text = source.Html });
            message.Body = parts;

            return message;
        }

        private static MailboxAddress ToMailAddress(this string address)
        {
            var parts = address.Replace(">","").Split('<');
            return new MailboxAddress(parts.First().Trim(), parts.Last().Trim());
        }
    }
}
