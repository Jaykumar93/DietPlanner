

using Domain.Data;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Services.AuthServices;
using Services.DTO;
using Services.MealPlanServices;
using Services.ViewModels;
using System.Numerics;
using System.Security.Claims;

namespace Web.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class MealPlannerController : Controller
    {
        private readonly DietContext _context;
        private readonly MealInfoSummarize _mealInfoSummarize;

        public MealPlannerController(DietContext dietContext, MealInfoSummarize mealInfoSummarize)
        {
            _context = dietContext;
            _mealInfoSummarize = mealInfoSummarize;
        }
        [NoCache]
        [HttpGet]

        public async Task<IActionResult> ViewMealPlan()
        {
            var allPlans = _context.TblMealPlans.ToList();

            var planViewModel = allPlans.Select(plan =>
            {
                Dictionary<string, string> nutritionInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(plan.NutritionInfo);

                return new MealPlanViewModel
                {
                    PlanName = plan.PlanName,
                    PlanDescription = plan.PlanDescription,
                    BreakfastMealName = _context.TblMeals.Where(meal => meal.MealId == plan.BreakfastMealId).Select(meal => meal.MealName).FirstOrDefault(),
                    LunchMealName = _context.TblMeals.Where(meal => meal.MealId == plan.LunchMealId).Select(meal => meal.MealName).FirstOrDefault(),
                    DinnerMealName = _context.TblMeals.Where(meal => meal.MealId == plan.DinnerMealId).Select(meal => meal.MealName).FirstOrDefault(),

                    TotalCalorieCount = int.Parse(nutritionInfo.GetValueOrDefault("CalorieCount", "0")),
                    PlanVitamin = nutritionInfo.GetValueOrDefault("MealVitamin", "None"),
                    PlanMinerals = nutritionInfo.GetValueOrDefault("MealMinerals", "None"),
                    PlanProtein = int.Parse(nutritionInfo.GetValueOrDefault("MealProtein", "0")),
                    PlanFat = int.Parse(nutritionInfo.GetValueOrDefault("MealFat", "0")),
                    PlanCarbohydrates = int.Parse(nutritionInfo.GetValueOrDefault("MealCarbohydrates", "0")),
                    PlanWater = int.Parse(nutritionInfo.GetValueOrDefault("MealWater", "0")),
                    CreatedBy = plan.CreatedBy,
                    CreatedDate = plan.CreatedDate,
                    ModifiedBy = plan.ModifiedBy,
                    ModifiedDate = plan.ModifiedDate,

                };
            }).ToList();

            return View(planViewModel);

        }
        [NoCache]
        public async Task<IActionResult> CreateMealPlan()
        {
            var BreakFastList = (from meal in _context.TblMeals
                                 where meal.MealType == "Breakfast"
                                 select new SelectListItem()
                                 {
                                     Text = meal.MealName,
                                     Value = meal.MealId.ToString(),
                                 }).ToList();

            var LunchList = (from meal in _context.TblMeals
                             where meal.MealType == "lunch"
                             select new SelectListItem()
                             {
                                 Text = meal.MealName,
                                 Value = meal.MealId.ToString(),
                             }).ToList();

            var DinnerList = (from meal in _context.TblMeals
                              where meal.MealType == "Dinner"
                              select new SelectListItem()
                              {
                                  Text = meal.MealName,
                                  Value = meal.MealId.ToString(),
                              }).ToList();

            ViewBag.ListOfBreakfast = BreakFastList;
            ViewBag.ListOfLunch = LunchList;
            ViewBag.ListOfDinner = DinnerList;


            return View();
        }
        [NoCache]
        [HttpPost]
        public async Task<IActionResult> CreateMealPlan(MealPlanViewModel model)
        {
            try
            {
                DateTime currentDate = DateTime.Today;
                var claims = HttpContext.User.Claims;

                // Retrieve the value of a specific claim
                string userName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

                string PlanInfo = _mealInfoSummarize.NutritionSummarize(model);
                var BreakfastMealId = _context.TblMeals.Where(meal => meal.MealId.ToString() == model.BreakfastMealName).FirstOrDefault();
                var LunchMealId = _context.TblMeals.Where(meal => meal.MealId.ToString() == model.LunchMealName).FirstOrDefault();
                var DinnerMealId = _context.TblMeals.Where(meal => meal.MealId.ToString() == model.DinnerMealName).FirstOrDefault();

                TblMealPlan mealplan = new TblMealPlan
                {
                    PlanName = model.PlanName,
                    PlanDescription = model.PlanDescription,
                    BreakfastMealId = _context.TblMeals.Where(meal => meal.MealId.ToString() == model.BreakfastMealName).Select(meal => meal.MealId).FirstOrDefault(),
                    LunchMealId = _context.TblMeals.Where(meal => meal.MealId.ToString() == model.LunchMealName).Select(meal => meal.MealId).FirstOrDefault(),
                    DinnerMealId = _context.TblMeals.Where(meal => meal.MealId.ToString() == model.DinnerMealName).Select(meal => meal.MealId).FirstOrDefault(),
                    NutritionInfo = PlanInfo,
                    CreatedBy = userName,
                    CreatedDate = DateOnly.Parse(currentDate.ToString("yyyy-MM-dd")),
                    ModifiedBy = userName,
                    ModifiedDate = DateOnly.Parse(currentDate.ToString("yyyy-MM-dd")),
                };
                await _context.TblMealPlans.AddAsync(mealplan);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Meal Plan Added Successfully";

                return RedirectToAction("ViewMealPlan", "MealPlanner");
            }
            catch (Exception ex)
            {

                TempData["Error"] = "An error occurred while creating the Meal Plan.";

                return RedirectToAction("ViewMealPlan", "MealPlanner");
            }
        }

        [NoCache]
        [HttpGet]
        public IActionResult UpdateMealPlan(string planName)
        {
            var planDetails = _context.TblMealPlans.Where(plan => plan.PlanName == planName).FirstOrDefault();
            var BreakFastList = (from meal in _context.TblMeals
                                 where meal.MealType == "Breakfast"
                                 select new SelectListItem()
                                 {
                                     Text = meal.MealName,
                                     Value = meal.MealId.ToString(),
                                 }).ToList();

            var LunchList = (from meal in _context.TblMeals
                             where meal.MealType == "lunch"
                             select new SelectListItem()
                             {
                                 Text = meal.MealName,
                                 Value = meal.MealId.ToString(),
                             }).ToList();

            var DinnerList = (from meal in _context.TblMeals
                              where meal.MealType == "Dinner"
                              select new SelectListItem()
                              {
                                  Text = meal.MealName,
                                  Value = meal.MealId.ToString(),
                              }).ToList();

            ViewBag.ListOfBreakfast = BreakFastList;
            ViewBag.ListOfLunch = LunchList;
            ViewBag.ListOfDinner = DinnerList;


            MealPlanViewModel updatePlan = new MealPlanViewModel
            {

                PlanName = planDetails.PlanName,
                PlanDescription = planDetails.PlanDescription,
                BreakfastMealName = _context.TblMeals.Where(meal => meal.MealId == planDetails.BreakfastMealId).Select(meal => meal.MealName).FirstOrDefault(),
                LunchMealName = _context.TblMeals.Where(meal => meal.MealId == planDetails.LunchMealId).Select(meal => meal.MealName).FirstOrDefault(),
                DinnerMealName = _context.TblMeals.Where(meal => meal.MealId == planDetails.DinnerMealId).Select(meal => meal.MealName).FirstOrDefault(),
                CreatedBy = planDetails.CreatedBy,
                CreatedDate = planDetails.CreatedDate,

            };

            return View(updatePlan);
        }

        [NoCache]
        [HttpPost]
        public async Task<IActionResult> UpdateMealPlan(MealPlanViewModel model)
        {
            try
            {
                DateTime currentDate = DateTime.Today;
                var claims = HttpContext.User.Claims;

                // Retrieve the value of a specific claim
                string userName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

                string PlanInfo = _mealInfoSummarize.NutritionSummarize(model);
                var BreakfastMealId = _context.TblMeals.Where(meal => meal.MealId.ToString() == model.BreakfastMealName).FirstOrDefault();
                var LunchMealId = _context.TblMeals.Where(meal => meal.MealId.ToString() == model.LunchMealName).FirstOrDefault();
                var DinnerMealId = _context.TblMeals.Where(meal => meal.MealId.ToString() == model.DinnerMealName).FirstOrDefault();

                var planDetails = await _context.TblMealPlans.FirstOrDefaultAsync(plan => plan.PlanName == model.PlanName);

                if (planDetails != null)
                {
                    planDetails.PlanName = model.PlanName;
                    planDetails.PlanDescription = model.PlanDescription;
                    planDetails.BreakfastMealId = _context.TblMeals.Where(meal => meal.MealId.ToString() == model.BreakfastMealName).Select(meal => meal.MealId).FirstOrDefault();
                    planDetails.LunchMealId = _context.TblMeals.Where(meal => meal.MealId.ToString() == model.LunchMealName).Select(meal => meal.MealId).FirstOrDefault();
                    planDetails.DinnerMealId = _context.TblMeals.Where(meal => meal.MealId.ToString() == model.DinnerMealName).Select(meal => meal.MealId).FirstOrDefault();
                    planDetails.NutritionInfo = PlanInfo;
                    planDetails.ModifiedBy = userName;
                    planDetails.ModifiedDate = DateOnly.Parse(currentDate.ToString("yyyy-MM-dd"));

                    _context.TblMealPlans.Update(planDetails);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "MealPlan Updated Successfully";
                }
                else
                {
                    TempData["Error"] = "Meal not found.";

                }
                return RedirectToAction("ViewMealPlan", "MealPlanner");
            }
            catch (Exception ex)
            {

                TempData["Error"] = "An error occurred while Updating the Meal Plan.";

                return RedirectToAction("ViewMealPlan", "MealPlanner");
            }
        }

    }
}
