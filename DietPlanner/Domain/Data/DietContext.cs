using System;
using System.Collections.Generic;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Domain.Data;

public partial class DietContext : DbContext
{
    public DietContext()
    {
    }

    public DietContext(DbContextOptions<DietContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblActivityTracking> TblActivityTrackings { get; set; }

    public virtual DbSet<TblChallenge> TblChallenges { get; set; }

    public virtual DbSet<TblConsultation> TblConsultations { get; set; }

    public virtual DbSet<TblMeal> TblMeals { get; set; }

    public virtual DbSet<TblMealPlan> TblMealPlans { get; set; }

    public virtual DbSet<TblPostComment> TblPostComments { get; set; }

    public virtual DbSet<TblPostLike> TblPostLikes { get; set; }

    public virtual DbSet<TblProfileDetail> TblProfileDetails { get; set; }

    public virtual DbSet<TblReward> TblRewards { get; set; }

    public virtual DbSet<TblRole> TblRoles { get; set; }

    public virtual DbSet<TblUserDetail> TblUserDetails { get; set; }

    public virtual DbSet<TblUserPost> TblUserPosts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblActivityTracking>(entity =>
        {
            entity.HasKey(e => e.ActivityId).HasName("PK__tbl_acti__482FBD63206654A4");

            entity.ToTable("tbl_activity_tracking");

            entity.Property(e => e.ActivityId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("activity_id");
            entity.Property(e => e.ActivityEndDatetime)
                .HasColumnType("datetime")
                .HasColumnName("activity_end_datetime");
            entity.Property(e => e.ActivityIntensity)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("activity_intensity");
            entity.Property(e => e.ActivityStartDatetime)
                .HasColumnType("datetime")
                .HasColumnName("activity_start_datetime");
            entity.Property(e => e.ActivityType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("activity_type");
            entity.Property(e => e.CalorieBurned).HasColumnName("calorie_burned");
            entity.Property(e => e.ProfileId).HasColumnName("profile_id");

            entity.HasOne(d => d.Profile).WithMany(p => p.TblActivityTrackings)
                .HasForeignKey(d => d.ProfileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_activ__profi__27F8EE98");
        });

        modelBuilder.Entity<TblChallenge>(entity =>
        {
            entity.HasKey(e => e.ChallengeId).HasName("PK__tbl_chal__CF635191DC73559D");

            entity.ToTable("tbl_challenges");

            entity.Property(e => e.ChallengeId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("challenge_id");
            entity.Property(e => e.ChallengeDescription)
                .IsUnicode(false)
                .HasColumnName("challenge_description");
            entity.Property(e => e.ChallengeGoals)
                .IsUnicode(false)
                .HasColumnName("challenge_goals");
            entity.Property(e => e.ChallengeName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("challenge_name");
            entity.Property(e => e.ChallengeStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("challenge_status");
            entity.Property(e => e.EndDatetime)
                .HasColumnType("datetime")
                .HasColumnName("end_datetime");
            entity.Property(e => e.StartDatetime)
                .HasColumnType("datetime")
                .HasColumnName("start_datetime");
        });

        modelBuilder.Entity<TblConsultation>(entity =>
        {
            entity.HasKey(e => e.ConsultationId).HasName("PK__tbl_cons__650FE0FBDD32CF56");

            entity.ToTable("tbl_consultation");

            entity.Property(e => e.ConsultationId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("consultation_id");
            entity.Property(e => e.AppointmentDateTime)
                .HasColumnType("datetime")
                .HasColumnName("appointment_date_time");
            entity.Property(e => e.AppointmentDuration).HasColumnName("appointment_duration");
            entity.Property(e => e.ConsultantAvailabilty)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("consultant_availabilty");
            entity.Property(e => e.ExpertProfileId).HasColumnName("expert_profile_id");
            entity.Property(e => e.UserProfileId).HasColumnName("user_profile_id");

            entity.HasOne(d => d.ExpertProfile).WithMany(p => p.TblConsultationExpertProfiles)
                .HasForeignKey(d => d.ExpertProfileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_consu__exper__2CBDA3B5");

            entity.HasOne(d => d.UserProfile).WithMany(p => p.TblConsultationUserProfiles)
                .HasForeignKey(d => d.UserProfileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_consu__user___2BC97F7C");
        });

        modelBuilder.Entity<TblMeal>(entity =>
        {
            entity.HasKey(e => e.MealId).HasName("PK__tbl_meal__2910B00FFA5CD6BD");

            entity.ToTable("tbl_meals");

            entity.Property(e => e.MealId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("meal_id");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.MealDescription)
                .IsUnicode(false)
                .HasColumnName("meal_description");
            entity.Property(e => e.MealName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("meal_name");
            entity.Property(e => e.MealType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("meal_type");
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("modified_by");
            entity.Property(e => e.ModifiedDate).HasColumnName("modified_date");
            entity.Property(e => e.NutritionInfo)
                .IsUnicode(false)
                .HasColumnName("nutrition_info");
        });

        modelBuilder.Entity<TblMealPlan>(entity =>
        {
            entity.HasKey(e => e.MealPlanId).HasName("PK__tbl_meal__05C5760741A04A08");

            entity.ToTable("tbl_meal_plans");

            entity.Property(e => e.MealPlanId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("meal_plan_id");
            entity.Property(e => e.CalorieCount).HasColumnName("calorie_count");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.MealId).HasColumnName("meal_id");
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("modified_by");
            entity.Property(e => e.ModifiedDate).HasColumnName("modified_date");
            entity.Property(e => e.NutritionInfo)
                .IsUnicode(false)
                .HasColumnName("nutrition_info");
            entity.Property(e => e.PlanDescription)
                .IsUnicode(false)
                .HasColumnName("plan_description");
            entity.Property(e => e.PlanName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("plan_name");

            entity.HasOne(d => d.Meal).WithMany(p => p.TblMealPlans)
                .HasForeignKey(d => d.MealId)
                .HasConstraintName("FK__tbl_meal___modif__078C1F06");
        });

        modelBuilder.Entity<TblPostComment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__tbl_post__E79576871BE4B380");

            entity.ToTable("tbl_post_comments");

            entity.Property(e => e.CommentId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("comment_id");
            entity.Property(e => e.CommentDateTime)
                .HasColumnType("datetime")
                .HasColumnName("comment_date_time");
            entity.Property(e => e.CommentDetail)
                .IsUnicode(false)
                .HasColumnName("comment_detail");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.ProfileId).HasColumnName("profile_id");

            entity.HasOne(d => d.Post).WithMany(p => p.TblPostComments)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_post___post___2334397B");

            entity.HasOne(d => d.Profile).WithMany(p => p.TblPostComments)
                .HasForeignKey(d => d.ProfileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_post___profi__24285DB4");
        });

        modelBuilder.Entity<TblPostLike>(entity =>
        {
            entity.HasKey(e => e.LikeId).HasName("PK__tbl_post__992C79307453016A");

            entity.ToTable("tbl_post_likes");

            entity.Property(e => e.LikeId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("like_id");
            entity.Property(e => e.LikeDateTime)
                .HasColumnType("datetime")
                .HasColumnName("like_date_time");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.ProfileId).HasColumnName("profile_id");

            entity.HasOne(d => d.Post).WithMany(p => p.TblPostLikes)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_post___post___1E6F845E");

            entity.HasOne(d => d.Profile).WithMany(p => p.TblPostLikes)
                .HasForeignKey(d => d.ProfileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_post___profi__1F63A897");
        });

        modelBuilder.Entity<TblProfileDetail>(entity =>
        {
            entity.HasKey(e => e.ProfileId).HasName("PK__tbl_prof__AEBB701FEA9D936E");

            entity.ToTable("tbl_profile_details");

            entity.Property(e => e.ProfileId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("profile_id");
            entity.Property(e => e.ChallengeId).HasColumnName("challenge_id");
            entity.Property(e => e.ImagePath)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("image_path");
            entity.Property(e => e.MealPlanId).HasColumnName("meal_plan_id");
            entity.Property(e => e.RewardId).HasColumnName("reward_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UserCalorieLimit)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("user_calorie_limit");
            entity.Property(e => e.UserCertification)
                .IsUnicode(false)
                .HasColumnName("user_Certification");
            entity.Property(e => e.UserGender)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("user_gender");
            entity.Property(e => e.UserGoals)
                .HasMaxLength(4000)
                .IsUnicode(false)
                .HasColumnName("user_goals");
            entity.Property(e => e.UserHeight)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("user_height");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UserSpeciality)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("user_speciality");
            entity.Property(e => e.UserWeight)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("user_weight");

            entity.HasOne(d => d.Challenge).WithMany(p => p.TblProfileDetails)
                .HasForeignKey(d => d.ChallengeId)
                .HasConstraintName("FK__tbl_profi__chall__13F1F5EB");

            entity.HasOne(d => d.MealPlan).WithMany(p => p.TblProfileDetails)
                .HasForeignKey(d => d.MealPlanId)
                .HasConstraintName("FK__tbl_profi__meal___12FDD1B2");

            entity.HasOne(d => d.Reward).WithMany(p => p.TblProfileDetails)
                .HasForeignKey(d => d.RewardId)
                .HasConstraintName("FK__tbl_profi__rewar__14E61A24");

            entity.HasOne(d => d.Role).WithMany(p => p.TblProfileDetails)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_profi__role___15DA3E5D");

            entity.HasOne(d => d.User).WithMany(p => p.TblProfileDetails)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_profi__user___1209AD79");
        });

        modelBuilder.Entity<TblReward>(entity =>
        {
            entity.HasKey(e => e.RewardId).HasName("PK__tbl_rewa__3DD599BC3BCED87A");

            entity.ToTable("tbl_rewards");

            entity.Property(e => e.RewardId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("reward_id");
            entity.Property(e => e.ChallengeId).HasColumnName("challenge_id");
            entity.Property(e => e.RewardDescription)
                .IsUnicode(false)
                .HasColumnName("reward_description");

            entity.HasOne(d => d.Challenge).WithMany(p => p.TblRewards)
                .HasForeignKey(d => d.ChallengeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_rewar__chall__0E391C95");
        });

        modelBuilder.Entity<TblRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__tbl_role__760965CC8D4414E4");

            entity.ToTable("tbl_role");

            entity.Property(e => e.RoleId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<TblUserDetail>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__tbl_user__B9BE370FE2B6EB41");

            entity.ToTable("tbl_user_details");

            entity.HasIndex(e => e.UserName, "UQ__tbl_user__7C9273C4CCBDA5CF").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__tbl_user__AB6E6164438F57C9").IsUnique();

            entity.Property(e => e.UserId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("user_id");
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("contact_number");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("last_name");
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("modified_by");
            entity.Property(e => e.ModifiedDate).HasColumnName("modified_date");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.PasswordSalt).HasColumnName("password_salt");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("user_name");
        });

        modelBuilder.Entity<TblUserPost>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__tbl_user__3ED7876683713794");

            entity.ToTable("tbl_user_post");

            entity.Property(e => e.PostId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("post_id");
            entity.Property(e => e.CreatedDateTime)
                .HasColumnType("datetime")
                .HasColumnName("created_date_time");
            entity.Property(e => e.MealPlanId).HasColumnName("meal_plan_id");
            entity.Property(e => e.PostContent)
                .IsUnicode(false)
                .HasColumnName("post_content");
            entity.Property(e => e.ProfileId).HasColumnName("profile_id");

            entity.HasOne(d => d.MealPlan).WithMany(p => p.TblUserPosts)
                .HasForeignKey(d => d.MealPlanId)
                .HasConstraintName("FK__tbl_user___meal___19AACF41");

            entity.HasOne(d => d.Profile).WithMany(p => p.TblUserPosts)
                .HasForeignKey(d => d.ProfileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_user___profi__1A9EF37A");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
