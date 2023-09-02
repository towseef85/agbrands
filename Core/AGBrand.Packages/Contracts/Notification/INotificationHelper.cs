namespace AGBrand.Packages.Contracts.Notification
{
    using Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface INotificationHelper
    {
        Task EmailAsync(IEnumerable<NotificationEmailAddress> tos, string body, string subject);

        Task EmailAsync(IEnumerable<NotificationEmailAddress> tos, IEnumerable<NotificationEmailAddress> bccs, string body, string subject);

        Task EmailBccAsync(IEnumerable<NotificationEmailAddress> bccs, string body, string subject);

        Task NotifyBccsAsync(NotificationAddresses notificationAddresses, string emailBody, string smsBody, string subject);

        Task SmsAsync(List<string> mobiles, string sms);
    }
}
