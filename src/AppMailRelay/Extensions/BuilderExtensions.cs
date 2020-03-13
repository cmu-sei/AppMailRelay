// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using AppMailRelay;

namespace Microsoft.AspNetCore.Builder
{
    public static class AppMailRelayBuilderExtensions
    {
        public static IApplicationBuilder UseAppMailRelay(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AppMailRelayMiddleware>();
        }
    }
}