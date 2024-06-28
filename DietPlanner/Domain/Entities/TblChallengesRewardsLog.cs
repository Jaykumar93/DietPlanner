using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class TblChallengesRewardsLog
{
    public Guid ChallengeLogId { get; set; }

    public Guid ProfileId { get; set; }

    public Guid? ChallengeId { get; set; }

    public Guid? RewardId { get; set; }

    public DateTime? StatusDatetime { get; set; }

    public string? Status { get; set; }

    public double? ChallengeProgress { get; set; }

    public virtual TblChallenge? Challenge { get; set; }

    public virtual TblProfileDetail Profile { get; set; } = null!;

    public virtual TblReward? Reward { get; set; }
}
