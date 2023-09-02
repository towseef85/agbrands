namespace AGBrand.Packages.Contracts
{
    using Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IMailer
    {
        Task SendAsync(NotificationEmailAddress from, IEnumerable<NotificationEmailAddress> tos, string subject, string body);
    }
}
