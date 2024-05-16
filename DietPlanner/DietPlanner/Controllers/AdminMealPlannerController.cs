using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Services.AuthServices;
using Domain.DTO;
using Services.MealPlanServices;
using System.Security.Claims;
using Services;
using Domain.Data;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace Web.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminMealPlannerController : Controller
    {
        private readonly DietContext _context;
        private readonly MealInfoSummarize _mealInfoSummarize;
        private readonly Upload _upload;
        private readonly INotyfService _notyf;

        public AdminMealPlannerController(DietContext dietContext, MealInfoSummarize mealInfoSummarize, Upload upload, INotyfService notyf)
        {
            _context = dietContext;
            _mealInfoSummarize = mealInfoSummarize;
            _upload = upload;
            _notyf = notyf;
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

                string imagePath = await _upload.UploadPlanImage(model.ImagePath);


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
                    PlanImagePath = imagePath,
                };
                await _context.TblMealPlans.AddAsync(mealplan);
                await _context.SaveChangesAsync();

                _notyf.Success("Meal Plan Added Successfully");
                return RedirectToAction("ViewMealPlan", "AdminMealPlanner");
            }
            catch (Exception ex)
            {

                _notyf.Error("An error occurred while creating the Meal Plan.");
                return RedirectToAction("ViewMealPlan", "AdminMealPlanner");
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
                string imagePath = await _upload.UploadPlanImage(model.ImagePath);

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
                    planDetails.PlanImagePath = imagePath;
                    _context.TblMealPlans.Update(planDetails);
                    await _context.SaveChangesAsync();
                    _notyf.Success("MealPlan Updated Successfully");
                }
                else
                {
                    _notyf.Warning("Meal Plan not Found");

                }
                return RedirectToAction("ViewMealPlan", "AdminMealPlanner");
            }
            catch (Exception ex)
            {

                _notyf.Error("An error occurred while Updating the Meal Plan.");
                return RedirectToAction("ViewMealPlan", "AdminMealPlanner");
            }
        }


        [NoCache]
        [HttpPost]
        public IActionResult DeleteMealPlan(string planName)
        {
            var plandetails = _context.TblMealPlans.Where(plan => plan.PlanName == planName).FirstOrDefault();
            _context.TblMealPlans.Remove(plandetails);
            _context.TblMealPlans.Remove(plandetails);
            _context.SaveChanges();
            return RedirectToAction("ViewMealPlan");
        }
    }
}
