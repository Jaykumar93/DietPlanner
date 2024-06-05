using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{

    public class ChallengeFilterViewModel : IEnumerable<ChallengesRewardViewModel>
    {
        public List<SelectListItem> Categories { get; set; }
        public List<ChallengesRewardViewModel.UserChallengeStatus> SelectedCategories { get; set; }
        public decimal? startDateTime { get; set; }
        public decimal? endDateTime { get; set; }
        public string SearchTerm { get; set; }
        public string SortOrder { get; set; }
        public IEnumerable<ChallengesRewardViewModel> ChallengesReward { get; set; }


        public ChallengeFilterViewModel()
        {
            ChallengesReward = new List<ChallengesRewardViewModel>();
        }

        // Implementing IEnumerable<T> interface
        public IEnumerator<ChallengesRewardViewModel> GetEnumerator()
        {
            return ChallengesReward.GetEnumerator();
        }

        // Implementing IEnumerable interface (explicitly)
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    public class ChallengesRewardViewModel
    {
        public enum UserChallengeStatus
        {
            Registered,
            NotRegistered,
            Started,
            OnGoing,
            Completed,
            NotCompleted
        }

        public enum ChallengeStatus
        {
            Yet_To_Start,
            Ongoing,
            Ended
        }
        
        public Guid? ChallengeId { get; set; }
        public string? ChallengeName { get; set; }
        
        public string ChallengeDescription { get; set; } = null!;

        public DateTime StartDatetime { get; set; }

        public DateTime EndDatetime { get; set; }

        public string ChallengeGoals { get; set; } = null!;

        public IFormFile? ChallengeImagePath { get; set; }
        public string? ChallengeImgLocation { get; set; }
        public string? RewardImgLocation { get; set; }

        public IFormFile? RewardImagePath { get; set; }

        public string RewardDescription { get; set; } = null!;

        public DateTime? UserLogDateTime { get; set; }

        public Guid ProfileId { get; set; }

        public UserChallengeStatus? UserStatus { get; set; } 

        public ChallengeStatus? Challenge { get; set; }
    }

}
