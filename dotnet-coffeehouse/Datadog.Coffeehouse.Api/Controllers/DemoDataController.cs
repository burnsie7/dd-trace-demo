using Datadog.Coffeehouse.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Datadog.Coffeehouse.Api.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class DemoDataController : ControllerBase
    {
        private readonly IDemoDataService _demoDataService;

        public DemoDataController(IDemoDataService demoDataService)
        {
            _demoDataService = demoDataService;
        }

        [HttpGet]
        public void Get()
            => _demoDataService.RunDemoDataBatchAsync();
    }
}
