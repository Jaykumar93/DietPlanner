using Domain.Data;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Repository.Interfaces;
using Services.AuthServices;
using Services.ViewModels;

namespace Web.Controllers
{
   
    public class MealDetailsController : Controller
    {
        private readonly DietContext _context;

        public MealDetailsController( DietContext context)
        {
            _context = context;
        }

        [NoCache]
        [Authorize]
        public async Task<IActionResult> ViewMeals()
        {
            var allMeals = _context.TblMeals.ToList();

            var mealViewModels = allMeals.Select(meal =>
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
                    MealFat = int.Parse(nutritionInfo.GetValueOrDefault("MealFat","0")),
                    MealCarbohydrates = int.Parse(nutritionInfo.GetValueOrDefault("MealCarbohydrates", "0")),
                    MealWater =int.Parse(nutritionInfo.GetValueOrDefault("MealWater", "0"))
                };
            }).ToList();

            return View(mealViewModels);
        }



        [HttpGet]
        [NoCache]

        public IActionResult GetMealDetails(string mealName)
        {
           /* if (_mealDetailRepository.GetMealDetails(mealName) == null)
                return NotFound();

            var mealDetail = _mealDetailRepository.GetMealDetails(mealName);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);*/

            return Ok();
        }

        [NoCache]
        [Authorize]

        public async Task<IActionResult> CreateMeal()
        {
            return View();
        }


        [NoCache]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateMeal(MealViewModel model, DateTime dateTime)
        {
            try
            {
                DateTime currentDate = DateTime.Today;

                var NutriInformation = new
                {
                    CalorieCount = model.CalorieCount,
                    MealVitamin = model.MealVitamin,
                    MealMinerals = model.MealMinerals,
                    MealProtein = model.MealProtein,
                    MealFat = model.MealFat,
                    MealCarbohydrates = model.MealCarbohydrates,
                    MealWater = model.MealWater

                };
                string SerilizedInfo = JsonConvert.SerializeObject(NutriInformation);

                TblMeal mealDetail = new TblMeal
                {
                    MealName = model.MealName,
                    MealDescription = model.MealDescription,
                    MealType = model.TypeOfMeal.ToString(),
                    NutritionInfo = SerilizedInfo,
                    CreatedBy = model.UserName,
                    CreatedDate = DateOnly.Parse(currentDate.ToString("yyyy-MM-dd")),
                    ModifiedBy = model.UserName,
                    ModifiedDate = DateOnly.Parse(currentDate.ToString("yyyy-MM-dd"))
                };

                await _context.TblMeals.AddAsync(mealDetail);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Meal Added Successfully";
                return RedirectToAction("ViewMeals", "MealDetails");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while creating the meal.";

                return RedirectToAction("ViewMeals", "MealDetails");
            }

        }
        
        
        public IActionResult UpdateMeal(string mealName)
        {
            var mealdetail = _context.TblMeals.Where(meal => meal.MealName == mealName).FirstOrDefault();
            Dictionary<string, string> nutritionInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(mealdetail.NutritionInfo);

            MealViewModel UpdateMeal = new MealViewModel
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
                MealWater = int.Parse(nutritionInfo.GetValueOrDefault("MealWater", "0"))
            };

            return View(UpdateMeal);
        }


        [NoCache]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateMeal(MealViewModel UpdatedDetails)
        {
            try
            {
                DateTime currentDate = DateTime.Today;

                var NutriInformation = new
                {
                    CalorieCount = UpdatedDetails.CalorieCount,
                    MealVitamin = UpdatedDetails.MealVitamin,
                    MealMinerals = UpdatedDetails.MealMinerals,
                    MealProtein = UpdatedDetails.MealProtein,
                    MealFat = UpdatedDetails.MealFat,
                    MealCarbohydrates = UpdatedDetails.MealCarbohydrates,
                    MealWater = UpdatedDetails.MealWater
                };
                string UpdatedSerialized = JsonConvert.SerializeObject(NutriInformation);

                var mealDetail = await _context.TblMeals.FirstOrDefaultAsync(meal => meal.MealName == UpdatedDetails.MealName);

                if (mealDetail != null)
                {
                    mealDetail.MealDescription = UpdatedDetails.MealDescription;
                    mealDetail.MealType = UpdatedDetails.TypeOfMeal.ToString();
                    mealDetail.NutritionInfo = UpdatedSerialized;
                    mealDetail.ModifiedBy = UpdatedDetails.UserName;
                    mealDetail.ModifiedDate = DateOnly.Parse(currentDate.ToString("yyyy-MM-dd"));

                    _context.TblMeals.Update(mealDetail);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Meal Updated Successfully";
                }
                else
                {
                    TempData["Error"] = "Meal not found.";
                }

                return RedirectToAction("ViewMeals", "MealDetails");
            }
            catch (Exception ex)
            {
                

                TempData["Error"] = "An unexpected error occurred while updating the meal. Please contact support.";
                return RedirectToAction("ViewMeals", "MealDetails");
            }
        }


        [NoCache]
        [HttpPost]
        public IActionResult DeleteMeal(string mealName)
        {
            var mealDetail = _context.TblMeals.Where(meal => meal.MealName == mealName).FirstOrDefault();
            _context.TblMeals.Remove(mealDetail);
            _context.SaveChanges();
            return RedirectToAction("ViewMeals");
        }
    }
}
