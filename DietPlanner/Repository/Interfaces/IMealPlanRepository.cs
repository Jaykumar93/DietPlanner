using Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IMealPlanRepository
    {
        public bool AddMealPlan(TblMealPlan tblMealPlan);

        public List<TblMealPlan> GetAllMealPlan();

        public TblMealPlan GetMealPlan(string mealPlanName);

        public bool UpdateMealPlan(TblMealPlan mealPlan);



        public bool DeleteMealPlan(string mealPlanName);
        


        public bool Save();

    }
}
