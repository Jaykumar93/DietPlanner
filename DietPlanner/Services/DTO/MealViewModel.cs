using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class MealViewModel
    {
        public enum MealType
        {
            Breakfast,
            Lunch,
            Dinner
        }


        public string? UserName { get; set; }
        public string? MealName { get; set; }

        public string MealDescription { get; set; } = null!;

        public MealType TypeOfMeal { get; set; }
        public int CalorieCount { get; set; }

        public string MealVitamin { get; set; } = null!;
        public string MealMinerals { get; set; } = null!;   
        public int MealProtein { get; set; } 
        public int MealFat { get; set; } 
        public int MealCarbohydrates { get; set; } 
        public int MealWater { get; set; } 

        public string MealNameOrder { get; set; }

        public string CalorieOrder { get; set; }
        public string UserNameOrder { get; set; }

     

    }
}

