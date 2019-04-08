using System.Collections.Generic;
using Datadog.Coffeehouse.Api.Filters;
using Datadog.Coffeehouse.Core.Interfaces;
using Datadog.Coffeehouse.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datadog.Coffeehouse.Api.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class OrderItemsController : ControllerBase
    {
        private readonly IOrderItemRepository _orderItemRepository;

        public OrderItemsController(IOrderItemRepository orderItemRepository)
        {
            _orderItemRepository = orderItemRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<OrderItem>> Get()
            => _orderItemRepository.Select();

        [HttpGet("{id}")]
        public ActionResult<OrderItem> Get(string id)
            => _orderItemRepository.Single(id);

        [HttpPost]
        public void Post([FromBody] OrderItem value)
            => _orderItemRepository.Add(value);

        [HttpPut("{id}")]
        [PutEntityIdMatchedValidationFilter]
        public void Put(string id, [FromBody] OrderItem value)
            => _orderItemRepository.Update(value);

        [HttpDelete("{id}")]
        public void Delete(string id)
            => _orderItemRepository.Delete(id);
    }
}
