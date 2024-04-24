using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class MealPlanViewModel
    {
        public string? PlanName { get; set; }

        public string PlanDescription { get; set; } = null!;

        public int CalorieCount { get; set; }

        public string NutritionInfo { get; set; } = null!;
    }
}
