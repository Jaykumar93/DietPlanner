using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Web.Controllers
{
    public class RoleBasedRedirectionController : Controller
    {

        public async Task<IActionResult> DashboardRedirection()
        {
            if (User.Identity.IsAuthenticated)
            {
                var claims = HttpContext.User.Claims;
                string roles = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (roles.Contains("Admin"))
                {
                    return RedirectToAction("Index", "Home");
               
                }
                else if (roles.Contains("User"))
                {
                    return RedirectToAction("Index", "Home");
                }

            }
            return View();
        }

        public async Task<IActionResult> MealPlannerRedirection()
        {
            if (User.Identity.IsAuthenticated)
            {
                var claims = HttpContext.User.Claims;
                string roles = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (roles.Contains("Admin"))
                {
                    return RedirectToAction("ViewMealPlan", "AdminMealPlanner");
                }
                else if (roles.Contains("User"))
                {
                    return RedirectToAction("PlanDashboard", "UserMealPlanner");
                }
                
            }
            return View();
        }

        public async Task<IActionResult> ChallengesRedirection()
        {
            if (User.Identity.IsAuthenticated)
            {
                var claims = HttpContext.User.Claims;
                string roles = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (roles.Contains("Admin"))
                {
                    return RedirectToAction("ViewChallenges", "AdminChallenges");
                }
                else if (roles.Contains("User"))
                {
                    return RedirectToAction("ChallengeDashboard", "UserChallenges");
                }

            }
            return View();
        }

        public async Task<IActionResult> ActivityRedirection()
        {
            if (User.Identity.IsAuthenticated)
            {
                var claims = HttpContext.User.Claims;
                string roles = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (roles.Contains("Admin"))
                {
                    return RedirectToAction("UserActivity", "UserActivityTracking");
                }
                else if (roles.Contains("User"))
                {
                    return RedirectToAction("UserActivity", "UserActivityTracking");
                }

            }
            return View();
        }
    }
   
}
