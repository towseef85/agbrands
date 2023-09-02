namespace AGBrand.Packages.Contracts.Notification
{
    using System.Collections.Generic;

    public interface INotificationSms
    {
        string Content { get; set; }

        List<string> Tos { get; set; }
    }
}
