using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.DTO;
using Services;
using Microsoft.EntityFrameworkCore;
using Domain.Data;
using Repository;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pipelines.Sockets.Unofficial.Arenas;
using System.Numerics;
using Domain.Entities;
using AspNetCoreHero.ToastNotification.Abstractions;
using Repository.Interfaces;
namespace Web.Controllers.User
{
    /*[Authorize(Roles = "User")]*/
    public class UserMealPlannerController : Controller
    {
        private readonly Domain.Data.DietContext _context;
        private readonly IMealDetailRepository _mealDetailRepository;
        private readonly IMealPlanRepository _mealPlanRepository;
        private readonly INotyfService _notyf;

        public UserMealPlannerController(Domain.Data.DietContext context, IMealDetailRepository mealDetailRepository, IMealPlanRepository mealPlanRepository,INotyfService notyf) 
        {
            _context = context;
            _mealDetailRepository = mealDetailRepository;
            _mealPlanRepository = mealPlanRepository;
            _notyf = notyf;
        }
        
        public IActionResult MealPlans(List<MealViewModel.MealType> selectedCategories,decimal? minCalorie, decimal? maxCalorie, string term="", string orderBy = "" )  
        {

            term = term.ToLower();
            var mealViewOrder = new MealViewModel();

            var allMeals = _context.TblMeals.ToList();

            var mealDetails = allMeals.Select(meal =>
            {
                Dictionary<string, string> nutritionInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(meal.NutritionInfo);

                return new MealViewModel
                {
                    UserName = meal.CreatedBy,
                    MealName = meal.MealName,
                    MealDescription = meal.MealDescription,
                    TypeOfMeal = (MealViewModel.MealType)Enum.Parse(typeof(MealViewModel.MealType), meal.MealType),
                    CalorieCount = int.Parse(nutritionInfo.GetValueOrDefault("CalorieCount", "0")),
                    MealVitamin = nutritionInfo.GetValueOrDefault("MealVitamin", "None"),
                    MealMinerals = nutritionInfo.GetValueOrDefault("MealMinerals", "None"),
                    MealProtein = int.Parse(nutritionInfo.GetValueOrDefault("MealProtein", "0")),
                    MealFat = int.Parse(nutritionInfo.GetValueOrDefault("MealFat", "0")),
                    MealCarbohydrates = int.Parse(nutritionInfo.GetValueOrDefault("MealCarbohydrates", "0")),
                    MealWater = int.Parse(nutritionInfo.GetValueOrDefault("MealWater", "0")),
                    Imglocation = meal.MealImagePath,
                };
            });

            if (selectedCategories?.Count > 0)
            {
                mealDetails = mealDetails.Where(p => selectedCategories.Contains(p.TypeOfMeal));
            }

            if (minCalorie.HasValue)
            {
                mealDetails = mealDetails.Where(meal => meal.CalorieCount >= minCalorie.Value);
            }

            if (maxCalorie.HasValue)
            {
                mealDetails = mealDetails.Where(meal => meal.CalorieCount <= maxCalorie.Value);
            }
            switch (orderBy)
            {
                case "meal_name_desc":
                    mealDetails = mealDetails.OrderByDescending(meal => meal.MealName);
                    break;
      
                case "calorie_count_desc":
                    mealDetails = mealDetails.OrderByDescending(meal => meal.CalorieCount);
                    break;
                case "calorie_count":
                    mealDetails = mealDetails.OrderBy(meal => meal.CalorieCount);
                    break;
                case "meal_name":
                    mealDetails = mealDetails.OrderBy(meal => meal.MealName);
                    break;
                default:
                    mealDetails = mealDetails.OrderBy(meal => meal.MealName);
                    break;
            }

            if (!string.IsNullOrEmpty(term))
            {
                mealDetails = mealDetails.Where(m => m.MealName.ToLower().Contains(term) ||
                                                m.MealDescription.ToLower().Contains(term) ||
                                                m.CalorieCount.ToString().Contains(term) ||
                                                m.MealVitamin.ToLower().Contains(term) ||
                                                m.MealMinerals.ToLower().Contains(term) ||
                                                m.MealProtein.ToString().Contains(term) ||
                                                m.MealCarbohydrates.ToString().Contains(term)||
                                                m.MealWater.ToString().Contains(term));
            }

            return View(mealDetails);
        }
    
       
        public IActionResult PlanDashboard(decimal? minCalorie, decimal? maxCalorie, string term = "", string orderBy = "")
        {
            term = term.ToLower();
            var planViewOrder = new MealPlanViewModel();

            var allPlans = _context.TblMealPlans.ToList();

            var planDetails = allPlans.Select(plan =>
            {
                Dictionary<string, string> nutritionInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(plan.NutritionInfo);

                return new MealPlanViewModel
                {
                    CreatedBy = plan.CreatedBy,
                    PlanName = plan.PlanName,
                    PlanDescription = plan.PlanDescription,
                    BreakfastMealName = _context.TblMeals.Where(meal=>meal.MealId == plan.BreakfastMealId).Select(meal=>meal.MealName).FirstOrDefault(),
                    LunchMealName = _context.TblMeals.Where(meal => meal.MealId == plan.LunchMealId).Select(meal => meal.MealName).FirstOrDefault(),
                    DinnerMealName = _context.TblMeals.Where(meal => meal.MealId == plan.DinnerMealId).Select(meal => meal.MealName).FirstOrDefault(),
                    PlanCalorieCount = int.Parse(nutritionInfo.GetValueOrDefault("CalorieCount", "0")),
                    PlanVitamin = nutritionInfo.GetValueOrDefault("MealVitamin", "None"),
                    PlanMinerals = nutritionInfo.GetValueOrDefault("MealMinerals", "None"),
                    PlanProtein = int.Parse(nutritionInfo.GetValueOrDefault("MealProtein", "0")),
                    PlanFat = int.Parse(nutritionInfo.GetValueOrDefault("MealFat", "0")),
                    PlanCarbohydrates = int.Parse(nutritionInfo.GetValueOrDefault("MealCarbohydrates", "0")),
                    PlanWater = int.Parse(nutritionInfo.GetValueOrDefault("MealWater", "0")),
                    ImageLocation = plan.PlanImagePath,
                };
            });

           /* if (selectedCategories?.Count > 0)
            {
                mealDetails = mealDetails.Where(p => selectedCategories.Contains(p.TypeOfMeal));
            }*/

            if (minCalorie.HasValue)
            {
                planDetails = planDetails.Where(plan => plan.PlanCalorieCount >= minCalorie.Value);
            }

            if (maxCalorie.HasValue)
            {
                planDetails = planDetails.Where(plan => plan.PlanCalorieCount <= maxCalorie.Value);
            }
            switch (orderBy)
            {
                case "plan_name_desc":
                    planDetails = planDetails.OrderByDescending(plan => plan.PlanName);
                    break;

                case "calorie_count_desc":
                    planDetails = planDetails.OrderByDescending(plan => plan.PlanCalorieCount);
                    break;
                case "calorie_count":
                    planDetails = planDetails.OrderBy(plan => plan.PlanCalorieCount);
                    break;
                case "plan_name":
                    planDetails = planDetails.OrderBy(plan => plan.PlanName);
                    break;
                default:
                    planDetails = planDetails.OrderBy(plan => plan.PlanName);
                    break;
            }

            if (!string.IsNullOrEmpty(term))
            {
                planDetails = planDetails.Where(p => p.PlanName.ToLower().Contains(term) ||
                                                p.PlanDescription.ToLower().Contains(term) ||
                                                p.PlanCalorieCount.ToString().Contains(term) ||
                                                p.BreakfastMealName.ToString().Contains(term) ||
                                                p.LunchMealName.ToString().Contains(term) ||
                                                p.DinnerMealName.ToString().Contains(term) ||
                                                p.PlanVitamin.ToLower().Contains(term) ||
                                                p.PlanMinerals.ToLower().Contains(term) ||
                                                p.PlanProtein.ToString().Contains(term) ||
                                                p.PlanCarbohydrates.ToString().Contains(term) ||
                                                p.PlanWater.ToString().Contains(term));
            }

           return View(planDetails);
        }


