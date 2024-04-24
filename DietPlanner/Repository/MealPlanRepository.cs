using Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Repository.Interfaces;

using System.Data;


namespace Repository
{
    public class MealPlanRepository : IMealPlanRepository
    {
        private readonly Domain.Data.DietContext _context;
        private readonly IConfiguration _config;
        
        public MealPlanRepository(Domain.Data.DietContext context, IConfiguration config) 
        {
            _context = context;
            _config = config;

        }


        private SqlConnection con;
        private void connection()
        {
            string constr = _config.GetConnectionString("DefaultConnection");
            con = new SqlConnection(constr);
        }

        public bool AddMealPlan(TblMealPlan tblMealPlan)
        {
            connection();
            SqlCommand com = new SqlCommand("AddNewEmpDetails", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@PlanName", tblMealPlan.PlanName);
            com.Parameters.AddWithValue("@Description", tblMealPlan.PlanDescription);
            com.Parameters.AddWithValue("@CalorieCount", tblMealPlan.CalorieCount);
            com.Parameters.AddWithValue("@NutritionInfo", tblMealPlan.NutritionInfo);

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

        public List<TblMealPlan> GetAllMealPlan()
        {
            connection();
            List<TblMealPlan> MealPlanList = new List<TblMealPlan>();


            SqlCommand com = new SqlCommand("sp_GetAllUsers", con);
            com.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(com);
            DataTable dt = new DataTable();

            con.Open();
            da.Fill(dt);
            con.Close();

            //Bind EmpModel generic list using dataRow     
            foreach (DataRow dr in dt.Rows)
            {

                MealPlanList.Add(new TblMealPlan
                {
                    PlanName = Convert.ToString(dr["Id"]),
                    PlanDescription = Convert.ToString(dr["Name"]),
                    CalorieCount = Convert.ToInt32(dr["CalorieCount"]),
                    NutritionInfo = Convert.ToString(dr["NutritionInfo"])

                });
            }
                
            return MealPlanList;
        }

        public TblMealPlan GetMealPlan(string mealPlanName)
        {
            return _context.TblMealPlans.Where(mealplan => mealplan.PlanName == mealPlanName).FirstOrDefault();
        }

        public bool UpdateMealPlan(TblMealPlan mealPlan)
        {
            _context.Update(mealPlan);
            return Save();
        }

        public bool DeleteMealPlan(string mealPlanName)
        {
            connection();
            SqlCommand com = new SqlCommand("DeleteMealPlanByName", con);

            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@MealPlanName", mealPlanName);

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
