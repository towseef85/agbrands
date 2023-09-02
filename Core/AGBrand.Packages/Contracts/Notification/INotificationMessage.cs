namespace AGBrand.Packages.Contracts.Notification
{
    using System;

    public interface INotificationMessage<T>
    {
        Enum NotificationType { get; set; }

        T Payload { get; set; }
    }
}
