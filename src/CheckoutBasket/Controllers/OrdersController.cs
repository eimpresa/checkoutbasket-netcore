using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CheckoutBasket.Constants;
using CheckoutBasket.Contracts;
using CheckoutBasket.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CheckoutBasket.Controllers
{
    [Authorize(Roles = UserRoles.BasketUser)]
    [Route("api/v1/[controller]")]
    public class OrdersController : Controller
    {
        private readonly IOrderService orderService;

        public OrdersController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder()
        {
            var userId = new Guid(User.Identity.Name);
            var createResponse = await orderService.CreateOrder(userId);
            if (createResponse.Success)
            {
                return Created(Url.Action("GetOrder", new { orderId = createResponse.Data.Id }), null);
            }
            return BadRequest(createResponse.Errors);
        }

        [HttpGet]
        [Route("{orderId}")]
        public async Task<IActionResult> GetOrder(Guid orderId)
        {
            var userId = new Guid(User.Identity.Name);
            var order = await orderService.GetOrder(orderId);
            if (order != null && order.UserId == userId)
            {
                return Json(order);
            }

            return NotFound();
        }

        [HttpPut]
        [Route("{orderId}")]
        public async Task<IActionResult> AddItem(Guid orderId, [FromBody]AddItemContract newItem)
        {
            var userId = new Guid(User.Identity.Name);
            var order = await orderService.GetOrder(orderId);
            if (order != null && order.UserId == userId)
            {
                var addItemResponse = await orderService.AddItem(orderId, newItem.ProductId, newItem.Quantity);
                if (addItemResponse.Success)
                {
                    return Ok();
                }

                return BadRequest(addItemResponse.Errors);
            }

            return NotFound();
        }

        [HttpDelete]
        [Route("{orderId}/{productId}")]
        public async Task<IActionResult> DeleteItem(Guid orderId, Guid productId)
        {
            var userId = new Guid(User.Identity.Name);
            var order = await orderService.GetOrder(orderId);
            if (order != null && order.UserId == userId)
            {
                var deleteItemResponse = await orderService.DeleteItem(orderId, productId);
                if (deleteItemResponse.Success)
                {
                    return Ok();
                }

                return BadRequest(deleteItemResponse.Errors);
            }

            return NotFound();
        }

        [HttpPut]
        [Route("{orderId}/{productId}")]
        public async Task<IActionResult> UpdateQuantity(Guid orderId, Guid productId, [FromBody]UpdateItemContract updateQuantity)
        {
            var userId = new Guid(User.Identity.Name);
            var order = await orderService.GetOrder(orderId);
            if (order != null && order.UserId == userId)
            {
                var updateQuantityResponse = await orderService.UpdateQuantity(orderId, productId, updateQuantity.quantity);
                if (updateQuantityResponse.Success)
                {
                    return Ok();
                }

                return BadRequest(updateQuantityResponse.Errors);
            }

            return NotFound();
        }
    }
}