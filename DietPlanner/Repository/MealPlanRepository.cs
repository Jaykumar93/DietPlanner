using Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Repository.Interfaces;

using System.Data;


namespace Repository
{
    public class MealPlanRepository 
    {
        private readonly Domain.Data.DietContext _context;
        private readonly IConfiguration _config;
        
        public MealPlanRepository(Domain.Data.DietContext context, IConfiguration config) 
        {
            _context = context;
            _config = config;

        }


  
       

     
    }
}
