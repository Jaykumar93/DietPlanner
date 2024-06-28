using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class CommunitySectionController : Controller
    {
        public IActionResult Feed()
        {
            return View();
        }
    }
}
