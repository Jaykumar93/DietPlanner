using Domain.DTO;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IChallengeRewardRepository
    {
        public List<ChallengesRewardViewModel> GetAllChallenges();


        public Task<bool> AddChallengesReward(ChallengesRewardViewModel model);


        public ChallengesRewardViewModel GetChallengeReward(string challengeName);


        public Task<bool> UpdateChallengeReward(ChallengesRewardViewModel UpdatedDetails);


        public Task<bool> DeleteChallengeReard(string challengeName);
       
    }
}
