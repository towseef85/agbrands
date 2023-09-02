namespace AGBrand.Packages.Models
{
    public sealed class RaiseError
    {
        public RaiseError(string code, string message)
        {
            Error = new CustomError { Code = code, Message = message };
        }

        public CustomError Error { get; set; }
    }
}
