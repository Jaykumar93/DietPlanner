using Domain.Data;
using Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Repository.Interfaces;
using System.Collections.Generic;
using System.Configuration;
using System.Data;


namespace Repository
{
    public class MealDetailRepository : IMealDetailRepository
    {
        private readonly Domain.Data.DietContext _context;
        private SqlConnection con;
        private readonly IConfiguration _config;
        private void connection()
        {
            string constr = _config.GetConnectionString("DefaultConnection");
            con = new SqlConnection(constr);
        }
        public MealDetailRepository(DietContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public List<TblMeal> GetAllMealDetails()
        {
            connection();
            List<TblMeal> mealDetailList = new List<TblMeal>();

            SqlCommand com = new SqlCommand("sp_GetAllMealDetails", con);
            com.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(com);
            DataTable dt = new DataTable();

            con.Open();
            da.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {

                mealDetailList.Add(new TblMeal
                {
                    MealName = Convert.ToString(dr["meal_name"]),
                    MealDescription = Convert.ToString(dr["meal_description"]),
                    NutritionInfo = Convert.ToString(dr["nutrition_info"]),
                    MealType = Convert.ToString(dr["meal_type"])

                });
            }

            return mealDetailList;

        }

        public TblMeal GetMealDetails(string mealName)
        {
            return _context.TblMeals.Where(meal => meal.MealName == mealName).FirstOrDefault();


        }

        public bool PostMealDetails(TblMeal meal)
        {
            _context.Add(meal);
            return Save();

            connection();
            SqlCommand com = new SqlCommand("sp_AddMealDetails", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@meal_name", meal.MealName);
            com.Parameters.AddWithValue("@meal_description", meal.MealDescription);
            com.Parameters.AddWithValue("@nutrition_info", meal.NutritionInfo);
            com.Parameters.AddWithValue("@meal_type", meal.MealType);


            con.Open();
            int i = com.ExecuteNonQuery();
            con.Close();
            if (i >= 1)
            {

                return true;

            }
            else
            {

                return false;
            }
        }

        public bool UpdateMealDetails(TblMeal meal)
        {
            _context.Update(meal);
            return Save();

            connection();
            SqlCommand com = new SqlCommand("sp_UpdateMealDetails", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@meal_name", meal.MealName);
            com.Parameters.AddWithValue("@meal_description", meal.MealDescription);
            com.Parameters.AddWithValue("@nutrition_info", meal.NutritionInfo);
            com.Parameters.AddWithValue("@meal_type", meal.MealType);


            con.Open();
            int i = com.ExecuteNonQuery();
            con.Close();
            if (i >= 1)
            {

                return true;

            }
            else
            {

                return false;
            }
        }

        public bool DeleteMealDetails(string mealName)
        {
            _context.Remove(mealName);
            return Save();

            connection();
            SqlCommand com = new SqlCommand("DeleteEmpById", con);

            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@meal_name", mealName);

            con.Open();
            int i = com.ExecuteNonQuery();
            con.Close();
            if (i >= 1)
            {
                return true;
            }
            else
            {

                return false;
            }
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }


    }
}
