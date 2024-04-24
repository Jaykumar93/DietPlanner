using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class TblProfileDetail
{
    public Guid ProfileId { get; set; }

    public Guid UserId { get; set; }

    public Guid RoleId { get; set; }

    public string? ImagePath { get; set; }

    public Guid? MealPlanId { get; set; }

    public Guid? ChallengeId { get; set; }

    public Guid? RewardId { get; set; }

    public string? UserSpeciality { get; set; }

    public string? UserCertification { get; set; }

    public string UserGender { get; set; } = null!;

    public string? UserWeight { get; set; }

    public string? UserHeight { get; set; }

    public string? UserCalorieLimit { get; set; }

    public string? UserGoals { get; set; }

    public virtual TblChallenge? Challenge { get; set; }

    public virtual TblMealPlan? MealPlan { get; set; }

    public virtual TblReward? Reward { get; set; }

    public virtual TblRole Role { get; set; } = null!;

    public virtual ICollection<TblActivityTracking> TblActivityTrackings { get; set; } = new List<TblActivityTracking>();

    public virtual ICollection<TblConsultation> TblConsultationExpertProfiles { get; set; } = new List<TblConsultation>();

    public virtual ICollection<TblConsultation> TblConsultationUserProfiles { get; set; } = new List<TblConsultation>();

    public virtual ICollection<TblPostComment> TblPostComments { get; set; } = new List<TblPostComment>();

    public virtual ICollection<TblPostLike> TblPostLikes { get; set; } = new List<TblPostLike>();

    public virtual ICollection<TblUserPost> TblUserPosts { get; set; } = new List<TblUserPost>();

    public virtual TblUserDetail User { get; set; } = null!;
}
