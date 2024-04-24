using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class TblMealPlan
{
    public Guid MealPlanId { get; set; }

    public Guid? MealId { get; set; }

    public string? PlanName { get; set; }

    public string PlanDescription { get; set; } = null!;

    public int CalorieCount { get; set; }

    public string NutritionInfo { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateOnly CreatedDate { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public DateOnly ModifiedDate { get; set; }

    public virtual TblMeal? Meal { get; set; }

    public virtual ICollection<TblProfileDetail> TblProfileDetails { get; set; } = new List<TblProfileDetail>();

    public virtual ICollection<TblUserPost> TblUserPosts { get; set; } = new List<TblUserPost>();
}
