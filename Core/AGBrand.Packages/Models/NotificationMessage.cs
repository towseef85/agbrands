using System;

namespace AGBrand.Packages.Models
{
    public sealed class NotificationMessage<T>
    {
        public Enum NotificationType { get; set; }

        public T Payload { get; set; }
    }
}
