using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.DTO;
using Services;
using Microsoft.EntityFrameworkCore;
using Domain.Data;
using Repository;
using Newtonsoft.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace Web.Controllers.User
{
    /*[Authorize(Roles = "User")]*/
    public class UserMealPlannerController : Controller
    {
        private readonly DietContext _context;
        private readonly MealDetailRepository _mealDetailRepository;

        public UserMealPlannerController(DietContext context, MealDetailRepository mealDetailRepository) 
        {
            _context = context;
            _mealDetailRepository = mealDetailRepository;
        }


        private List<SelectListItem> GetCategorySelectList()
        {
            return Enum.GetValues(typeof(MealViewModel.MealType))
                .Cast<MealViewModel.MealType>()
                .Select(e => new SelectListItem
                {
                    Value = e.ToString(),
                    Text = e.ToString()
                }).ToList();
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
                mealDetails = mealDetails.Where(meal => meal.CalorieCount >= maxCalorie.Value);
            }
            switch (orderBy)
            {
                case "meal_name_desc":
                    mealDetails = mealDetails.OrderByDescending(meal => meal.MealName);
                    break;
                case "user_name_desc":
                    mealDetails = mealDetails.OrderByDescending(meal => meal.UserName);
                    break;
                case "user_name":
                    mealDetails = mealDetails.OrderBy(meal => meal.UserName);
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

            ViewBag.Categories = GetCategorySelectList();
            ViewBag.SelectedCategories = selectedCategories;
            return View(mealDetails);
        }

        
        public IActionResult _GridView()
        {
            return View();   
        }

        
    }
}

