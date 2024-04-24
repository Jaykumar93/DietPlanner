using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class TblUserPost
{
    public Guid PostId { get; set; }

    public Guid ProfileId { get; set; }

    public string PostContent { get; set; } = null!;

    public Guid? MealPlanId { get; set; }

    public DateTime CreatedDateTime { get; set; }

    public virtual TblMealPlan? MealPlan { get; set; }

    public virtual TblProfileDetail Profile { get; set; } = null!;

    public virtual ICollection<TblPostComment> TblPostComments { get; set; } = new List<TblPostComment>();

    public virtual ICollection<TblPostLike> TblPostLikes { get; set; } = new List<TblPostLike>();
}
