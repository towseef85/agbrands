using System.Threading.Tasks;
using AGBrand.Contracts;
using AGBrand.Models.Api.Products;
using AGBrand.Packages.Contracts;
using AGBrand.Packages.Models;
using AGBrand.Packages.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGBrand.Api.Controllers
{

    /// <summary>
    /// Products Api Controller
    /// </summary>
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IWrapper _wrapper;

        /// <summary>
        /// Products Controller Constructor
        /// </summary>
        /// <param name="wrapper"></param>
        public ProductsController(IWrapper wrapper)
        {
            _wrapper = wrapper;
        }

        /// <summary>
        /// Get All Products
        /// </summary>
        /// <response code="200">Successfully Fetched Products Collection</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProductsResponse))]
        public async Task<IActionResult> GetAsync([FromQuery] PagerArgs pagerArgs)
        {
            return await _wrapper.ProductsBL.GetAsync(pagerArgs).RespondAsync((IApiRespondService)_wrapper);
        }

        ////[HttpGet("queryCheck")]
        ////[AllowAnonymous]
        ////public async Task<IActionResult> QueryCheckAsync()
        ////{
        ////    var data = (await sqlContext.CategoryProductsCache
        ////        .Include(c => c.Product)
        ////        .ThenInclude(c => c.ProductVariantValues)
        ////        .ThenInclude(c => c.VariantValue)
        ////        .ThenInclude(c => c.Variant)
        ////        .Where(c => c.CategoryId == 1)
        ////        .ToListAsync())
        ////        .SelectMany(c => c.Product.ProductVariantValues.Select(d => d.VariantValue.Variant))
        ////        .Distinct()
        ////        .Select(c => new
        ////        {
        ////            Variant = c.Title,
        ////            Values = c.VariantValues.Select(c => c.Value)
        ////        })
        ////        .ToList();

        ////    return Ok(data);
        ////}

        /// <summary>
        /// Get Product By Id
        /// </summary>
        /// <response code="200">Successfully Fetched Single Product By Id</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProductsResponse))]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            return await Task.FromResult(Ok(id));
        }

        /// <summary>
        /// Add Product
        /// </summary>
        /// <param name="postAddProductRequest"></param>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddAsync([FromBody] PostAddProductRequest postAddProductRequest)
        {
            return await _wrapper.ProductsBL.AddAsync(postAddProductRequest).RespondAsync((IApiRespondService)_wrapper);
        }
    }
}
