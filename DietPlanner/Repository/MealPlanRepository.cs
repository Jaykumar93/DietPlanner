using Domain.Data;
using Domain.DTO;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Services;
using Services.MealPlanServices;
using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;



namespace Repository
{
    public class MealPlanRepository
    {
        private readonly Domain.Data.DietContext _context;
        private readonly IConfiguration _config;
        private readonly Upload _upload;
        private readonly MealInfoSummarize _mealInfoSummarize;


        public MealPlanRepository(Domain.Data.DietContext context, IConfiguration config, Upload upload, MealInfoSummarize mealInfoSummarize)
        {
            _context = context;
            _config = config;
            _upload = upload;
            _mealInfoSummarize = mealInfoSummarize;
        }


        public IEnumerable<MealPlanViewModel> GetAllMealPlans()
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

                    PlanCalorieCount = int.Parse(nutritionInfo.GetValueOrDefault("CalorieCount", "0")),
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
            });
            return planViewModel;
        }

        public async Task<bool> CreateMealPlans(MealPlanViewModel model, IEnumerable<Claim> claims)
        {
            DateTime currentDate = DateTime.Today;


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
            return true;
        }

        public async Task<bool> UpdateMealPlans(MealPlanViewModel model, IEnumerable<Claim> claims)
        {
            DateTime currentDate = DateTime.Today;
            // Retrieve the value of a specific claim
            string userName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            string PlanInfo = _mealInfoSummarize.NutritionSummarize(model);
            var BreakfastMealId = _context.TblMeals.Where(meal => meal.MealId.ToString() == model.BreakfastMealName).FirstOrDefault();
            var LunchMealId = _context.TblMeals.Where(meal => meal.MealId.ToString() == model.LunchMealName).FirstOrDefault();
            var DinnerMealId = _context.TblMeals.Where(meal => meal.MealId.ToString() == model.DinnerMealName).FirstOrDefault();
            string imagePath = null;


            var planDetails = await _context.TblMealPlans.FirstOrDefaultAsync(plan => plan.PlanName == model.PlanName);

            if (model.ImagePath != null)
            {
                imagePath = await _upload.UploadPlanImage(model.ImagePath);
            }
            else
            {
                imagePath = planDetails.PlanImagePath;
            }
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
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> DeleteMealPlans(string planName)
        {
            var plandetails = _context.TblMealPlans.Where(plan => plan.PlanName == planName).FirstOrDefault();
            _context.TblMealPlans.Remove(plandetails);
            _context.SaveChanges();
            return true;
        }

        public async Task<bool> AddPlanToUser(string PlanName, string UserName)
        {
            var profileDetail = await (from user in _context.TblUserDetails
                                       join profile in _context.TblProfileDetails
                                       on user.UserId equals profile.UserId
                                       where user.UserName == UserName
                                       select profile).FirstOrDefaultAsync();

            var planId = await _context.TblMealPlans.Where(plan => plan.PlanName == PlanName).Select(plan => plan.MealPlanId).FirstOrDefaultAsync();
            if (profileDetail != null)
            {
                profileDetail.MealPlanId = planId;
                _context.TblProfileDetails.Update(profileDetail);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
