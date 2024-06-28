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
using Repository.Interfaces;
using Microsoft.CodeAnalysis.Elfie.Serialization;


namespace DietPlanner.Controllers
{
    [NoCache]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserDetailRepository _userDetailRepository;
        private readonly IChallengeRewardRepository _challengeRewardRepository;
        private readonly IDistributedCache _cache;
        private readonly Domain.Data.DietContext _context;

        public HomeController(ILogger<HomeController> logger, IUserDetailRepository userDetailRepository,IChallengeRewardRepository challengeRewardRepository, IDistributedCache cache, Domain.Data.DietContext context)
        {
            _logger = logger;
            _userDetailRepository = userDetailRepository;
            _challengeRewardRepository = challengeRewardRepository;
            _cache = cache;
            _context = context;
        }


        [Authorize]
        public async Task<IActionResult> Index()
        {
            var claims = HttpContext.User.Claims;
            var serializedChartData = _userDetailRepository.GetUserActivityDetails(claims);



            ViewBag.Data = serializedChartData;

            return View();
        }

        [Authorize]
        public async Task<IActionResult> ViewUserMealPlan(string UserName, [FromServices] IDistributedCache cache)
        {

            var MealPlanDetail = await (from user in _context.TblUserDetails
                                        join profile in _context.TblProfileDetails
                                        on user.UserId equals profile.UserId
                                        join mealPlan in _context.TblMealPlans
                                        on profile.MealPlanId equals mealPlan.MealPlanId
                                        where user.UserName == UserName
                                        select mealPlan).FirstOrDefaultAsync();

            var cacheKey = $"{MealPlanDetail.PlanName}_{UserName}";
            var cachedUserMeal = await cache.GetStringAsync(cacheKey);

            List<object> mealDetails;

            if (string.IsNullOrEmpty(cachedUserMeal))
            {
               

                if (MealPlanDetail != null)
                {
                    var nutritionInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(MealPlanDetail.NutritionInfo);

                    mealDetails = new List<object>
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
                        var detail = await _context.TblMeals
                            .Where(meal => meal.MealId == mealId)
                            .Select(meal => new
                            {
                                ModelId = meal.MealId,
                                ModelName = meal.MealName,
                                ModelDescription = meal.MealDescription,
                                ModelImage = meal.MealImagePath
                            })
                            .FirstOrDefaultAsync();

                        if (detail != null)
                        {
                            mealDetails.Add(detail);
                        }
                    }

                    var mealDetailsJson = JsonConvert.SerializeObject(mealDetails);
                    var cacheOptions = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                    };

                    await cache.SetStringAsync(cacheKey, mealDetailsJson, cacheOptions);
                }
                else
                {
                    mealDetails = new List<object>();
                }
            }
            else
            {
                mealDetails = JsonConvert.DeserializeObject<List<object>>(cachedUserMeal);
            }

            ViewBag.MealDetailsJson = JsonConvert.SerializeObject(mealDetails);

            return PartialView("ViewUserMealPlan");
        }


        public async Task<IActionResult> OngoingChallenges()
        {
            var claims = HttpContext.User.Claims;
            var ongoingChallenges = _challengeRewardRepository.GetOngoingChallenges(claims);

            return PartialView("OngoingChallenges", ongoingChallenges);
        }


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
