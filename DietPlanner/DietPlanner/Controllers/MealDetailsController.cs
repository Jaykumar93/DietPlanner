using Domain.Data;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository;
using Repository.Interfaces;
using Services.AuthServices;
using Services.ViewModels;
using System.Text.Json;
using System.Threading.Tasks;

namespace Web.Controllers
{
   
    public class MealDetailController : Controller
    {
        private readonly IMealDetailRepository _mealDetailRepository;
        private readonly Domain.Data.DietContext _context;

        public MealDetailController(IMealDetailRepository mealDetailRepository, Domain.Data.DietContext context) 
        {
            _mealDetailRepository = mealDetailRepository;
            _context = context;
        }



        [NoCache]
        [Authorize]
        public IActionResult ViewMealDetails()
        {
            ModelState.Clear();
            return View();
        }


        [HttpGet]
        [NoCache]
        public IActionResult GetMealDetails(string mealName)
        {
            if (_mealDetailRepository.GetMealDetails(mealName) == null)
                return NotFound();

            var mealDetail = _mealDetailRepository.GetMealDetails(mealName);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(mealDetail);
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
                string SerilizedInfo = JsonSerializer.Serialize(NutriInformation);

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
                return RedirectToAction("ViewMealDetails", "MealDetails");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while creating the meal.";

                return RedirectToAction("ViewMealDetails", "MealDetails");
            }

        }
    


        [NoCache]
        public IActionResult DeleteMealDetails()
        {
            return View();
        }

        [NoCache]




        public IActionResult UpdateMeal(TblMeal mealDetail)
        {
            try
            {
                _mealDetailRepository.UpdateMealDetails(mealDetail);
                return RedirectToAction("GetAllMealDetails");
            }
            catch
            {
                return View();
            }
        }

    }
}
