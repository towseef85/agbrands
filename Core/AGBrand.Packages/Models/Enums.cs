namespace AGBrand.Packages.Models
{
    public enum NotificationType
    {
        Email = 1,
        Sms = 2,
        All = 3
    }

    public enum SortDirection
    {
        ASC,
        DESC
    }

    public enum StatusType
    {
        NoStatus,
        Error,
        Info,
        Success,
        Warning
    }
}
