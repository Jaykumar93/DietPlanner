using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Services.AuthServices;
using Domain.DTO;
using Services.MealPlanServices;
using System.Security.Claims;
using Services;
using Domain.Data;
using AspNetCoreHero.ToastNotification.Abstractions;
using Repository;
using Repository.Interfaces;

namespace Web.Controllers.Admin
{
    [NoCache]
    [Authorize(Roles = "Admin")]
    public class AdminMealPlannerController : Controller
    {
        private readonly Domain.Data.DietContext _context;
        private readonly IMealPlanRepository _mealPlanRepository;
        private readonly MealInfoSummarize _mealInfoSummarize;
        private readonly Upload _upload;
        private readonly INotyfService _notyf;

        public AdminMealPlannerController(Domain.Data.DietContext dietContext, IMealPlanRepository mealPlanRepository ,MealInfoSummarize mealInfoSummarize, Upload upload, INotyfService notyf)
        {
            _context = dietContext;
            _mealPlanRepository = mealPlanRepository;
            _mealInfoSummarize = mealInfoSummarize;
            _upload = upload;
            _notyf = notyf;
        }


        [HttpGet]
        public async Task<IActionResult> ViewMealPlan()
        {

            List<MealPlanViewModel> allMealPlans = _mealPlanRepository.GetAllMealPlans().ToList();

            return View(allMealPlans);

        }

        [HttpGet]
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
        
        
        [HttpPost]
        public async Task<IActionResult> CreateMealPlan(MealPlanViewModel model)
        {
            try
            {
                var claims = HttpContext.User.Claims;

                bool isMealPlanAdded = await _mealPlanRepository.CreateMealPlans(model, claims);
                if(isMealPlanAdded)
                {
                    _notyf.Success("Meal Plan Added Successfully");
                }
                else
                {
                    _notyf.Warning("Meal Plan Not Added");
                }
                return RedirectToAction("ViewMealPlan", "AdminMealPlanner");
            }
            catch (Exception ex)
            {

                _notyf.Error("An error occurred while creating the Meal Plan.");
                return RedirectToAction("ViewMealPlan", "AdminMealPlanner");
            }
        }

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

        [HttpPost]
        public async Task<IActionResult> UpdateMealPlan(MealPlanViewModel model)
        {
            try
            {                
                var claims = HttpContext.User.Claims;
                bool isMealPlanUpdated = await _mealPlanRepository.UpdateMealPlans(model, claims);
                
                
                if(isMealPlanUpdated)
                { 
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


        [HttpPost]
        public async Task<IActionResult> DeleteMealPlan(string planName)
        {
            bool isMealPlanDeleted = await _mealPlanRepository.DeleteMealPlans(planName);
            if (isMealPlanDeleted)
            {
                _notyf.Success("Meal Plan Deleted Successfully");
            }
            else
            {
                _notyf.Error("Error While Deleting Meal Plans");
            }
            return RedirectToAction("ViewMealPlan");
        }
    }
}
