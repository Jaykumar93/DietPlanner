﻿using Domain.DTO;
using System.Security.Claims;


namespace Repository.Interfaces
{
    public interface IChallengeRewardRepository
    {
        public IEnumerable<ChallengesRewardViewModel>GetAllChallenges();
        

        public Task<bool>AddChallengesReward(ChallengesRewardViewModel model);
       

        public IEnumerable<ChallengesRewardViewModel> GetChallengesBasedOnStatus(ChallengesRewardViewModel.ChallengeStatus Status1, ChallengesRewardViewModel.ChallengeStatus Status2);

        public IEnumerable<ChallengesRewardViewModel> GetOngoingChallenges(IEnumerable<Claim> claims);
        public ChallengesRewardViewModel GetChallengeReward(string challengeName);
      

        public Task<bool> UpdateChallengeReward(ChallengesRewardViewModel UpdatedDetails);
       

        public Task<bool> DeleteChallengeReard(string challengeName);
      
    }
}
