using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.User
{
    [Authorize(Roles = "User")]
    public class MealPlannerController : Controller
    {
        public IActionResult MealPlanner()
        {
            return View();
        }
    }
}
