
using AspNetCoreHero.ToastNotification.Abstractions;
using Domain.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository;
using Repository.Interfaces;
using Services;
using Services.AuthServices;

namespace Web.Controllers.Admin
{
    [Authorize(Roles = "Admin")]

    public class AdminMealDetailsController : Controller
    {
        private readonly Domain.Data.DietContext _context;
        private readonly IMealDetailRepository _mealDetailRepository;
        private readonly Upload _upload;
        private readonly INotyfService _notyf;


        public AdminMealDetailsController(Domain.Data.DietContext context, IMealDetailRepository mealDetailRepository, Upload upload, INotyfService notyf)
        {
            _context = context;
            _mealDetailRepository = mealDetailRepository;
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


            var mealDetailModel = _mealDetailRepository.GetAllMeals();

            switch (orderBy)
            {
                case "meal_name_desc":
                    mealDetailModel = mealDetailModel.OrderByDescending(meal => meal.MealName);
                    break;
                case "user_name_desc":
                    mealDetailModel = mealDetailModel.OrderByDescending(meal => meal.UserName);
                    break;
                case "user_name":
                    mealDetailModel = mealDetailModel.OrderBy(meal => meal.UserName);
                    break;
                case "calorie_count_desc":
                    mealDetailModel = mealDetailModel.OrderByDescending(meal => meal.CalorieCount);
                    break;
                case "calorie_count":
                    mealDetailModel = mealDetailModel.OrderBy(meal => meal.CalorieCount);
                    break;
                case "meal_name":
                    mealDetailModel = mealDetailModel.OrderBy(meal => meal.MealName);
                    break;
                default:
                    mealDetailModel = mealDetailModel.OrderBy(meal => meal.MealName);
                    break;
            }

            if (!string.IsNullOrEmpty(term))
            {
                mealDetailModel = mealDetailModel.Where(m => m.MealName.ToLower().Contains(term));
            }


            return View(mealDetailModel);
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
                bool IsMealAdded = await _mealDetailRepository.AddMealDetails(model);
                if (IsMealAdded)
                {
                    _notyf.Success("Meal Added Successfully");
                }
                else
                {
                    _notyf.Warning("Meal Is Not Added");
                }

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
            MealViewModel UpdatedMeal = _mealDetailRepository.GetMealByMealname(mealName);

            return View(UpdatedMeal);
        }


        [NoCache]
        [HttpPost]
        public async Task<IActionResult> UpdateMeal(MealViewModel UpdatedDetails)
        {
            try
            {
                bool IsMealUpdated = await _mealDetailRepository.UpdateMeal(UpdatedDetails);
                if (IsMealUpdated)
                {
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
        public async Task<IActionResult> DeleteMealAsync(string mealName)
        {
            bool IsMealDeleted = await _mealDetailRepository.DeleteMealDetail(mealName);
            if (IsMealDeleted)
            {
                _notyf.Success("Meal Deleted Successfuly");
            }
            else
            {
                _notyf.Error("Error While Deleting Meal");
            }
            return RedirectToAction("ViewMeals");
        }
    }
}
