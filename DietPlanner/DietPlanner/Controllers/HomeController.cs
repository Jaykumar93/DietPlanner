using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Repository;
using Domain.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Services.AuthServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Security.Claims;
using Domain.DTO;
using static Domain.DTO.ActivityTrackingViewModel;


namespace DietPlanner.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserDetailRepository _userDetailRepository;
        private readonly IDistributedCache _cache;
        private readonly Domain.Data.DietContext _context;

        public HomeController(ILogger<HomeController> logger, UserDetailRepository userDetailRepository, IDistributedCache cache, Domain.Data.DietContext context)
        {
            _logger = logger;
            _userDetailRepository = userDetailRepository;
            _cache = cache;
            _context = context;
        }


        [NoCache]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var claims = HttpContext.User.Claims;
            string Email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var profileId = (from user in _context.TblUserDetails
                             join profile in _context.TblProfileDetails on user.UserId equals profile.UserId
                             where user.Email == Email
                             select profile.ProfileId).FirstOrDefault();

            var allActivity = _context.TblActivityTrackings.Where(activity => activity.ProfileId == profileId).ToList();
            var activityViewModel = allActivity.Select(activity =>
            {
                return new ActivityTrackingViewModel
                {
                    Email = Email,
                    ActivityType = activity.ActivityType,
                    ActivityStartDatetime = activity.ActivityStartDatetime,
                    ActivityEndDatetime = activity.ActivityEndDatetime,
                    ActivityIntensity = (ActivityIntensityType)Enum.Parse(typeof(ActivityIntensityType), activity.ActivityIntensity),
                    CalorieBurned = activity.CalorieBurned,
                };
            });
            var chartData = activityViewModel
                    .Select(activity => new
                    {
                        x = activity.ActivityEndDatetime.ToString("o"), // ISO 8601 format
                        y = activity.CalorieBurned
                    }).OrderBy(data => DateTime.Parse(data.x)).ToList();


            ViewBag.Data = JsonConvert.SerializeObject(chartData);

            return View();
        }

        public async Task<IActionResult> ViewUserMealPlan(string UserName)
            {
            var MealPlanDetail = await (from user in _context.TblUserDetails
                                        join profile in _context.TblProfileDetails
                                        on user.UserId equals profile.UserId
                                        join mealPlan in _context.TblMealPlans
                                        on profile.MealPlanId equals mealPlan.MealPlanId
                                        where user.UserName == UserName
                                        select mealPlan).FirstOrDefaultAsync();

            Dictionary<string, string> nutritionInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(MealPlanDetail.NutritionInfo);

            var mealDetails = new List<object>
            {
                new
                {
                    ModelId = MealPlanDetail.MealPlanId,
                    ModelName = MealPlanDetail.PlanName,
                    ModelDescription = MealPlanDetail.PlanDescription,
                    ModelImage = MealPlanDetail.PlanImagePath
                }
            };

            var mealIds = new[]
            {
                MealPlanDetail.BreakfastMealId,
                MealPlanDetail.LunchMealId,
                MealPlanDetail.DinnerMealId
            };

            foreach (var mealId in mealIds)
            {
                var detail = _context.TblMeals
                    .Where(meal => meal.MealId == mealId)
                    .Select(meal => new
                    {
                        ModelId = meal.MealId,
                        ModelName = meal.MealName,
                        ModelDescription = meal.MealDescription,
                        ModelImage = meal.MealImagePath
                    })
                    .FirstOrDefault();

                if (detail != null)
                {
                    mealDetails.Add(detail);
                }
            }

            ViewBag.MealDetailsJson = JsonConvert.SerializeObject(mealDetails);

            return PartialView("ViewUserMealPlan");
        }

        /*public async Task<IActionResult> User([FromServices] IDistributedCache cache)
        {
            var usersinfo = new List<TblUserDetail>();
            if (string.IsNullOrEmpty(cache.GetString("users")))
            {
                usersinfo = _context.TblUserDetails.ToList();
                
                    
                var userstring = JsonConvert.SerializeObject(usersinfo);
                cache.SetString("users", userstring);
            }
            else
            {
                var userFromCache = cache.GetString("users");
                usersinfo = JsonConvert.DeserializeObject<List<TblUserDetail>>(userFromCache);
            }
            return View(usersinfo);
            }*/

        [NoCache]

        public IActionResult Privacy()
        {
            return View();
        }
        [NoCache]

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
