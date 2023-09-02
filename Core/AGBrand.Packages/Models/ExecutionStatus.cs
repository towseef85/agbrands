namespace AGBrand.Packages.Models
{
    using System.Net;

    public sealed class ExecutionStatus
    {
        private StatusType _statusType;

        public HttpStatusCode Code { get; set; }
        public string Message { get; set; }
        public object Object { get; set; }

        public object StatusType
        {
            get => _statusType.ToString();
            set => _statusType = (StatusType)value;
        }
    }
}
