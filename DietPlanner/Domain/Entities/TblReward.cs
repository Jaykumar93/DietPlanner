using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class TblReward
{
    public Guid RewardId { get; set; }

    public Guid ChallengeId { get; set; }

    public string RewardDescription { get; set; } = null!;

    public string? RewardImagePath { get; set; }

    public virtual TblChallenge Challenge { get; set; } = null!;

    public virtual ICollection<TblProfileDetail> TblProfileDetails { get; set; } = new List<TblProfileDetail>();
}
