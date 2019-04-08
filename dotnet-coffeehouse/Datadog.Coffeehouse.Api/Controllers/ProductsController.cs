using System.Collections.Generic;
using Datadog.Coffeehouse.Api.Filters;
using Datadog.Coffeehouse.Core.Interfaces;
using Datadog.Coffeehouse.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datadog.Coffeehouse.Api.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> Get()
            => _productRepository.Select();

        [HttpGet("{id}")]
        public ActionResult<Product> Get(string id)
            => _productRepository.Single(id);

        [HttpPost]
        public void Post([FromBody] Product value)
            => _productRepository.Add(value);

        [HttpPut("{id}")]
        [PutEntityIdMatchedValidationFilter]
        public void Put(string id, [FromBody] Product value)
            => _productRepository.Update(value);

        [HttpDelete("{id}")]
        public void Delete(string id)
            => _productRepository.Delete(id);
    }
}
