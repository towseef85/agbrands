namespace AGBrand.Packages.Providers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Versioning;

    public sealed class ErrorReponsesSuppressor : IErrorResponseProvider
    {
        public IActionResult CreateResponse(ErrorResponseContext context)
        {
            return new BadRequestResult();
        }
    }
}
