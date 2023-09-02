namespace AGBrand.Packages.Contracts.Notification
{
    using System.Collections.Generic;

    public interface INotificationEmail
    {
        List<string> Bccs { get; set; }
        string Content { get; set; }

        string From { get; set; }

        string Name { get; set; }

        string Subject { get; set; }
        List<string> Tos { get; set; }
    }
}
