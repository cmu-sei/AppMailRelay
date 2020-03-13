// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

namespace AppMailRelay
{
    public class AppSettings
    {
        public RelayOptions Relay { get; set; }
        public MailOptions Mail { get; set; }
        public RedisOptions Redis { get; set; }
    }

    public class RelayOptions
    {
        public string RequestPath { get; set; } = "/msg";
        public string UsageFile { get; set; } = "Readme.md";
        public string[] ClientKeys { get; set; }  = new string[]{};
    }

    public class MailOptions
    {
        public string Host { get; set; }
        public int Port { get; set; } = 25;
        public string User { get; set; }
        public string Password { get; set; }
        public string Sender { get; set; }
        public string CcRecipients { get; set; }
        public string BccRecipients { get; set; }
        public int StatusCacheMinutes { get; set; } = 10;
    }

    public class RedisOptions
    {
        public string Url { get; set; }
        public string Name { get; set; } = "AppMailRelay";
    }
}
