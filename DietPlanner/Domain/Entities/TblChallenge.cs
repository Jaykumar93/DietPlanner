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

    public string ChallengeStatus { get; set; } = null!;

    public string? ChallengeImagePath { get; set; }

    public virtual ICollection<TblProfileDetail> TblProfileDetails { get; set; } = new List<TblProfileDetail>();

    public virtual ICollection<TblReward> TblRewards { get; set; } = new List<TblReward>();
}
