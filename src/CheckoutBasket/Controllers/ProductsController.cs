using CheckoutBasket.Constants;
using CheckoutBasket.Contracts;
using CheckoutBasket.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckoutBasket.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    [Route("api/v1/[controller]")]
    public class ProductsController : Controller
    {
        private readonly IProductService productService;

        public ProductsController(IProductService productService)
        {
            this.productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody]CreateProductContract createProductContract)
        {
            var createResponse = await productService.CreateProduct(createProductContract);
            if (createResponse.Success)
            {
                return Created(Url.Action("GetProduct", new { productId = createResponse.Data.id }), null);
            }
            return BadRequest(createResponse.Errors);
        }

        [HttpGet]
        [Route("{productId}")]
        public async Task<IActionResult> GetProduct(Guid productId)
        {
            var product = await productService.GetProduct(productId);
            if (product != null)
            {
                return Ok(product);
            }

            return NotFound();
        }
    }
}
