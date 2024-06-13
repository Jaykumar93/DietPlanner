using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class FeedSectionController : Controller
    {
        public IActionResult PublicFeed()
        {
            return View();
        }
    }
}
