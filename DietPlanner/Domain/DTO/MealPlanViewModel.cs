﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public class MealPlanViewModel
    {
        public string? PlanName { get; set; }

        public string PlanDescription { get; set; } = null!;

        public string? BreakfastMealName { get; set; }

        public string? LunchMealName { get; set; }

        public string? DinnerMealName { get; set; }

        public int PlanCalorieCount { get; set; }
        public string PlanVitamin { get; set; } = null!;
        public string PlanMinerals { get; set; } = null!;
        public int PlanProtein { get; set; }
        public int PlanFat { get; set; }
        public int PlanCarbohydrates { get; set; }
        public int PlanWater { get; set; }
        public IFormFile? ImagePath { get; set; }

        public string ImageLocation {  get; set; }
        public string CreatedBy { get; set; } = null!;

        public DateOnly CreatedDate { get; set; }

        public string ModifiedBy { get; set; }

        public DateOnly ModifiedDate { get; set; }

        public List<SelectListItem> ListOfMeals {  get; set; }
    }
}
