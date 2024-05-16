using EasyCaching.Core;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("/Redis")]
    public class RedisController : ControllerBase
    {
        private IEasyCachingProvider cachingProvider;
        private IEasyCachingProviderFactory cachingProviderFactory;

        public RedisController(IEasyCachingProviderFactory cachingProviderFactory)
        {
            this.cachingProviderFactory = cachingProviderFactory;
            this.cachingProvider = this.cachingProviderFactory.GetCachingProvider("redis1");
        }

        [HttpGet("Set")]
        public IActionResult SetItemInQueue()
        {
            this.cachingProvider.Set("TestKey123", "Here is my value", TimeSpan.FromDays(100));

            return Ok();
        }

        [HttpGet("Get")]
        public IActionResult GetItemInQueue()
        {
            var item = this.cachingProvider.Get<string>("TestKey123");

            return Ok(item);
        }
    }

    
}
 