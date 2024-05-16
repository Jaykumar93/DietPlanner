using Domain.Data;
using Domain.DTO;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Repository.Interfaces;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;


namespace Repository
{
    public class MealDetailRepository 
    {
        private readonly Domain.Data.DietContext _context;
        
       
        public MealDetailRepository(Domain.Data.DietContext context)
        {
            _context = context;
        }

        

        


    }
}