        public ActionResult MealInfoDetails(string mealName)
        {
            var mealdetail = _context.TblMeals.Where(meal => meal.MealName == mealName).FirstOrDefault();
            Dictionary<string, string> nutritionInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(mealdetail.NutritionInfo);

            MealViewModel mealmodel = new MealViewModel
            {
                UserName = mealdetail.CreatedBy,
                MealName = mealdetail.MealName,
                MealDescription = mealdetail.MealDescription,
                TypeOfMeal = (MealViewModel.MealType)Enum.Parse(typeof(MealViewModel.MealType), mealdetail.MealType),
                CalorieCount = int.Parse(nutritionInfo.GetValueOrDefault("CalorieCount", "0")),
                MealVitamin = nutritionInfo.GetValueOrDefault("MealVitamin", "None"),
                MealMinerals = nutritionInfo.GetValueOrDefault("MealMinerals", "None"),
                MealProtein = int.Parse(nutritionInfo.GetValueOrDefault("MealProtein", "0")),
                MealFat = int.Parse(nutritionInfo.GetValueOrDefault("MealFat", "0")),
                MealCarbohydrates = int.Parse(nutritionInfo.GetValueOrDefault("MealCarbohydrates", "0")),
                MealWater = int.Parse(nutritionInfo.GetValueOrDefault("MealWater", "0")),
                Imglocation = mealdetail.MealImagePath

            };
            return PartialView("MealInfoDetails",mealmodel);
        }


