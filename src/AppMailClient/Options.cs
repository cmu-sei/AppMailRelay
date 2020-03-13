// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

namespace AppMailClient
{
    public class Options
    {
        public string Url { get; set; }
        public string Key { get; set; }
        public string KeyHeader { get; set; } = "X-API-KEY";
        public string From { get; set; }
        public string CcRecipients { get; set; }
        public string BccRecipients { get; set; }

    }
}
