using Domain.DTO;
using System.Security.Claims;


namespace Repository.Interfaces
{
    public interface IMealPlanRepository
    {
        public IEnumerable<MealPlanViewModel> GetAllMealPlans();

        public Task<bool> CreateMealPlans(MealPlanViewModel model, IEnumerable<Claim> claims);

        public Task<bool> UpdateMealPlans(MealPlanViewModel model, IEnumerable<Claim> claims);

        public Task<bool> DeleteMealPlans(string planName);
        

    }
}
