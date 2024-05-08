using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class TblMealPlan
{
    public Guid MealPlanId { get; set; }

    public string? PlanName { get; set; }

    public string PlanDescription { get; set; } = null!;

    public Guid? BreakfastMealId { get; set; }

    public Guid? LunchMealId { get; set; }

    public Guid? DinnerMealId { get; set; }

    public string NutritionInfo { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateOnly CreatedDate { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public DateOnly ModifiedDate { get; set; }

    public virtual TblMeal? BreakfastMeal { get; set; }

    public virtual TblMeal? DinnerMeal { get; set; }

    public virtual TblMeal? LunchMeal { get; set; }

    public virtual ICollection<TblProfileDetail> TblProfileDetails { get; set; } = new List<TblProfileDetail>();

    public virtual ICollection<TblUserPost> TblUserPosts { get; set; } = new List<TblUserPost>();
}
