using System.Collections.Generic;
using Datadog.Coffeehouse.Api.Filters;
using Datadog.Coffeehouse.Core.Interfaces;
using Datadog.Coffeehouse.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datadog.Coffeehouse.Api.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrdersController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Order>> Get()
            => _orderRepository.Select();

        [HttpGet("{id}")]
        public ActionResult<Order> Get(string id)
            => _orderRepository.Single(id);

        [HttpPost]
        public void Post([FromBody] Order value)
            => _orderRepository.Add(value);

        [HttpPut("{id}")]
        [PutEntityIdMatchedValidationFilter]
        public void Put(string id, [FromBody] Order value)
            => _orderRepository.Update(value);

        [HttpDelete("{id}")]
        public void Delete(string id)
            => _orderRepository.Delete(id);
    }
}
