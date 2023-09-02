using System.Threading.Tasks;
using AGBrand.Contracts;
using AGBrand.Models.Api.Categories;
using AGBrand.Packages.Contracts;
using AGBrand.Packages.Models;
using AGBrand.Packages.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGBrand.Api.Controllers
{
    /// <summary>
    /// Categories Api Controller
    /// </summary>
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IWrapper _wrapper;

        /// <summary>
        /// Categories Controller Constructor
        /// </summary>
        /// <param name="wrapper"></param>
        public CategoriesController(IWrapper wrapper)
        {
            _wrapper = wrapper;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="pagerArgs"></param>
        ///// <returns></returns>
        ////[HttpGet]
        ////[AllowAnonymous]
        ////[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCategoriesResponse))]
        ////public async Task<IActionResult> GetAsync([FromQuery] PagerArgs pagerArgs)
        ////{
        ////    return await _wrapper.CategoriesBL.GetAsync(pagerArgs).RespondAsync((IApiRespondService)_wrapper);
        ////}

        /// <summary>
        /// Get All Categories
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCategoriesResponse))]
        public async Task<IActionResult> GetAsync()
        {
            return await _wrapper.CategoriesBL.GetAsync().RespondAsync((IApiRespondService)_wrapper);
        }


        /// <summary>
        /// Get Category By Id
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCategoryResponse))]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            return await _wrapper.CategoriesBL.GetByIdAsync(id).RespondAsync((IApiRespondService)_wrapper);
        }

        /// <summary>
        /// Add Category
        /// </summary>
        /// <param name="postAddCategoryRequest"></param>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddAsync([FromForm] PostAddCategoryRequest postAddCategoryRequest)
        {
            return await _wrapper.CategoriesBL.AddAsync(postAddCategoryRequest).RespondAsync((IApiRespondService)_wrapper);
        }

        /// <summary>
        /// Delete Category
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            return await _wrapper.CategoriesBL.DeleteAsync(id).RespondAsync((IApiRespondService)_wrapper);
        }

        /// <summary>
        /// Edit Category
        /// </summary>
        /// <param name="postUpdateCategoryRequest"></param>
        [HttpPut]
        [AllowAnonymous]
        public async Task<IActionResult> EditAsync([FromForm] PostUpdateCategoryRequest postUpdateCategoryRequest)
        {
            return await _wrapper.CategoriesBL.EditAsync(postUpdateCategoryRequest).RespondAsync((IApiRespondService)_wrapper);
        }

        /// <summary>
        /// Get Products for Category
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pagerArgs"></param>
        /// <returns></returns>
        [HttpGet("{id:int}/Products")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryProductsAsync([FromRoute] int id, [FromQuery] PagerArgs pagerArgs)
        {
            return await _wrapper.CategoriesBL.GetCategoryProducts(new GetCategoryProductsRequest
            {
                Id = id,
                PagerArgs = pagerArgs
            }).RespondAsync((IApiRespondService)_wrapper);
        }
    }
}
