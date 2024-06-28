using Domain.Data;
using Newtonsoft.Json;
using Domain.DTO;


namespace Services.MealPlanServices
{
    public class MealInfoSummarize
    {
        private readonly DietContext _context;

        public MealInfoSummarize(DietContext context) 
        {
            _context = context;
        }

        public string NutritionSummarize(MealPlanViewModel model)
        {
            var breakfast = _context.TblMeals.Where(meal => meal.MealId.ToString() == model.BreakfastMealName)
                   .Select(meal => meal.NutritionInfo).FirstOrDefault();

            var lunch = _context.TblMeals.Where(meal => meal.MealId.ToString() == model.LunchMealName)
                    .Select(meal => meal.NutritionInfo).FirstOrDefault();
            var dinner = _context.TblMeals.Where(meal => meal.MealId.ToString() == model.DinnerMealName)
                    .Select(meal => meal.NutritionInfo).FirstOrDefault();

            Console.WriteLine(breakfast.GetType());
            
            

            Dictionary<string, object> BreakfastInfo =JsonConvert.DeserializeObject<Dictionary<string, object>>(breakfast);
            
            Dictionary<string, object> LunchInfo =JsonConvert.DeserializeObject<Dictionary<string, object>>(lunch);
           
            Dictionary<string, object> DinnerInfo =JsonConvert.DeserializeObject<Dictionary<string, object>>(dinner);
           
            Dictionary<string, object> PlanInfo = new Dictionary<string, object>();
            IEnumerable<string> allKeys = LunchInfo.Keys.Union(DinnerInfo.Keys).Union(BreakfastInfo.Keys);

            foreach (string key in allKeys)
            {
                List<object> values = new List<object>();

                // Collect values from BreakfastInfo, LunchInfo, and DinnerInfo
                if (BreakfastInfo.TryGetValue(key, out var value1))
                    values.Add(value1);

                if (LunchInfo.TryGetValue(key, out var value2))
                    values.Add(value2);

                if (DinnerInfo.TryGetValue(key, out var  value3))
                    values.Add(value3);

                if (values != null && values.Any())
                {
                    if (values.All(v => v is Int64))
                    {
                        PlanInfo[key] = values.Select(v => (Int64)v).Sum();

                    }
                    else if (values.All(v => v is string))
                    {
                        PlanInfo[key] = string.Join(", ", values.Cast<string>());
                    }
                    else if (values.All(v => v is object))
                    {
                        PlanInfo[key] = values;
                    }
                }
                else
                {
                    PlanInfo[key] = "No values found";
                }
            }
            string SerilizedInfo = JsonConvert.SerializeObject(PlanInfo);
            return (SerilizedInfo);
        }
    }
}
