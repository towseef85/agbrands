using AGBrand.Contracts;
using AGBrand.Models.Api.Brands;
using AGBrand.Packages.Contracts;
using AGBrand.Packages.Models;
using AGBrand.Packages.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AGBrand.Api.Controllers
{
    /// <summary>
    /// Brand Api Controller
    /// </summary>
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IWrapper _wrapper;

        /// <summary>
        /// Brands Controller Constructor
        /// </summary>
        /// <param name="wrapper"></param>
        public BrandsController(IWrapper wrapper)
        {
            _wrapper = wrapper;
        }

        /// <summary>
        /// Get All Brands
        /// </summary>
        /// <response code="200">Successfully Fetched Brands Collection</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetBrandsResponse))]
        public async Task<IActionResult> GetAsync([FromQuery] PagerArgs pagerArgs)
        {
            return await _wrapper.BrandsBL.GetAsync(pagerArgs).RespondAsync((IApiRespondService)_wrapper);
        }

        /// <summary>
        /// Get Brand By Id
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetBrandResponse))]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            return await _wrapper.BrandsBL.GetByIdAsync(id).RespondAsync((IApiRespondService)_wrapper);
        }

        /// <summary>
        /// Add Brand
        /// </summary>
        /// <param name="postAddBrandRequest"></param>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddAsync([FromForm] PostAddBrandRequest postAddBrandRequest)
        {
            return await _wrapper.BrandsBL.AddAsync(postAddBrandRequest).RespondAsync((IApiRespondService)_wrapper);
        }

        /// <summary>
        /// Delete Brand
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            return await _wrapper.BrandsBL.DeleteAsync(id).RespondAsync((IApiRespondService)_wrapper);
        }

        /// <summary>
        /// Edit Brand
        /// </summary>
        /// <param name="postUpdateBrandRequest"></param>
        [HttpPut]
        [AllowAnonymous]
        public async Task<IActionResult> EditAsync([FromForm] PostUpdateBrandRequest postUpdateBrandRequest)
        {
            return await _wrapper.BrandsBL.EditAsync(postUpdateBrandRequest).RespondAsync((IApiRespondService)_wrapper);
        }
    }
}
