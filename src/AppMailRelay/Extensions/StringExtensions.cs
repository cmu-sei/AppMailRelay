// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

namespace AppMailRelay.Extensions
{
    public static class StringExtensions
    {
        public static bool HasValue(this string s)
        {
            return !System.String.IsNullOrEmpty(s);
        }

        public static string Tag(this string s)
        {
            if (s.HasValue())
            {
                int x = s.IndexOf('#');
                if (x >= 0)
                    return s.Substring(x+1);
            }
            return "";
        }

        public static string Untagged(this string s)
        {
            if (s.HasValue())
            {
                int x = s.IndexOf('#');
                if (x >= 0)
                    return s.Substring(0, x);
            }
            return s;
        }
    }
}
