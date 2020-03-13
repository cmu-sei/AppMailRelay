// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using System.Threading.Tasks;

namespace AppMailClient
{
    public interface IAppMailClient
    {
        Task<MailMessageStatus> Send(MailMessage message);
        Task<MailMessageStatus> Status(string referenceId);
    }
}
