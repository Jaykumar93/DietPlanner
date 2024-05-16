using Domain.Data;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Repository.Interfaces;
using Services.AuthServices;
using Domain.DTO;
using Services;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace Web.Controllers.Admin
{
    [Authorize(Roles = "Admin")]

    public class AdminMealDetails : Controller
    {
        private readonly DietContext _context;
        private readonly Upload _upload;
        private readonly INotyfService _notyf;


        public AdminMealDetails(DietContext context, Upload upload,INotyfService notyf)
        {
            _context = context;
            _upload = upload;
            _notyf = notyf;
        }

        [NoCache]
        public async Task<IActionResult> ViewMeals(string term = "", string orderBy = "")
        {
            term = term.ToLower();
            var mealViewOrder = new MealViewModel();

            mealViewOrder.MealNameOrder = string.IsNullOrEmpty(orderBy) ? "meal_name_desc" : "";
            mealViewOrder.UserNameOrder = orderBy == "UserName" ? "user_name_desc" : "user_name";
            mealViewOrder.CalorieOrder = orderBy == "CalorieCount" ? "calorie_count_desc" : "calorie_count";



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
                    MealFat = int.Parse(nutritionInfo.GetValueOrDefault("MealFat", "0")),
                    MealCarbohydrates = int.Parse(nutritionInfo.GetValueOrDefault("MealCarbohydrates", "0")),
                    MealWater = int.Parse(nutritionInfo.GetValueOrDefault("MealWater", "0"))
                };
            });


            switch (orderBy)
            {
                case "meal_name_desc":
                    mealViewModels = mealViewModels.OrderByDescending(meal => meal.MealName);
                    break;
                case "user_name_desc":
                    mealViewModels = mealViewModels.OrderByDescending(meal => meal.UserName);
                    break;
                case "user_name":
                    mealViewModels = mealViewModels.OrderBy(meal => meal.UserName);
                    break;
                case "calorie_count_desc":
                    mealViewModels = mealViewModels.OrderByDescending(meal => meal.CalorieCount);
                    break;
                case "calorie_count":
                    mealViewModels = mealViewModels.OrderBy(meal => meal.CalorieCount);
                    break;
                case "meal_name":
                    mealViewModels = mealViewModels.OrderBy(meal => meal.MealName);
                    break;
                default:
                    mealViewModels = mealViewModels.OrderBy(meal => meal.MealName);
                    break;
            }

            if (!string.IsNullOrEmpty(term))
            {
                mealViewModels = mealViewModels.Where(m => m.MealName.ToLower().Contains(term));
            }


            return View(mealViewModels);
        }


        [NoCache]
        public async Task<IActionResult> CreateMeal()
        {
            return View();
        }

        [NoCache]
        [HttpPost]
        public async Task<IActionResult> CreateMeal(MealViewModel model)
        {
            try
            {
                DateTime currentDate = DateTime.Today;
                string imagePath = await _upload.UploadMealImage(model.ImagePath);

                var NutriInformation = new
                {
                    model.CalorieCount,
                    model.MealVitamin,
                    model.MealMinerals,
                    model.MealProtein,
                    model.MealFat,
                    model.MealCarbohydrates,
                    model.MealWater

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
                    ModifiedDate = DateOnly.Parse(currentDate.ToString("yyyy-MM-dd")),
                    MealImagePath = imagePath,
                };

                await _context.TblMeals.AddAsync(mealDetail);
                await _context.SaveChangesAsync();
                _notyf.Success("Meal Added Successfully");
                return RedirectToAction("ViewMeals", "MealDetails");
            }
            catch (Exception ex)
            {
                _notyf.Error("An error occurred while creating the meal.");
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
        public async Task<IActionResult> UpdateMeal(MealViewModel UpdatedDetails)
        {
            try
            {
                DateTime currentDate = DateTime.Today;
                string imagePath;
                
                var NutriInformation = new
                {
                    UpdatedDetails.CalorieCount,
                    UpdatedDetails.MealVitamin,
                    UpdatedDetails.MealMinerals,
                    UpdatedDetails.MealProtein,
                    UpdatedDetails.MealFat,
                    UpdatedDetails.MealCarbohydrates,
                    UpdatedDetails.MealWater
                };
                string UpdatedSerialized = JsonConvert.SerializeObject(NutriInformation);

                var mealDetail = await _context.TblMeals.FirstOrDefaultAsync(meal => meal.MealName == UpdatedDetails.MealName);
                if (UpdatedDetails.ImagePath != null)
                {
                    imagePath = await _upload.UploadMealImage(UpdatedDetails.ImagePath);

                }
                else
                {
                    imagePath = mealDetail.MealImagePath;
                }
                if (mealDetail != null)
                {
                    mealDetail.MealDescription = UpdatedDetails.MealDescription;
                    mealDetail.MealType = UpdatedDetails.TypeOfMeal.ToString();
                    mealDetail.NutritionInfo = UpdatedSerialized;
                    mealDetail.ModifiedBy = UpdatedDetails.UserName;
                    mealDetail.ModifiedDate = DateOnly.Parse(currentDate.ToString("yyyy-MM-dd"));
                    mealDetail.MealImagePath = imagePath;


                    _context.TblMeals.Update(mealDetail);
                    await _context.SaveChangesAsync();
                    _notyf.Success("Meal Updated Successfully");
          
                }
                else
                {
                    _notyf.Warning("Meal not found.");
                }

                return RedirectToAction("ViewMeals", "AdminMealDetails");
            }
            catch (Exception ex)
            {

                _notyf.Error("An unexpected error occurred while updating the meal. Please contact support.");
                
                return RedirectToAction("ViewMeals", "AdminMealDetails");
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
