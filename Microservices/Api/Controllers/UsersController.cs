using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AGBrand.Models.Api.Users;
using AGBrand.Packages.Contracts;
using AGBrand.Contracts;
using AGBrand.Packages.Util;

namespace AGBrand.Api.Controllers
{
    /// <summary>
    /// Users Api Controller
    /// </summary>
    [Produces("application/json")]
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    public class UsersController : ControllerBase
    {
        private readonly IWrapper _wrapper;

        /// <summary>
        /// UsersController Constructor
        /// </summary>
        public UsersController(IWrapper wrapper)
        {
            _wrapper = wrapper;
        }

        /// <summary>
        /// Create User
        /// </summary>
        /// <param name="postAddUserRequest"></param>
        /// <response code="201">User Created Successfully</response>
        /// <response code="400">Invalid Data Object</response>
        /// <response code="401">Action Token Invalid/Expired or Signature Invalid</response>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PostAddUserResponse))]
        public async Task<IActionResult> AddAsync([FromBody] PostAddUserRequest postAddUserRequest)
        {
            return await _wrapper.UsersBL.AddAsync(postAddUserRequest).RespondAsync((IApiRespondService)_wrapper);
        }

    }
}
