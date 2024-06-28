using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Authorize]
    public class FeedController : Controller
    {
        public IActionResult PublicFeed()
        {
            return View();
        }
    }
}