        public ActionResult PlanInfoDetails(string planName)
        {
            var planDetails = _context.TblMealPlans.Where(plan => plan.PlanName == planName).FirstOrDefault();
            Dictionary<string, string> nutritionInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(planDetails.NutritionInfo);

            MealPlanViewModel planModel = new MealPlanViewModel
            {
                CreatedBy = planDetails.CreatedBy,
                PlanName = planDetails.PlanName,
                PlanDescription = planDetails.PlanDescription,
                BreakfastMealName = _context.TblMeals.Where(meal => meal.MealId == planDetails.BreakfastMealId).Select(meal => meal.MealName).FirstOrDefault(),
                LunchMealName = _context.TblMeals.Where(meal => meal.MealId == planDetails.LunchMealId).Select(meal => meal.MealName).FirstOrDefault(),
                DinnerMealName = _context.TblMeals.Where(meal => meal.MealId == planDetails.DinnerMealId).Select(meal => meal.MealName).FirstOrDefault(),
                PlanCalorieCount = int.Parse(nutritionInfo.GetValueOrDefault("CalorieCount", "0")),
                PlanVitamin = nutritionInfo.GetValueOrDefault("MealVitamin", "None"),
                PlanMinerals = nutritionInfo.GetValueOrDefault("MealMinerals", "None"),
                PlanProtein = int.Parse(nutritionInfo.GetValueOrDefault("MealProtein", "0")),
                PlanFat = int.Parse(nutritionInfo.GetValueOrDefault("MealFat", "0")),
                PlanCarbohydrates = int.Parse(nutritionInfo.GetValueOrDefault("MealCarbohydrates", "0")),
                PlanWater = int.Parse(nutritionInfo.GetValueOrDefault("MealWater", "0")),
                ImageLocation = planDetails.PlanImagePath

            };
            return PartialView("PlanInfoDetails", planModel);
        }

        public async Task<ActionResult> AddPlanToUser(string PlanName, string UserName)
        {
            bool IsMealAddedToProfile =await _mealPlanRepository.AddPlanToUser(PlanName, UserName);
            if(IsMealAddedToProfile)
            {
                return Json(new { success = true, message = $"{PlanName} Meal Plan is Added to You Profile({UserName})" });

            }
            else
            {
                return Json(new { error = true, message = $"Error While Adding {PlanName} Meal Plan To Your Profile ({UserName})" });
            }
        }
    }
    
}

