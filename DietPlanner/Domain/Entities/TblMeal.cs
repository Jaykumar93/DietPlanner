using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class TblMeal
{
    public Guid MealId { get; set; }

    public string? MealName { get; set; }

    public string MealDescription { get; set; } = null!;

    public string MealType { get; set; } = null!;

    public string NutritionInfo { get; set; } = null!;

    public DateOnly CreatedDate { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string ModifiedBy { get; set; } = null!;

    public DateOnly ModifiedDate { get; set; }

    public virtual ICollection<TblMealPlan> TblMealPlanBreakfastMeals { get; set; } = new List<TblMealPlan>();

    public virtual ICollection<TblMealPlan> TblMealPlanDinnerMeals { get; set; } = new List<TblMealPlan>();

    public virtual ICollection<TblMealPlan> TblMealPlanLunchMeals { get; set; } = new List<TblMealPlan>();
}
