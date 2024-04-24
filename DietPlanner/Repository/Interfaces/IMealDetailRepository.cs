using Domain.Entities;


namespace Repository.Interfaces
{
    public interface IMealDetailRepository
    {

        
       
        public bool PostMealDetails(TblMeal tblMealPlan);

        public TblMeal GetMealDetails(string mealName);

        public List<TblMeal> GetAllMealDetails();

        public bool DeleteMealDetails(string mealName);

        public bool UpdateMealDetails(TblMeal meal);
    }
}
