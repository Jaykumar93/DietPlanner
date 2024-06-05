using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class TblChallenge
{
    public Guid ChallengeId { get; set; }

    public string? ChallengeName { get; set; }

    public string ChallengeDescription { get; set; } = null!;

    public DateTime StartDatetime { get; set; }

    public DateTime EndDatetime { get; set; }

    public string ChallengeGoals { get; set; } = null!;

    public string? ChallengeImagePath { get; set; }

    public string? ChallengeStatus { get; set; }

    public virtual ICollection<TblChallengesRewardsLog> TblChallengesRewardsLogs { get; set; } = new List<TblChallengesRewardsLog>();

    public virtual ICollection<TblReward> TblRewards { get; set; } = new List<TblReward>();
}
