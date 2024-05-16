using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public class MealFilterViewModel: IEnumerable<MealViewModel>
    {
        public List<SelectListItem> Categories { get; set; }
        public List<MealViewModel.MealType> SelectedCategories { get; set; }
        public decimal? MinCalorie { get; set; }
        public decimal? MaxCalorie { get; set; }
        public string SearchTerm { get; set; }
        public string SortOrder { get; set; }
        public IEnumerable<MealViewModel> MealPlans { get; set; }

        private List<MealViewModel> meals;

        public MealFilterViewModel()
        {
            meals = new List<MealViewModel>();
        }

        // Implementing IEnumerable<T> interface
        public IEnumerator<MealViewModel> GetEnumerator()
        {
            return meals.GetEnumerator();
        }

        // Implementing IEnumerable interface (explicitly)
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
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
        public IFormFile? ImagePath { get; set; }
        public string Imglocation {  get; set; }
        public string MealNameOrder { get; set; }

        public string CalorieOrder { get; set; }
        public string UserNameOrder { get; set; }

     

    }
}

