using Domain.Data;
using Domain.DTO;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Repository.Interfaces;
using Services;
using System.Data;


namespace Repository
{
    public class MealDetailRepository : IMealDetailRepository
    {
        private readonly Domain.Data.DietContext _context;
        private readonly Upload _upload;

        public MealDetailRepository(Domain.Data.DietContext context,Upload upload)
        {
            _context = context;
            _upload = upload;
        }

        public IEnumerable<MealViewModel> GetAllMeals()
        {
            var allMeals = _context.TblMeals.ToList();

            var mealViewModels = allMeals.Select(meal =>
            {
                Dictionary<string, string> nutritionInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(meal.NutritionInfo);

                return new MealViewModel
                {
                    UserName = meal.CreatedBy,
                    MealName = meal.MealName,
                    MealDescription = meal.MealDescription,
                    TypeOfMeal = (MealViewModel.MealType)Enum.Parse(typeof(MealViewModel.MealType), meal.MealType),
                    CalorieCount = int.Parse(nutritionInfo.GetValueOrDefault("CalorieCount", "0")),
                    MealVitamin = nutritionInfo.GetValueOrDefault("MealVitamin", "None"),
                    MealMinerals = nutritionInfo.GetValueOrDefault("MealMinerals", "None"),
                    MealProtein = int.Parse(nutritionInfo.GetValueOrDefault("MealProtein", "0")),
                    MealFat = int.Parse(nutritionInfo.GetValueOrDefault("MealFat", "0")),
                    MealCarbohydrates = int.Parse(nutritionInfo.GetValueOrDefault("MealCarbohydrates", "0")),
                    MealWater = int.Parse(nutritionInfo.GetValueOrDefault("MealWater", "0"))
                };
            });
            return mealViewModels;
        }

        public MealViewModel GetMealByMealname(string mealName)
        {
            var mealdetail = _context.TblMeals.Where(meal => meal.MealName == mealName).FirstOrDefault();
            Dictionary<string, string> nutritionInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(mealdetail.NutritionInfo);

            MealViewModel UpdateMeal = new MealViewModel
            {
                UserName = mealdetail.CreatedBy,
                MealName = mealdetail.MealName,
                MealDescription = mealdetail.MealDescription,
                TypeOfMeal = (MealViewModel.MealType)Enum.Parse(typeof(MealViewModel.MealType), mealdetail.MealType),
                CalorieCount = int.Parse(nutritionInfo.GetValueOrDefault("CalorieCount", "0")),
                MealVitamin = nutritionInfo.GetValueOrDefault("MealVitamin", "None"),
                MealMinerals = nutritionInfo.GetValueOrDefault("MealMinerals", "None"),
                MealProtein = int.Parse(nutritionInfo.GetValueOrDefault("MealProtein", "0")),
                MealFat = int.Parse(nutritionInfo.GetValueOrDefault("MealFat", "0")),
                MealCarbohydrates = int.Parse(nutritionInfo.GetValueOrDefault("MealCarbohydrates", "0")),
                MealWater = int.Parse(nutritionInfo.GetValueOrDefault("MealWater", "0"))
            };
            return UpdateMeal;
        }

        public async Task<bool> AddMealDetails(MealViewModel model)
        {
            DateTime currentDate = DateTime.Today;
            string imagePath = await _upload.UploadMealImage(model.ImagePath);

            var NutriInformation = new
            {
                model.CalorieCount,
                model.MealVitamin,
                model.MealMinerals,
                model.MealProtein,
                model.MealFat,
                model.MealCarbohydrates,
                model.MealWater

            };
            string SerilizedInfo = JsonConvert.SerializeObject(NutriInformation);

            TblMeal mealDetail = new TblMeal
            {
                MealName = model.MealName,
                MealDescription = model.MealDescription,
                MealType = model.TypeOfMeal.ToString(),
                NutritionInfo = SerilizedInfo,
                CreatedBy = model.UserName,
                CreatedDate = DateOnly.Parse(currentDate.ToString("yyyy-MM-dd")),
                ModifiedBy = model.UserName,
                ModifiedDate = DateOnly.Parse(currentDate.ToString("yyyy-MM-dd")),
                MealImagePath = imagePath,
            };

            await _context.TblMeals.AddAsync(mealDetail);
            await _context.SaveChangesAsync();
            return true;    
        }
        
        public async Task<bool> UpdateMeal(MealViewModel model)
        {
            DateTime currentDate = DateTime.Today;
            string imagePath;

            var NutriInformation = new
            {
                model.CalorieCount,
                model.MealVitamin,
                model.MealMinerals,
                model.MealProtein,
                model.MealFat,
                model.MealCarbohydrates,
                model.MealWater
            };
            string UpdatedSerialized = JsonConvert.SerializeObject(NutriInformation);

            var mealDetail = await _context.TblMeals.FirstOrDefaultAsync(meal => meal.MealName == model.MealName);
            if (model.ImagePath != null)
            {
                imagePath = await _upload.UploadMealImage(model.ImagePath);

            }
            else
            {
                imagePath = mealDetail.MealImagePath;
            }
            if (mealDetail != null)
            {
                mealDetail.MealDescription = model.MealDescription;
                mealDetail.MealType = model.TypeOfMeal.ToString();
                mealDetail.NutritionInfo = UpdatedSerialized;
                mealDetail.ModifiedBy = model.UserName;
                mealDetail.ModifiedDate = DateOnly.Parse(currentDate.ToString("yyyy-MM-dd"));
                mealDetail.MealImagePath = imagePath;


                _context.TblMeals.Update(mealDetail);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public async Task<bool> DeleteMealDetail(string mealName)
        {
            var mealDetail = _context.TblMeals.Where(meal => meal.MealName == mealName).FirstOrDefault();
            _context.TblMeals.Remove(mealDetail);
            _context.SaveChanges();
            return true;
        }
    }
}
