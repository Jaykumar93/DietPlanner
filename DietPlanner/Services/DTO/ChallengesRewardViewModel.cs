using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO
{
    public class ChallengesRewardViewModel
    {
        public enum StatusType
        {
            Registered,
            NotRegistered,
            Started,
            OnGoing,
            Completed,
            NotCompleted
        }
        public string? ChallengeName { get; set; }

        public string ChallengeDescription { get; set; } = null!;

        public DateTime StartDatetime { get; set; }

        public DateTime EndDatetime { get; set; }

        public string ChallengeGoals { get; set; } = null!;

        public string ChallengeStatus { get; set; } = null!;

        public string RewardDescription { get; set; } = null!;
    }
}
