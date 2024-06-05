using Domain.DTO;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Services;


namespace Repository.Interfaces
{
    public interface IMealDetailRepository
    {

        public IEnumerable<MealViewModel> GetAllMeals();

        public MealViewModel GetMealByMealname(string mealName);

        public Task<bool> AddMealDetails(MealViewModel model);

        public Task<bool> UpdateMeal(MealViewModel model);

        public Task<bool> DeleteMealDetail(string mealName);
        
    }
}
