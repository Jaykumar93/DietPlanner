﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public class ProfileDetailViewModel
    {
        public string UserName { get; set; }

        public string RoleName { get; set; }    
        public IFormFile? ImagePath { get; set; }

        public Guid? MealPlanId { get; set; }

        public Guid? ChallengeId { get; set; }

        public Guid? RewardId { get; set; }

        public string? UserSpeciality { get; set; }

        public IFormFile? UserCertification { get; set; }

        public string UserGender { get; set; } = null!;

        public string? UserWeight { get; set; }

        public string? UserHeight { get; set; }

        public string? UserCalorieLimit { get; set; }

        public string? UserGoals { get; set; }

    }
}
