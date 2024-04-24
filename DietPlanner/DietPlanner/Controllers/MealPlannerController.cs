using Domain.Data;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using Repository;
using Repository.Interfaces;
using Services.AuthServices;
namespace Web.Controllers
{
    public class MealPlannerController : Controller
    {
        private readonly IMealPlanRepository _mealPlanRepository;

        public MealPlannerController(IMealPlanRepository mealPlanRepository)
        {
            _mealPlanRepository = mealPlanRepository;
        }

        [NoCache]
        [Authorize]

        public IActionResult ViewMealPlans()
        {
           
            ModelState.Clear();
            return View(_mealPlanRepository.GetAllMealPlan());
        }

        [NoCache]
        [Authorize]

        public IActionResult GetMealPlans()
        {
            return View();
        }

        [NoCache]
        [Authorize]

        public IActionResult PostMealPlans(TblMealPlan mealPlan)
        {
            try
            {

                if (ModelState.IsValid)
                {

                    if (_mealPlanRepository.AddMealPlan(mealPlan))
                    {
                        ViewBag.Message = "Employee details added successfully";
                    }
                }

                return View();
            }
            catch
            {
                return View();
            }
        }

        [NoCache]
        [Authorize]

        public IActionResult DeleteMealPlans(string planName)
        {
            try
            {


                if (_mealPlanRepository.DeleteMealPlan(planName))
                {
                    ViewBag.AlertMsg = "Employee details deleted successfully";

                }
                return RedirectToAction("GetAllEmpDetails");

            }
            catch
            {
                return View();
            }
        }


        [NoCache]
        [Authorize]

        public IActionResult UpdateMealPlans(string Name)
        {

            return View(_mealPlanRepository.GetAllMealPlan().Find(mealplan => mealplan.PlanName == Name));
        }

        [NoCache]
        [Authorize]

        public IActionResult UpdateMealPlans(string Name, TblMealPlan obj)
        {
            try
            {

                _mealPlanRepository.UpdateMealPlan(obj);
                return RedirectToAction("GetAllEmpDetails");
            }
            catch
            {
                return View();
            }
        }

    }
}
